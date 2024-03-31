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
                    indent--;
                    break;
                case ELSE:
                    indent--;
                    break;
                case ENDIF:
                    indent--;
                    break;
                case ENDFOR:
                    indent--;
                    break;
                case ENDFOREACHKEY:
                    indent--;
                    break;
                case ENDFOREACHLIST:
                    indent--;
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
                parens--;
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
}
