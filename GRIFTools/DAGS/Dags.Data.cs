using static GRIFTools.DAGSConstants;

namespace GRIFTools;

public partial class Dags
{
    private Dictionary<string, string> undo1 = [];
    private Dictionary<string, string> undo2 = [];
    private string saveCommand1 = "";
    private string saveCommand2 = "";

    private string Get(string key)
    {
        var result = Data.Get(key);
        if (result == NULL_VALUE)
        {
            result = "";
        }
        return result;
    }

    private void Set(string key, string value)
    {
        if (value == NULL_VALUE)
        {
            value = "";
        }
        Data.Set(key, value);
    }

    private Dictionary<string, string?> GetByPrefix(string prefix)
    {
        Dictionary<string, string?> result = [];
        List<string> keys;
        keys = [.. Data.Keys().Where(x => x.StartsWith(prefix, OIC))];
        foreach (string k in keys)
        {
            result.Add(k, Get(k));
        }
        return result;
    }

    private int GetInt(string key)
    {
        var value = Data.Get(key);
        if (value == "")
        {
            return 0;
        }
        if (int.TryParse(value, out int result))
        {
            return result;
        }
        throw new SystemException($"Value is not an integer: {key}: {value}");
    }

    #region List routines

    private string GetListLength(string key)
    {
        string list = Data.Get(key);
        List<string> items = [.. list.Split(',')];
        return items.Count.ToString();
    }

    private string GetListItem(string key, string index)
    {
        if (!int.TryParse(index, out int tempIndex))
        {
            throw new ArgumentException($"List index is not an integer: {index}");
        }
        return GetListItem(key, tempIndex);
    }

    private string GetListItem(string key, int index)
    {
        string list = Data.Get(key);
        List<string> items = [.. list.Split(',')];
        if (items.Count <= index)
        {
            return "";
        }
        return items[index];
    }

    private void SetListItem(string key, string index, string value)
    {
        if (!int.TryParse(index, out int tempIndex))
        {
            throw new ArgumentException($"List index is not an integer: {index}");
        }
        SetListItem(key, tempIndex, value);
    }

    private void SetListItem(string key, int index, string value)
    {
        if (index < 0)
        {
            throw new SystemException($"List index cannot be negative: {key}: {index}");
        }
        if (value.Contains(','))
        {
            throw new SystemException($"List items cannot contain commas: {key}: {value}");
        }
        string list = Data.Get(key);
        List<string> items = [.. list.Split(',')];
        while (items.Count <= index)
        {
            items.Add("");
        }
        items[index] = value;
        Data.Set(key, string.Join(',', items));
    }

    private void AddListItem(string key, string value)
    {
        if (value.Contains(','))
        {
            throw new SystemException($"List items cannot contain commas: {key}: {value}");
        }
        string list = Data.Get(key);
        List<string> items = [.. list.Split(',')];
        if (list == "")
        {
            if (value == "")
            {
                throw new SystemException($"Cannot add a blank value to an empty list: {key}");
            }
            items[0] = value;
        }
        else
        {
            items.Add(value);
        }
        Data.Set(key, string.Join(',', items));
    }

    private void InsertAtListItem(string key, string index, string value)
    {
        if (!int.TryParse(index, out int tempIndex))
        {
            throw new ArgumentException($"List index is not an integer: {index}");
        }
        InsertAtListItem(key, tempIndex, value);
    }

    private void InsertAtListItem(string key, int index, string value)
    {
        if (index < 0)
        {
            throw new SystemException($"List index cannot be negative: {key}: {index}");
        }
        if (value.Contains(','))
        {
            throw new SystemException($"List items cannot contain commas: {key}: {value}");
        }
        string list = Data.Get(key);
        List<string> items = [.. list.Split(',')];
        while (items.Count < index)
        {
            items.Add("");
        }
        if (items.Count == index)
        {
            items.Add(value);
        }
        else
        {
            items.Insert(index, value);
        }
        Data.Set(key, string.Join(',', items));
    }

    private void RemoveAtListItem(string key, string index)
    {
        if (!int.TryParse(index, out int tempIndex))
        {
            throw new ArgumentException($"List index is not an integer: {index}");
        }
        RemoveAtListItem(key, tempIndex);
    }

    private void RemoveAtListItem(string key, int index)
    {
        string list = Data.Get(key);
        List<string> items = [.. list.Split(',')];
        if (items.Count <= index)
        {
            return;
        }
        items.RemoveAt(index);
        Data.Set(key, string.Join(',', items));
    }

    #endregion

    #region Array routines

    private void ClearArray(string key)
    {
        if (key == "")
        {
            throw new SystemException("Array keys cannot be blank.");
        }
        var keys = Data.Keys().Where(x => x.StartsWith($"{key}:", OIC)).ToList();
        foreach (string s in keys)
        {
            Data.Remove(s);
        }
    }

    private string GetArrayItem(string key, int y, int x)
    {
        if (key == "")
        {
            throw new SystemException("Array keys cannot be blank.");
        }
        if (y < 0 || x < 0)
        {
            throw new SystemException($"Array indexes cannot be negative: {key}: {y},{x}");
        }
        var itemKey = $"{key}:{y}";
        return GetListItem(itemKey, x);
    }

    private void SetArrayItem(string key, int y, int x, string value)
    {
        if (key == "")
        {
            throw new SystemException("Array keys cannot be blank.");
        }
        if (y < 0 || x < 0)
        {
            throw new SystemException($"Array indexes cannot be negative: {key}: {y},{x}");
        }
        var itemKey = $"{key}:{y}";
        SetListItem(itemKey, x, value);
    }

    #endregion

    #region Undo routines

    /// <summary>
    /// Must make two backups, one for last command, one for this UNDO command which is ignored.
    /// </summary>
    private void PrepareUndoData()
    {
        if (!Data.UseOverlay) return;
        if (!ConvertToBool(Get(UNDO_MODE))) return;
        undo1.Clear();
        foreach (string key in undo2.Keys)
        {
            undo1.Add(key, undo2[key]);
        }
        undo2.Clear();
        foreach (string key in Data.Keys(GrodEnums.WhichData.Overlay))
        {
            undo2.Add(key, Get(key));
        }
    }

    /// <summary>
    /// Restore the undo data from undo1 back into the overlay.
    /// </summary>
    private void RestoreUndoData()
    {
        if (!Data.UseOverlay) return;
        if (!ConvertToBool(Get(UNDO_MODE))) return;
        if (undo1.Count == 0) return;
        Data.Clear(GrodEnums.WhichData.Overlay);
        foreach (string key in undo1.Keys)
        {
            Set(key, undo1[key]);
        }
        ClearUndoData();
    }

    /// <summary>
    /// Clear the undo data until the next time.
    /// </summary>
    private void ClearUndoData()
    {
        undo1.Clear();
        undo2.Clear();
    }

    #endregion

    #region Again command

    /// <summary>
    /// Must make two copies, one for last command, one for this AGAIN command which is ignored.
    /// </summary>
    private void SaveLastCommand(string script)
    {
        saveCommand1 = saveCommand2;
        saveCommand2 = script;
    }

    #endregion
}
