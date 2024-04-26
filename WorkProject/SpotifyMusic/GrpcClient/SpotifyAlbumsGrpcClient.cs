using Grpc.Net.Client;
using Spotify;

using SpotifyAlbumsService = Spotify.SpotifyAlbumsService;

namespace WorkProject.GrpcClient;

public class SpotifyAlbumsGrpcClient
{
    private readonly SpotifyAlbumsService.SpotifyAlbumsServiceClient _client;

    public SpotifyAlbumsGrpcClient(string grpcServiceUrl)
    {
        // Створення каналу для з'єднання з gRPC сервісом
        var chanell = GrpcChannel.ForAddress(grpcServiceUrl);
        _client = new SpotifyAlbumsService.SpotifyAlbumsServiceClient(chanell);
    }

    public async Task<AlbumsResponse> GetNewReleasesAsync(string countryCode, int limit)
    {
        var request = new GetNewReleasesRequest
        {
            CountryCode = countryCode,
            Limit = limit
        };

        return await _client.GetNewReleasesAsync(request);
    }
}