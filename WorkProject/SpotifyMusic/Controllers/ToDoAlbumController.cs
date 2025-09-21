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
    public async Task<IActionResult> GetAlbumById(string id)
    {
        var album = await _grpcClient.GetAlbumByIdAsync(id);
        return Ok(album.Albums.FirstOrDefault());
    }
    
    [HttpGet("favorites")]
    public async Task<IActionResult> GetFavoriteAlbums()
    {
        var albums = await _grpcClient.GetFavoriteAlbumsAsync();
        return Ok(albums.Albums);
    }
    
    [HttpGet("artist/{artistId}")]
    public async Task<IActionResult> GetArtistAlbums(string artistId)
    {
        var albums = await _grpcClient.GetArtistAlbumsAsync(artistId);
        return Ok(albums.Albums);
    }
    
    [HttpGet("search")]
    public async Task<IActionResult> SearchAlbums([FromQuery] string q)
    {
        // якщо користувач не передав значення або ввів лише пробіли — повертаємо 400.
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest("Search query cannot be empty");

        var albums = await _grpcClient.SearchAlbumsAsync(q);
        return Ok(albums.Albums);
    }
}