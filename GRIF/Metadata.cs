using GRIFTools;
using System.Text;
using static GRIF.Constants;

namespace GRIF;

public static class Metadata
{
    public static void Init(Grod grod, Dags dags)
    {
        Metadata.grod = grod;
        Metadata.dags = dags;
    }

    public static void CheckMetaCommand(string input, StringBuilder result)
    {
        if (input.StartsWith("#exec ", OIC))
        {
            dags.RunScriptBackground(input[6..], result);
            if (result.Length > 0 &&
                !result.ToString().EndsWith("\r\n") &&
                !result.ToString().EndsWith(DAGSConstants.NL_VALUE))
            {
                result.Append(DAGSConstants.NL_VALUE);
            }
            return;
        }
        result.AppendLine(SystemData.DontUnderstand(input));
    }

    #region Private

    private static Grod grod = new();
    private static Dags dags = new(grod);

    #endregion
}
