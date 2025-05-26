using KawaiiList.Models;
using KawaiiList.Stores;
using Supabase.Gotrue;
using System.IO;
using System.Text.Json;
using static Supabase.Postgrest.Constants;

namespace KawaiiList.Services
{
    public class AuthService : IAuthService
    {
        private const string SessionFilePath = "session.json";

        private readonly SupabaseClientStore _supabaseClientStore;
        private readonly UserStore _userStore;
        private readonly ISupaBaseService<Profiles> _profilesService;
        private readonly ISupaBaseService<UserImage> _userImagesService;
        private readonly IStorageSupabaseService _storageSupabaseService;
        private readonly Supabase.Client _client;

        public AuthService(SupabaseClientStore supabaseClientStore,
            UserStore userStore,
            ISupaBaseService<Profiles> profilesService,
            ISupaBaseService<UserImage> userImagesService, 
            IStorageSupabaseService storageSupabaseService)
        {
            _supabaseClientStore = supabaseClientStore;
            _userStore = userStore;
            _client = _supabaseClientStore.Client;
            _profilesService = profilesService;
            _userImagesService = userImagesService;
            _storageSupabaseService = storageSupabaseService;
        }

        public async Task<bool> SignUpAsync(string email, string password, string username, string nickname)
        {
            try
            {
                var existingProfiles = await _profilesService.GetFilter("*", "username", Operator.Equals, username);

                if (existingProfiles.Count() > 0)
                {
                    Console.WriteLine("Username уже занят.");
                    return false;
                }

                Session? session = await _client.Auth.SignUp(email: email, password: password);

                if (session == null)
                {
                    return false;
                }

                await _supabaseClientStore.UppdateSession(session);

                if (!await _storageSupabaseService.CreateBucket())
                {
                    return false;
                }

                string path = Path.Combine(AppContext.BaseDirectory, "Resources", "Images", "Logo.png");
                byte[] fileBytes = await File.ReadAllBytesAsync(path);


                bool isLoad;
                string? filePath;
                (isLoad, filePath) = await _storageSupabaseService.UploadImage(fileBytes, path, "avatar");

                if (!isLoad)
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

                if (!await _profilesService.Insert(profileData))
                {
                    return false;
                }


                UserImage userImage = new UserImage
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = session.User.Id,
                    TypeImage = "avatar",
                    FileName = filePath,
                    UploadedAt = DateTime.UtcNow
                };

                if (!await _userImagesService.Insert(userImage))
                {
                    return false;
                }

                await SignOutAsync();

                return true;
            }
            catch (Exception ex)
            {
                await SignOutAsync();

                Console.WriteLine($"SignUp Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SignInAsync(string username, string password)
        {
            try
            {
                var profileResult = await _profilesService.GetFilter("*", "username", Operator.Equals, username);

                var profile = profileResult.FirstOrDefault();
                if (profile == null)
                {
                    return false;
                }

                var session = await _client.Auth.SignIn(profile.Email, password);

                if (session != null)
                {
                    var user = session.User;

                    var userImages = await _userImagesService.GetFilter("*", "user_id", Operator.Equals, user.Id, "uploaded_at", Ordering.Descending);
                    var userImage = userImages.FirstOrDefault();

                    Models.User userApp = new Models.User()
                    {
                        Id = user.Id,
                        Nickname = profile.Nickname,
                        Username = profile.Username,
                        Email = user.Email,
                        Images = new UserImageProfil()
                        {
                            AvatarUrl = $"images-{user.Id}/" + userImage.FileName
                        }
                    };

                    _userStore.CurrentUser = userApp;
                    await _supabaseClientStore.UppdateSession(session);

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
            {
                return false;
            }

            try
            {
                var json = await File.ReadAllTextAsync(SessionFilePath);

                if (string.IsNullOrWhiteSpace(json))
                {
                    return false;
                }

                var saved = JsonSerializer.Deserialize<SavedSession>(json);

                if (saved == null)
                {
                    return false;
                }

                Session? session = await _client.Auth.SetSession(saved.AccessToken, saved.RefreshToken);

                if (session == null)
                {
                    return false;
                }

                var profileResult = await _profilesService.GetFilter("*", "id", Operator.Equals, _client.Auth.CurrentUser.Id);

                var profile = profileResult.FirstOrDefault();

                if (profile == null)
                {
                    return false;
                }

                var user = _client.Auth.CurrentUser;

                var userImages = await _userImagesService.GetFilter("*", "user_id", Operator.Equals, user.Id, "uploaded_at", Ordering.Descending);
                var userImage = userImages.FirstOrDefault();

                Models.User userApp = new Models.User()
                {
                    Id = user.Id,
                    Nickname = profile.Nickname,
                    Username = profile.Username,
                    Email = user.Email,
                    Images = new UserImageProfil()
                    {
                        AvatarUrl = $"images-{user.Id}/" + userImage.FileName

                    }
                };

                _userStore.CurrentUser = userApp;

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RestoreSession Error: {ex.Message}");
            }

            return false;
        }

        private async Task SaveSessionAsync(Session session)
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