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
        if (_deleted.Contains(key))
        {
            return "";
        }
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

    /// <summary>
    /// Return the value, or the supplied default value if not found
    /// </summary>
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
        _deleted.Remove(key);
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
        if (which == WhichData.Both)
        {
            _deleted.Clear();
        }
    }

    /// <summary>
    /// Returns a list of keys from the dictionary(s).
    /// </summary>
    public List<string> Keys(WhichData which = WhichData.Both)
    {
        List<string> result = [];
        if (UseOverlay && which == WhichData.Overlay)
        {
            result = [.. _overlay.Keys];
        }
        else if (!UseOverlay || which == WhichData.Base)
        {
            result = [.. _base.Keys];
        }
        else
        {
            result = [.. _base.Keys.Union(_overlay.Keys)];
        }
        result.RemoveAll(x => _deleted.Contains(x));
        return result;
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
    /// Indicates whether the dictionary(s) contains the key.
    /// </summary>
    public bool ContainsKey(string key)
    {
        key = NormalizeKey(key);
        if (_deleted.Contains(key))
        {
            return false;
        }
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
    /// Adds the key to the _deleted list, but it may still exist in the dictionaries
    /// </summary>
    public void Delete(string key)
    {
        key = NormalizeKey(key);
        if (!_deleted.Contains(key))
        {
            _deleted.Add(key);
        }
    }

    /// <summary>
    /// Remove the key from the _deleted list
    /// </summary>
    public void Undelete(string key)
    {
        key = NormalizeKey(key);
        _deleted.Remove(key);
    }

    /// <summary>
    /// Reset the overlay value to the base value
    /// </summary>
    public void ResetValue(string key)
    {
        if (UseOverlay)
        {
            key = NormalizeKey(key);
            if (!_deleted.Contains(key))
            {
                if (_base.TryGetValue(key, out var value))
                {
                    _overlay[key] = value ?? "";
                }
                else
                {
                    _overlay[key] = "";
                }
            }
        }
    }
}
