using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SpotifyApi.NetCore;

namespace SpotifyWebApi.Models;

public class Artist
{
    public ExternalUrls external_urls { get; set; }
    public string href { get; set; }
    public string id { get; set; }
    public string name { get; set; }
    public string type { get; set; }
    public string uri { get; set; }
}

public class ArtistAct //Виступи
{
    [Key]
    public int ArtistActId { get; set; }
    public DateTime Begindate { get; set; }
    public DateTime Enddate { get; set; }
    
    [ForeignKey("Artist")]
    public int ArtistId { get; set; }
    
    [ForeignKey("Act")]
    public int ActId { get; set; }
}

