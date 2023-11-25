namespace Application.Utilities
{
    public static class FileCopier
    {
        public static void MoveFiles(Dictionary<string, string> fileMappings, string destinationPath, Dictionary<string, string> fileToFullPathMap)
        {
            foreach (var mapping in fileMappings)
            {
                string originalPath = fileToFullPathMap[mapping.Key];
                string newPath = Path.Combine(destinationPath, mapping.Value);

                // Create the directory for the new file path if it doesn't exist.
                string? newDirectory = Path.GetDirectoryName(newPath);
                if (!Directory.Exists(newDirectory))
                {
                    Directory.CreateDirectory(newDirectory!);
                }

                // Move the file to the new location.
                if (File.Exists(originalPath))
                {
                    File.Copy(originalPath, newPath);
                }
            }
        }
    }
}