using System.Text;
using static GRIFTools.DAGSConstants;

namespace GRIFTools;

/// <summary>
/// Generate a DAGS script object and assign its dictionary.
/// </summary>
public partial class Dags(Grod data)
{
    /// <summary>
    /// Grod dictionary of (key,value) pairs
    /// </summary>
    public Grod Data { get; set; } = data;

    /// <summary>
    /// Receives metadata from the calling program, such as text input.
    /// </summary>
    public Queue<string> InChannel { get; set; } = new();

    /// <summary>
    /// Sends metadata or commands back to the calling program.
    /// </summary>
    public Queue<string> OutChannel { get; set; } = new();

    /// <summary>
    /// Run one script and return any text in result.
    /// </summary>
    public void RunScript(string script, StringBuilder result)
    {
        if (string.IsNullOrWhiteSpace(script) || script.Equals(NULL_VALUE, OIC))
        {
            return;
        }
        if (!script.TrimStart().StartsWith('@'))
        {
            result.Append(script);
            return;
        }
        try
        {
            var tokens = SplitTokens(script);
            int index = 0;
            while (index < tokens.Length)
            {
                RunOneCommand(tokens, ref index, result);
            }
        }
        catch (Exception ex)
        {
            if (result.Length > 0) result.AppendLine();
            if (!ex.Message.StartsWith("Error", OIC)) result.Append("ERROR: ");
            result.AppendLine(ex.Message);
        }
    }

    /// <summary>
    /// Run one script specified by key and return any text in result.
    /// Use the unpacked script if it exists. If not, unpack it and store it for next time.
    /// </summary>
    public void RunScriptKey(string key, StringBuilder result)
    {
        var script = Data.Get(key);
        if (string.IsNullOrWhiteSpace(script) || script.Equals(NULL_VALUE, OIC))
        {
            return;
        }
        if (!script.TrimStart().StartsWith('@'))
        {
            result.Append(script);
            return;
        }
        try
        {
            var tokens = Data.GetUnpacked(key);
            if (tokens == null)
            {
                tokens = SplitTokens(script);
                Data.SetUnpacked(key, tokens);
            }
            int index = 0;
            while (index < tokens.Length)
            {
                RunOneCommand(tokens, ref index, result);
            }
        }
        catch (Exception ex)
        {
            if (result.Length > 0) result.AppendLine();
            if (!ex.Message.StartsWith("Error", OIC)) result.Append("ERROR: ");
            result.AppendLine(ex.Message);
        }
    }
}
