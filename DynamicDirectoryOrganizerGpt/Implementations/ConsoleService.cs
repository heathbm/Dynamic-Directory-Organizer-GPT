using Application.Interfaces;
using Models;
using Spectre.Console;

namespace DynamicDirectoryOrganizerGpt.Implementations
{
    public class ConsoleService : IConsoleService
    {
        public void PrintTitle(string message, bool centered = false, bool skipTopSpacing = false)
        {
            if (!skipTopSpacing)
            {
                Console.WriteLine("");
            }

            var table = new Table();
            table.AddColumn(message);

            if (centered)
            {
                table = table.Centered();
            }

            AnsiConsole.Write(table);

            Console.WriteLine("");
        }

        public void PrintWarning(string message)
        {
            Console.WriteLine("");

            var table = new Table()
                .BorderStyle(new Style(Color.Orange1));
            table.AddColumn($"[orange1]{message}[/]");

            AnsiConsole.Write(table);

            Console.WriteLine("");
        }

        public string GetUserInput(string message)
        {
            return AnsiConsole.Ask<string>($"[lime]{message}[/]");
        }

        public void PrintBarChar(Dictionary<string, int> keyValuePairs)
        {
            BarChart chart = new();

            // Skip 0 as it is black and it would not be visible on most consoles
            int colorIndex = 1;

            foreach (var keyValuePair in keyValuePairs)
            {
                ConsoleColor consoleColor = (ConsoleColor)colorIndex++;
                Color color = Color.FromConsoleColor(consoleColor);

                chart.AddItem(keyValuePair.Key, keyValuePair.Value, color);

                // There are a max of 15 colors in ConsoleColor, loop over
                if (colorIndex == 16)
                {
                    // Skip 0 as it is black and it would not be visible on most consoles
                    colorIndex = 1;
                }
            }

            Console.WriteLine("");
            AnsiConsole.Write(chart);
        }

        public void PrintTree(Node root)
        {
            var queue = new Queue<(Node node, Tree tree)>();
            var rootTree = new Tree(new Markup(root.Name!));
            queue.Enqueue((root, rootTree));

            while (queue.Count > 0)
            {
                var (node, tree) = queue.Dequeue();

                foreach (var child in node.Children.Values)
                {
                    string emoji = child.Children.Count > 0 ? ":file_folder:" : ":page_facing_up:";
                    var childTree = new Tree(new Markup($"{emoji} {child.Name}"));
                    tree.AddNode(childTree);
                    queue.Enqueue((child, childTree));
                }
            }

            Console.WriteLine("");
            AnsiConsole.Write(rootTree);
        }

        public string ProvideUserSelectionPrompt(params string[] subjects)
        {
            Console.WriteLine("");
            return AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Are you satisfied with the new structure?")
                            .PageSize(10)
                            .AddChoices(subjects));
        }

        public void CreateSpinner(Task task)
        {
            _ = AnsiConsole.Status()
                .StartAsync("Waiting for AI", async ctx =>
                {
                    ctx.Spinner(Spinner.Known.Moon);
                    await task;
                });
        }

        public void PrintLineBreak()
        {
            Console.WriteLine("");
        }

        public void WriteLine(string text)
        {
            Console.WriteLine(text);
        }
    }
}
