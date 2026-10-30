using Microsoft.EntityFrameworkCore;
using Npgsql;
using VehicleCare.Domain;
using VehicleCare.Infrastructure;

var action = args.FirstOrDefault() ?? "check";
try
{
    var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__VehicleCare")
        ?? throw new InvalidOperationException("Missing database configuration.");
    var settings = new NpgsqlConnectionStringBuilder(connectionString);
    if (action == "provision")
    {
        var target = settings.Database;
        if (target != "vehiclecare") throw new InvalidOperationException("Provisioning only supports vehiclecare.");
        settings.Database = "postgres";
        await using var admin = new NpgsqlConnection(settings.ConnectionString);
        await admin.OpenAsync();
        await using var exists = new NpgsqlCommand("SELECT 1 FROM pg_database WHERE datname = @name", admin);
        exists.Parameters.AddWithValue("name", target);
        if (await exists.ExecuteScalarAsync() is null)
        {
            await using var create = new NpgsqlCommand("CREATE DATABASE vehiclecare", admin);
            await create.ExecuteNonQueryAsync();
            Console.WriteLine("Created database vehiclecare.");
        }
        else Console.WriteLine("Database vehiclecare already exists; left unchanged.");
        return;
    }
    await using var connection = new NpgsqlConnection(connectionString);
    await connection.OpenAsync();
    await using var query = new NpgsqlCommand("SELECT current_database(), version(), COALESCE((SELECT ssl FROM pg_stat_ssl WHERE pid = pg_backend_pid()), false)", connection);
    await using (var reader = await query.ExecuteReaderAsync())
    {
        await reader.ReadAsync();
        Console.WriteLine($"Database: {reader.GetString(0)}; Server: {reader.GetString(1)}; TLS: {reader.GetBoolean(2)}");
    }
    if (action == "check") return;
    if (action != "smoke") throw new InvalidOperationException("Unknown action.");
    var options = new DbContextOptionsBuilder<GarageDbContext>().UseNpgsql(connectionString).Options;
    var id = Guid.NewGuid();
    // Persist a disposable fixture, read it in a new context, then remove only its unique identity row.
    try
    {
        await using (var db = new GarageDbContext(options))
        {
            db.Users.Add(new ApplicationUser { Id = id, UserName = $"smoke-{id:N}", NormalizedUserName = $"SMOKE-{id:N}" });
            db.Accounts.Add(new Account { Id = id, Name = "Connection smoke fixture" });
            db.Settings.Add(new UserSettings { AccountId = id });
            await db.SaveChangesAsync();
        }
        await using (var db = new GarageDbContext(options))
        {
            if (!await db.Accounts.AnyAsync(a => a.Id == id)) throw new InvalidOperationException("Persistence check failed.");
        }
        Console.WriteLine("Persistence verified across separate DbContexts.");
    }
    finally
    {
        await using var cleanup = new GarageDbContext(options);
        await cleanup.Users.Where(u => u.Id == id).ExecuteDeleteAsync();
    }
    Console.WriteLine("Smoke fixture removed.");
}
catch (PostgresException e)
{
    Console.Error.WriteLine($"Database operation failed (SQLSTATE {e.SqlState}); check permissions/schema. No credentials printed.");
    Environment.ExitCode = 1;
}
catch (Exception e)
{
    Console.Error.WriteLine($"Database operation failed ({e.GetType().Name}); check configuration and connectivity. No credentials printed.");
    Environment.ExitCode = 1;
}
