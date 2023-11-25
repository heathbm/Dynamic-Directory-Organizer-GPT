namespace Application.Utilities
{
    public static class FileExtensionCounter
    {
        public static Dictionary<string, int> ParseFileExtensionBreakdowns(string[] allFiles)
        {
            Dictionary<string, int> keyValuePairs = [];

            foreach (string file in allFiles)
            {
                var segments = file.Split('.');
                if (segments.Length != 2)
                {
                    AddOrIncrementValueInDictionary(keyValuePairs, "Unknown");
                    keyValuePairs["Unknown"]++;
                }
                else
                {
                    string ext = segments[1].ToLower();
                    AddOrIncrementValueInDictionary(keyValuePairs, ext);
                }
            }

            return keyValuePairs;
        }

        private static void AddOrIncrementValueInDictionary(Dictionary<string, int> keyValuePairs, string key)
        {
            keyValuePairs.TryGetValue(key, out int count);
            keyValuePairs[key] = count + 1;
        }
    }
}