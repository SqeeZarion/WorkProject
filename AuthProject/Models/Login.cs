using System.ComponentModel.DataAnnotations;

public class Login
{
    [Required] // означає, що поле обов'язково має бути
    // диктує значення формату Email
    public string UserName { get; set; }

    [Required]
    public string Password { get; set; }
    
}