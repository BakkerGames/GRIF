using static GRIFTools.DAGSConstants;

namespace GRIFTools;

public partial class Dags
{
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
        keys = Data.Keys().Where(x => x.StartsWith(prefix, OIC)).ToList();
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
        throw new SystemException($"Value is not an int: {key}: {value}");
    }

    private void SetInt(string key, int value)
    {
        Data.Set(key, value.ToString());
    }

    private string GetListItem(string key, string index)
    {
        if (!int.TryParse(index, out int tempIndex))
        {
            throw new ArgumentException($"Value is not an integer: {index}");
        }
        return GetListItem(key, tempIndex);
    }

    private string GetListItem(string key, int index)
    {
        var itemKey = $"{key}.{index}";
        return Data.Get(itemKey);
    }

    private void SetListItem(string key, string index, string value)
    {
        if (!int.TryParse(index, out int tempIndex))
        {
            throw new ArgumentException($"Value is not an integer: {index}");
        }
        SetListItem(key, tempIndex, value);
    }

    private void SetListItem(string key, int index, string? value)
    {
        if (index < 0)
        {
            throw new SystemException($"List index cannot be negative: {key}: {index}");
        }
        var list = Data.Get(key);
        var items = list.Split(',').ToList<string>();
        while (items.Count < index - 1)
        {
            items.Add("");
        }
        items[index] = value ?? "";
        Data.Set(key, string.Join(',', items));
        // TODO ### left off here
    }

    private void AddListItem(string key, string? value)
    {
        var maxKey = $"{key}.max";
        var max = Data.Get(maxKey);
        var index = 0;
        if (max != "" && int.TryParse(max, out int maxValue))
        {
            index = maxValue + 1;
        }
        Data.Set(maxKey, index.ToString());
        var itemKey = $"{key}.{index}";
        Data.Set(itemKey, value);
    }

    private string GetArrayItem(string key, int y, int x)
    {
        var itemKey = $"{key}.{y}.{x}";
        return Data.Get(itemKey);
    }

    private void SetArrayItem(string key, int y, int x, string value)
    {
        if (y < 0 || x < 0)
        {
            throw new SystemException($"Array indexes cannot be negative: {key}: {y},{x}");
        }
        var yMaxKey = $"{key}.max.y";
        var yMax = Data.Get(yMaxKey);
        if (yMax == "" || !int.TryParse(yMax, out int yMaxValue) || yMaxValue < y)
        {
            Data.Set(yMaxKey, y.ToString());
        }
        var xMaxKey = $"{key}.max.x";
        var xMax = Data.Get(xMaxKey);
        if (xMax == "" || !int.TryParse(xMax, out int xMaxValue) || xMaxValue < x)
        {
            Data.Set(xMaxKey, x.ToString());
        }
        var itemKey = $"{key}.{y}.{x}";
        Data.Set(itemKey, value);
    }
}
