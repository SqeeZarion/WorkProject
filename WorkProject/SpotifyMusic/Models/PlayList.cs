using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpotifyWebApi.Models;

public class PlayList
{
    [Key]
    public int PlayListId { get; set; }
    public string PlayListName { get; set; }
    public string PlayListDescr { get; set; }
    public byte[] PlayListPicture { get; set; }
    public DateTime CreationDate { get; set; }
    
    [ForeignKey("PlayListType")]
    public int PlayListTypeId { get; set; }
    
    public virtual ICollection<PlayListRecording> PlayListRecording { get; set; }
    public virtual ICollection<PlayListType> PlayListType { get; set; }
    
}

public class PlayListType
{
    [Key]
    public int PlayListTypeId { get; set; }
    public string Playlisttypename { get; set; }
}

public class PlayListRecording
{
    [Key]
    public int PlayListRecordingId { get; set; }
    
    [ForeignKey("PlayList")]
    public int PlayListId { get; set; }
    
    [ForeignKey("Recording")]
    public int RecordingId { get; set; }
}
