using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTracker.Application.Movies.Commands.AddMovie;
using MovieTracker.Application.Movies.Commands.AddNotes;
using MovieTracker.Application.Movies.Commands.DeleteMovie;
using MovieTracker.Application.Movies.Commands.MarkAsWatched;
using MovieTracker.Application.Movies.Commands.RateMovie;
using MovieTracker.Application.Movies.Queries.GetMyMovies;

namespace MovieTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MoviesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MoviesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> AddMovie([FromBody] AddMovieCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyMovies([FromQuery] bool? isWatched = null)
    {
        var query = new GetMyMoviesQuery { IsWatched = isWatched };
        var result = await _mediator.Send(query);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    [HttpPut("{id}/watched")]
    public async Task<IActionResult> MarkAsWatched(Guid id, [FromBody] MarkAsWatchedRequest request)
    {
        var command = new MarkAsWatchedCommand 
        { 
            MovieId = id, 
            IsWatched = request.IsWatched 
        };
        var result = await _mediator.Send(command);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    [HttpPut("{id}/rating")]
    public async Task<IActionResult> RateMovie(Guid id, [FromBody] RateMovieRequest request)
    {
        var command = new RateMovieCommand 
        { 
            MovieId = id, 
            Rating = request.Rating 
        };
        var result = await _mediator.Send(command);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    [HttpPut("{id}/notes")]
    public async Task<IActionResult> AddNotes(Guid id, [FromBody] AddNotesRequest request)
    {
        var command = new AddMovieNotesCommand 
        { 
            MovieId = id, 
            Notes = request.Notes 
        };
        var result = await _mediator.Send(command);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }
    
    [HttpDelete("{movieId}")]
    public async Task<IActionResult> DeleteMovie(Guid movieId)
    {
        var command = new DeleteMovieCommand { MovieId = movieId };
        var result = await _mediator.Send(command);
    
        if (!result.Success)
            return BadRequest(result);
    
        return Ok(result);
    }
}

public record MarkAsWatchedRequest(bool IsWatched);
public record RateMovieRequest(decimal Rating);
public record AddNotesRequest(string? Notes);