using Grpc.Core;
using System;
using System.Threading.Tasks;
using Spotify; 
using Google.Protobuf.WellKnownTypes;
using System.Net.Http;
using Newtonsoft.Json;
using SpotifyAlbumsService = Spotify.SpotifyAlbumsService;

namespace WorkProject.GrpcService;

public class SpotifyAlbumsGrpcService: SpotifyAlbumsService.SpotifyAlbumsServiceBase
{
    private readonly HttpClient _httpClient;

    public SpotifyAlbumsGrpcService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
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
        string spotifyApiUrl = $"https://api.spotify.com/v1/browse/new-releases?country={request.CountryCode}&limit={request.Limit}";
        
        var response = await _httpClient.GetAsync(spotifyApiUrl);
        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to fetch data from Spotify API.");
        
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var spotifyAlbums = JsonConvert.DeserializeObject<AlbumsResponse>(jsonResponse); // Припустимо, що відповідь вже відповідає формату AlbumsResponse
    
        // Збереження даних в базі даних
        // await _databaseService.SaveAlbumsToDatabaseAsync(spotifyAlbums.Albums);
    
        // Повернення відповіді клієнту
        return spotifyAlbums;
        //і розбирись з сервісом і клієнтом (вони працюють через rpc) з гпт щоб працювало
    }
}