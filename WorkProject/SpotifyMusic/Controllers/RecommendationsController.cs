using Microsoft.AspNetCore.Mvc;
using WorkProject.GrpcClient.Recommendations;

namespace WorkProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecommendationsController : ControllerBase
{
    private readonly RecommendationsGrpcClient _grpcClient;

    public RecommendationsController(RecommendationsGrpcClient grpcClient)
    {
        _grpcClient = grpcClient;
    }
    
    [HttpGet("top-artists-with-albums")]
    public async Task<IActionResult> GetTopArtistsWithAlbums(CancellationToken cancellationToken)
    {
        try
        {
            var recommendations = await _grpcClient.GetTopArtistsWithAlbumsAsync(cancellationToken);
            
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
            var recommendations = await _grpcClient.GetSavedAlbumsOnlyAsync(cancellationToken);
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
            var recommendations = await _grpcClient.GetTrackRecommendationsAsync(cancellationToken);
            
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
            var recommendations = await _grpcClient.GetSavedPlaylistsOnlyAsync(cancellationToken);

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