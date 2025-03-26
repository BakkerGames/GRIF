namespace GRIFTools.Parse;

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

    public void ClearForNoun()
    {
        Preposition = "";
        PrepositionWord = "";
        Object = "";
        ObjectWord = "";
        CommandKey = "";
        Error = "";
    }
}
