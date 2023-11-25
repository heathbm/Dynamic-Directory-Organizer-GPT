using Application.Interfaces;
using Models;
using System.Text.RegularExpressions;

namespace Application.Services
{
    public partial class MappingsParser()
    {
        public void ParseMappings(string mappings, Dictionary<string, string> newMappings)
        {
            MatchCollection matches = ExtractMappingsRegex().Matches(mappings);

            foreach (Match match in matches.Cast<Match>())
            {
                newMappings[match.Groups[1].Value] = match.Groups[2].Value;
            }
        }

        public Node CreateTree(Dictionary<string, string> fileMappings, string folderName)
        {
            Node root = new(folderName);

            foreach (var path in fileMappings.Values)
            {
                Node currentNode = root;
                string[] parts = path.Split('/');

                foreach (string part in parts)
                {
                    currentNode.Children.TryAdd(part, new Node(part));
                    currentNode = currentNode.Children[part];
                }
            }

            return root;
        }

        [GeneratedRegex(@"\['(.*?)': '(.*?)'\]")]
        private static partial Regex ExtractMappingsRegex();
    }
}