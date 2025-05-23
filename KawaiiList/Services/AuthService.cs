using KawaiiList.Models;
using KawaiiList.Stores;
using Supabase.Gotrue;
using System.IO;
using System.Text.Json;

namespace KawaiiList.Services
{
    public class AuthService : IAuthService
    {
        private const string SessionFilePath = "session.json";

        private readonly Supabase.Client _client;
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
                 var existingProfiles = await _client
                    .From<Profiles>()
                    .Select("*")
                    .Filter("username", Supabase.Postgrest.Constants.Operator.Equals, username)
                    .Get();

                if (existingProfiles.Models.Count > 0)
                {
                    Console.WriteLine("Username уже занят.");
                    return false;
                }

                Session session = await _client.Auth.SignUp(email: email, password: password);

                if (session == null)
                {
                    return false;
                }

                Profiles profileData = new Profiles
                {
                    Id = session.User.Id,
                    Username = username,
                    Nickname = nickname,
                    Email = email,
                };
                
                var insertResponse = await _client.From<Profiles>().Insert(profileData);

                await SaveSessionAsync(session);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignUp Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SignInAsync(string username, string password)
        {
            try
            {
                var profileResult = await _client
                    .From<Profiles>()
                    .Select("*")
                    .Filter("username", Supabase.Postgrest.Constants.Operator.Equals, username)
                    .Get();

                var profile = profileResult.Models[0];
                if (profile == null)
                {
                    return false;
                }

                var session = await _client.Auth.SignIn(profile.Email, password);

                if (session != null)
                {
                    var user = session.User;

                    Models.User userApp = new Models.User()
                    {
                        Id = user.Id,
                        Nickname = profile.Nickname,
                        Username = profile.Username,
                        Email = user.Email,
                        //    Images = new UserImages()
                        //    {
                        //        AvatarUrl = user.UserMetadata["avatar_url"].ToString(),
                        //        BannerUrl = user.UserMetadata["banner_url"].ToString()
                        //    }
                    };

                    _userStore.CurrentUser = userApp;

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

        public async Task<bool> SignOutAsync()
        {
            try
            {
                await _client.Auth.SignOut();
                _userStore.CurrentUser = null;
                DelateSeesion();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignOut Error: {ex.Message}");
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
                {
                    return false;
                }

                await _client.Auth.SetSession(saved.AccessToken, saved.RefreshToken);

                var profileResult = await _client
                    .From<Profiles>()
                    .Select("*")
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, _client.Auth.CurrentUser.Id)
                    .Get();

                var profile = profileResult.Models[0];

                if (profile == null)
                {
                    return false;
                }
                    
                if (_client.Auth.CurrentSession != null)
                {
                    var user = _client.Auth.CurrentUser;

                    Models.User userApp = new Models.User()
                    {
                        Id = user.Id,
                        Nickname = profile.Nickname,
                        Username = profile.Username,
                        Email = user.Email,
                        //    Images = new UserImages()
                        //    {
                        //        AvatarUrl = user.UserMetadata["avatar_url"].ToString(),
                        //        BannerUrl = user.UserMetadata["banner_url"].ToString()
                        //    }
                    };

                    _userStore.CurrentUser = userApp;

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

            var json = JsonSerializer.Serialize(sessionState);
            await File.WriteAllTextAsync(SessionFilePath, json);
        }

        private void DelateSeesion()
        {
            File.Delete(SessionFilePath);
        }
    }
}
