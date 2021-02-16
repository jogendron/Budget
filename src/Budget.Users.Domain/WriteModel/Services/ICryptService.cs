namespace Budget.Users.Domain.WriteModel.Services
{
    public interface ICryptService
    {
        string Crypt(string content);
    }
}