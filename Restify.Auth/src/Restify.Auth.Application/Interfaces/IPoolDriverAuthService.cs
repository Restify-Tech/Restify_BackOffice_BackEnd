using Restify.Auth.Application.DTOs.Auth;
using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Application.DTOs.PoolDriverAuth;

namespace Restify.Auth.Application.Interfaces;

public interface IPoolDriverAuthService
{
    Task<Result<PoolDriverLoginResponse>> RegisterAsync(PoolDriverRegisterRequest request, CancellationToken cancellationToken = default);
    Task<Result<PoolDriverLoginResponse>> LoginAsync(PoolDriverLoginRequest request, CancellationToken cancellationToken = default);
    Task<Result<TokenResponse>> RefreshTokenAsync(PoolDriverRefreshTokenRequest request, CancellationToken cancellationToken = default);
}
