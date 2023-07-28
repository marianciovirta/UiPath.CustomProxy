using System.Collections.Generic;

namespace UiPath.CustomProxy.Contracts
{
    internal interface IFileSystemService
    {
        bool DirectoryExists(string path);

        void CreateDirectory(string path);

        bool TrySelectDirectory(out string path);

        bool FileExists(string path);

        bool TryGetFileNameWithoutExtension(string path, out string filename);

        bool TryReadAllText(string path, out string content);

        bool TryWriteAllText(string path, string content);

        bool TryGetFiles(string directoryPath, string searchPattern, out string[] files);
    }
}
