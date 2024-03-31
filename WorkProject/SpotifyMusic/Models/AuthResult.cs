using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace SpotifyWebApi.Models;

public class AuthResult
{
    public string access_token { get; set; }
    public string token_type { get; set; }
    public int expires_in { get; set; }
}

public class Access
{
    [Key]
    public int AccessId { get; set; }
    public DateTime AccessDate { get; set; }
    
    [ForeignKey("UserType")]
    public int UserTypeId { get; set; }
    
    public virtual ICollection<UserType> UserType { get; set; }
    public virtual ICollection<PlayListRecording> PlayListRecordings { get; set; }
}