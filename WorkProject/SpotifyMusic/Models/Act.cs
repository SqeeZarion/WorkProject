using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpotifyWebApi.Models;

public class Act
{
    [Key]
    public int ActId { get; set; }
    public string ActName { get; set; }
    public string ActDescr { get; set; }
    public byte[] ActImage { get; set; }
}

public class ActRecording
{
    [Key]
    public int ActrecordingId { get; set; }
    
    [ForeignKey("Recording")]
    public int RecordingId { get; set; }
    
    [ForeignKey("Act")]
    public int ActId { get; set; }
}

public class Tblactrecording
{
    public int Actrecordingid { get; set; }
}

public class Recording
{
    [Key]
    public int RecordingId { get; set; }
    public string RecordingName { get; set; }
    public double Duration { get; set; }
    
    [ForeignKey("Song")]   
    public int SongId { get; set; }
    
    [ForeignKey("Genre")]
    public int GenreId { get; set; }
}

public class Genre
{
    [Key]
    public int GenreId { get; set; }
    public string GenreName { get; set; }
    public string GenreDescr { get; set; }
}
