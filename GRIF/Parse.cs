using GRIFTools;
using static GRIF.Constants;

namespace GRIF;

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

        input = input.Replace(",", " ").Trim();
        if (input.EndsWith('.') || input.EndsWith('!') || input.EndsWith('?'))
        {
            input = input[..^1]; // remove punctuation
        }
        var words = input.Split(' ', SPLIT_OPTIONS);
        if (words.Length == 0)
        {
            return result;
        }

        // check through patterns:
        //     verb
        //     verb <number>
        //     verb <unknown_word>
        //     verb preposition [article] [adjectives...] object
        //     verb [article] [adjectives...] noun
        //     verb [article] [adjectives...] noun preposition [article] [adjectives...] object

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
            GetPrepositionAndObject(result, words, ref index);
            if (result.Error != "" || result.Preposition == "" || result.Object == "")
            {
                result.ClearForNoun();
                GetNoun(result, words, ref index);
                if (result.Error != "")
                {
                    return result;
                }
                if (result.Noun == "")
                {
                    // might be unknown or number, so save NounWord for later
                    result.NounWord = words[index++];
                }
                if (result.Noun != "" && index < words.Length)
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
        }
        if (index < words.Length)
        {
            result.Error = SystemData.DontUnderstand(input);
            return result;
        }

        // find a matching command script key in the dictionary

        var tempCommandKey = $"{COMMAND_PREFIX}{result.Verb}";
        if (result.NounWord == "") // don't check result.Noun here
        {
            if (result.Preposition != "" && result.Object != "")
            {
                if (grod.ContainsKey($"{tempCommandKey}.{result.Preposition}.{result.Object}"))
                {
                    result.CommandKey = $"{tempCommandKey}.{result.Preposition}.{result.Object}";
                }
                else if (grod.ContainsKey($"{tempCommandKey}.{result.Preposition}.*"))
                {
                    result.CommandKey = $"{tempCommandKey}.{result.Preposition}.*";
                }
            }
            else if (grod.ContainsKey(tempCommandKey))
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
        else if (result.Noun != "" && grod.ContainsKey($"{tempCommandKey}.{result.Noun}"))
        {
            result.CommandKey = $"{tempCommandKey}.{result.Noun}";
        }
        else if (grod.ContainsKey($"{tempCommandKey}.#") && int.TryParse(result.NounWord, out int number))
        {
            result.Noun = "#"; // special indicator for any number
            result.NounWord = number.ToString(); // normalized number
            result.CommandKey = $"{tempCommandKey}.#";
        }
        else if (result.Noun != "" && grod.ContainsKey($"{tempCommandKey}.*"))
        {
            result.CommandKey = $"{tempCommandKey}.*";
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
        var adjectiveNounList = CheckAdjectives(words, ref index);

        // get the noun
        string origWord = words[index];
        string noun = GetMatchingWord(NOUN_PREFIX, words, ref index);
        if (noun == "")
        {
            return;
        }

        // check adjectives
        if (adjectiveNounList != null && !adjectiveNounList.Contains(noun))
        {
            // noun doesn't match adjectives
            return;
        }

        result.Noun = noun;
        result.NounWord = origWord;
    }

    private static void GetPrepositionAndObject(ParseResult result, string[] words, ref int index)
    {
        int newIndex = index;

        // get the preposition
        string origWord1 = words[newIndex];
        var preposition = GetMatchingWord(PREPOSITION_PREFIX, words, ref newIndex);
        if (preposition == "")
        {
            return;
        }

        // articles
        CheckArticles(words, ref newIndex);

        // adjectives
        var adjectiveNounList = CheckAdjectives(words, ref newIndex);

        // get the preposition's object
        string origWord2 = words[newIndex];
        string noun = GetMatchingWord(NOUN_PREFIX, words, ref newIndex);
        if (noun == "")
        {
            return;
        }

        // check adjectives
        if (adjectiveNounList != null && !adjectiveNounList.Contains(noun))
        {
            // noun doesn't match adjectives
            return;
        }

        // found a match
        index = newIndex;
        result.Preposition = preposition;
        result.PrepositionWord = origWord1;
        result.Object = noun;
        result.ObjectWord = origWord2;
    }

    private static string GetMatchingWord(string keyPrefix, string[] words, ref int index)
    {
        string answer = "";

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
                var compareWord = FixLength(item);
                if (compareWord.Equals(shortWord, OIC) && answer == "")
                {
                    answer = key[keyPrefix.Length..];
                    break;
                }
            }
            if (answer != "") break;
        }

        if (answer != "")
        {
            // bump index past matched word(s)
            index = newIndex;
        }

        return answer;
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

    private static List<string>? CheckAdjectives(string[] words, ref int index)
    {
        // adjectives have keys "adjective.<noun>" and a list of adjectives for that noun.
        // when checking multiple adjectives, all must match the same noun(s).
        // the "intersect" removes any not matching on either side.
        // upon returning, the following noun must be in the adjective noun list, or it fails.

        bool firstTime = true;
        bool foundOne = false;
        List<string> nounList = [];
        List<string> newNounList = [];

        int newIndex = index;

        do
        {
            foundOne = false;
            string origWord = words[newIndex];
            var shortWord = FixLength(origWord);
            newNounList.Clear();

            // check adjectives
            foreach (string key in grod.Keys().Where(x => x.StartsWith(ADJECTIVE_PREFIX, OIC)))
            {
                var wordList = (grod.Get(key) ?? "").Split(',', SPLIT_OPTIONS);
                foreach (string item in wordList)
                {
                    var compareWord = FixLength(item);
                    if (compareWord.Equals(shortWord, OIC))
                    {
                        var noun = key[ADJECTIVE_PREFIX.Length..];
                        newNounList.Add(noun);
                        foundOne = true;
                        newIndex++;
                        break;
                    }
                }
            }

            if (foundOne)
            {
                if (firstTime)
                {
                    // no intersect first time, just copy
                    nounList = [.. newNounList];
                    firstTime = false;
                }
                else
                {
                    // intersect both lists, only keeping nouns in both
                    nounList = [.. nounList.Intersect(newNounList)];
                }
            }

        } while (foundOne);

        if (firstTime) // never found an adjective
        {
            return null;
        }

        // found at least one adjective. nounList could be empty though.
        index = newIndex;
        return nounList;
    }

    private static string FixLength(string word)
    {
        return SystemData.WordSize() > 0 && word.Length > SystemData.WordSize()
            ? word[..SystemData.WordSize()]
            : word;
    }
}
