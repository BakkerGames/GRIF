namespace GRIFTools.GROD;

internal class UndoSnapshot
{
    public Stack<UndoItem> Items = [];
}

internal class UndoItem(string key, string oldValue)
{
    public string Key { get; } = key;

    public string OldValue { get; } = oldValue;
}
