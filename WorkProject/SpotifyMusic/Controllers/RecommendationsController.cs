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
    public async Task<IActionResult> GetTopArtistsWithAlbums()
    {
        try
        {
            var recommendations = await _grpcClient.GetTopArtistsWithAlbumsAsync();
            
            if (recommendations == null)
                return NotFound("Рекомендації по альбомах не знайдено");
            
            return Ok(recommendations);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Помилка при отриманні рекомендацій по альбомах: {ex.Message}");
        }
    }
    
    [HttpGet("saved-albums")]
    public async Task<IActionResult> GetSavedAlbums()
    {
        try
        {
            var recommendations = await _grpcClient.GetSavedAlbumsOnlyAsync();
            if (recommendations == null)
                return NotFound("Збережені альбоми не знайдено");

            return Ok(recommendations);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Помилка при отриманні збережених альбомів: {ex.Message}");
        }
    }

    [HttpGet("tracks")]
    public async Task<IActionResult> GetTrackRecommendations()
    {
        try
        {
            var recommendations = await _grpcClient.GetTrackRecommendationsAsync();
            
            if (recommendations == null)
                return NotFound("Рекомендації по треках не знайдено");
            
            
            return Ok(recommendations);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Помилка при отриманні рекомендацій по треках: {ex.Message}");
        }
    }
}