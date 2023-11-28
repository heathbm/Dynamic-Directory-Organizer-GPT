using Models;

namespace Application.Interfaces
{
    public interface IConsoleService
    {
        void PrintTitle(string message, bool centered = false, bool skipTopSpacing = false);
        void PrintWarning(string message);
        string GetUserInput(string message);
        void PrintBarChar(Dictionary<string, int> keyValuePairs);
        void PrintTree(Node root);
        string ProvideUserSelectionPrompt(params string[] subjects);
        void CreateSpinner(Task task);
        void PrintLineBreak();
        void WriteLine(string text);
    }
}
