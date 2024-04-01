using DAGS;
using GROD;
using static GRIF.Constants;

namespace GRIF;

public class ParseResult
{
    public string Verb = "";
    public string VerbWord = "";
    public string Noun = "";
    public string NounWord = "";
    public string CommandScript = "";
    public string Error = "";
}

public static class Parse
{
    public static void Init(Grod grod, Dags dags)
    {
        Parse.grod = grod;
        Parse.dags = dags;
    }

    public static ParseResult ParseInput(string input)
    {
        var result = new ParseResult();

        // Clear verb/noun
        grod[$"{INPUT_PREFIX}verb"] = "";
        grod[$"{INPUT_PREFIX}verbword"] = "";
        grod[$"{INPUT_PREFIX}noun"] = "";
        grod[$"{INPUT_PREFIX}nounword"] = "";

        var words = input.Split(' ', SPLIT_OPTIONS);
        if (words.Length == 0)
        {
            return result;
        }

        var tempWord0 = words[0];
        var tempWord1 = words.Length > 1 ? words[1] : "";
        if (SystemData.WordSize() > 0)
        {
            if (tempWord0.Length > SystemData.WordSize())
            {
                tempWord0 = tempWord0[..SystemData.WordSize()];
            }
            if (tempWord1.Length > SystemData.WordSize())
            {
                tempWord1 = tempWord1[..SystemData.WordSize()];
            }
        }

        // check for verb and noun
        foreach (string key in grod.Keys.Where(x => x.StartsWith(VERB_PREFIX, OIC)))
        {
            var verbList = grod[key].Split(',', SPLIT_OPTIONS);
            foreach (string v in verbList)
            {
                var verb = v;
                if (SystemData.WordSize() > 0 && verb.Length > SystemData.WordSize())
                {
                    verb = verb[..SystemData.WordSize()];
                }
                if (verb.Equals(tempWord0, OIC))
                {
                    result.Verb = key[VERB_PREFIX.Length..];
                    result.VerbWord = words[0];
                    break;
                }
            }
            if (result.Verb != "") break;
        }

        if (tempWord1 != "")
        {
            foreach (string key in grod.Keys.Where(x => x.StartsWith(NOUN_PREFIX, OIC)))
            {
                var nounList = grod[key].Split(',', SPLIT_OPTIONS);
                foreach (string n in nounList)
                {
                    var noun = n;
                    if (SystemData.WordSize() > 0 && noun.Length > SystemData.WordSize())
                    {
                        noun = noun[..SystemData.WordSize()];
                    }
                    if (noun.Equals(tempWord1, OIC))
                    {
                        result.Noun = key[NOUN_PREFIX.Length..];
                        result.NounWord = words[1];
                        break;
                    }
                }
                if (result.Noun != "") break;
            }
        }

        if (result.Verb == "")
        {
            if (result.Noun != "")
            {
                result.Error = SystemData.DoWhatWith(result.NounWord);
                return result;
            }
            else
            {
                result.Error = SystemData.DontUnderstand(input);
                return result;
            }
        }

        if (words.Length > 1)
        {
            result.CommandScript = grod[$"{COMMAND_PREFIX}{result.Verb}.{result.Noun}"];
            if (result.CommandScript == "")
            {
                if (grod[$"{COMMAND_PREFIX}{result.Verb}.?"] != "")
                {
                    result.Noun = "?"; // special indicator for any value, like a filename
                    result.NounWord = tempWord1; // actual value entered
                    result.CommandScript = grod[$"{COMMAND_PREFIX}{result.Verb}.?"];
                }
                else if (grod[$"{COMMAND_PREFIX}{result.Verb}.#"] != "" && int.TryParse(tempWord1, out int number))
                {
                    result.Noun = "#"; // special indicator for any number
                    result.NounWord = number.ToString(); // normalized number
                    result.CommandScript = grod[$"{COMMAND_PREFIX}{result.Verb}.#"];
                }
                else
                {
                    result.CommandScript = grod[$"{COMMAND_PREFIX}{result.Verb}.*"];
                }
            }
        }
        else
        {
            result.CommandScript = grod[$"{COMMAND_PREFIX}{result.Verb}"];
        }

        if (result.CommandScript == "")
        {
            result.Error = SystemData.DontUnderstand(input);
            return result;
        }

        // Send verb/noun for use in scripts
        grod[$"{INPUT_PREFIX}verb"] = result.Verb;
        grod[$"{INPUT_PREFIX}verbword"] = result.VerbWord;
        grod[$"{INPUT_PREFIX}noun"] = result.Noun;
        grod[$"{INPUT_PREFIX}nounword"] = result.NounWord;

        return result;
    }

    #region Private

    private static Grod grod = [];
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
    private static Dags dags = new(grod);

    #endregion
}
