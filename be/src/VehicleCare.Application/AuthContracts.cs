namespace VehicleCare.Application;

/// <summary>Payload for creating an account.</summary>
public sealed record RegisterRequest(string Name, string Email, string Password, string? Phone);

/// <summary>Payload for creating an authenticated session.</summary>
public sealed record LoginRequest(string Email, string Password);

/// <summary>Public account information returned by authentication endpoints.</summary>
public sealed record AccountResponse(Guid Id, string Name, string Email, string Phone);

/// <summary>CSRF token returned to a browser client.</summary>
public sealed record CsrfResponse(string Token);

public interface IAuthService
{
    Task<AccountResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
    Task<(AccountResponse Account, Guid SessionId)?> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
    Task<AccountResponse?> GetAccountAsync(Guid accountId, CancellationToken cancellationToken);
    Task RevokeSessionAsync(Guid accountId, Guid sessionId, CancellationToken cancellationToken);
}
