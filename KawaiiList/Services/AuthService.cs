using KawaiiList.Models;
using KawaiiList.Stores;
using Supabase;
using System.IO;
using System.Text.Json;

namespace KawaiiList.Services
{
    public class AuthService : IAuthService
    {
        private const string SessionFilePath = "session.json";

        private readonly Client _client;
        private readonly SupabaseClientStore _supabaseClientStore;
        private readonly UserStore _userStore;

        public AuthService(SupabaseClientStore supabaseClientStore, UserStore userStore)
        {
            _supabaseClientStore = supabaseClientStore;
            _userStore = userStore;
            _client = _supabaseClientStore.Client;
        }

        public async Task<bool> SignUpAsync(string email, string password, string username, string nickname)
        {
            try
            {
                var userMetadata = new Supabase.Gotrue.SignUpOptions
                {
                    Data = new Dictionary<string, object>
                    {
                        { "nickname", nickname },
                        { "display_name", username }
                    }
                };

                Supabase.Gotrue.Session session = await _client.Auth.SignUp(email: email, password: password, options: userMetadata);

                if (session != null && session.User != null)
                {
                    await SaveSessionAsync(session);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignUp Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SignInAsync(string email, string password)
        {
            try
            {   
                var session = await _client.Auth.SignIn(email, password);
                if (session != null)
                {
                    var user = session.User;
                    
                    UserApp userApp = new UserApp()
                    {
                        Id = user.Id,
                        Nickname = user.UserMetadata["nickname"].ToString(),
                        Username = user.UserMetadata["display_name"].ToString(),
                        Email = user.Email,
                        Images = new UserImages()
                        {
                            AvatarUrl = user.UserMetadata["avatar_url"].ToString(),
                            BannerUrl = user.UserMetadata["banner_url"].ToString()
                        }
                    };

                    await SaveSessionAsync(session);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignIn Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> TryRestoreSessionAsync()
        {
            if (!File.Exists(SessionFilePath))
                return false;

            try
            {
                var json = await File.ReadAllTextAsync(SessionFilePath);
                var saved = JsonSerializer.Deserialize<SavedSession>(json);

                if (saved == null)
                    return false;

                await _client.Auth.SetSession(saved.AccessToken, saved.RefreshToken);

                if (_client.Auth.CurrentSession != null)
                {
                    var user = _client.Auth.CurrentUser;

                    UserApp userApp = new UserApp()
                    {
                        Id = user.Id,
                        Nickname = user.UserMetadata["nickname"].ToString(),
                        Username = user.UserMetadata["display_name"].ToString(),
                        Email = user.Email,
                        Images = new UserImages()
                        {
                            AvatarUrl = user.UserMetadata["avatar_url"].ToString(),
                            BannerUrl = user.UserMetadata["banner_url"].ToString()
                        }
                    };

                    _userStore.CurrentUserApp = userApp;

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RestoreSession Error: {ex.Message}");
            }

            return false;
        }

        private async Task SaveSessionAsync(Supabase.Gotrue.Session session)
        {
            var sessionState = new SavedSession
            {
                AccessToken = session.AccessToken,
                RefreshToken = session.RefreshToken
            };

            var json = JsonSerializer.Serialize(session);
            await File.WriteAllTextAsync(SessionFilePath, json);
        }
    }
}
