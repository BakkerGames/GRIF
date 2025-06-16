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

    public const string BACKGROUND_PREFIX = "background.";
}
