using Microsoft.AspNetCore.Mvc;
using Spotify.ToDoAlbum;

namespace WorkProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ToDoAlbumController : ControllerBase
{
    private readonly ToDoAlbumService.ToDoAlbumServiceClient _grpcClient;

    public ToDoAlbumController(ToDoAlbumService.ToDoAlbumServiceClient grpcClient)
    {
        _grpcClient = grpcClient;
    }

    // 🔹 Отримати альбом за ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAlbumById(string id, CancellationToken cancellationToken)
    {
        var request = new GetAlbumRequest
        {
            AlbumId = id
        };

        var response = await _grpcClient.GetAlbumAsync(request, cancellationToken: cancellationToken);
        return Ok(response.Albums.FirstOrDefault());
    }

    // 🔹 Отримати улюблені альбоми
    [HttpGet("favorites")]
    public async Task<IActionResult> GetFavoriteAlbums(CancellationToken cancellationToken)
    {
        var request = new GetFavoriteAlbumsRequest();

        var response = await _grpcClient.GetFavoriteAlbumsAsync(request, cancellationToken: cancellationToken);
        return Ok(response.Albums);
    }

    // 🔹 Отримати альбоми артиста
    [HttpGet("artist/{artistId}")]
    public async Task<IActionResult> GetArtistAlbums(string artistId, CancellationToken cancellationToken)
    {
        var request = new GetArtistAlbumsRequest
        {
            ArtistId = artistId
        };

        var response = await _grpcClient.GetArtistAlbumsAsync(request, cancellationToken: cancellationToken);
        return Ok(response.Albums);
    }

    // 🔹 Пошук альбомів
    [HttpGet("search")]
    public async Task<IActionResult> SearchAlbums([FromQuery] string q, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest("Search query cannot be empty");

        var request = new SearchAlbumsRequest
        {
            Query = q
        };

        var response = await _grpcClient.SearchAlbumsAsync(request, cancellationToken: cancellationToken);
        return Ok(response.Albums);
    }
}
