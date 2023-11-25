namespace Application.Utilities
{
    public static class DirectoryService
    {
        public static string[] GetFiles(string path)
        {
            return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        }

        public static bool IsValidPath(string? path) =>
            !string.IsNullOrEmpty(path) && Path.Exists(path);
    }
}