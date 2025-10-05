using Grpc.Core;
using Newtonsoft.Json;
using Spotify.ToDoAlbum;
using SpotifyWebApi.Models;
using WorkProject.Models.External;
using GrpcAlbum = Spotify.ToDoAlbum.Album;
using GrpcImage = Spotify.ToDoAlbum.Image;

namespace WorkProject.GrpcService.Albums;

public class ToDoAlbumGrpcService : ToDoAlbumService.ToDoAlbumServiceBase
{
    private readonly HttpClient _httpClient;

    public ToDoAlbumGrpcService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new AggregateException();
    }

    // Приватний метод для трансформації даних (Spotify API -> gRPC формат)
    private AlbumsResponse MapSpotifyAlbumsToResponse(SpotifyAlbumsResponse spotifyResponse)
    {
        try
        {
            var albumsResponse = new AlbumsResponse();

            foreach (var item in spotifyResponse.Albums.Items)
            {
                var album = new GrpcAlbum
                {
                    Id = item.Id,
                    Name = item.Name,
                    AlbumType = item.Type,
                    TotalTracks = item.TotalTracks,
                    Artists = { item.Artists.Select(ar => ar.Name) },
                    ReleaseDate = item.release_date,
                    ImageUrl = item.Images.FirstOrDefault()?.Url,
                    SpotifyUrl = item.ExternalUrls.Spotify
                };

                album.Images.AddRange(item.Images.Select(i => new GrpcImage
                {
                    Height = i.Height,
                    Url = i.Url,
                    Width = i.Width
                }));

                albumsResponse.Albums.Add(album);
            }

            return albumsResponse;
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal,
                $"Failed to fetch track recommendations: {ex.Message}"));
        }
    }

    // Отримати альбом за ID

    public override async Task<AlbumsResponse> GetAlbum(GetAlbumRequest request, ServerCallContext context)
    {
        string spotifyUrl = $"v1/albums?id={request.AlbumId}";

        var response = await _httpClient.GetAsync(spotifyUrl, context.CancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to fetch data from Spotify API.");

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var spotifyAlbumsResponse = JsonConvert.DeserializeObject<SpotifyAlbumsResponse>(jsonResponse);

        return MapSpotifyAlbumsToResponse(spotifyAlbumsResponse);
    }

    
    // Улюблені альбоми користувача

    public override async Task<AlbumsResponse> GetFavoriteAlbums(GetFavoriteAlbumsRequest request,
        ServerCallContext context)
    {
        string spotifyUrl = "v1/me/albums";

        var response = await _httpClient.GetAsync(spotifyUrl, context.CancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to fetch favorite albums from Spotify API.");

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var spotifyAlbumsResponse = JsonConvert.DeserializeObject<SpotifyAlbumsResponse>(jsonResponse);

        return MapSpotifyAlbumsToResponse(spotifyAlbumsResponse);
    }

    // Альбоми конкретного виконавця

    public override async Task<AlbumsResponse> GetArtistAlbums(GetArtistAlbumsRequest request,
        ServerCallContext context)
    {
        string spotifyUrl = $"v1/artists/{request.ArtistId}/albums?include_groups=album";

        var response = await _httpClient.GetAsync(spotifyUrl, context.CancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to fetch artist albums from Spotify API.");

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var spotifyAlbumsResponse = JsonConvert.DeserializeObject<SpotifyAlbumsResponse>(jsonResponse);

        return MapSpotifyAlbumsToResponse(spotifyAlbumsResponse);
    }

    // Пошук альбомів

    public override async Task<AlbumsResponse> SearchAlbums(SearchAlbumsRequest request, ServerCallContext context)
    {
        string spotifyUrl = $"v1/search?q={Uri.EscapeDataString(request.Query)}&type=album";

        var response = await _httpClient.GetAsync(spotifyUrl, context.CancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to fetch albums from Spotify API.");

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var spotifySearchResponse = JsonConvert.DeserializeObject<SpotifyAlbumsResponse>(jsonResponse);

        return MapSpotifyAlbumsToResponse(spotifySearchResponse);
    }
}