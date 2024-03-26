using System.ComponentModel.DataAnnotations;

namespace SpotifyWebApi.Models;

public class SubscriptionType
{
    [Key]
    public int SubscriptionTypeId { get; set; }
    public string SubscriptionTypeName { get; set; }
    public string SubscriptionTypeDescr { get; set; }
}