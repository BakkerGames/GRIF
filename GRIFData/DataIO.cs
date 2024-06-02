using DAGS;
using GROD;
using System.Globalization;
using System.Text;

namespace GRIFData;

public static class DataIO
{
    /// <summary>
    /// Read file data and add to GROD. Data is not cleared. Duplicate keys are overwritten.
    /// </summary>
    public static void LoadDataFromFile(string path, Grod grod)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(path);
        }

        LoadDataFromString(File.ReadAllText(path), grod);
    }

    /// <summary>
    /// Read string data and add to GROD. Data is not cleared. Duplicate keys are overwritten.
    /// </summary>
    public static void LoadDataFromString(string data, Grod grod)
    {
        var index = 0;

        try
        {
            SkipWhitespace(data, ref index);
            if (index < data.Length && data[index] == '{')
            {
                index++;
                while (index < data.Length && data[index] != '}')
                {
                    (string key, string value) = GetKeyValue(data, ref index);
                    if (key == "") continue;
                    grod[key] = value;
                }
            }
        }
        catch (Exception ex)
        {
            throw new SystemException("Error loading data: " + ex.Message);
        }
    }

    /// <summary>
    /// Save all GROD data to a file in GRIF format.
    /// </summary>
    public static void SaveDataToFile(string path, Grod grod)
    {
        var keys = grod.Keys.ToList();
        File.WriteAllText(path, ExportData(grod, keys));
    }

    /// <summary>
    /// Save only the GROD Overlay data to a file in GRIF format.
    /// </summary>
    public static void SaveOverlayDataToFile(string path, Grod grod)
    {
        var keys = grod.KeysOverlay.ToList();
        File.WriteAllText(path, ExportData(grod, keys));
    }

    /// <summary>
    /// Save all GROD data to a string in GRIF format.
    /// </summary>
    public static string SaveDataToString(Grod grod)
    {
        var keys = grod.Keys.ToList();
        return ExportData(grod, keys);
    }

    /// <summary>
    /// Save only the GROD Overlay data to a string in GRIF format.
    /// </summary>
    public static string SaveOverlayDataToString(Grod grod)
    {
        var keys = grod.KeysOverlay.ToList();
        return ExportData(grod, keys);
    }

    #region Private

    private static readonly StringComparison OIC = StringComparison.OrdinalIgnoreCase;

    private static string ExportData(Grod grod, List<string> keys)
    {
        keys.Sort(CompareKeys);
        StringBuilder result = new();
        result.AppendLine("{");
        foreach (string key in keys)
        {
            var value = grod[key];
            result.Append("\t\"");
            result.Append(EncodeString(key));
            result.Append("\":");
            if (value.TrimStart().StartsWith('@'))
            {
                result.AppendLine();
                result.Append("\t\t\"");
                try
                {
                    value = Dags.PrettyScript(value);
                    value = value.TrimStart().Replace("\r\n", "\r\n\t\t");
                }
                catch (Exception)
                {
                    // don't format
                }
                result.Append(EncodeString(value));
            }
            else
            {
                result.Append(" \"");
                result.Append(EncodeString(value));
            }
            result.AppendLine("\",");
        }
        result.AppendLine("}");
        return result.ToString();
    }

    private static void SkipWhitespace(string data, ref int index)
    {
        bool found;
        do
        {
            found = false;
            while (index < data.Length && char.IsWhiteSpace(data[index]))
            {
                index++;
                found = true;
            }
            // skip // comments until newline
            if (index < data.Length - 1 && data.Substring(index, 2) == "//")
            {
                found = true;
                index += 2;
                while (index < data.Length && data[index] != '\n')
                {
                    index++;
                }
                index++;
            }
            // skip /* */ comments even across lines
            if (index < data.Length - 3 && data.Substring(index, 2) == "/*")
            {
                found = true;
                index += 2;
                while (index < data.Length - 1 && data.Substring(index, 2) != "*/")
                {
                    index++;
                }
                index += 2;
            }
        } while (found && index < data.Length);
    }

    private static (string key, string value) GetKeyValue(string data, ref int index)
    {
        string key = "";
        string value = "";
        try
        {
            SkipWhitespace(data, ref index);
            while (data[index] == ',')
            {
                index++;
                SkipWhitespace(data, ref index);
            }
            if (data[index] == '}')
            {
                return (key, value);
            }
            if (data[index] != '"')
            {
                throw new InvalidDataException($"Invalid char at {index} - \"{data[index]}\" should be quote");
            }
            key = GetString(data, ref index);
            SkipWhitespace(data, ref index);
            if (data[index] != ':')
            {
                throw new InvalidDataException($"Invalid char at {index} - \"{data[index]}\" should be \":\"");
            }
            index++;
            SkipWhitespace(data, ref index);
            if (data[index] != '"')
            {
                throw new InvalidDataException($"Invalid char at {index} - \"{data[index]}\" should be quote");
            }
            value = GetString(data, ref index);
            SkipWhitespace(data, ref index);
            if (data[index] != ',' && data[index] != '}')
            {
                throw new InvalidDataException($"Invalid char at {index} - \"{data[index]}\" should be comma or \"}}\"");
            }
            while (data[index] == ',')
            {
                index++;
                SkipWhitespace(data, ref index);
            }
            return (key, value);
        }
        catch (Exception ex)
        {
            if (index >= data.Length)
            {
                throw new InvalidDataException("Unexpected end of file");
            }
            if (key != "")
            {
                throw new InvalidDataException($"Key \"{key}\": {ex.Message}");
            }
            throw;
        }
    }

    private static string GetString(string data, ref int index)
    {
        StringBuilder result = new();
        var lastSlash = false;
        index++; // skip first quote
        while (lastSlash || data[index] != '"')
        {
            var c = data[index++];
            if (lastSlash)
            {
                lastSlash = false;
                if (c == 'n')
                    result.Append('\n');
                else if (c == 'r')
                    result.Append('\r');
                else if (c == 't')
                    result.Append('\t');
                else if (c == '"' || c == '\\' || c == '/')
                {
                    result.Append(c);
                }
                else if (c == 'u')
                {
                    if (index + 4 >= data.Length)
                    {
                        throw new InvalidDataException($"Parsing \"u####\" failed, index={index}, not enough chars");
                    }
                    var hex = data[index..(index + 4)];
                    if (!int.TryParse(hex, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out int value))
                    {
                        throw new InvalidDataException($"Parsing \"u####\" failed, index={index}, invalid hexadecimal \"{hex}\"");
                    }
                    result.Append((char)value);
                    index += 4;
                }
                else
                {
                    throw new InvalidDataException($"Unexpected escaped char: \"\\{c}\"");
                }
            }
            else if (c == '\\')
            {
                lastSlash = true;
            }
            else
            {
                result.Append(c);
            }
        }
        index++;
        return result.ToString();
    }

    private static string EncodeString(string value)
    {
        StringBuilder result = new();
        var isScript = value.StartsWith('@');
        foreach (char c in value)
        {
            if (c < ' ' || c > '~')
            {
                if (isScript && (c == '\r' || c == '\n' || c == '\t'))
                {
                    result.Append(c);
                }
                else if (c == '\r')
                {
                    result.Append("\\r");
                }
                else if (c == '\n')
                {
                    result.Append("\\n");
                }
                else if (c == '\t')
                {
                    result.Append("\\t");
                }
                else
                {
                    result.Append("\\u");
                    result.Append($"{(int)c:x4}");
                }
            }
            else if (c == '"' || c == '\\')
            {
                result.Append('\\');
                result.Append(c);
            }
            else
            {
                result.Append(c);
            }
        }
        return result.ToString();
    }

    /// <summary>
    /// Key comparison function, returns -1/0/1. Used in keys.Sort(CompareKeys);
    /// Handles numeric key sections in numeric order, not alphabetic order.
    /// </summary>
    private static int CompareKeys(string x, string y)
    {
        if (x == null)
        {
            if (y == null) return 0;
            return -1;
        }
        if (y == null)
        {
            return 1;
        }
        if (x.Equals(y, OIC)) return 0;
        var xTokens = x.Split('.');
        var yTokens = y.Split('.');
        for (int i = 0; i < Math.Max(xTokens.Length, yTokens.Length); i++)
        {
            if (i >= xTokens.Length) return -1; // x is shorter and earlier
            if (i >= yTokens.Length) return 1; // y is shorter and earlier
            if (xTokens[i].Equals(yTokens[i], OIC)) continue;
            if (xTokens[i] == "*") return -1; // "*" comes first so x is earlier
            if (yTokens[i] == "*") return 1; // "*" comes first so y is earlier
            if (xTokens[i] == "?") return -1; // "?" comes next so x is earlier
            if (yTokens[i] == "?") return 1; // "?" comes next so y is earlier
            if (xTokens[i] == "#") return -1; // "#" comes next so x is earlier
            if (yTokens[i] == "#") return 1; // "#" comes next so y is earlier
            if (int.TryParse(xTokens[i], out int xVal) && int.TryParse(yTokens[i], out int yVal))
            {
                if (xVal == yVal) continue;
                return (xVal < yVal) ? -1 : 1;
            }
            return string.Compare(xTokens[i], yTokens[i], OIC);
        }
        return 0;
    }

    #endregion
}
