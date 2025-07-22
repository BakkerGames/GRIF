namespace GRIFTools.Parse;

public static class ParseValues
{
    private const string CONFIG_ADJECTIVE_PREFIX = "system.adjective_prefix";
    private const string CONFIG_ARTICLE_LIST = "system.article_list";
    private const string CONFIG_COMMAND_PREFIX = "system.command_prefix";
    private const string CONFIG_DONT_UNDERSTAND = "system.dont_understand";
    private const string CONFIG_DONT_UNDERSTAND_THAT = "system.dont_understand_that";
    private const string CONFIG_DONT_UNDERSTAND_WORD = "system.dont_understand_word";
    private const string CONFIG_DO_WHAT_WITH_NOUN = "system.do_what_with_noun";
    private const string CONFIG_NOUN_PREFIX = "system.noun_prefix";
    private const string CONFIG_PREPOSITION_PREFIX = "system.preposition_prefix";
    private const string CONFIG_VERB_PREFIX = "system.verb_prefix";
    private const string CONFIG_WORDSIZE = "system.wordsize";

    public static readonly StringComparison OIC = StringComparison.OrdinalIgnoreCase;

    public static readonly StringSplitOptions SPLIT_OPTIONS =
        StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

    private static Grod _grod = new();

    public static void Init(Grod grod)
    {
        _grod = grod;
    }

    public static string ARTICLE_LIST_KEY
    {
        get
        {
            if (_grod.ContainsKey(CONFIG_ARTICLE_LIST))
            {
                return _grod.Get(CONFIG_ARTICLE_LIST);
            }
            return "articles";
        }
    }

    public static string ADJECTIVE_PREFIX
    {
        get
        {
            if (_grod.ContainsKey(CONFIG_ADJECTIVE_PREFIX))
            {
                return _grod.Get(CONFIG_ADJECTIVE_PREFIX);
            }
            return "adjective.";
        }
    }

    public static string COMMAND_PREFIX
    {
        get
        {
            if (_grod.ContainsKey(CONFIG_COMMAND_PREFIX))
            {
                return _grod.Get(CONFIG_COMMAND_PREFIX);
            }
            return "command.";
        }
    }

    public const string INPUT_PREFIX = "input."; // cannot be modified

    public static string NOUN_PREFIX
    {
        get
        {
            if (_grod.ContainsKey(CONFIG_NOUN_PREFIX))
            {
                return _grod.Get(CONFIG_NOUN_PREFIX);
            }
            return "noun.";
        }
    }

    public static string PREPOSITION_PREFIX
    {
        get
        {
            if (_grod.ContainsKey(CONFIG_PREPOSITION_PREFIX))
            {
                return _grod.Get(CONFIG_PREPOSITION_PREFIX);
            }
            return "preposition.";
        }
    }

    public static string VERB_PREFIX
    {
        get
        {
            if (_grod.ContainsKey(CONFIG_VERB_PREFIX))
            {
                return _grod.Get(CONFIG_VERB_PREFIX);
            }
            return "verb.";
        }
    }

    public static string DONT_UNDERSTAND_THAT
    {
        get
        {
            if (_grod.ContainsKey(CONFIG_DONT_UNDERSTAND_THAT))
            {
                return _grod.Get(CONFIG_DONT_UNDERSTAND_THAT);
            }
            return "I don't understand that.";
        }
    }

    public static string DONT_UNDERSTAND_INPUT
    {
        get
        {
            if (_grod.ContainsKey(CONFIG_DONT_UNDERSTAND))
            {
                return _grod.Get(CONFIG_DONT_UNDERSTAND);
            }
            return "I don't understand \"{0}\".";
        }
    }

    public static string DO_WHAT_WITH_NOUN
    {
        get
        {
            if (_grod.ContainsKey(CONFIG_DO_WHAT_WITH_NOUN))
            {
                return _grod.Get(CONFIG_DO_WHAT_WITH_NOUN);
            }
            return "Do what with the {0}?";
        }
    }

    public static string DONT_UNDERSTAND_WORD
    {
        get
        {
            if (_grod.ContainsKey(CONFIG_DONT_UNDERSTAND_WORD))
            {
                return _grod.Get(CONFIG_DONT_UNDERSTAND_WORD);
            }
            return "I don't know the word \"{0}\".";
        }
    }

    public static int WORDSIZE
    {
        get
        {
            if (_grod.ContainsKey(CONFIG_WORDSIZE) && int.TryParse(_grod.Get(CONFIG_WORDSIZE), out int answer))
            {
                return answer;
            }
            return 0;
        }
    }
}
