using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace GROD;

/// <summary>
/// GROD - Game Resource Overlay Dictionary
/// </summary>
public partial class Grod : IDictionary<string, string>
{
    private readonly Dictionary<string, string> _base = [];
    private readonly Dictionary<string, string> _overlay = [];

    /// <summary>
    /// Enable or disable the overlay collection.
    /// </summary>
    public bool UseOverlay { get; set; } = false;

    /// <summary>
    /// Gets or sets the element with the specified key. Returns "" or adds the item if key is not found. Key cannot be null or only whitespace. Null values are changed to "".
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public string this[string key]
    {
        get
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (UseOverlay && _overlay.TryGetValue(key, out string? value1))
                return value1 ?? "";
            else if (_base.TryGetValue(key, out string? value2))
                return value2 ?? "";
            else
                return "";
        }
        set
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            value ??= "";
            if (UseOverlay)
            {
                if (!_overlay.TryAdd(key, value))
                    _overlay[key] = value;
            }
            else
            {
                if (!_base.TryAdd(key, value))
                    _base[key] = value;
            }
        }
    }

    public ICollection<string> Keys =>
        UseOverlay ? _base.Keys.Union(_overlay.Keys).ToList() : _base.Keys;

    /// <summary>
    /// Gets a list containing the keys from the overlay collection.
    /// </summary>
    public ICollection<string> KeysOverlay =>
        UseOverlay ? _overlay.Keys : new List<string>();

    public ICollection<string> Values =>
        UseOverlay ? AllValues() : _base.Values;

    /// <summary>
    /// Gets the number of items in the base or base plus overlay collection, depending on UseOverlay. Counts duplicate keys only once.
    /// </summary>
    public int Count =>
        (UseOverlay ? _base.Keys.Union(_overlay.Keys) : _base.Keys).Count();

    public bool IsReadOnly => false;

    /// <summary>
    /// Adds or updates the element with the specified key and value. Key cannot be null or only whitespace. Null values are changed to "". Does not throw an error if it already exists.
    /// </summary>
    public void Add(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));
        this[key] = value;
    }

    /// <summary>
    /// Adds or updates the element with the specified KeyValuePair. Key cannot be null or only whitespace. Null values are changed to "". Does not throw an error if it already exists.
    /// </summary>
    public void Add(KeyValuePair<string, string> item)
    {
        if (string.IsNullOrWhiteSpace(item.Key))
            throw new ArgumentNullException(nameof(item));
        this[item.Key] = item.Value;
    }

    /// <summary>
    /// Removes all items from both the base and overlay collections.
    /// </summary>
    public void Clear()
    {
        _overlay.Clear();
        _base.Clear();
    }

    /// <summary>
    /// Removes all items from the base collection.
    /// </summary>
    public void ClearBase()
    {
        _base.Clear();
    }

    /// <summary>
    /// Removes all items from the overlay collection.
    /// </summary>
    public void ClearOverlay()
    {
        _overlay.Clear();
    }

    public bool Contains(KeyValuePair<string, string> item)
    {
        if (string.IsNullOrWhiteSpace(item.Key))
            throw new ArgumentNullException(nameof(item));
        if (UseOverlay && _overlay.TryGetValue(item.Key, out string? value1))
            return (value1 ?? "") == (item.Value ?? "");
        if (_base.TryGetValue(item.Key, out string? value2))
            return (value2 ?? "") == (item.Value ?? "");
        return false;
    }

    public bool ContainsKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));
        return (UseOverlay && _overlay.ContainsKey(key)) || _base.ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
    {
        var i = 0;
        foreach (string key in Keys)
        {
            array[i + arrayIndex] = new KeyValuePair<string, string>(key, this[key]);
            i++;
        }
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        List<KeyValuePair<string, string>> result = [];
        foreach (string key in Keys)
            result.Add(new KeyValuePair<string, string>(key, this[key]));
        return (IEnumerator<KeyValuePair<string, string>>)result;
    }

    /// <summary>
    /// Merge all overlay key/values into base and clears overlay.
    /// </summary>
    public void MergeOverlay()
    {
        if (UseOverlay)
        {
            foreach (string key in _overlay.Keys)
            {
                if (!_base.TryAdd(key, _overlay[key]))
                    _base[key] = _overlay[key];
            }
            _overlay.Clear();
        }
    }

    public bool Remove(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));
        bool result = _base.Remove(key);
        if (UseOverlay)
            result |= _overlay.Remove(key);
        return result;
    }

    public bool Remove(KeyValuePair<string, string> item)
    {
        if (string.IsNullOrWhiteSpace(item.Key))
            throw new ArgumentNullException(nameof(item));
        bool result = _base.Contains(item) && _base.Remove(item.Key);
        if (UseOverlay)
            result |= _overlay.Contains(item) && _overlay.Remove(item.Key);
        return result;
    }

    /// <summary>
    /// Revert the overlay value back to the base value
    /// </summary>
    public void Revert(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));
        if (UseOverlay)
            _overlay.Remove(key);
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));
        if (UseOverlay && _overlay.ContainsKey(key))
            return _overlay.TryGetValue(key, out value);
        else
            return _base.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return (IEnumerator)(UseOverlay ? _base.Union(_overlay) : _base);
    }

    #region Private

    private List<string> AllValues()
    {
        List<string> result = [];
        foreach (string key in Keys)
            result.Add(this[key]);
        return result;
    }

    #endregion
}