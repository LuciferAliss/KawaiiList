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

        public async Task<(bool, string)> SignUpAsync(string email, string password, string username, string nickname)
        {
            try
            {
                var filter = new FiltersQuery()
                {
                    ColumnName = "username",
                    OperatorFilter = Operator.Equals,
                    Value = username
                };
                var existingProfiles = await _profilesService.GetFilter("*", filter);

                if (existingProfiles.Count() > 0)
                {
                    return (false, "Имя пользователя уже занято");
                }

                Session? session = await _client.Auth.SignUp(email: email, password: password);

                if (session == null)
                {
                    return (false, "Электронная почта уже зарегистрирована");
                }

                await _supabaseClientStore.UppdateSession(session);

                if (!await _storageSupabaseService.CreateBucket())
                {
                    return (false, "Ошибка регистрации, попробуйте позже");
                }

                string path = Path.Combine(AppContext.BaseDirectory, "Resources", "Images", "Logo.png");
                byte[] fileBytes = await File.ReadAllBytesAsync(path);

                bool isLoad;
                string? filePath;
                (isLoad, filePath) = await _storageSupabaseService.UploadImage(fileBytes, path, "avatar");

                if (!isLoad)
                {
                    return (false, "Ошибка регистрации, попробуйте позже");
                }

                UserImage userAvatarImage = new UserImage
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = session.User.Id,
                    TypeImage = "avatar",
                    FileName = filePath,
                    UploadedAt = DateTime.UtcNow
                };

                if (!await _userImagesService.Upsert(userAvatarImage))
                {
                    return (false, "Ошибка регистрации, попробуйте позже");
                }

                path = Path.Combine(AppContext.BaseDirectory, "Resources", "Images", "Banner.jpg");
                fileBytes = await File.ReadAllBytesAsync(path);

                (isLoad, filePath) = await _storageSupabaseService.UploadImage(fileBytes, path, "banner");

                if (!isLoad)
                {
                    return (false, "Ошибка регистрации, попробуйте позже");
                }

                UserImage userBannerImage = new UserImage
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = session.User.Id,
                    TypeImage = "banner",
                    FileName = filePath,
                    UploadedAt = DateTime.UtcNow
                };

                if (!await _userImagesService.Upsert(userBannerImage))
                {
                    return (false, "Ошибка регистрации, попробуйте позже");
                }

                Profiles profileData = new Profiles
                {
                    Id = session.User.Id,
                    Username = username,
                    Nickname = nickname,
                    Email = email,
                };

                if (!await _profilesService.Upsert(profileData))
                {
                    return (false, "Ошибка регистрации, попробуйте позже");
                }

                await SignOutAsync();

                return (true, "");
            }
            catch (Exception ex)
            {
                await SignOutAsync();

                Console.WriteLine($"SignUp Error: {ex.Message}");
                return (false, "Ошибка регистрации, попробуйте позже");
            }
        }

        public async Task<(bool, string)> SignInAsync(string username, string password)
        {
            try
            {
                var filter = new FiltersQuery()
                {
                    ColumnName = "username",
                    OperatorFilter = Operator.Equals,
                    Value = username
                };
                var profileResult = await _profilesService.GetFilter("*", filter);

                var profile = profileResult.FirstOrDefault();
                if (profile == null)
                {
                    return (false, "Неверно введено имя пользователя или пароль");
                }

                var session = await _client.Auth.SignIn(profile.Email, password);

                if (session != null)
                {
                    Models.User userApp = await LoadUserData(profile);

                    _userStore.CurrentUser = userApp;
                    await _supabaseClientStore.UppdateSession(session);

                    await SaveSessionAsync(session);
                    return (true, "");
                }

                return (false, "Неверно введено имя пользователя или пароль");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignIn Error: {ex.Message}");
                return (false, "Неверно введено имя пользователя или пароль");
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

                var filter = new FiltersQuery()
                {
                    ColumnName = "id",
                    OperatorFilter = Operator.Equals,
                    Value = _client.Auth.CurrentUser.Id
                };

                var profileResult = await _profilesService.GetFilter("*", filter);

                var profile = profileResult.FirstOrDefault();

                if (profile == null)
                {
                    return false;
                }

                Models.User userApp = await LoadUserData(profile);

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

        private async Task<Models.User> LoadUserData(Profiles profile)
        {
            List<FiltersQuery> filters = new List<FiltersQuery>()
            {
                new FiltersQuery()
                {
                    ColumnName = "user_id",
                    OperatorFilter = Operator.Equals,
                    Value = profile.Id
                },
                new FiltersQuery()
                {
                    ColumnName = "type_image",
                    OperatorFilter = Operator.Equals,
                    Value = "avatar"
                }
            };
            var userAvatarImages = await _userImagesService.GetFilter("*", filters, "uploaded_at", Ordering.Descending);
            var userAvatarImage = userAvatarImages.FirstOrDefault();

            filters = new List<FiltersQuery>()
            {
                new FiltersQuery()
                {
                    ColumnName = "user_id",
                    OperatorFilter = Operator.Equals,
                    Value = profile.Id
                },
                new FiltersQuery()
                {
                    ColumnName = "type_image",
                    OperatorFilter = Operator.Equals,
                    Value = "banner"
                }
            };
            var userBannerImages = await _userImagesService.GetFilter("*", filters, "uploaded_at", Ordering.Descending);
            var userBannerImage = userBannerImages.FirstOrDefault();

            Models.User userApp = new Models.User()
            {
                Id = profile.Id,
                Nickname = profile.Nickname,
                Username = profile.Username,
                Email = profile.Email,
                Images = new UserImageProfil()
                {
                    AvatarUrl = $"images-{profile.Id}/" + userAvatarImage.FileName,
                    BannerUrl = $"images-{profile.Id}/" + userBannerImage.FileName
                }
            };

            return userApp;
        }
    }
}