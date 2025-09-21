namespace WorkProject.Models.External;

public class SpotifyUserResponse
{
    public string Id { get; set; }
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public string Country { get; set; }
    public string Product { get; set; }
    public string Href { get; set; }
    public string Uri { get; set; }
    public Dictionary<string, string> ExternalUrls { get; set; }
    public List<Image> Images { get; set; }
}


