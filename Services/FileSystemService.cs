using Microsoft.Win32;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using UiPath.CustomProxy.Contracts;

namespace UiPath.CustomProxy.Services
{
    internal class FileSystemService : IFileSystemService
    {
        public bool DirectoryExists(string path) => Directory.Exists(path);

        public void CreateDirectory(string path) => Directory.CreateDirectory(path);

        public bool FileExists(string path) => File.Exists(path);

        public bool TryGetFileNameWithoutExtension(string path, out string fileName)
        {
            fileName = GetFileNameWithoutExtensionSafe(path);
            return !string.IsNullOrWhiteSpace(path);
        }

        public string GetFileNameWithoutExtensionSafe(string path)
        {
            try
            {
                return Path.GetFileNameWithoutExtension(path);
            }
            catch
            {
                return null;
            }
        }

        public bool TryGetFiles(string directoryPath, string searchPattern, out string[] files)
        {
            files = null;
            try
            {
                files = Directory.GetFiles(directoryPath, searchPattern);
                return files.Any();
            }
            catch
            {
            }

            return false;
        }

        public bool TryWriteAllText(string path, string content)
        {
            try
            {
                File.WriteAllText(path, content);
                return true;
            }
            catch
            {
            }

            return false;
        }

        public bool TryReadAllText(string path, out string content)
        {
            content = null;
            try
            {
                if (!FileExists(path))
                    return false;

                content = File.ReadAllText(path);
                return content != null;
            }
            catch
            {
            }

            return false;
        }

        public bool TrySelectDirectory(out string path)
        {
            path = null;
            var sf = new FolderBrowserDialog();
            if (sf.ShowDialog() != DialogResult.OK)
                return false;

            path = sf.SelectedPath;
            return true;
        }
    }
}
