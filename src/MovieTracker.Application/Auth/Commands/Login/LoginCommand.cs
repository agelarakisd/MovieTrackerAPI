using MediatR;
using MovieTracker.Application.Common.Models;
using MovieTracker.Application.Auth.Commands.Register;

namespace MovieTracker.Application.Auth.Commands.Login;

public class LoginCommand : IRequest<ApiResponse<AuthResponse>>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}