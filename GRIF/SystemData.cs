using GRIFTools;
using System.Text;

namespace GRIF;

public static class SystemData
{
    public static void Init(Grod grod, Dags dags)
    {
        SystemData.grod = grod;
        SystemData.dags = dags;
    }

    public static bool SystemValidate(StringBuilder result)
    {
        bool ok = true;
        ok = ok && CheckValue(GAMENAME, result);
        return ok;
    }

    public static string GameName()
    {
        return grod.Get(GAMENAME) ?? "";
    }

    public static bool Uppercase()
    {
        if (grod.ContainsKey(UPPERCASE))
        {
            if (bool.TryParse(grod.Get(UPPERCASE), out bool answer))
            {
                return answer;
            }
        }
        return false;
    }

    public static string DontUnderstand(string input)
    {
        var value = grod.Get(DONT_UNDERSTAND) ?? "";
        if (value.StartsWith('@'))
        {
            StringBuilder result = new();
            dags.RunScript(value, result);
            value = result.ToString();
        }
        if (value == "")
        {
            value = DONT_UNDERSTAND_DEFAULT;
        }
        if (Uppercase())
        {
            input = input.ToUpper();
        }
        if (value.Contains("{0}"))
        {
            return value.Replace("{0}", input);
        }
        return value;
    }

    public static string DoWhatWith(string input)
    {
        var value = grod.Get(DO_WHAT_WITH) ?? "";
        if (value.StartsWith('@'))
        {
            StringBuilder result = new();
            dags.RunScript(value, result);
            value = result.ToString();
        }
        if (value == "")
        {
            value = DO_WHAT_WITH_DEFAULT;
        }
        if (Uppercase())
        {
            input = input.ToUpper();
        }
        if (value.Contains("{0}"))
        {
            return value.Replace("{0}", input);
        }
        return value;
    }

    public static string Prompt()
    {
        if (grod.ContainsKey(PROMPT))
        {
            var value = grod.Get(PROMPT) ?? "";
            if (value.StartsWith('@'))
            {
                StringBuilder result = new();
                dags.RunScript(value, result);
                return result.ToString();
            }
            return value;
        }
        else
        {
            return PROMPT_DEFAULT;
        }
    }

    public static string AfterPrompt()
    {
        if (grod.ContainsKey(AFTER_PROMPT))
        {
            var value = grod.Get(AFTER_PROMPT) ?? "";
            if (value.StartsWith('@'))
            {
                StringBuilder result = new();
                dags.RunScript(value, result);
                return result.ToString();
            }
            return value;
        }
        else
        {
            return AFTER_PROMPT_DEFAULT;
        }
    }

    public static int OutputWidth()
    {
        if (grod.ContainsKey(OUTPUT_WIDTH))
        {
            if (int.TryParse(grod.Get(OUTPUT_WIDTH), out int answer))
            {
                return answer;
            }
        }
        return 0;
    }

    public static string Syntax()
    {
        StringBuilder result = new();
        result.AppendLine("GRIF - Game Runner for Interactive Fiction");
        result.AppendLine();
        result.AppendLine("grif <filename.grif | directory>");
        result.AppendLine("     [-i | --input  <filename>]");
        result.AppendLine("     [-o | --output <filename>]");
        result.AppendLine("     [-m | --mod    <filename.grif | directory>]");
        result.AppendLine();
        result.AppendLine("There may be multiple \"-m\" or \"--mod\" parameters.");
        return result.ToString();
    }

    public static string Intro()
    {
        return grod.ContainsKey(INTRO) ? (grod.Get(INTRO) ?? "") : "";
    }

    #region Private

    private static Grod grod = new();
    private static Dags dags = new(grod);

    private const string AFTER_PROMPT = "system.after_prompt";
    private const string DO_WHAT_WITH = "system.do_what_with";
    private const string DONT_UNDERSTAND = "system.dont_understand";
    private const string GAMENAME = "system.gamename";
    private const string INTRO = "system.intro";
    private const string OUTPUT_WIDTH = "system.output_width";
    private const string PROMPT = "system.prompt";
    private const string UPPERCASE = "system.uppercase";

    private const string AFTER_PROMPT_DEFAULT = "";
    private const string DONT_UNDERSTAND_DEFAULT = "I don't understand \"{0}\".";
    private const string DO_WHAT_WITH_DEFAULT = "Do what with the \"{0}\"?";
    private const string PROMPT_DEFAULT = ">";

    private static bool CheckValue(string key, StringBuilder result)
    {
        if (grod.Get(key) == "")
        {
            result.Append("Missing value: ");
            result.AppendLine(key);
            return false;
        }
        return true;
    }

    #endregion
}
