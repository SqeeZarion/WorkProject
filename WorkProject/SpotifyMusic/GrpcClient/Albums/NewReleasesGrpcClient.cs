using Grpc.Net.Client;
using Spotify;

using NewReleasesService = Spotify.NewReleasesService;

namespace WorkProject.GrpcClient;

public class NewReleasesGrpcClient
{
    private readonly NewReleasesService.NewReleasesServiceClient _client;

    public NewReleasesGrpcClient(string grpcServiceUrl)
    {
        // Створення каналу для з'єднання з gRPC сервісом
        var channel = GrpcChannel.ForAddress(grpcServiceUrl);
        _client = new NewReleasesService.NewReleasesServiceClient(channel);
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