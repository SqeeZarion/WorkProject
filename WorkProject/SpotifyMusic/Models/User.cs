using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace SpotifyWebApi.Models;

public class User
{
    [Key] 
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string UserFname { get; set; }
    public string UserLname { get; set; }
    public DateTime BirthDate { get; set; }
    public string City { get; set; }
    public int ZipCode { get; set; }
    public string refreshToken { get; set; }
    
    [ForeignKey("Country")]
    public int CountryId { get; set; }
    
    public virtual ICollection<Country> Country { get; set; }
    public virtual ICollection<UserType> UserType { get; set; }
}

public class Country
{
    [Key] 
    public int CountryId { get; set; }
    public string CountryName { get; set; }
}

public class UserType
{
    [Key]
    public int UserTypeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    [ForeignKey("User")]
    public int UserId { get; set; }
    
    [ForeignKey("SubscriptionType")]
    public int SubscriptionTypeId { get; set; }
    
    public virtual ICollection<User> User { get; set; }
}

public class Follower
{
    [Key]
    public int FollowerId { get; set; }
    public DateTime Begindate { get; set; }
    
    [ForeignKey("UserType")]
    public int UserTypeId_A { get; set; }
    
    [ForeignKey("UserType")]
    public int UserTypeId_B { get; set; }
}

public class CollaBorator
{
    [Key]
    public int Collaboratorid { get; set; }
    
    [ForeignKey("CollaboratorType")]
    public int CollaboratorTypeId { get; set; }
    
    [ForeignKey("UserType")]
    public int UserTypeId { get; set; }
    
    [ForeignKey("PlayList")]
    public int PlayListId { get; set; }
    
}


public class CollaboratorType
{
    [Key]
    public int CollaboratorTypeId { get; set; }
    public string CollaboratorTypeName { get; set; }
    public DateTime DateAdded { get; set; }
}
