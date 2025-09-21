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
        public async Task<RecommendationsResponse> GetTopArtistsWithAlbumsAsync()
        {
            return await _client.GetTopArtistsWithAlbumsAsync(new GetUserRecommendationsRequest());
        }

        /// <summary>
        /// Отримати лише збережені альбоми користувача.
        /// </summary>
        public async Task<RecommendationsResponse> GetSavedAlbumsOnlyAsync()
        {
            return await _client.GetSavedAlbumsOnlyAsync(new GetUserRecommendationsRequest());
        }

        /// <summary>
        /// Отримати рекомендації по треках (топ та збережені).
        /// </summary>
        public async Task<RecommendationsResponse> GetTrackRecommendationsAsync()
        {
            return await _client.GetTrackRecommendationsAsync(new GetUserRecommendationsRequest());
        }

        /// <summary>
        /// Отримати лише збережені плейлісти користувача.
        /// </summary>
        public async Task<RecommendationsResponse> GetSavedPlaylistsOnlyAsync()
        {
            return await _client.GetSavedPlaylistsOnlyAsync(new GetUserRecommendationsRequest());
        }
    }
}