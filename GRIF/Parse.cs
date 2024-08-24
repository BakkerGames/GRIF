using GRIFTools;
using static GRIF.Constants;

namespace GRIF;

// This is a simple VERB or VERB NOUN parser. It looks up matches using
// "verb." and "noun." as prefixes in the dictionary. These contain one or
// more comma separated words, such as "GO,RUN,WALK,CLIMB" for the key
// "verb.go". When matching, case is ignored.
// 
// The answers will be stored into "input." values, with "input.verb" and
// "input.noun" being the second half of the key values found, and
// "input.verbword" and "input.nounword" being the actual words matched.
// 
// When matching succeeds, a CommandKey is returned. This is the script key
// for the proper command to be run. It is stored in the dictionary as
// "command.verb" or "command.verb.noun" using the verb and noun identified
// above. It must exist for this routine to succeed.
// 
// If a noun is entered but the command is not found, three special
// commands will be checked to see if they are in the dictionary for this
// verb. Avoid using more than one of these for each verb.
//     "command.verb.#" will match any noun which is an integer number.
//     "command.verb.*" will match any noun in the dictionary.
//     "command.verb.?" will match anything entered as a noun.
// "command.verb.#" is useful for combination locks, pacing off steps
// before digging, etc., where any number might be entered.
// "command.verb.*" is useful for generic handling, as "command.take.*"
// displaying "I don't see that here."
// "command.verb.?" is good for unknown words, or filenames for saving,
// or anything undefined. Good for examining items, when the noun is not
// defined but might be mentioned in the room text. "command.examine.?"
// could display "There is nothing special about the " and the noun.
// When the "#" or "?" special commands are matched, "input.noun" will
// contain "?" or "#" and "input.nounword" will contain what was entered.

public class ParseResult
{
    public string Verb = "";
    public string VerbWord = "";
    public string Noun = "";
    public string NounWord = "";
    public string CommandKey = "";
    public string Error = "";

    public void Clear()
    {
        Verb = "";
        VerbWord = "";
        Noun = "";
        NounWord = "";
        CommandKey = "";
        Error = "";
    }
}

public static class Parse
{
    public static void Init(Grod grod)
    {
        Parse.grod = grod;
    }

    public static ParseResult ParseInput(string input)
    {
        var result = new ParseResult();

        // remove comments
        if (input.Contains("//"))
        {
            input = input[..input.IndexOf("//")].Trim();
        }

        // Clear answers
        grod[$"{INPUT_PREFIX}verb"] = "";
        grod[$"{INPUT_PREFIX}verbword"] = "";
        grod[$"{INPUT_PREFIX}noun"] = "";
        grod[$"{INPUT_PREFIX}nounword"] = "";

        var words = input.Split(' ', SPLIT_OPTIONS);
        if (words.Length == 0)
        {
            return result;
        }
        if (words.Length > 2)
        {
            result.Error = SystemData.DontUnderstand(input);
            return result;
        }

        var tempWord0 = words[0];
        var tempWord1 = words.Length > 1 ? words[1] : "";
        var tempWord1Full = tempWord1;
        if (tempWord0.StartsWith('@') || tempWord1.StartsWith('@'))
        {
            // avoid injection attacks. ;)
            result.Error = SystemData.DontUnderstand(input);
            return result;
        }
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
            var verbList = (grod[key] ?? "").Split(',', SPLIT_OPTIONS);
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
                var nounList = (grod[key] ?? "").Split(',', SPLIT_OPTIONS);
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

        // find a matching command script key in the dictionary

        var tempCommandKey = $"{COMMAND_PREFIX}{result.Verb}";
        if (words.Length > 1)
        {
            if (grod.ContainsKey(tempCommandKey + "." + result.Noun))
            {
                result.CommandKey = tempCommandKey + "." + result.Noun;
            }
            else if (grod.ContainsKey(tempCommandKey + ".*") && result.Noun != "")
            {
                result.CommandKey = tempCommandKey + ".*";
            }
            else if (grod.ContainsKey(tempCommandKey + ".#") && int.TryParse(tempWord1Full, out int number))
            {
                result.Noun = "#"; // special indicator for any number
                result.NounWord = number.ToString(); // normalized number
                result.CommandKey = tempCommandKey + ".#";
            }
            else if (grod.ContainsKey(tempCommandKey + ".?"))
            {
                result.Noun = "?"; // special indicator for any value, like a filename
                result.NounWord = tempWord1Full; // actual value entered, not shortened
                result.CommandKey = tempCommandKey + ".?";
            }
        }
        else if (grod.ContainsKey(tempCommandKey))
        {
            result.CommandKey = tempCommandKey;
        }

        if (result.CommandKey == "")
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

    #endregion
}
