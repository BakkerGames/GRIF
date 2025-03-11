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
    private static Grod grod = new();

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

        // find a matching command script key in the dictionary

        var tempCommandKey = $"{COMMAND_PREFIX}{result.Verb}";
        if (result.Noun == "")
        {
            if (grod.ContainsKey(tempCommandKey))
            {
                result.CommandKey = tempCommandKey;
            }
        }
        else if (result.Preposition != "" && result.Object != "")
        {
            if (grod.ContainsKey($"{tempCommandKey}.{result.Noun}.{result.Preposition}.{result.Object}"))
            {
                result.CommandKey = $"{tempCommandKey}.{result.Noun}.{result.Preposition}.{result.Object}";
            }
            else if (grod.ContainsKey($"{tempCommandKey}.*.{result.Preposition}.{result.Object}"))
            {
                result.CommandKey = $"{tempCommandKey}.*.{result.Preposition}.{result.Object}";
            }
            else if (grod.ContainsKey($"{tempCommandKey}.{result.Noun}.{result.Preposition}.*"))
            {
                result.CommandKey = $"{tempCommandKey}.{result.Noun}.{result.Preposition}.*";
            }
        }
        else if (grod.ContainsKey($"{tempCommandKey}.{result.Noun}"))
        {
            result.CommandKey = $"{tempCommandKey}.{result.Noun}";
        }
        else if (grod.ContainsKey($"{tempCommandKey}.*"))
        {
            result.CommandKey = $"{tempCommandKey}.*";
        }
        else if (grod.ContainsKey($"{tempCommandKey}.#") && int.TryParse(result.NounWord, out int number))
        {
            result.Noun = "#"; // special indicator for any number
            result.NounWord = number.ToString(); // normalized number
            result.CommandKey = $"{tempCommandKey}.#";
        }
        else if (grod.ContainsKey($"{tempCommandKey}.?"))
        {
            result.Noun = "?"; // special indicator for any value, like a filename
            result.CommandKey = $"{tempCommandKey}.?";
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

    private static string GetMatchingWord(string keyPrefix, string[] words, ref int index)
    {
        string answer = "";
        Dictionary<string, string> multiWords = [];

        string origWord = words[index];
        int newIndex = index + 1;

        // get the origWord cut to the proper length
        var shortWord = FixLength(origWord);

        // compare against each possible word
        foreach (string key in grod.Keys().Where(x => x.StartsWith(keyPrefix, OIC)))
        {
            var wordList = (grod.Get(key) ?? "").Split(',', SPLIT_OPTIONS);
            foreach (string item in wordList)
            {
                // Check for items made of two or more words
                if (item.Contains(' '))
                {
                    multiWords.Add(key[keyPrefix.Length..], item);
                }
                var compareWord = FixLength(item);
                if (compareWord.Equals(shortWord, OIC) && answer == "")
                {
                    answer = key[keyPrefix.Length..];
                    break; // TODO ### have to remove for multi word checking
                }
            }
            if (answer != "") break; // TODO ### have to remove for multi word checking
        }

        // check multi word list
        // TODO ### check multi word list

        if (answer != "")
        {
            // bump index past matched word(s)
            index = newIndex;
        }

        return answer;
    }

    private static void GetVerb(ParseResult result, string[] words, ref int index)
    {
        string origWord = words[index];
        result.Verb = GetMatchingWord(VERB_PREFIX, words, ref index);
        if (result.Verb == "")
        {
            return;
        }
        result.VerbWord = origWord;
    }

    private static void GetNoun(ParseResult result, string[] words, ref int index)
    {
        // articles
        CheckArticles(words, ref index);

        // adjectives
        var adjNounList = CheckAdjectives(words, ref index);

        // get the noun
        string origWord = words[index];
        string noun = GetMatchingWord(NOUN_PREFIX, words, ref index);
        if (noun == "")
        {
            return;
        }
        // check adjectives
        if (adjNounList.Count > 0 && !adjNounList.Contains(noun))
        {
            // noun doesn't match adjectives
            return;
        }
        result.Noun = noun;
        result.NounWord = origWord;
    }

    private static void GetPrepositionAndObject(ParseResult result, string[] words, ref int index)
    {
        // get the preposition
        string origWord = words[index];
        result.Preposition = GetMatchingWord(PREPOSITION_PREFIX, words, ref index);
        if (result.Preposition == "")
        {
            return;
        }
        result.PrepositionWord = origWord;

        // articles
        CheckArticles(words, ref index);

        // adjectives
        var adjNounList = CheckAdjectives(words, ref index);

        // get the preposition's object
        origWord = words[index];
        string noun = GetMatchingWord(NOUN_PREFIX, words, ref index);
        if (result.Object == "")
        {
            return;
        }
        // check adjectives
        if (adjNounList.Count > 0 && !adjNounList.Contains(noun))
        {
            // noun doesn't match adjectives
            return;
        }
        result.Object = noun;
        result.ObjectWord = origWord;
    }

    private static void CheckArticles(string[] words, ref int index)
    {
        // look for articles (a, an, the) and ignore them

        string origWord = words[index];
        int newIndex = index + 1;

        // get the origWord cut to the proper length
        var shortWord = FixLength(origWord);

        var wordList = (grod.Get(ARTICLE_LIST) ?? "").Split(',', SPLIT_OPTIONS);
        foreach (string item in wordList)
        {
            var compareWord = FixLength(item);
            if (compareWord.Equals(shortWord, OIC))
            {
                // found a matching article
                index = newIndex;
                return;
            }
        }
    }

    private static List<string> CheckAdjectives(string[] words, ref int index)
    {
        List<string> adjNounList = [];

        // TODO ### check adjectives

        return adjNounList;
    }

    private static string FixLength(string word)
    {
        return SystemData.WordSize() > 0 && word.Length > SystemData.WordSize()
            ? word[..SystemData.WordSize()]
            : word;
    }
}
