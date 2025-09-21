using Grpc.Net.Client;
using Spotify.ToDoAlbum;
using SpotifyToDoAlbumService = Spotify.ToDoAlbum.ToDoAlbumService;

namespace WorkProject.GrpcClient.Albums;

// (репозиторій приймає запити ззовні й зєднює з сервісом)
// Клієнт здійснює віддалені виклики до сервера, передаючи потрібні дані і отримуючи відповіді,
// які потім можуть використовуватись у клієнтській програмі.
public class ToDoAlbumGrpcClient
{
    private readonly SpotifyToDoAlbumService.ToDoAlbumServiceClient _client;

    public ToDoAlbumGrpcClient(string grpcServiceUrl)
    {
        // Створення каналу для з'єднання з gRPC сервісом
        var channel = GrpcChannel.ForAddress(grpcServiceUrl);
        _client = new SpotifyToDoAlbumService.ToDoAlbumServiceClient(channel);
    }
    
    public async Task<AlbumsResponse> GetAlbumByIdAsync(string albumId)
    {
        var request = new GetAlbumRequest
        {
            AlbumId = albumId
        };

        return await _client.GetAlbumAsync(request);
    }
    
    public async Task<AlbumsResponse> GetFavoriteAlbumsAsync()
    {
        var request = new GetFavoriteAlbumsRequest();
        return await _client.GetFavoriteAlbumsAsync(request);
    }
    
    public async Task<AlbumsResponse> GetArtistAlbumsAsync(string artistId)
    {
        var request = new GetArtistAlbumsRequest
        {
            ArtistId = artistId
        };
        return await _client.GetArtistAlbumsAsync(request);
    }
    
    public async Task<AlbumsResponse> SearchAlbumsAsync(string query)
    {
        var request = new SearchAlbumsRequest
        {
            Query = query
        };
        return await _client.SearchAlbumsAsync(request);
    }
}