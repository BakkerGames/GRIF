namespace GRIF;

public static class Constants
{
    public static readonly StringComparison OIC = StringComparison.OrdinalIgnoreCase;
    public static readonly StringSplitOptions SPLIT_OPTIONS =
        StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

    public const string APP_NAME = "GRIF";
    public const string DATA_EXTENSION = ".grif";

    public const string SAVE_FILENAME = "save";
    public const string SAVE_EXTENSION = ".dat";

    public const string ADJECTIVE_PREFIX = "adjective.";
    public const string ARTICLE_PREFIX = "article.";
    public const string BACKGROUND_PREFIX = "background.";
    public const string COMMAND_PREFIX = "command.";
    public const string INPUT_PREFIX = "input.";
    public const string NOUN_PREFIX = "noun.";
    public const string PREPOSITION_PREFIX = "preposition.";
    public const string VERB_PREFIX = "verb.";

    public const string OUTCHANNEL_ASK = "#ASK;";
    public const string OUTCHANNEL_ENTER = "#ENTER;";
    public const string OUTCHANNEL_EXISTS_SAVE = "#EXISTS;";
    public const string OUTCHANNEL_EXISTS_SAVE_NAME = "#EXISTSNAME;";
    public const string OUTCHANNEL_GAMEOVER = "#GAMEOVER;";
    public const string OUTCHANNEL_RESTART = "#RESTART;";
    public const string OUTCHANNEL_RESTORE = "#RESTORE;";
    public const string OUTCHANNEL_RESTORE_NAME = "#RESTORENAME;";
    public const string OUTCHANNEL_SAVE = "#SAVE;";
    public const string OUTCHANNEL_SAVE_NAME = "#SAVENAME;";
}
