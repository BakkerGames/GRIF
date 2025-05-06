using static GRIFTools.GrodEnums;

namespace GRIFTools;

public partial class Grod
{
    /// <summary>
    /// Indicates if the overlay is used for storing data.
    /// </summary>
    public bool UseOverlay { get; set; } = false;

    /// <summary>
    /// Gets a single value, or "" if not found.
    /// </summary>
    public string Get(string key)
    {
        key = NormalizeKey(key);
        string? item;
        if (UseOverlay && _overlay.TryGetValue(key, out item))
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
    }

    /// <summary>
    /// Returns a list of keys from the proper dictionary(s).
    /// </summary>
    public List<string> Keys(WhichData which = WhichData.Both)
    {
        if (UseOverlay && which == WhichData.Overlay)
        {
            return _overlay.Keys.ToList();
        }
        if (!UseOverlay || which == WhichData.Base)
        {
            return _base.Keys.ToList();
        }
        return _base.Keys.Union(_overlay.Keys).ToList();
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
        _base.Remove(key);
        _overlay.Remove(key);
    }
}
