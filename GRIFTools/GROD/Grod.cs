using GRIFTools.GROD;
using static GRIFTools.GrodEnums;

namespace GRIFTools;

public partial class Grod
{
    /// <summary>
    /// Indicates if the overlay is used for storing data.
    /// </summary>
    public bool UseOverlay { get; set; } = false;

    /// <summary>
    /// Indicates if the Undo snapshots are tracked.
    /// </summary>
    public bool AllowUndo { get; set; } = false;

    /// <summary>
    /// Gets a single value, or "" if not found.
    /// </summary>
    public string Get(string key)
    {
        key = NormalizeKey(key);
        if (UseOverlay && _overlay.TryGetValue(key, out string? item))
        {
            return item ?? "";
        }
        if (_base.TryGetValue(key, out item))
        {
            return item ?? "";
        }
        return "";
    }

    public string GetOrDefault(string key, string value)
    {
        var result = Get(key);
        if (result == "") result = value;
        return result;
    }

    /// <summary>
    /// Saves a single value into the proper dictionary.
    /// </summary>
    public void Set(string key, string? item)
    {
        key = NormalizeKey(key);
        if (AllowUndo)
        {
            _snapshot.Items.Push(new UndoItem(key, Get(key)));
        }
        if (UseOverlay)
        {
            if (!_overlay.TryAdd(key, item ?? ""))
            {
                _overlay[key] = item ?? "";
            }
        }
        else if (!_base.TryAdd(key, item ?? ""))
        {
            _base[key] = item ?? "";
        }
    }

    /// <summary>
    /// Clears the specified dictionary(s).
    /// </summary>
    public void Clear(WhichData which = WhichData.Both)
    {
        if (which == WhichData.Both || which == WhichData.Base)
        {
            _base.Clear();
        }
        if (which == WhichData.Both || which == WhichData.Overlay)
        {
            _overlay.Clear();
        }
        if (AllowUndo)
        {
            _undo.Clear();
        }
    }

    /// <summary>
    /// Returns a list of keys from the proper dictionary(s).
    /// </summary>
    public List<string> Keys(WhichData which = WhichData.Both)
    {
        if (UseOverlay && which == WhichData.Overlay)
        {
            return [.. _overlay.Keys];
        }
        if (!UseOverlay || which == WhichData.Base)
        {
            return [.. _base.Keys];
        }
        return [.. _base.Keys.Union(_overlay.Keys)];
    }

    /// <summary>
    /// Count the number of items the specified dictionary(s).
    /// </summary>
    public int Count(WhichData which = WhichData.Both)
    {
        // count the Keys so duplicates are only counted once
        return Keys(which).Count;
    }

    /// <summary>
    /// Indicates whether the proper dictionary(s) contains the key.
    /// </summary>
    public bool ContainsKey(string key)
    {
        key = NormalizeKey(key);
        if (_base.ContainsKey(key))
        {
            return true;
        }
        if (UseOverlay && _overlay.ContainsKey(key))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Removed the specified key from both dictonaries.
    /// </summary>
    public void Remove(string key)
    {
        key = NormalizeKey(key);
        if (AllowUndo)
        {
            _snapshot.Items.Push(new UndoItem(key, Get(key)));
        }
        _base.Remove(key);
        _overlay.Remove(key);
    }

    /// <summary>
    /// Save snapshot image into undo stack
    /// </summary>
    public void SaveSnapshot()
    {
        if (AllowUndo && _snapshot.Items.Count > 0)
        {
            _undo.Push(_snapshot);
            _snapshot.Items.Clear();
        }
    }

    /// <summary>
    /// Undo one snapshot image
    /// </summary>
    public void UndoSnapshot()
    {
        if (AllowUndo && _undo.Count > 0)
        {
            var snapshot = _undo.Pop();
            while (snapshot.Items.Count > 0)
            {
                var item = snapshot.Items.Pop();
                if (UseOverlay)
                {
                    if (!_overlay.TryAdd(item.Key, item.OldValue))
                    {
                        _overlay[item.Key] = item.OldValue;
                    }
                }
                else if (!_base.TryAdd(item.Key, item.OldValue))
                {
                    _base[item.Key] = item.OldValue;
                }
            }
        }
    }
}
