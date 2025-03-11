using GRIFTools;
using static GRIF.Constants;

namespace GRIF;


public class ParseResult
{
    public string Verb = "";
    public string VerbWord = "";
    public string Noun = "";
    public string NounWord = "";
    public string Preposition = "";
    public string PrepositionWord = "";
    public string Object = "";
    public string ObjectWord = "";

    public string CommandKey = "";
    public string Error = "";

    public void Clear()
    {
        Verb = "";
        VerbWord = "";
        Noun = "";
        NounWord = "";
        Preposition = "";
        PrepositionWord = "";
        Object = "";
        ObjectWord = "";
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
        grod.Set($"{INPUT_PREFIX}verb", "");
        grod.Set($"{INPUT_PREFIX}verbword", "");
        grod.Set($"{INPUT_PREFIX}noun", "");
        grod.Set($"{INPUT_PREFIX}nounword", "");
        grod.Set($"{INPUT_PREFIX}preposition", "");
        grod.Set($"{INPUT_PREFIX}prepositionword", "");
        grod.Set($"{INPUT_PREFIX}object", "");
        grod.Set($"{INPUT_PREFIX}objectword", "");

        var words = input.Split(' ', SPLIT_OPTIONS);
        if (words.Length == 0)
        {
            return result;
        }

        int index = 0;
        GetVerb(result, words, ref index);
        if (result.Error != "")
        {
            return result;
        }
        if (result.Verb == "")
        {
            result.Error = SystemData.DontUnderstand(input);
            return result;
        }
        if (index < words.Length)
        {
            GetNoun(result, words, ref index);
            if (result.Error != "")
            {
                return result;
            }
            if (result.Noun == "")
            {
                result.Error = SystemData.DontUnderstand(input);
                return result;
            }
            if (index < words.Length)
            {
                GetPrepositionAndObject(result, words, ref index);
                if (result.Error != "")
                {
                    return result;
                }
                if (result.Preposition == "" || result.Object == "")
                {
                    result.Error = SystemData.DontUnderstand(input);
                    return result;
                }
            }
        }
        if (index < words.Length)
        {
            result.Error = SystemData.DontUnderstand(input);
            return result;
        }

        //TODO ### left off here

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
        foreach (string key in grod.Keys().Where(x => x.StartsWith(VERB_PREFIX, OIC)))
        {
            var verbList = (grod.Get(key) ?? "").Split(',', SPLIT_OPTIONS);
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

        // Set values for use in scripts
        grod.Set($"{INPUT_PREFIX}verb", result.Verb);
        grod.Set($"{INPUT_PREFIX}verbword", result.VerbWord);
        grod.Set($"{INPUT_PREFIX}noun", result.Noun);
        grod.Set($"{INPUT_PREFIX}nounword", result.NounWord);
        grod.Set($"{INPUT_PREFIX}preposition", result.Preposition);
        grod.Set($"{INPUT_PREFIX}prepositionword", result.PrepositionWord);
        grod.Set($"{INPUT_PREFIX}object", result.Object);
        grod.Set($"{INPUT_PREFIX}objectword", result.ObjectWord);

        return result;
    }

    private static string GetMatchingWord(string keyPrefix, string origWord)
    {
        // get the origWord cut to the proper length
        var shortWord = SystemData.WordSize() > 0 && origWord.Length > SystemData.WordSize()
            ? origWord[..SystemData.WordSize()]
            : origWord;

        // compare against each possible word
        foreach (string key in grod.Keys().Where(x => x.StartsWith(keyPrefix, OIC)))
        {
            var wordList = (grod.Get(key) ?? "").Split(',', SPLIT_OPTIONS);
            foreach (string item in wordList)
            {
                var compareWord = SystemData.WordSize() > 0 && item.Length > SystemData.WordSize()
                    ? item[..SystemData.WordSize()]
                    : item;
                if (compareWord.Equals(shortWord, OIC))
                {
                    return key[keyPrefix.Length..];
                }
            }
        }

        // not found
        return "";
    }

    private static void GetVerb(ParseResult result, string[] words, ref int index)
    {
        var origWord = words[index++];
        var verb = GetMatchingWord(VERB_PREFIX, origWord);
        result.Verb = origWord;
        result.VerbWord = verb;
    }

    private static void GetNoun(ParseResult result, string[] words, ref int index)
    {
        var origNoun = words[index++];
        var tempNoun = SystemData.WordSize() > 0 && origNoun.Length > SystemData.WordSize()
            ? origNoun[..SystemData.WordSize()]
            : origNoun;

            foreach (string key in grod.Keys().Where(x => x.StartsWith(NOUN_PREFIX, OIC)))
        {
            var nounList = (grod.Get(key) ?? "").Split(',', SPLIT_OPTIONS);
            foreach (string n in nounList)
            {
                var noun = n;
                if (SystemData.WordSize() > 0 && noun.Length > SystemData.WordSize())
                {
                    noun = noun[..SystemData.WordSize()];
                }
                if (noun.Equals(tempNoun, OIC))
                {
                    result.Noun = key[NOUN_PREFIX.Length..];
                    result.NounWord = tempNoun;
                    break;
                }
            }
            if (result.Noun != "") break;
        }
    }

    private static void GetPrepositionAndObject(ParseResult result, string[] words, ref int index)
    {
        throw new NotImplementedException();
    }

    #region Private

    private static Grod grod = new();

    #endregion
}
