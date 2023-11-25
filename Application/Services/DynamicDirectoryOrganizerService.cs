using Application.Interfaces;
using Application.Utilities;
using Microsoft.Extensions.Hosting;
using Models.Enums;

namespace Application.Services
{
    public class DynamicDirectoryOrganizerService(
        IHostApplicationLifetime lifetime,
        IFileGptService fileGptService,
        FilesService filesHelper,
        IConsoleService console,
        UserInputAndAiMappingsRefiningService userInputAndAiMappingsRefiningService) : IHostedService
    {
        private async Task InitiateUserPromptFlow()
        {
            Introduction();

            // Directory with files that need organizing
            string[] allFilesPaths = filesHelper.GetFilesFromUserInput();
            filesHelper.PrintBreakDown(allFilesPaths);
            Dictionary<string, string> fileToFullPathMap = filesHelper.CreateFileToFullPathMap(allFilesPaths);
            string commaSeparatedListOfFileNames = filesHelper.CreateCommaSeparatedListOfFileNames(fileToFullPathMap.Keys);

            // Destination folder
            string destinationPath = filesHelper.GetDestinationPath();
            string folderName = filesHelper.GetFolderName(destinationPath);

            // User and AI interactions
            Dictionary<string, string> mappings = [];
            await InitializeAiWithRoleAndFiles(commaSeparatedListOfFileNames);
            UserFlowStateEnum finalUserChoice = await userInputAndAiMappingsRefiningService.RefineMappingsWithUserInputAndAi(mappings, folderName);

            // Finish
            PerformFinalAction(mappings, finalUserChoice, destinationPath, fileToFullPathMap);
        }

        private void Introduction()
        {
            console.PrintLineBreak();
            console.PrintTitle(":rocket: Dynamic Directory Organizer Gpt :globe_showing_americas:", true, true);
            console.PrintTitle("I can organize files by analyzing file names and extensions, combined with how you wish to have them organized.", true, true);
        }

        private async Task InitializeAiWithRoleAndFiles(string filesString)
        {
            TaskCompletionSource waitingForAiTask = new();
            console.CreateSpinner(waitingForAiTask.Task);

            await fileGptService.ProvideSystemPrompt();
            await fileGptService.ProvideFiles(filesString);

            waitingForAiTask.SetResult();
        }

        private void PerformFinalAction(Dictionary<string, string> mappings, UserFlowStateEnum userFlowState, string destinationPath, Dictionary<string, string> fileToFullPathMap)
        {
            if (userFlowState == UserFlowStateEnum.CopyAndExit)
            {
                FileCopier.MoveFiles(mappings, destinationPath, fileToFullPathMap);
            }

            console.PrintTitle(":smiling_cat_with_heart_eyes: Thanks for trying this out! :beaming_face_with_smiling_eyes:\t");

            lifetime.StopApplication();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            cancellationToken.Register(lifetime.StopApplication);

            // Helps CTRL + C TODO
            _ = Task.Run(InitiateUserPromptFlow, cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
