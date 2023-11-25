using Application.Interfaces;
using Models;
using Models.Enums;
using Spectre.Console;

namespace Application.Services
{
    public class UserInputAndAiMappingsRefiningService(
        IFileGptService fileGptService,
        MappingsParser mappingsParser,
        IConsoleService console)
    {
        public async Task<UserFlowStateEnum> RefineMappingsWithUserInputAndAi(Dictionary<string, string> mappings, string folderName)
        {
            console.PrintTitle(":broom: I can organize your files based on the context of the [underline]file name[/] and [underline]extension type[/]. I'll show you what I am thinking and you can refine the result. :broom:");

            bool firstPrompt = true;
            UserFlowStateEnum userFlowState = UserFlowStateEnum.Active;

            while (userFlowState == UserFlowStateEnum.Active)
            {
                if (!firstPrompt
                    && mappings.Count > 0) // The mappings need to exist before asking the user if they would like to use them
                {
                    userFlowState = AskUserForDirection();
                }

                if (userFlowState == UserFlowStateEnum.Active)
                {
                    await PromptUserAndRefineMappings(mappings, folderName);
                }

                firstPrompt = false;
            }

            return userFlowState;
        }

        private async Task PromptUserAndRefineMappings(Dictionary<string, string> mappings, string folderName)
        {
            string input = console.GetUserInput("How would you like your files to be organized?");

            if (string.IsNullOrEmpty(input))
            {
                console.PrintWarning("Please provide a valid prompt or command");
                ClearMappings(mappings);
                return;
            }

            Dictionary<string, string> newMappings = [];
            bool isValidTree = await SendUserInputToAiAndSetMappings(input, folderName, newMappings);
            ReplaceMappingsIfValid(isValidTree, mappings, newMappings);
        }

        private void ReplaceMappingsIfValid(bool isValidTree, Dictionary<string, string> mappings, Dictionary<string, string> newMappings)
        {
            if (!isValidTree)
            {
                console.PrintWarning("Sorry, I didn't understand how you wanted to sort your files, please try again.");
                ClearMappings(mappings);
                return;
            }

            // The New Mappings have been validated, replace the existing ones
            ReplaceMappings(mappings, newMappings);
        }

        private async Task<bool> SendUserInputToAiAndSetMappings(string input, string folderName, Dictionary<string, string> newMappings)
        {
            TaskCompletionSource waitingForAiTask = new();
            console.CreateSpinner(waitingForAiTask.Task);

            bool isValidTree = false;
            int tries = 0;
            int maxTries = 5;
            Node? tree = null;

            // On rare occasions, a prompt will only work most of the time, simply try it again to increase odds.
            while (!isValidTree && tries < maxTries)
            {
                tries++;
                string aiAnswer = await fileGptService.GetChatCompletionsAsync(input);
                mappingsParser.ParseMappings(aiAnswer, newMappings);
                tree = mappingsParser.CreateTree(newMappings, folderName);
                isValidTree = tree.IsValidTree;
            }

            if (isValidTree)
            {
                waitingForAiTask.SetResult();
                console.PrintTree(tree!);
            }

            return isValidTree;
        }

        private UserFlowStateEnum AskUserForDirection()
        {
            const string refineChoice = "I want to refine the structure";
            const string finishAndCopyChoice = "Yes please, copy them over";
            const string cancelAndExitChoice = "Cancel and exit";

            string choice = console.ProvideUserSelectionPrompt(refineChoice, finishAndCopyChoice, cancelAndExitChoice);

            return choice switch
            {
                finishAndCopyChoice => UserFlowStateEnum.CopyAndExit,
                cancelAndExitChoice => UserFlowStateEnum.CancelAndExit,
                _ => UserFlowStateEnum.Active,
            };
        }

        private void ClearMappings(Dictionary<string, string> mappings)
        {
            // Overwrite existing mappings
            ReplaceMappings(mappings, []);
        }

        private static void ReplaceMappings(Dictionary<string, string> mappings, Dictionary<string, string> tempMaps)
        {
            mappings.Clear();
            foreach (KeyValuePair<string, string> tempMap in tempMaps)
            {
                mappings.Add(tempMap.Key, tempMap.Value);
            }
        }
    }
}
