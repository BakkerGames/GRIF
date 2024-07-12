using System.Text;

namespace GRIF;

public static class UserIO
{
    public static void InitInputFile(string filename)
    {
        var lines = File.ReadAllLines(filename);
        foreach (string line in lines)
        {
            var tempLine = line.Trim();
            if (tempLine != "")
            {
                InputQueue.Enqueue(tempLine);
            }
        }
    }

    public static void InitLogFile(string filename)
    {
        if (File.Exists(filename))
        {
            File.Delete(filename);
        }
        LogFilename = filename;
    }

    public static string GetInput()
    {
        return GetInput(false);
    }

    public static string GetInput(bool emptyAllowed)
    {
        string result;
        if (InputQueue.Count > 0)
        {
            do
            {
                result = InputQueue.Dequeue();
                Output(result, true);
                if (result.TrimStart().StartsWith("//"))
                {
                    result = "";
                }
            } while (InputQueue.Count > 0 & (result == "" && !emptyAllowed));
            return result;
        }
        do
        {
            result = Console.ReadLine() ?? "";
            if (result.TrimStart().StartsWith("//"))
            {
                if (LogFilename != "")
                {
                    // can't use Output() here
                    File.AppendAllText(LogFilename, result + Environment.NewLine);
                }
                result = "";
            }
        } while (result == "" && !emptyAllowed);
        LastLen = 0; // moved to new line
        if (LogFilename != "")
        {
            // can't use Output() here
            File.AppendAllText(LogFilename, result + Environment.NewLine);
        }
        return result;
    }

    public static void Output(StringBuilder result)
    {
        Output(result.ToString(), false);
    }

    public static void Output(string value)
    {
        Output(value, false);
    }

    public static void Output(string value, bool withNL)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }
        StringBuilder result = new();
        var lines = value.Split("\\n");
        for (int i = 0; i < lines.Length - 1; i++)
        {
            var line = lines[i];
            if (SystemData.OutputWidth() > 0)
            {
                while (LastLen + line.Length > SystemData.OutputWidth())
                {
                    int pos = line[..(LastLen + SystemData.OutputWidth() + 1)].LastIndexOf(' ');
                    result.AppendLine(line[..pos].TrimEnd());
                    LastLen = 0;
                    line = line[(pos + 1)..];
                }
            }
            result.AppendLine(line.TrimEnd());
            LastLen = 0;
        }
        result.Append(lines[^1]);
        LastLen = lines[^1].Length;
        if (withNL)
        {
            result.AppendLine();
            LastLen = 0;
        }
        if (result.Length > 0)
        {
            Console.Write(result.ToString());
            if (LogFilename != "")
            {
                File.AppendAllText(LogFilename, result.ToString());
            }
        }
    }

    #region Private

    private static int LastLen { get; set; } = 0;

    private static Queue<string> InputQueue { get; set; } = new();

    private static string LogFilename { get; set; } = "";

    #endregion
}
