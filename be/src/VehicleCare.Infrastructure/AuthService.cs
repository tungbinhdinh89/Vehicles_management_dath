using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VehicleCare.Application;
using VehicleCare.Domain;

namespace VehicleCare.Infrastructure;

public sealed class AuthService(
    GarageDbContext db,
    UserManager<ApplicationUser> users,
    IDataProtectionProvider protectionProvider,
    IBusinessClock clock) : IAuthService
{
    private readonly IDataProtector ticketProtector =
        protectionProvider.CreateProtector("VehicleCare.AuthSession");

    public async Task<AccountResponse> RegisterAsync(
        RegisterRequest request, CancellationToken cancellationToken)
    {
        ValidateRequest(request);
        var email = request.Email.Trim();
        var normalizedEmail = users.NormalizeEmail(email);
        if (await users.Users.AnyAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken))
            throw new AuthConflictException("Email đã được đăng ký.");

        var account = new Account { Name = request.Name.Trim(), Phone = request.Phone?.Trim() ?? "" };
        var user = new ApplicationUser
        {
            Id = account.Id,
            Email = email,
            UserName = email,
            EmailConfirmed = true
        };

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var result = await users.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new AuthValidationException(result.Errors.Select(error => error.Description).ToArray());

        db.Accounts.Add(account);
        db.Settings.Add(new UserSettings { AccountId = account.Id });
        await db.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        return ToResponse(account, email);
    }

    public async Task<(AccountResponse Account, Guid SessionId)?> LoginAsync(
        LoginRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email?.Trim();
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(request.Password))
            return null;
        var user = await users.FindByEmailAsync(email);
        if (user is null || await users.IsLockedOutAsync(user))
            return null;

        if (!await users.CheckPasswordAsync(user, request.Password))
        {
            await users.AccessFailedAsync(user);
            return null;
        }

        await users.ResetAccessFailedCountAsync(user);
        var account = await db.Accounts.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == user.Id, cancellationToken);
        if (account is null)
            return null;

        var session = CreateSession(account.Id);
        db.Sessions.Add(session);
        await db.SaveChangesAsync(cancellationToken);
        return (ToResponse(account, user.Email ?? email), session.Id);
    }

    public async Task<AccountResponse?> GetAccountAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var result = await (
            from account in db.Accounts.AsNoTracking()
            join user in db.Users.AsNoTracking() on account.Id equals user.Id
            where account.Id == accountId
            select new AccountResponse(account.Id, account.Name, user.Email!, account.Phone))
            .SingleOrDefaultAsync(cancellationToken);
        return result;
    }

    public async Task RevokeSessionAsync(Guid accountId, Guid sessionId, CancellationToken cancellationToken)
    {
        var session = await db.Sessions
            .SingleOrDefaultAsync(x => x.Id == sessionId && x.AccountId == accountId, cancellationToken);
        if (session is null)
            return;

        session.RevokedAtUtc = clock.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
    }

    public string ProtectTicket(Guid sessionId) =>
        ticketProtector.Protect(sessionId.ToString("N"));

    private static void ValidateRequest(RegisterRequest request)
    {
        var name = request.Name?.Trim();
        var email = request.Email?.Trim();
        var validEmail = false;
        if (!string.IsNullOrWhiteSpace(email))
        {
            try
            {
                validEmail = new System.Net.Mail.MailAddress(email).Address.Equals(
                    email, StringComparison.OrdinalIgnoreCase);
            }
            catch (FormatException)
            {
                validEmail = false;
            }
        }

        if (name is not { Length: >= 2 and <= 60 } || !validEmail)
            throw new AuthValidationException(["Tên hoặc email không hợp lệ."]);
        var phone = request.Phone?.Trim();
        if (!string.IsNullOrEmpty(phone) &&
            !System.Text.RegularExpressions.Regex.IsMatch(phone, "^0\\d{9}$"))
            throw new AuthValidationException(["Số điện thoại không hợp lệ."]);
    }

    private AuthSession CreateSession(Guid accountId)
    {
        var sessionId = Guid.NewGuid();
        return new AuthSession
        {
            Id = sessionId,
            AccountId = accountId,
            ProtectedTicket = ProtectTicket(sessionId),
            CreatedAtUtc = clock.UtcNow,
            ExpiresAtUtc = clock.UtcNow.AddHours(8)
        };
    }

    private static AccountResponse ToResponse(Account account, string email) =>
        new(account.Id, account.Name, email, account.Phone);
}

public sealed class AuthConflictException(string message) : Exception(message);

public sealed class AuthValidationException(IReadOnlyCollection<string> errors) : Exception
{
    public IReadOnlyCollection<string> Errors { get; } = errors;
}
