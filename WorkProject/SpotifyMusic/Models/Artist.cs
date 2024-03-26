using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpotifyWebApi.Models;

public class Artist
{
    [Key]
    public int ArtistId { get; set; }
    public string ArtistFname { get; set; }
    public string ArtistLname { get; set; }
    public string ArtistDescr { get; set; } // TODO: biographiya
    public DateTime ArtistBirthDate { get; set; }
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

