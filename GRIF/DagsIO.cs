using GRIFTools;
using System.Text;
using static GRIF.Constants;
using static GRIFTools.DAGSConstants;

namespace GRIF;

public static class DagsIO
{
    public static void Init(Grod grod, Dags dags)
    {
        DagsIO.grod = grod;
        DagsIO.dags = dags;
    }

    public static bool GameOver { get; set; } = false;

    public static void RunBackgroundScripts(StringBuilder result)
    {
        var backgroundKeys = grod.Keys().Where(x => x.StartsWith(BACKGROUND_PREFIX)).ToList();
        foreach (string key in backgroundKeys)
        {
            dags.RunScriptBackground(grod.Get(key) ?? "", result);
        }
    }

    public static void CheckOutChannel()
    {
        StringBuilder result = new();

        while (dags.OutChannel.Count > 0)
        {
            dags.OutChannel.TryDequeue(out string? value);
            if (string.IsNullOrEmpty(value))
            {
                continue;
            }
            if (value.Equals(OUTCHANNEL_GAMEOVER, OIC))
            {
                GameOver = true;
                return;
            }
            if (value.Equals(OUTCHANNEL_EXISTS_SAVE, OIC))
            {
                if (File.Exists(GetSavePath(SAVE_FILENAME, SAVE_EXTENSION)))
                {
                    dags.InChannel.Enqueue("true");
                }
                else
                {
                    dags.InChannel.Enqueue("false");
                }
                continue;
            }
            if (value.Equals(OUTCHANNEL_EXISTS_SAVE_NAME, OIC))
            {
                dags.OutChannel.TryDequeue(out string? restorename);
                if (string.IsNullOrEmpty(restorename))
                {
                    UserIO.Output("Missing filename.", true);
                    dags.OutChannel.Clear();
                    continue;
                }
                if (File.Exists(GetSavePath(restorename, SAVE_EXTENSION)))
                {
                    dags.InChannel.Enqueue("true");
                }
                else
                {
                    dags.InChannel.Enqueue("false");
                }
                continue;
            }
            if (value.Equals(OUTCHANNEL_SAVE, OIC))
            {
                SaveState(GetSavePath(SAVE_FILENAME, SAVE_EXTENSION));
                continue;
            }
            if (value.Equals(OUTCHANNEL_SAVE_NAME, OIC))
            {
                dags.OutChannel.TryDequeue(out string? savename);
                if (string.IsNullOrEmpty(savename))
                {
                    UserIO.Output("Missing filename for save.", true);
                    continue;
                }
                SaveState(GetSavePath(savename, SAVE_EXTENSION));
                continue;
            }
            if (value.Equals(OUTCHANNEL_RESTORE, OIC))
            {
                if (!File.Exists(GetSavePath(SAVE_FILENAME, SAVE_EXTENSION)))
                {
                    UserIO.Output("File not found for restore.", true);
                    dags.OutChannel.Clear();
                    continue;
                }
                RestoreState(GetSavePath(SAVE_FILENAME, SAVE_EXTENSION));
                continue;
            }
            if (value.Equals(OUTCHANNEL_RESTORE_NAME, OIC))
            {
                dags.OutChannel.TryDequeue(out string? restorename);
                if (string.IsNullOrEmpty(restorename))
                {
                    UserIO.Output("Missing filename for restore.", true);
                    dags.OutChannel.Clear();
                    continue;
                }
                if (!File.Exists(GetSavePath(restorename, SAVE_EXTENSION)))
                {
                    UserIO.Output("File not found for restore.", true);
                    dags.OutChannel.Clear();
                    continue;
                }
                RestoreState(GetSavePath(restorename, SAVE_EXTENSION));
                continue;
            }
            if (value.Equals(OUTCHANNEL_RESTART, OIC))
            {
                Restart();
                continue;
            }
            if (value.Equals(OUTCHANNEL_ASK, OIC))
            {
                UserIO.Output(SystemData.Prompt());
                var input = UserIO.GetInput();
                UserIO.Output(SystemData.AfterPrompt());
                dags.InChannel.Enqueue(input);
                continue;
            }
            if (value.Equals(OUTCHANNEL_ENTER, OIC))
            {
                UserIO.Output(SystemData.Prompt());
                _ = UserIO.GetInput(true);
                UserIO.Output(SystemData.AfterPrompt());
                continue;
            }
            if (value.StartsWith('@'))
            {
                result.Clear();
                try
                {
                    dags.RunScriptBackground(value, result);
                }
                catch (Exception ex)
                {
                    if (result.Length > 0) result.AppendLine();
                    result.AppendLine(ex.Message);
                }
                UserIO.Output(result);
                continue;
            }
            UserIO.Output($"Unknown OutChannel command: {value}", true);
        }
    }

    #region Private

    private static Grod grod = new();
    private static Dags dags = new(grod);

    private static string GetSavePath(string filebase, string fileext)
    {
        var result = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        result = Path.Combine(result, APP_NAME);
        result = Path.Combine(result, SystemData.GameName());
        if (!Directory.Exists(result))
        {
            Directory.CreateDirectory(result);
        }
        result = Path.Combine(result, filebase + fileext);
        return result;
    }

    private static void Restart()
    {
        grod.Clear(GrodEnums.WhichData.Overlay);
    }

    private static void SaveState(string filename)
    {
        // save in valid json format
        GrodDataIO.SaveOverlayDataToFile(filename, grod, true);
    }

    private static void RestoreState(string filename)
    {
        grod.Clear(GrodEnums.WhichData.Overlay);
        GrodDataIO.LoadDataFromFile(filename, grod);
    }

    #endregion
}
