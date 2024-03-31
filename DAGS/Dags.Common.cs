namespace DAGS;

public partial class Dags
{
    private static readonly StringComparison OIC = StringComparison.OrdinalIgnoreCase;

    private readonly IDictionary<string, string> _dict = dict;

    private readonly Random _random = new();
}