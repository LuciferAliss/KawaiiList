namespace KawaiiList.Services
{
    public interface IStorageSupabaseService
    {
        Task<bool> CreateBucket();
        Task<(bool, string?)> UploadImage(byte[] fileBytes, string originalFileName, string typeImage);
    }
}