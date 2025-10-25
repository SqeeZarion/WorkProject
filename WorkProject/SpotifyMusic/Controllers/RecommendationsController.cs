using Microsoft.AspNetCore.Mvc;
using Spotify.Recommendations;


namespace WorkProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecommendationsController : ControllerBase
{
    private readonly SpotifyRecommendationsService.SpotifyRecommendationsServiceClient _serviceClient;

    public RecommendationsController(SpotifyRecommendationsService.SpotifyRecommendationsServiceClient serviceClient)
    {
        _serviceClient = serviceClient;
    }

    [HttpGet("top-artists-with-albums")]
    public async Task<IActionResult> GetTopArtistsWithAlbums(CancellationToken cancellationToken)
    {
        try
        {
            var recommendations = await _serviceClient.GetTopArtistsWithAlbumsAsync(new GetUserRecommendationsRequest(),
                cancellationToken: cancellationToken);

            if (recommendations == null)
                return NotFound("Recommendations for albums not found");

            return Ok(recommendations);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error receiving album recommendations: {ex.Message}");
        }
    }

    [HttpGet("saved-albums")]
    public async Task<IActionResult> GetSavedAlbums(CancellationToken cancellationToken)
    {
        try
        {
            var recommendations = await _serviceClient.GetSavedAlbumsOnlyAsync(new GetUserRecommendationsRequest(),
                cancellationToken: cancellationToken);
            if (recommendations == null)
                return NotFound("No saved albums found");

            return Ok(recommendations);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error when receiving saved albums: {ex.Message}");
        }
    }

    [HttpGet("tracks")]
    public async Task<IActionResult> GetTrackRecommendations(CancellationToken cancellationToken)
    {
        try
        {
            var recommendations = await _serviceClient.GetTrackRecommendationsAsync(new GetUserRecommendationsRequest(),
                cancellationToken: cancellationToken);

            if (recommendations == null)
                return NotFound("No recommendations found for tracks");


            return Ok(recommendations);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error receiving track recommendations: {ex.Message}");
        }
    }

    [HttpGet("saved-playlists")]
    public async Task<IActionResult> GetSavedPlaylist(CancellationToken cancellationToken)
    {
        try
        {
            var recommendations = await _serviceClient.GetSavedPlaylistsOnlyAsync(new GetUserRecommendationsRequest(),
                cancellationToken: cancellationToken);

            if (recommendations == null)
                return NotFound("No saved playlists found");

            return Ok(recommendations);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error receiving saved playlists: {ex.Message}");
        }
    }
}