using Microsoft.AspNetCore.Mvc;
using WorkProject.GrpcClient.Albums;

namespace WorkProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ToDoAlbumController : ControllerBase
{
    private readonly ToDoAlbumGrpcClient _grpcClient;

    public ToDoAlbumController(ToDoAlbumGrpcClient grpcClient)
    {
        _grpcClient = grpcClient;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAlbumById(string id, CancellationToken cancellationToken)
    {
        var album = await _grpcClient.GetAlbumByIdAsync(id, cancellationToken);
        return Ok(album.Albums.FirstOrDefault());
    }
    
    [HttpGet("favorites")]
    public async Task<IActionResult> GetFavoriteAlbums()
    {
        var albums = await _grpcClient.GetFavoriteAlbumsAsync();
        return Ok(albums.Albums);
    }
    
    [HttpGet("artist/{artistId}")]
    public async Task<IActionResult> GetArtistAlbums(string artistId, CancellationToken cancellationToken)
    {
        var albums = await _grpcClient.GetArtistAlbumsAsync(artistId, cancellationToken);
        return Ok(albums.Albums);
    }
    
    [HttpGet("search")]
    public async Task<IActionResult> SearchAlbums([FromQuery] string q, CancellationToken cancellationToken)
    {
        // якщо користувач не передав значення або ввів лише пробіли — повертаємо 400.
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest("Search query cannot be empty");

        var albums = await _grpcClient.SearchAlbumsAsync(q, cancellationToken);
        return Ok(albums.Albums);
    }
}