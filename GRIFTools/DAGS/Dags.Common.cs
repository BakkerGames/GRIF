namespace GRIFTools;

public partial class Dags
{
    private static readonly StringComparison OIC = StringComparison.OrdinalIgnoreCase;

    private readonly Random _random = new();

    private const string DEBUG_MODE = "system.debug";
    private const string UNDO_MODE = "system.enable_undo";
}
