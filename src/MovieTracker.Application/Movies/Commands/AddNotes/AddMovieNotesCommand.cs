using MediatR;
using MovieTracker.Application.Common.Models;

namespace MovieTracker.Application.Movies.Commands.AddNotes;

public class AddMovieNotesCommand : IRequest<ApiResponse<bool>>
{
    public Guid MovieId { get; set; }
    public string? Notes { get; set; }
}