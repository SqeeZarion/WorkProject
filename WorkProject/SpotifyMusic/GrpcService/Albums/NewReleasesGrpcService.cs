using Grpc.Core;
using Spotify;
using Newtonsoft.Json;
using SpotifyWebApi.Models;
using SpotifyAlbumsService = Spotify.NewReleasesService;

namespace WorkProject.GrpcService;

public class NewReleasesGrpcService : SpotifyAlbumsService.NewReleasesServiceBase
{
    private readonly HttpClient _httpClient;    
    private readonly AlbumsResponse _albumsResponse;

    public NewReleasesGrpcService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _albumsResponse = new AlbumsResponse();
    }

    public override async Task<AlbumsResponse> GetNewReleases(GetNewReleasesRequest request, ServerCallContext context)
    {
        // Перевірка наявності даних в базі даних
        // var albumsInDb = await _databaseService.FetchAlbumsFromDatabaseAsync(request.CountryCode, request.Limit);
        // if (albumsInDb != null && albumsInDb.Count > 0)
        // {
        //     return new AlbumsResponse { Albums = { albumsInDb } };
        // }

        //потім поміняй Додавання конфігурації HttpClient (у гпт глянь) також добав токен доступу глянь на зразлк і в гпт
        string spotifyApiUrl =
            $"https://api.spotify.com/v1/browse/new-releases?country={request.CountryCode}&limit={request.Limit}&offset={request.Offset}";
        
        var response = await _httpClient.GetAsync(spotifyApiUrl);
        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to fetch data from Spotify API.");

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var spotifyAlbumsResponse = JsonConvert.DeserializeObject<SpotifyAlbumsResponse>(jsonResponse);
        

        foreach (var item in spotifyAlbumsResponse.albums.Items)
        {
            var album = new Spotify.Album
            {
                AlbumId = item.id,
                AlbumName = item.name,
                AlbumType = item.type,
                TotalTracks = item.total_tracks,
                Artist = string.Join(",", item.artists.Select(a => a.name)),
                Date = item.release_date,
                ImageUrl = item.images.FirstOrDefault()?.url,
                LinqUrl = item.external_urls.Spotify,
                Images =
                {
                    item.images.Select(i => new Spotify.Image
                        {
                            Height = i.height,
                            Url = i.url, Width = i.width
                        })
                        .ToList()
                }
            };
            _albumsResponse.Albums.Add(album);
        }
        // Збереження даних в базі даних
        // await _databaseService.SaveAlbumsToDatabaseAsync(spotifyAlbums.Albums);

        // Повернення відповіді клієнту
        return _albumsResponse;
        //і розбирись з сервісом і клієнтом (вони працюють через rpc) з гпт щоб працювало
    }
}