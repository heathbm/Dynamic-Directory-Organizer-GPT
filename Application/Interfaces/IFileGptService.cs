namespace Application.Interfaces
{
    public interface IFileGptService
    {
        Task ProvideSystemPrompt();
        Task ProvideFiles(string files);
        Task<string> GetChatCompletionsAsync(string query);
    }
}
