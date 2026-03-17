using Restify.Auth.Application.DTOs.Auth;
using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Application.DTOs.DriverAuth;

namespace Restify.Auth.Application.Interfaces;

public interface IDriverAuthService
{
    Task<Result<DriverLoginResponse>> RegisterAsync(DriverRegisterRequest request, CancellationToken cancellationToken = default);
    Task<Result<DriverLoginResponse>> LoginAsync(DriverLoginRequest request, CancellationToken cancellationToken = default);
    Task<Result<TokenResponse>> RefreshTokenAsync(DriverRefreshTokenRequest request, CancellationToken cancellationToken = default);
}
