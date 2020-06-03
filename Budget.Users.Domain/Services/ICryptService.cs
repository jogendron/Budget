namespace Budget.Users.Domain.Services
{
    public interface ICryptService
    {
        string Crypt(string content);
    }
}