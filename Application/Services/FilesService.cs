using Application.Interfaces;
using Application.Utilities;

namespace Application.Services
{
    public class FilesService(IConsoleService console)
    {
        public string GetDestinationPath()
        {
            console.PrintTitle(":file_folder: Where would you like the files to end up? :file_folder:");

            string? path;
            do
            {
                path = console.GetUserInput("Please provide a valid directory path:");
            }
            while (!DirectoryService.IsValidPath(path));

            return path;
        }

        public string[] GetFilesFromUserInput()
        {
            string? path = null;

            do
            {
                if (!DirectoryService.IsValidPath(path))
                {
                    path = console.GetUserInput("Please provide a valid directory path:");
                    continue;
                }

                string[] allFilesPaths = DirectoryService.GetFiles(path!);
                if (allFilesPaths.Length == 0)
                {
                    path = console.GetUserInput("This path has no files. Please provide another path:");
                    continue;
                }

                return allFilesPaths;
            }
            while (true);
        }

        public string CreateCommaSeparatedListOfFileNames(ICollection<string> fileNames) =>
            string.Join(",", fileNames.Select(file => $"'{file}'"));

        public Dictionary<string, string> CreateFileToFullPathMap(string[] allFilesPaths) =>
            allFilesPaths
                .ToDictionary(
                    filePath => Path.GetFileName(filePath)!,
                    filePath => filePath);

        public void PrintBreakDown(string[] allFiles)
        {
            console.PrintTitle(":bar_chart: This is the breakdown of the directory you provided :bar_chart:");
            Dictionary<string, int> directoryTree = FileExtensionCounter.ParseFileExtensionBreakdowns(allFiles);
            console.PrintBarChar(directoryTree);
        }

        public string GetFolderName(string path) =>
            Path.GetFileName(path.TrimEnd(Path.DirectorySeparatorChar));
    }
}