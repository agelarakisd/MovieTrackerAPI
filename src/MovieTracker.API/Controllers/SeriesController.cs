using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTracker.Application.Series.Commands.AddSeries;
using MovieTracker.Application.Series.Commands.AddSeriesNotes;
using MovieTracker.Application.Series.Commands.DeleteSeries;
using MovieTracker.Application.Series.Commands.MarkEpisodeWatched;
using MovieTracker.Application.Series.Commands.RateSeries;
using MovieTracker.Application.Series.Queries.GetMySeries;
using MovieTracker.Application.Series.Queries.GetSeriesEpisodes;

namespace MovieTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SeriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SeriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> AddSeries([FromBody] AddSeriesCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetMySeries()
    {
        var query = new GetMySeriesQuery();
        var result = await _mediator.Send(query);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    [HttpGet("{seriesId}/episodes")]
    public async Task<IActionResult> GetSeriesEpisodes(Guid seriesId)
    {
        var query = new GetSeriesEpisodesQuery { SeriesId = seriesId };
        var result = await _mediator.Send(query);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    [HttpPut("episodes/{episodeId}/watched")]
    public async Task<IActionResult> MarkEpisodeWatched(Guid episodeId, [FromBody] MarkEpisodeWatchedRequest request)
    {
        var command = new MarkEpisodeWatchedCommand
        {
            EpisodeId = episodeId,
            IsWatched = request.IsWatched
        };
        var result = await _mediator.Send(command);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    [HttpPut("{seriesId}/rating")]
    public async Task<IActionResult> RateSeries(Guid seriesId, [FromBody] RateSeriesRequest request)
    {
        var command = new RateSeriesCommand
        {
            SeriesId = seriesId,
            Rating = request.Rating
        };
        var result = await _mediator.Send(command);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    [HttpPut("{seriesId}/notes")]
    public async Task<IActionResult> AddSeriesNotes(Guid seriesId, [FromBody] AddSeriesNotesRequest request)
    {
        var command = new AddSeriesNotesCommand
        {
            SeriesId = seriesId,
            Notes = request.Notes
        };
        var result = await _mediator.Send(command);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }
    
    [HttpDelete("{seriesId}")]
    public async Task<IActionResult> DeleteSeries(Guid seriesId)
    {
        var command = new DeleteSeriesCommand { SeriesId = seriesId };
        var result = await _mediator.Send(command);
    
        if (!result.Success)
            return BadRequest(result);
    
        return Ok(result);
    }
}

public record MarkEpisodeWatchedRequest(bool IsWatched);
public record RateSeriesRequest(decimal Rating);
public record AddSeriesNotesRequest(string? Notes);