using WorkProject.Models.Entities;
using WorkProject.Models.External;


namespace WorkProject.Mappers;

public static class UserMappers
{
    public static void ApplySpotifyProfile(this UserEnity e, SpotifyUserResponse s)
    {
        e.SpotifyUserId = s.Id ?? throw new InvalidOperationException("Spotify id is missing.");
        e.DisplayName = s.DisplayName ?? e.DisplayName;
        e.Email = s.Email ?? e.Email;
        e.Country = s.Country ?? e.Country;
        e.Product = s.Product ?? e.Product;
        e.Href = s.Href ?? e.Href;
        e.Uri = s.Uri ?? e.Uri;
        e.SpotifyProfileUrl = s.ExternalUrls.Spotify ?? e.SpotifyProfileUrl;
        e.ImageUrl = s.Images?.FirstOrDefault()?.Url ?? e.ImageUrl;
    }
}