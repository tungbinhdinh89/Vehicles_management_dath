using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.HttpResults;
using VehicleCare.Application;
using VehicleCare.Infrastructure;

namespace VehicleCare.Api;

public static class AuthEndpoints
{
    private const string CsrfCookie = "vehiclecare-csrf";
    private const string CsrfHeader = "X-CSRF-TOKEN";

    public static void MapAuth(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/auth").WithTags("Auth");
        group.MapGet("/csrf", GetCsrf)
            .WithSummary("Issue a CSRF token")
            .Produces<CsrfResponse>();
        group.MapPost("/register", Register)
            .WithSummary("Register an account")
            .Produces<AccountResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);
        group.MapPost("/login", Login)
            .WithSummary("Create an authenticated session")
            .Produces<AccountResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        group.MapGet("/me", Me)
            .RequireAuthorization()
            .WithSummary("Get the current account")
            .Produces<AccountResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        group.MapPost("/logout", Logout)
            .RequireAuthorization()
            .WithSummary("Revoke the current session")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private static IResult GetCsrf(HttpContext context, IDataProtectionProvider protectionProvider)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var protectedToken = protectionProvider.CreateProtector("VehicleCare.Csrf").Protect(token);
        context.Response.Cookies.Append(CsrfCookie, protectedToken, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Secure = context.Request.IsHttps || context.Request.Host.Host is not ("localhost" or "127.0.0.1")
        });
        return TypedResults.Ok(new CsrfResponse(token));
    }

    private static async Task<Results<Created<AccountResponse>, ValidationProblem, Conflict>> Register(
        RegisterRequest request, HttpContext context, IAuthService auth,
        IDataProtectionProvider protectionProvider, CancellationToken cancellationToken)
    {
        if (!IsCsrfValid(context, protectionProvider))
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["csrf"] = ["CSRF token không hợp lệ."]
            });
        try
        {
            var result = await auth.RegisterAsync(request, cancellationToken);
            return TypedResults.Created("/api/v1/auth/me", result);
        }
        catch (AuthValidationException exception)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["password"] = exception.Errors.ToArray()
            });
        }
        catch (AuthConflictException)
        {
            return TypedResults.Conflict();
        }
    }

    private static async Task<Results<Ok<AccountResponse>, UnauthorizedHttpResult, ValidationProblem>> Login(
        LoginRequest request, HttpContext context, IAuthService auth,
        IDataProtectionProvider protectionProvider, CancellationToken cancellationToken)
    {
        if (!IsCsrfValid(context, protectionProvider))
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["csrf"] = ["CSRF token không hợp lệ."]
            });
        var result = await auth.LoginAsync(request, cancellationToken);
        if (result is null)
            return TypedResults.Unauthorized();
        await SignIn(context, result.Value.Account, result.Value.SessionId);
        return TypedResults.Ok(result.Value.Account);
    }

    private static async Task<Results<Ok<AccountResponse>, UnauthorizedHttpResult>> Me(
        ClaimsPrincipal principal, IAuthService auth, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out var accountId))
            return TypedResults.Unauthorized();
        var account = await auth.GetAccountAsync(accountId, cancellationToken);
        return account is null ? TypedResults.Unauthorized() : TypedResults.Ok(account);
    }

    private static async Task<Results<NoContent, ValidationProblem>> Logout(
        HttpContext context, IAuthService auth, IDataProtectionProvider protectionProvider,
        CancellationToken cancellationToken)
    {
        if (!IsCsrfValid(context, protectionProvider))
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["csrf"] = ["CSRF token không hợp lệ."]
            });
        if (Guid.TryParse(context.User.FindFirstValue(ClaimTypes.NameIdentifier), out var accountId) &&
            Guid.TryParse(context.User.FindFirstValue("sid"), out var sessionId))
            await auth.RevokeSessionAsync(accountId, sessionId, cancellationToken);
        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return TypedResults.NoContent();
    }

    private static bool IsCsrfValid(HttpContext context, IDataProtectionProvider protectionProvider)
    {
        if (!context.Request.Headers.TryGetValue(CsrfHeader, out var header) ||
            !context.Request.Cookies.TryGetValue(CsrfCookie, out var cookie) ||
            header.Count != 1)
            return false;
        try
        {
            var token = protectionProvider.CreateProtector("VehicleCare.Csrf").Unprotect(cookie);
            return CryptographicOperations.FixedTimeEquals(
                Convert.FromBase64String(token), Convert.FromBase64String(header[0]!));
        }
        catch (FormatException)
        {
            return false;
        }
        catch (System.Security.Cryptography.CryptographicException)
        {
            return false;
        }
    }

    private static Task SignIn(HttpContext context, AccountResponse account, Guid sessionId)
    {
        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()));
        identity.AddClaim(new Claim(ClaimTypes.Email, account.Email));
        identity.AddClaim(new Claim("sid", sessionId.ToString()));
        return context.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = false, AllowRefresh = false });
    }
}
