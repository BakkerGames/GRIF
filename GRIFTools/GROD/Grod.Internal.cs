namespace GRIFTools;

public partial class Grod
{
    private readonly Dictionary<string, string> _base = [];
    private readonly Dictionary<string, string> _overlay = [];

    private static string NormalizeKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key));
        }
        key = key.ToLower().Trim();
        foreach (char c in key)
        {
            if (char.IsWhiteSpace(c))
            {
                throw new ArgumentException($"Invalid key: {key}");
            }
        }
        return key;
    }
}
