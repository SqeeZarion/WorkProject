using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using WebApplication1.Interface;
using WebApplication1.Models;

namespace WebAuthCommon;

//шифрування або розшифрування за допомогою алгоритму Advanced Encryption Standard (AES)
//у режимі шифрування (CBC - Cipher Block Chaining).
//AES є симетричним алгоритмом, тобто використовує один ключ для шифрування та розшифрування даних.

public class PasswordService : IPasswordService
{
    
    private readonly EncryptOptions encryptOptions;

    public PasswordService(IOptions<EncryptOptions> encryptOptions)
    {
       
        this.encryptOptions = encryptOptions.Value;
    }

    private byte[] convertBytes(string eny)
    {
        string[] hexValues = eny.Split(", ");
        
        byte[] byteArray = new byte[hexValues.Length];
        for (int i = 0; i < hexValues.Length; i++)
            byteArray[i] = Convert.ToByte(hexValues[i], 16);

        return byteArray;
    }
    
    public string Encode(string password)
    {
        string encryptKey = encryptOptions.Key;
        string encryptSalt = encryptOptions.Salt;

        byte[] byteSalt = convertBytes(encryptSalt);
        byte[] passwordBytes = Encoding.Unicode.GetBytes(password);

        using (Aes encryptor = Aes.Create())
        {
            
            //допомагає отримати ключ і вектор ініціалізації (IV) з основного ключа encryptKey та солі encryptSalt.
            //Rfc2898DeriveBytes використовує алгоритм PBKDF2 для отримання ключа та вектора ініціалізації із початкових даних.
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptKey, byteSalt);

            //ключ довжиною 32 байти, тобто AES-256.
            encryptor.Key = pdb.GetBytes(32);
            
            //вектор ініціалізації (IV) AES (16 байтів)
            encryptor.IV = pdb.GetBytes(16);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cr = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cr.Write(passwordBytes, 0, passwordBytes.Length);
                    cr.Close();
                }
                
                string encryptedPassword = Convert.ToBase64String(ms.ToArray());
                return encryptedPassword;
            }
        }
    }

    // Метод для розшифрування пароля
    public string Decrypt(string encryptedPassword)
    {
        string encryptKey = encryptOptions.Key;
        string encryptSalt = encryptOptions.Salt;
        
        byte[] byteSalt = convertBytes(encryptSalt);
        byte[] passwordBytes = Convert.FromBase64String(encryptedPassword);

        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptKey, byteSalt);
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(passwordBytes, 0, passwordBytes.Length);
                    cs.Close();
                }

                string decryptedPassword = Encoding.Unicode.GetString(ms.ToArray());
                return decryptedPassword;
            }
        }
    }

    
    public bool Verify(string password, string encryptedPassword)
    {
       
        string decryptedPassword = Decrypt(encryptedPassword);

        return password == decryptedPassword;
    }
}