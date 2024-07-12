using DAGS;
using GROD;
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
        if (input.StartsWith("#debug ", OIC))
        {
            dags.DebugLog(input[7..], result);
            return;
        }
        if (input.StartsWith("#exec ", OIC))
        {
            dags.RunScript(input[6..], result);
            if (result.Length > 0 && !result.ToString().EndsWith("\r\n"))
            {
                result.AppendLine();
            }
            return;
        }
        result.AppendLine(SystemData.DontUnderstand(input));
    }

    #region Private

    private static Grod grod = [];
    private static Dags dags = new(grod);

    #endregion
}
