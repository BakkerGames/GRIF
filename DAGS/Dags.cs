using System.Text;
using static DAGS.Constants;

namespace DAGS;

/// <summary>
/// Generate a DAGS script object and assign its dictionary.
/// </summary>
public partial class Dags(IDictionary<string, string> dict)
{
    /// <summary>
    /// Receives metadata from the calling program, such as text input.
    /// </summary>
    public Queue<string> InChannel { get; set; } = new();

    /// <summary>
    /// Sends metadata or commands back to the calling program.
    /// </summary>
    public Queue<string> OutChannel { get; set; } = new();

    /// <summary>
    /// Run one script and return any text in result.
    /// </summary>
    public void RunScript(string script, StringBuilder result)
    {
        if (string.IsNullOrWhiteSpace(script) || script.Equals(NULL_VALUE, OIC))
        {
            return;
        }
        if (!script.TrimStart().StartsWith('@'))
        {
            result.Append(script);
            return;
        }
        try
        {
            var tokens = SplitTokens(script);
            if (_debugLogFlag)
            {
                _debugLogResult.AppendLine();
                _debugLogResult.AppendLine("### RunScript()");
                _debugLogResult.AppendLine(script.Replace("\t", "    "));
            }
            int index = 0;
            while (index < tokens.Length)
            {
                RunOneCommand(tokens, ref index, result);
            }
        }
        catch (Exception ex)
        {
            if (result.Length > 0) result.AppendLine();
            result.AppendLine($"{ex.Message}{Environment.NewLine}{script}");
        }
    }

    /// <summary>
    /// Format the script with line breaks and indents.
    /// </summary>
    public static string PrettyScript(string script)
    {
        if (!script.TrimStart().StartsWith('@'))
        {
            return script;
        }

        StringBuilder result = new();
        int indent = 0;
        int parens = 0;
        bool ifLine = false;
        bool forLine = false;
        bool forEachKeyLine = false;
        bool forEachListLine = false;
        var tokens = SplitTokens(script);

        foreach (string s in tokens)
        {
            switch (s)
            {
                case ELSEIF:
                    if (indent > 0) indent--;
                    break;
                case ELSE:
                    if (indent > 0) indent--;
                    break;
                case ENDIF:
                    if (indent > 0) indent--;
                    break;
                case ENDFOR:
                    if (indent > 0) indent--;
                    break;
                case ENDFOREACHKEY:
                    if (indent > 0) indent--;
                    break;
                case ENDFOREACHLIST:
                    if (indent > 0) indent--;
                    break;
            }
            if (parens == 0)
            {
                if (ifLine)
                {
                    result.Append(' ');
                }
                else
                {
                    if (result.Length > 0)
                    {
                        result.AppendLine();
                    }
                    if (indent > 0)
                    {
                        result.Append(new string('\t', indent));
                    }
                }
            }
            result.Append(s);
            switch (s)
            {
                case IF:
                    ifLine = true;
                    break;
                case ELSEIF:
                    ifLine = true;
                    break;
                case ELSE:
                    indent++;
                    break;
                case THEN:
                    indent++;
                    ifLine = false;
                    break;
                case FOR:
                    forLine = true;
                    break;
                case FOREACHKEY:
                    forEachKeyLine = true;
                    break;
                case FOREACHLIST:
                    forEachListLine = true;
                    break;
            }
            if (s.EndsWith('('))
            {
                parens++;
            }
            else if (s == ")")
            {
                if (parens > 0) parens--;
                if (forLine && parens == 0)
                {
                    forLine = false;
                    indent++;
                }
                else if (forEachKeyLine && parens == 0)
                {
                    forEachKeyLine = false;
                    indent++;
                }
                else if (forEachListLine && parens == 0)
                {
                    forEachListLine = false;
                    indent++;
                }
            }
        }
        return result.ToString();
    }

    /// <summary>
    /// Expand a value containing a list into a list of strings
    /// </summary>
    public static List<string> ExpandList(string value)
    {
        int pos = 0;
        return ExpandList(value, ref pos);
    }

    /// <summary>
    /// Compress a list of strings into a value
    /// </summary>
    public static string CollapseList(List<string> list)
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

    public static List<List<string>> ExpandArray(string list)
    {
        List<List<string>> result = [];
        int pos = 0;
        bool first = true;
        while (pos < list.Length)
        {
            char c = list[pos++];
            if (char.IsWhiteSpace(c)) continue;
            if (c == '[' && first)
            {
                first = false;
                continue;
            }
            if (c == ',') continue;
            if (c == ']') break;
            pos--;
            result.Add(ExpandList(list, ref pos));
        }
        return result;
    }

    public static string CollapseArray(List<List<string>> list)
    {
        StringBuilder result = new();
        result.Append('[');
        var comma = false;
        foreach (var sublist in list)
        {
            if (comma)
                result.Append(',');
            else
                comma = true;
            result.Append(CollapseList(sublist));
        }
        result.Append(']');
        return result.ToString();
    }
}
