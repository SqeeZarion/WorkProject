
namespace WebAuthCommon;

    public class AuthToken
    {
        public string Issuer { get; set; } //той, хто згенерував токен
        
        public string Audience { get; set; } //для кого призначений
        
        public string Secret { get; set; } // для генерації симетричному шифруванні
        
        public int TokenLifetime { get; set; } //життя токену

    }
