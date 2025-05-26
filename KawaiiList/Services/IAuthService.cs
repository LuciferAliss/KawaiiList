namespace KawaiiList.Services
{
    public interface IAuthService
    {
        Task<bool> SignUpAsync(string email, string password, string username, string nickname);
        Task<bool> SignInAsync(string email, string password);
        Task<bool> SignOutAsync();
        Task<bool> TryRestoreSessionAsync();
    }
}
