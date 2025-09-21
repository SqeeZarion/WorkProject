using Grpc.Core;
using Newtonsoft.Json;
using Spotify.Recommendations;
using WorkProject.Models.External;
using GrpcImage = Spotify.Recommendations.Image;
using Track = Spotify.Recommendations.Track;
using RecentlyPlayed = Spotify.Recommendations.RecentlyPlayed;


namespace WorkProject.GrpcService.Recommendations;

public class RecommendationsGrpcService : SpotifyRecommendationsService.SpotifyRecommendationsServiceBase
{
    private readonly HttpClient _httpClient;
    private const int Limit = 3;

    public RecommendationsGrpcService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public override async Task<RecommendationsResponse> GetTopArtistsWithAlbums(GetUserRecommendationsRequest request,
        ServerCallContext context)
    {
        var response = new RecommendationsResponse();

        try
        {
            // Отримуємо топ артистів
            var topArtists = await FetchGetTopArtists();
            if (topArtists?.items?.Any() == true)
            {
                // Отримуємо альбоми артиста
                foreach (var artist in topArtists.items)
                {
                    var artistAlbums = await FetchGetArtistAlbums(artist.Id);
                    if (artistAlbums?.Albums?.Items?.Any() == true)
                    {
                        response.SavedAlbums.AddRange(artistAlbums.Albums.Items.Select(a => new Album
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Artists = { a.Artists.Select(ar => ar.Name) },
                            ReleaseDate = a.release_date,
                            Popularity = a.Popularity,
                            TotalTracks = a.TotalTracks,
                            AlbumType = a.album_type,
                            SpotifyUrl = a.ExternalUrls?.Spotify ?? "",
                            ImageUrl = a.Images?.FirstOrDefault()?.Url ?? "",
                            Images =
                            {
                                a.Images != null
                                    ? a.Images.Select(img => new GrpcImage
                                    {
                                        Height = img.Height,
                                        Url = img.Url,
                                        Width = img.Width
                                    })
                                    : Enumerable.Empty<GrpcImage>()
                            }
                        }));
                    }

                    response.TopArtists.Add(new Artist
                    {
                        Id = artist.Id,
                        Name = artist.Name,
                        Genres = { artist.Genres ?? Array.Empty<string>() },
                        Popularity = artist.Popularity
                    });
                }
            }
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal,
                $"Failed to fetch top artists with albums: {ex.Message}"));
        }

        return response;
    }

    public override async Task<RecommendationsResponse> GetSavedAlbumsOnly(GetUserRecommendationsRequest request,
        ServerCallContext context)
    {
        var response = new RecommendationsResponse();

        try
        {
            // Отримуємо збережені альбоми 
            var savedAlbums = await FetchGetSavedAlbums();
            if (savedAlbums?.Albums?.Items?.Any() == true)
            {
                response.SavedAlbums.AddRange(savedAlbums.Albums.Items.Select(a => new Album
                {
                    Id = a.Id,
                    Name = a.Name,
                    Artists = { a.Artists.Select(ar => ar.Name) },
                    ReleaseDate = a.release_date,
                    Popularity = a.Popularity,
                    TotalTracks = a.TotalTracks,
                    AlbumType = a.album_type,
                    SpotifyUrl = a.ExternalUrls?.Spotify ?? "",
                    ImageUrl = a.Images?.FirstOrDefault()?.Url ?? "",
                    Images =
                    {
                        a.Images != null
                            ? a.Images.Select(img => new GrpcImage
                            {
                                Height = img.Height,
                                Url = img.Url,
                                Width = img.Width
                            })
                            : Enumerable.Empty<GrpcImage>()
                    }
                }));
            }
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, $"Failed to fetch saved albums: {ex.Message}"));
        }

        return response;
    }

    public override async Task<RecommendationsResponse> GetTrackRecommendations(GetUserRecommendationsRequest request,
        ServerCallContext context)
    {
        var response = new RecommendationsResponse();

        try
        {
            // Отримуємо топ треків
            var topTracks = await FetchGetTopTracks();
            if (topTracks?.items?.Any() == true)
            {
                response.TopTracks.AddRange(topTracks.items.Select(t => new Track
                {
                    Id = t.Id,
                    Name = t.Name,
                    Artists = { t.Artists.Select(a => a.name) },
                    AlbumId = t.Album?.id ?? "",
                    ReleaseDate = t.Album.release_date,
                    Popularity = t.Popularity
                }));
            }

            // Отримуємо збережені треки
            var savedTracks = await FetchGetSavedTracks();
            if (savedTracks?.items?.Any() == true)
            {
                response.SavedTracks.AddRange(savedTracks.items.Select(t => new Track
                {
                    Id = t.track.Id,
                    Name = t.track.Name,
                    Artists = { t.track.Artists.Select(a => a.name) },
                    AlbumId = t.track.Album.id,
                    ReleaseDate = t.track.Album.release_date,
                    Popularity = t.track.Popularity
                }));
            }

            var recentlyPlayed = await FetchGetRecentlyPlayed();
            // Отримуємо нещодавно відтворені треки
            response.RecentlyPlayed.AddRange(recentlyPlayed.items.Select(rp => new RecentlyPlayed
            {
                TrackId = rp.track.Id,
                TrackName = rp.track.Name,
                PlayedAt = rp.played_at,
                ArtistName = rp.track.Artists.FirstOrDefault()?.name ?? "",
                AlbumName = rp.track.Album?.name ?? "",
                ImageUrl = rp.track.Album?.images?.FirstOrDefault()?.url ?? ""
            }));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal,
                $"Failed to fetch track recommendations: {ex.Message}"));
        }

        return response;
    }

    public override async Task<RecommendationsResponse> GetSavedPlaylistsOnly(GetUserRecommendationsRequest request,
        ServerCallContext context)
    {
        var response = new RecommendationsResponse();
        try
        {
            var playlists = await FetchGetPlaylist();
            if (playlists?.items.Any() == true)
            {
                response.SavedPlaylist.AddRange(playlists.items.Select(p => new Playlist
                {
                    Id = Guid.NewGuid().ToString(),
                    SpotifyId = p.id,
                    Name = p.name,
                    Description = p.description,
                    OwnerName = p.owner?.display_name ?? p.owner?.id ?? string.Empty,
                    IsPublic = p.@public,
                    ImageUrl = p.images?.FirstOrDefault()?.url ?? string.Empty,
                    SpotifyUrl = p.external_urls?.spotify ?? string.Empty,
                    TotalTracks = p.tracks?.total ?? 0,
                    CreatedAt = DateTime.UtcNow.ToString("o")
                }));
            }
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal,
                $"Failed to fetch track recommendations: {ex.Message}"));
        }

        return response;
    }

    private async Task<SpotifyArtistsResponse?> FetchGetTopArtists()
    {
        var response = await _httpClient.GetAsync($"me/top/artists?limit={Limit}");
        if (!response.IsSuccessStatusCode)
            throw new Exception("Spotify API returned error when fetching Get Top Artists");

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<SpotifyArtistsResponse>(content);
    }

    private async Task<SpotifyTracksResponse?> FetchGetTopTracks()
    {
        var response = await _httpClient.GetAsync($"me/top/tracks?limit={Limit}");
        if (!response.IsSuccessStatusCode)
            throw new Exception("Spotify API returned error when fetching Get Top Tracks");

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<SpotifyTracksResponse>(content);
    }

    private async Task<SpotifySavedTracksResponse?> FetchGetSavedTracks()
    {
        var response = await _httpClient.GetAsync($"me/tracks?limit={Limit}");
        if (!response.IsSuccessStatusCode)
            throw new Exception("Spotify API returned error when fetching Get Saved Tracks");

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<SpotifySavedTracksResponse>(content);
    }

    private async Task<SpotifyAlbumsResponse?> FetchGetSavedAlbums()
    {
        var response = await _httpClient.GetAsync($"me/albums?limit={Limit}");
        if (!response.IsSuccessStatusCode)
            throw new Exception("Spotify API returned error when fetching Get Saved Albums");

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<SpotifyAlbumsResponse>(content);
    }

    private async Task<SpotifyRecentlyPlayedResponse?> FetchGetRecentlyPlayed()
    {
        var response = await _httpClient.GetAsync($"me/player/recently-played?limit={Limit}");
        if (!response.IsSuccessStatusCode)
            throw new Exception("Spotify API returned error when fetching Recently Played");

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<SpotifyRecentlyPlayedResponse>(content);
    }

    private async Task<SpotifyAlbumsResponse?> FetchGetArtistAlbums(string artistId)
    {
        var response =
            await _httpClient.GetAsync($"/artists/{artistId}/albums?limit={Limit}&include_groups=album,single");
        if (!response.IsSuccessStatusCode) throw new Exception("Spotify API returned error when fetching playlists");

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<SpotifyAlbumsResponse>(content);
    }

    private async Task<SpotifyPlaylistsResponse?> FetchGetPlaylist()
    {
        var response =
            await _httpClient.GetAsync("me/playlists?limit=20");
        if (!response.IsSuccessStatusCode) throw new Exception("Spotify API returned error when Get Playlist");

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<SpotifyPlaylistsResponse>(content);
    }
}