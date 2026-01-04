using MediatR;
using MovieTracker.Application.Common.Models;
using MovieTracker.Application.Auth.Commands.Register;

namespace MovieTracker.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<ApiResponse<AuthResponse>>
{
    public string RefreshToken { get; set; } = string.Empty;
}