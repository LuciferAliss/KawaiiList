using KawaiiList.Stores;
using Supabase;
using System.Diagnostics;
using System.IO;
using System.Net.Http;

namespace KawaiiList.Services
{
    public class StorageSupabaseService : IStorageSupabaseService
    {
        private readonly SupabaseClientStore _supabaseClientStore;
        private Client _client;

        public StorageSupabaseService(SupabaseClientStore supabaseClientStore)
        {
            _supabaseClientStore = supabaseClientStore;
            _client = _supabaseClientStore.Client;

            _supabaseClientStore.CurrentClientChanged += UpdateSession;
        }

        public string GetPublicUrl(string imagesName)
        {
            var userId = _client.Auth.CurrentUser?.Id;
            string bucketName = $"images-{userId}";
            string fileName = $"{userId}-{imagesName}";

            // Вернёт прямую ссылку, если файл публичный
            return _client.Storage
                .From(bucketName)
                .GetPublicUrl(fileName);
        }

        public async Task<(bool, string?)> UploadImage(byte[] fileBytes, string originalFileName, string typeImage)
        {
            try
            {
                var userId = _client.Auth.CurrentUser?.Id;

                string? contentType, ext;
                (contentType, ext) = GetContentType(originalFileName);

                if (string.IsNullOrEmpty(contentType))
                {
                    Console.WriteLine("❌ Неподдерживаемый формат файла. Разрешены только .jpg, .jpeg, .png");
                    return (false, null);
                }

                var filePath = $"{typeImage}/{Guid.NewGuid()}.{ext}";

                var options = new Supabase.Storage.FileOptions
                {
                    ContentType = contentType,
                    CacheControl = "3600",
                    Upsert = true
                };

                string bucketName = $"images-{userId}";
                string result = await _client.Storage
                    .From(bucketName)
                    .Update(fileBytes, filePath, options);

                return (result != null, result != null ? filePath : null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка в UploadImage: {ex.Message}");
                return (false, null);
            }
        }

        public async Task<bool> CreateBucket()
        {
            try
            {
                var userId = _client.Auth.CurrentUser?.Id;

                string bucketName = $"images-{userId}";

                var options = new Supabase.Storage.BucketUpsertOptions
                {
                    Public = true
                };

                var response = await _client.Storage.CreateBucket(bucketName, options);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка в CreateBucket: {ex.Message}");
                return false;
            }
        }

        private (string?, string?) GetContentType(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLower();

            return ext switch
            {
                ".jpg" => ("image/jpeg", "jpg"),
                ".jpeg" => ("image/jpeg", "jpeg"),
                ".png" => ("image/png", "png"),
                _ => (null, null)
            };
        }

        private void UpdateSession()
        {
            _client = _supabaseClientStore.Client;
        }
    }
}
