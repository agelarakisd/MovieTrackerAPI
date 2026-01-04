namespace MovieTracker.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "User"; // "User" or "Admin"
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    public ICollection<Series> Series { get; set; } = new List<Series>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}