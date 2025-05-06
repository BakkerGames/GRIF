namespace GRIFTools;

public partial class Grod
{
    private readonly Dictionary<string, string> _base = [];
    private readonly Dictionary<string, string> _overlay = [];

    private readonly Stack<UndoSnapshot> _undo = [];

    private UndoSnapshot _snapshot = new();

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

internal class UndoSnapshot
{
    public Stack<UndoItem> Items = [];
}

internal class UndoItem(string key, string oldValue)
{
    public string Key { get; } = key;

    public string OldValue { get; } = oldValue;

    public override string ToString()
    {
        return $"{{\"{Key}\",\"{OldValue}\"}}";
    }
}
