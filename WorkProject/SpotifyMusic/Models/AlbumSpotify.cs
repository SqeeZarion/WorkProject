using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WorkProject.Models.External;

namespace SpotifyWebApi.Models;


public class Items
{
    public string album_type { get; set; }
    public int total_tracks { get; set; }
    public string[] available_markets { get; set; }
    public External_urls external_urls { get; set; }
    public string href { get; set; }
    public int id { get; set; }
    public List<Images> images { get; set; }
    public string name { get; set; }
    public string release_date { get; set; }
    public string release_date_precision { get; set; }
    public Restrictions restrictions { get; set; }
    public string type { get; set; }
    public string uri { get; set; }
    public Copyrights[] copyrights { get; set; }
    public External_ids external_ids { get; set; }
    public string[] genres { get; set; }
    public string label { get; set; }
    public int popularity { get; set; }
    public string album_group { get; set; }
    public Artists[] artists { get; set; }
    public List<SpotifyTrack> items { get; set; }
    public SpotifyTrack[] tracks { get; set; }
}

public class External_urls
{
    public string spotify { get; set; }
}

public class Images
{
    public int Id { get; set; }
    public string url { get; set; }
    public int height { get; set; }
    public int width { get; set; }
}

public class Restrictions
{
    public string reason { get; set; }
}

public class Copyrights
{
    public string text { get; set; }
    public string type { get; set; }
}

public class External_ids
{
    public string isrc { get; set; }
    public string ean { get; set; }
    public string upc { get; set; }
}

public class Artists
{
    public External_urls1 external_urls { get; set; }
    public string href { get; set; }
    public string id { get; set; }
    public string name { get; set; }
    public string type { get; set; }
    public string uri { get; set; }
}

public class External_urls1
{
    public string spotify { get; set; }
}

