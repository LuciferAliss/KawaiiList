using System.IO;

namespace KawaiiList.Models
{
    public class FileModel
    {
        public FileModel(string? file)
        {
            this.File = file;
            this.FileName = Path.GetFileName(file);
        }

        public FileModel() { }

        public string? FileName { get; set; }
        public string? File { get; set; }
    }
}
