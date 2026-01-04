using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieTracker.Application.Auth.Commands.Register;
using MovieTracker.Application.Common.Interfaces;
using MovieTracker.Application.Common.Models;

namespace MovieTracker.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<AuthResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(IApplicationDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<ApiResponse<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

        if (user == null)
        {
            return ApiResponse<AuthResponse>.FailureResult("Invalid username or password");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return ApiResponse<AuthResponse>.FailureResult("Invalid username or password");
        }

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = await _tokenService.CreateRefreshTokenAsync(user.Id, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        var response = new AuthResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token
        };

        return ApiResponse<AuthResponse>.SuccessResult(response, "Login successful");
    }
}