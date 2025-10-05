using Grpc.Net.Client;
using Spotify.Recommendations;

namespace WorkProject.GrpcClient.Recommendations
{
    public class RecommendationsGrpcClient
    {
        private readonly SpotifyRecommendationsService.SpotifyRecommendationsServiceClient _client;

        public RecommendationsGrpcClient(SpotifyRecommendationsService.SpotifyRecommendationsServiceClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Отримати топ артистів з альбомами.
        /// </summary>
        public async Task<RecommendationsResponse> GetTopArtistsWithAlbumsAsync(CancellationToken cancellationToken)
        {
            return await _client.GetTopArtistsWithAlbumsAsync(new GetUserRecommendationsRequest(), cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Отримати лише збережені альбоми користувача.
        /// </summary>
        public async Task<RecommendationsResponse> GetSavedAlbumsOnlyAsync(CancellationToken cancellationToken)
        {
            return await _client.GetSavedAlbumsOnlyAsync(new GetUserRecommendationsRequest(), cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Отримати рекомендації по треках (топ та збережені).
        /// </summary>
        public async Task<RecommendationsResponse> GetTrackRecommendationsAsync(CancellationToken cancellationToken)
        {
            return await _client.GetTrackRecommendationsAsync(new GetUserRecommendationsRequest(), cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Отримати лише збережені плейлісти користувача.
        /// </summary>
        public async Task<RecommendationsResponse> GetSavedPlaylistsOnlyAsync(CancellationToken cancellationToken)
        {
            return await _client.GetSavedPlaylistsOnlyAsync(new GetUserRecommendationsRequest(), cancellationToken: cancellationToken);
        }
    }
}