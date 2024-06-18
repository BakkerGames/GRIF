using System.Text;
using static DAGS.Constants;

namespace DAGS;

public partial class Dags
{
    /// <summary>
    /// Converts a string to an integer, or throws an error
    /// </summary>
    private static int ConvertToInt(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Equals(NULL_VALUE, OIC) || value.Equals(FALSE_VALUE, OIC))
        {
            return 0;
        }
        if (value.Equals(TRUE_VALUE, OIC))
        {
            return 1;
        }
        if (int.TryParse(value.Trim(), out int answer))
        {
            return answer;
        }
        throw new SystemException($"Value is not numeric: {value}");
    }

    /// <summary>
    /// Converts a truthy or falsey value to boolean, or throws an error
    /// </summary>
    private static bool ConvertToBool(string value)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            value.Equals(NULL_VALUE, OIC) ||
            value.Equals(FALSE_VALUE, OIC) ||
            value.Equals("f", OIC) ||
            value.Equals("off", OIC) ||
            value.Equals("no", OIC) ||
            value.Equals("n", OIC) ||
            value == "0")
        {
            return false;
        }
        if (value.Equals(TRUE_VALUE, OIC) ||
            value.Equals("t", OIC) ||
            value.Equals("on", OIC) ||
            value.Equals("yes", OIC) ||
            value.Equals("y", OIC) ||
            value == "1" ||
            value == "-1")
        {
            return true;
        }
        throw new SystemException($"Value is not boolean: {value}");
    }

    /// <summary>
    /// Converts a true or false to a boolean string
    /// </summary>
    private static string ConvertToBoolString(bool value)
    {
        return value ? TRUE_VALUE : FALSE_VALUE;
    }

    /// <summary>
    /// Split the script into tokens for processing
    /// </summary>
    private static string[] SplitTokens(string script)
    {
        List<string> result = [];
        StringBuilder token = new();
        bool inToken = false;
        bool inQuote = false;
        bool inAtFunction = false;
        bool lastSlash = false;
        foreach (char c in script)
        {
            if (inQuote)
            {
                if (c == '"' && !lastSlash)
                {
                    token.Append(c);
                    if (inAtFunction)
                    {
                        result.Add(token.ToString().ToLower());
                        inAtFunction = false;
                    }
                    else
                    {
                        result.Add(token.ToString().Replace("\\\"", "\""));
                    }
                    token.Clear();
                    inQuote = false;
                    inToken = false;
                    continue;
                }
                if (c == '"' && lastSlash)
                {
                    token.Append("\\\\\"");
                    lastSlash = false;
                    continue;
                }
                if (c == '\\' && !lastSlash)
                {
                    lastSlash = true;
                    continue;
                }
                if (lastSlash)
                {
                    token.Append('\\');
                    lastSlash = false;
                }
                token.Append(c);
                continue;
            }
            if (c == ',' || c == ')')
            {
                if (token.Length > 0)
                {
                    if (inAtFunction)
                    {
                        result.Add(token.ToString().ToLower());
                        inAtFunction = false;
                    }
                    else
                    {
                        result.Add(token.ToString());
                    }
                    token.Clear();
                }
                result.Add(c.ToString());
                inToken = false;
                continue;
            }
            if (!inToken)
            {
                if (char.IsWhiteSpace(c))
                {
                    continue;
                }
                if (c == '"')
                {
                    inQuote = true;
                    token.Append(c);
                }
                else
                {
                    inAtFunction = (c == '@');
                    token.Append(c);
                }
                inToken = true;
                continue;
            }
            if (c == '@')
            {
                result.Add(token.ToString());
                token.Clear();
                token.Append(c);
                continue;
            }
            if (c == '(')
            {
                if (token.Length > 0)
                {
                    token.Append(c);
                    if (inAtFunction)
                    {
                        result.Add(token.ToString().ToLower());
                        inAtFunction = false;
                    }
                    else
                    {
                        result.Add(token.ToString());
                    }
                    token.Clear();
                }
                inToken = false;
                continue;
            }
            if (char.IsWhiteSpace(c))
            {
                if (token.Length > 0)
                {
                    if (inAtFunction)
                    {
                        result.Add(token.ToString().ToLower());
                        inAtFunction = false;
                    }
                    else
                    {
                        result.Add(token.ToString());
                    }
                    token.Clear();
                }
                inToken = false;
                continue;
            }
            token.Append(c);
        }
        if (token.Length > 0)
        {
            if (inAtFunction)
            {
                result.Add(token.ToString().ToLower());
            }
            else
            {
                result.Add(token.ToString());
            }
            token.Clear();
        }
        return [.. result];
    }

    /// <summary>
    /// Check that the number of parameters is correct, or throws an error
    /// </summary>
    private static void CheckParamCount(string token, List<string> paramList, int expected)
    {
        if (paramList.Count == expected)
            return;
        throw new SystemException($"Incorrect number of parameters: {token}({expected}) - Found: {paramList.Count}");
    }

    /// <summary>
    /// Check for at least the specified number of parameters, or throws an error
    /// </summary>
    private static void CheckParamCountAtLeast(string token, List<string> paramList, int expected)
    {
        if (paramList.Count >= expected)
            return;
        throw new SystemException($"Incorrect number of parameters: {token}({expected}) - Found: {paramList.Count}");
    }

    /// <summary>
    /// Expand a value containing a list into a list of strings
    /// </summary>
    private static List<string> ExpandList(string value)
    {
        List<string> result = [];
        bool inQuote = false;
        bool lastSlash = false;
        StringBuilder token = new();
        value = value.Trim();
        var startAt = 0;
        var endAt = value.Length;
        if (value.StartsWith('[')) startAt++;
        if (value.EndsWith(']')) endAt--;
        if (startAt >= endAt)
        {
            return result; // empty list
        }
        for (int i = startAt; i < endAt; i++)
        {
            char c = value[i];
            if (!inQuote && (c == '[' || c == ']'))
            {
                throw new SystemException($"Unexpected character within list: {c}");
            }
            if (inQuote)
            {
                if (c == '\\' && !lastSlash)
                {
                    lastSlash = true;
                    continue;
                }
                if (lastSlash)
                {
                    switch (c)
                    {
                        case 't':
                            token.Append('\t');
                            break;
                        case 'n':
                            token.Append('\n');
                            break;
                        case 'r':
                            token.Append('\r');
                            break;
                        default:
                            token.Append(c);
                            break;
                    }
                    lastSlash = false;
                    continue;
                }
                if (c == '"')
                {
                    inQuote = false;
                    continue;
                }
                token.Append(c);
                continue;
            }
            if (c == ',')
            {
                if (token.Length >= 2 && token[0] == '"' && token[^1] == '"')
                {
                    result.Add(token.ToString()[1..^1]);
                }
                else
                {
                    result.Add(token.ToString());
                }
                token.Clear();
                continue;
            }
            token.Append(c);
        }
        if (token.Length >= 2 && token[0] == '"' && token[^1] == '"')
        {
            result.Add(token.ToString()[1..^1]);
        }
        else
        {
            result.Add(token.ToString());
        }
        return result;
    }

    /// <summary>
    /// Compress a list of strings into a value
    /// </summary>
    private static string CollapseList(List<string> list)
    {
        StringBuilder result = new();
        result.Append('[');
        bool addComma = false;
        foreach (string s in list)
        {
            if (addComma)
                result.Append(',');
            else
                addComma = true;
            var quote = false;
            foreach (char c in s)
            {
                switch (c)
                {
                    case ',':
                    case '"':
                    case ' ':
                    case '\t':
                    case '\n':
                    case '\r':
                    case '\\':
                    case '[':
                    case ']':
                        quote = true;
                        break;
                }
                if (quote)
                    break;
            }
            var value = s;
            if (quote)
            {
                result.Append('"');
                value = value
                    .Replace("\\", "\\\\")
                    .Replace("\"", "\\\"");
                result.Append(value);
                result.Append('"');
            }
            else if (value != NULL_VALUE)
            {
                result.Append(value);
            }
        }
        result.Append(']');
        return result.ToString();
    }
}
