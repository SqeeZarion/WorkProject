namespace WebApplication1.Interface;

public interface IPasswordService
{
    public string Encode(string password);
    public string Decrypt(string password);
    public bool Verify(string password, string passwordEnc);
}