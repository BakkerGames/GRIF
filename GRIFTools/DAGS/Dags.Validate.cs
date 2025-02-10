using System.Text;
using static DAGSScriptLibrary.DAGSConstants;

namespace GRIFTools;

public partial class Dags
{
    /// <summary>
    /// Validates the information in the dictionary to make sure it will work with DAGS.
    /// </summary>
    public bool ValidateDictionary(StringBuilder result)
    {
        var ok = true;
        foreach (string s in Data.Keys())
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                ok = false;
                result.AppendLine("Empty or whitespace key found");
            }
            else
            {
                var value = Data.Get(s)?.TrimStart() ?? "";
                if (value.StartsWith('@'))
                {
                    ok = ok && ValidateScript(value, result);
                }
            }
        }
        return ok;
    }

    /// <summary>
    /// Validates one script to make sure the syntax and function names are correct.
    /// </summary>
    public bool ValidateScript(string script, StringBuilder result)
    {
        if (string.IsNullOrWhiteSpace(script) || !script.TrimStart().StartsWith('@'))
        {
            return true;
        }
        if (!ValidateSyntax(script, result))
        {
            return false;
        }

        bool ok = true;
        int index = 0;
        var tokens = SplitTokens(script);
        while (index < tokens.Length)
        {
            var token = tokens[index++];
            if (token.StartsWith('@'))
            {
                var found = false;
                foreach (string key in KEYWORDS)
                {
                    if (token.Equals(key, OIC))
                    {
                        found = true;
                        break;
                    }
                }
                if (found) continue;
                if (token.EndsWith('('))
                {
                    var dict = GetByPrefix(token);
                    if (dict.Count == 0)
                    {
                        ok = false;
                        result.AppendLine($"Function not found: {token}");
                    }
                }
                else
                {
                    var value = Get(token);
                    if (value == "")
                    {
                        ok = false;
                        result.AppendLine($"Function not found: {token}");
                    }
                }
            }
        }
        return ok;
    }

    /// <summary>
    /// Validates one script to make sure the syntax is correct.
    /// </summary>
    public static bool ValidateSyntax(string script, StringBuilder result)
    {
        if (string.IsNullOrWhiteSpace(script) || !script.TrimStart().StartsWith('@'))
        {
            return true;
        }
        try
        {
            bool ok = true;
            var tokens = SplitTokens(script);
            int index = 0;
            int parenLevel = 0;
            int ifCount = 0;
            int elseifCount = 0;
            int thenCount = 0;
            int endifCount = 0;
            int forLevel = 0;
            int forEachKeyLevel = 0;
            int forEachListLevel = 0;
            string ifLast = ENDIF;
            while (index < tokens.Length)
            {
                if (tokens[index].Equals(IF, OIC))
                {
                    if (ifLast != THEN && ifLast != ELSE && ifLast != ENDIF)
                    {
                        ok = false;
                        throw new SystemException($"{IF} at {index} is invalid.");
                    }
                    ifCount++;
                    ifLast = IF;
                }
                else if (tokens[index].Equals(THEN, OIC))
                {
                    if (ifLast != IF & ifLast != ELSEIF)
                    {
                        ok = false;
                        throw new SystemException($"{THEN} at {index} is invalid.");
                    }
                    thenCount++;
                    ifLast = THEN;
                }
                else if (tokens[index].Equals(ELSEIF, OIC))
                {
                    if (ifLast != THEN && ifLast != ENDIF)
                    {
                        ok = false;
                        throw new SystemException($"{ELSEIF} at {index} is invalid.");
                    }
                    elseifCount++;
                    ifLast = ELSEIF;
                }
                else if (tokens[index].Equals(ELSE, OIC))
                {
                    if (ifLast != THEN && ifLast != ENDIF)
                    {
                        ok = false;
                        throw new SystemException($"{ELSE} at {index} is invalid.");
                    }
                    ifLast = ELSE;
                }
                else if (tokens[index].Equals(ENDIF, OIC))
                {
                    if (ifLast != THEN && ifLast != ELSE && ifLast != ENDIF)
                    {
                        ok = false;
                        throw new SystemException($"{ENDIF} at {index} is invalid.");
                    }
                    endifCount++;
                    ifLast = ENDIF;
                }
                else if (tokens[index].Equals(FOR, OIC))
                {
                    forLevel++;
                }
                else if (tokens[index].Equals(ENDFOR, OIC))
                {
                    forLevel--;
                }
                else if (tokens[index].Equals(FOREACHKEY, OIC))
                {
                    forEachKeyLevel++;
                }
                else if (tokens[index].Equals(ENDFOREACHKEY, OIC))
                {
                    forEachKeyLevel--;
                }
                else if (tokens[index].Equals(FOREACHLIST, OIC))
                {
                    forEachListLevel++;
                }
                else if (tokens[index].Equals(ENDFOREACHLIST, OIC))
                {
                    forEachListLevel--;
                }
                if (tokens[index].StartsWith('@'))
                {
                    if (tokens[index].EndsWith('('))
                    {
                        parenLevel++;
                    }
                }
                else
                {
                    if (tokens[index] == ")")
                    {
                        parenLevel--;
                    }
                }
                index++;
            }
            if (parenLevel != 0)
            {
                ok = false;
                result.AppendLine("Mismatched parenthesis");
            }
            if (ifCount != endifCount)
            {
                ok = false;
                result.AppendLine($"Mismatched {IF}/{ENDIF} counts");
            }
            if (ifCount + elseifCount != thenCount)
            {
                ok = false;
                result.AppendLine($"Mismatched {IF}/{ELSEIF} vs {THEN} counts");
            }
            if (forLevel != 0)
            {
                ok = false;
                result.AppendLine($"Mismatched {FOR.Replace("(", "")}/{ENDFOR}");
            }
            if (forEachKeyLevel != 0)
            {
                ok = false;
                result.AppendLine($"Mismatched {FOREACHKEY.Replace("(", "")}/{ENDFOREACHKEY}");
            }
            if (forEachListLevel != 0)
            {
                ok = false;
                result.AppendLine($"Mismatched {FOREACHLIST.Replace("(", "")}/{ENDFOREACHLIST}");
            }
            return ok;
        }
        catch (Exception ex)
        {
            result.AppendLine(ex.Message);
            return false;
        }
    }

    #region Keywords

    private static readonly List<string> KEYWORDS =
    [
        ABS,
        ADD,
        ADDLIST,
        ADDTO,
        AND,
        CLEARARRAY,
        CLEARLIST,
        COMMENT,
        CONCAT,
        DIV,
        DIVTO,
        ELSE,
        ELSEIF,
        ENDFOR,
        ENDFOREACHKEY,
        ENDFOREACHLIST,
        ENDIF,
        EQ,
        EXEC,
        EXISTS,
        FALSE,
        FOR,
        FOREACHKEY,
        FOREACHLIST,
        FORMAT,
        GE,
        GET,
        GETARRAY,
        GETINCHANNEL,
        GETLIST,
        GETVALUE,
        GOLABEL,
        GT,
        IF,
        INSERTATLIST,
        ISBOOL,
        ISNUMBER,
        ISSCRIPT,
        LABEL,
        LE,
        LISTLENGTH,
        LOWER,
        LT,
        MOD,
        MODTO,
        MSG,
        MUL,
        MULTO,
        NE,
        NEG,
        NEGTO,
        NL,
        NOT,
        NULL,
        OR,
        RAND,
        REMOVEATLIST,
        REPLACE,
        RETURN,
        RND,
        SCRIPT,
        SET,
        SETARRAY,
        SETLIST,
        SETOUTCHANNEL,
        SUB,
        SUBSTRING,
        SUBTO,
        SWAP,
        THEN,
        TRIM,
        TRUE,
        UPPER,
        WRITE,
        WRITELINE,
    ];

    #endregion
}
