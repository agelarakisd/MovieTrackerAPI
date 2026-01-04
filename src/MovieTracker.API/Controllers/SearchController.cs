using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTracker.Application.Search.Queries.SearchMovies;
using MovieTracker.Application.Search.Queries.SearchSeries;

namespace MovieTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SearchController : ControllerBase
{
    private readonly IMediator _mediator;

    public SearchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("movies")]
    public async Task<IActionResult> SearchMovies([FromQuery] string query)
    {
        var command = new SearchMoviesQuery { Query = query };
        var result = await _mediator.Send(command);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    [HttpGet("series")]
    public async Task<IActionResult> SearchSeries([FromQuery] string query)
    {
        var command = new SearchSeriesQuery { Query = query };
        var result = await _mediator.Send(command);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }
}