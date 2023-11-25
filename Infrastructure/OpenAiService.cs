using Application.Interfaces;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using Models;

namespace Infrastructure
{
    public class OpenAiService : IFileGptService
    {
        private readonly OpenAIClient openAiClient;
        private readonly ChatCompletionsOptions completionOptions;

        // Provide this prompt for each query to help GPT keep track of how to respond.
        const string Prompt = "Please create a map between the original file and the newly organized location for that file. Important: Wrap each mapping in square brackets so I can easily find them, with a colon separator and wrap original file name and the mapping in single quotes. Important: Make sure the mapping has the new directory path and file name and extension";
        private const string SystemRolePrompt = "you are a helpful assistant that can help people organize their files and folders based on dynamic requests. Please always return all files that were provided. don't include any explanations in your responses.";
        private const string InitialOrganizePrompt = "please help me organize the following files based on context in the name of the file:";
        private const string MapFormatPrompt = "Please create a map between the original file and the newly organized location for that file.";

        public OpenAiService(
            IOptions<Settings> options,
            OpenAiClientFactory openAiClientFactory)
        {
            completionOptions = new()
            {
                MaxTokens = 2048,
                Temperature = 0.1f,
                NucleusSamplingFactor = 1f,
                DeploymentName = options.Value.Model,
            };

            openAiClient = openAiClientFactory.GetOpenApiClient();
        }

        public async Task ProvideSystemPrompt() => 
            await GetChatCompletionsAsync(ChatRole.System, SystemRolePrompt);

        public async Task ProvideFiles(string files) => 
            await GetChatCompletionsAsync(ChatRole.User, $"{InitialOrganizePrompt} {files}. {Prompt}");

        public async Task<string> GetChatCompletionsAsync(string query) =>
            await GetChatCompletionsAsync(ChatRole.User, $"{query}. {MapFormatPrompt} {Prompt}");

        private async Task<string> GetChatCompletionsAsync(ChatRole role, string question)
        {
            completionOptions.Messages.Add(new ChatMessage(role, question));
            var result = await openAiClient.GetChatCompletionsAsync(completionOptions);
            return result.Value.Choices[0].Message.Content;
        }
    }
}
