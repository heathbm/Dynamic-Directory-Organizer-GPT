namespace Models
{
    public class Node(string? name)
    {
        public string? Name { get; } = name;
        public Dictionary<string, Node> Children { get; } = [];

        public bool IsValidTree => Children?.Count > 0;
    }
}