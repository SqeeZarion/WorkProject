using Grpc.Net.Client;
using Spotify.Newrelease;
using SpotifyReleasesService = Spotify.Newrelease.NewReleasesService;
namespace WorkProject.GrpcClient.Albums;

// Клієнт gRPC — здійснює віддалені виклики до Spotify gRPC сервісу

public class NewReleasesGrpcClient
{
    private readonly SpotifyReleasesService.NewReleasesServiceClient _client;

    public NewReleasesGrpcClient(string grpcServiceUrl)
    {
        // Створення каналу для з'єднання з gRPC сервісом
        var channel = GrpcChannel.ForAddress(grpcServiceUrl);
        _client = new SpotifyReleasesService.NewReleasesServiceClient(channel);
    }

    public async Task<AlbumsResponse> GetNewReleasesAsync(string countryCode, int limit, int offset, CancellationToken cancellationToken)
    {
        var request = new GetNewReleasesRequest
        {
            CountryCode = countryCode,
            Limit = limit,
            Offset = offset
        };

        return await _client.GetNewReleasesAsync(request, cancellationToken: cancellationToken);
    }
}