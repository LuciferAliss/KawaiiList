﻿namespace KawaiiList.Services
{
    public interface IAuthService
    {
        Task<(bool, string)> SignUpAsync(string email, string password, string username, string nickname);
        Task<(bool, string)> SignInAsync(string email, string password);
        Task<bool> SignOutAsync();
        Task<bool> TryRestoreSessionAsync();
    }
}
