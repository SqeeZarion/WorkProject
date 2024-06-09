using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using SpotifyWebApi.Interface;
using SpotifyWebApi.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SpotifyWebApi.Repositories;

// public class SpotifyAlbumsService : ISpotifyAlbumsService
// {
//     private readonly HttpClient _httpClient;
//     
//
//     public SpotifyAlbumsService(HttpClient httpClient)
//     {
//         _httpClient = httpClient;
//     }
//
//     public async Task<AlbumDetails> GetAlbumAsync(string id, string accessToken)
//     {
//         _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
//
//         var response = await _httpClient.GetAsync($"albums/{id}");
//
//         response.EnsureSuccessStatusCode();
//
//         //зчитуєм контент
//         using var responseStream = await response.Content.ReadAsStreamAsync();
//         var responseObject = await JsonSerializer.DeserializeAsync<SpotifyAlbumResponse>(responseStream);
//
//         var album = new AlbumDetails
//         {
//             Name = responseObject!.Name,
//             AlbumType = responseObject.AlbumType,
//             TotalTracks = responseObject.TotalTracks,
//             Date = responseObject.ReleaseDate,
//             ImageUrl = responseObject.Images.FirstOrDefault()?.url!,
//             LinqUrl = responseObject.ExternalUrls.spotify,
//             // Artist = string.Join(",", responseObject.artists.Select(a => a.name))
//         };
//
//         Console.WriteLine();
//         return album;
//     }
//
//     public async Task<Tracks> GetListTracksAsync(string albumId, string accessToken, int limit, int offset)
//     {
//         _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
//         var response = await _httpClient.GetAsync($"albums/{albumId}/tracks?limit={limit}&offset={offset}");
//             
//         response.EnsureSuccessStatusCode();
//             
//         using var responseStream = await response.Content.ReadAsStreamAsync();
//         var responseObject = await JsonSerializer.DeserializeAsync<Tracks>(responseStream);
//
//         return responseObject!;
//     }
//
//     // public async Task<Albums> GetTrackAsync(string albumId, string accessToken, int limit, int offset)
//     // {
//     // }
//
//     // public async Task<IEnumerable<Album>> GetReleaseAsync(string countryCode, int limit, string accessToken)
//     // {
//     //     _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
//     //
//     //     var response = await _httpClient.GetAsync($"browse/new-releases?country={countryCode}&limit={limit}");
//     //
//     //     response.EnsureSuccessStatusCode();
//     //
//     //     //зчитуєм контент
//     //     using var responseStream = await response.Content.ReadAsStreamAsync();
//     //     var responseObject = await JsonSerializer.DeserializeAsync<SpotifyAlbumsResponse>(responseStream);
//     //
//     //     return responseObject!.albums.Items.Select(i => new Album
//     //     {
//     //         AlbumName = i.name,
//     //         AlbumId = i.id,
//     //         ReleaseDate = i.release_date,
//     //         Images = i.images,
//     //         AlbumType = i.album_type,
//     //         TotalTracks = i.total_tracks,
//     //         Date = i.release_date,
//     //         ImageUrl = i.images.FirstOrDefault()!.url,
//     //         LinqUrl = i.external_urls.spotify,
//     //         Artist = string.Join(",", i.artists.Select(i => i.name))
//     //     });
//     // }
// }