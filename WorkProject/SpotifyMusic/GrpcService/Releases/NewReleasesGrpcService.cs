using Grpc.Core;
using Newtonsoft.Json;
using Spotify.Newrelease;
using SpotifyWebApi.Models;
using WorkProject.Models.External;

// Уникаємо конфлікту назв
using GrpcAlbum = Spotify.Newrelease.Album;
using GrpcImage = Spotify.Newrelease.Image;


namespace WorkProject.GrpcService.Albums;

//це серверна частина, яка визначає інтерфейси і реалізує бізнес-логіку.
//обробляє вхідні запити від клієнтів, виконує необхідну обробку даних
public class NewReleasesGrpcService : NewReleasesService.NewReleasesServiceBase
{
    private readonly HttpClient _httpClient;

    public NewReleasesGrpcService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public override async Task<AlbumsResponse> GetNewReleases(GetNewReleasesRequest request, ServerCallContext context)
    {
        var response = new AlbumsResponse();

        try
        {
            var spotifyAlbumsResponse = await FetchNewReleases(request, context.CancellationToken);

            if (spotifyAlbumsResponse?.Albums?.Items?.Any() == true)
            {
                response.Albums.AddRange(spotifyAlbumsResponse.Albums.Items.Select(a => new GrpcAlbum
                {
                    Id = a.Id,
                    Name = a.Name,
                    AlbumType = a.album_type,
                    TotalTracks = a.TotalTracks,
                    Artists = { a.Artists.Select(ar => ar.Name) },
                    ReleaseDate = a.release_date,
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
            throw new RpcException(new Status(StatusCode.Internal, $"Failed to fetch new releases: {ex.Message}"));
        }

        return response;
    }

    private async Task<SpotifyAlbumsResponse?> FetchNewReleases(GetNewReleasesRequest request, CancellationToken cancellationToken)
    {
        var spotifyApiUrl =
            $"browse/new-releases?country={request.CountryCode}&limit={request.Limit}&offset={request.Offset}";
        var spotifyResponse = await _httpClient.GetAsync(spotifyApiUrl, cancellationToken);

        if (!spotifyResponse.IsSuccessStatusCode)
            throw new Exception("Spotify API returned error when fetching new releases");

        var jsonResponse = await spotifyResponse.Content.ReadAsStringAsync(cancellationToken);
        return JsonConvert.DeserializeObject<SpotifyAlbumsResponse>(jsonResponse);
    }
}