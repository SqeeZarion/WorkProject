using Grpc.Net.Client;
using Spotify.Newrelease;
using SpotifyReleasesService = Spotify.Newrelease.NewReleasesService;
namespace WorkProject.GrpcClient.Albums;

// (репозиторій приймає запити ззовні й зєднює з сервісом)
// Клієнт здійснює віддалені виклики до сервера, передаючи потрібні дані і отримуючи відповіді,
// які потім можуть використовуватись у клієнтській програмі.
public class NewReleasesGrpcClient
{
    private readonly SpotifyReleasesService.NewReleasesServiceClient _client;

    public NewReleasesGrpcClient(string grpcServiceUrl)
    {
        // Створення каналу для з'єднання з gRPC сервісом
        var channel = GrpcChannel.ForAddress(grpcServiceUrl);
        _client = new SpotifyReleasesService.NewReleasesServiceClient(channel);
    }

    public async Task<AlbumsResponse> GetNewReleasesAsync(string countryCode, int limit, int offset)
    {
        var request = new GetNewReleasesRequest
        {
            CountryCode = countryCode,
            Limit = limit,
            Offset = offset
        };

        return await _client.GetNewReleasesAsync(request);
    }
}