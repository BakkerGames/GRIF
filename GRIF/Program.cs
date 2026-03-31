using System.Text;
using GrifLib;
using static GrifLib.Common;

namespace Grif;

internal class Program
{
    #region Private Constants

    private const string OUTCHANNEL_SLEEP = "#SLEEP;";

    private static readonly Queue<string> inputQueue = new();

    private static int outputCount = 0;
    private static int maxOutputWidth = 0;
    private static string tabChars = "    ";
    private static bool uppercaseInput = false;

    private static readonly List<string> inputFilenames = [];
    private static string? splitInput;
    private static string? outputFilename;

    #endregion

    internal static async Task Main(string[] args)
    {
        Grod baseGrod = new();
        var parseResult = ParseParameters(args, ref baseGrod);
        if (baseGrod == null)
        {
            Environment.Exit(1);
            return;
        }
        if (parseResult != 0)
        {
            Environment.Exit(parseResult);
            return;
        }
        // load data
        var game = new IFGame();
        var gameName = baseGrod.Get(GAMENAME, true);
        if (string.IsNullOrWhiteSpace(gameName))
        {
            OutputText($"Error: {GAMENAME} not found in data file.");
            Environment.Exit(1);
            return;
        }
        game.Initialize(baseGrod, gameName, null);
        string inputFilename = "";
        try
        {
            foreach (var filename in inputFilenames)
            {
                inputFilename = filename;
                var inStream = File.ReadAllLines(inputFilename);
                foreach (var line in inStream)
                {
                    var tempLine = line;
                    if (tempLine.Contains("//"))
                    {
                        tempLine = tempLine[..tempLine.IndexOf("//")].Trim();
                    }
                    if (!string.IsNullOrWhiteSpace(tempLine))
                    {
                        if (splitInput != null && tempLine.Contains(splitInput))
                        {
                            var splitLines = tempLine.Split(splitInput);
                            foreach (var splitLine in splitLines)
                            {
                                inputQueue.Enqueue(splitLine.Trim());
                            }
                        }
                        else
                        {
                            inputQueue.Enqueue(tempLine);
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
            OutputText($"Error opening input file: {inputFilename}");
            return;
        }
        // get settings
        maxOutputWidth = (int)(baseGrod.GetNumber(OUTPUT_WIDTH, true) ?? 0);
        if ((baseGrod.GetNumber(OUTPUT_TAB_LENGTH, true) ?? 0) > 0)
        {
            tabChars = new string(' ', (int)(baseGrod.GetNumber(OUTPUT_TAB_LENGTH, true) ?? 4));
        }
        uppercaseInput = baseGrod.GetBool(UPPERCASE, true) ?? false;
        // start game loop
        game.InputEvent += Input;
        game.OutputEvent += Output;
        await game.Intro();
        await game.GameLoop();
    }

    #region Private Methods

    /// <summary>
    /// Output syntax information.
    /// </summary>
    private static string Syntax()
    {
        StringBuilder result = new();
        result.AppendLine("GRIF - Game Runner for Interactive Fiction");
        result.AppendLine();
        result.AppendLine($"Version {Common.Version}");
        result.AppendLine();
        result.AppendLine("grif <filename.grif | filename.grifstack | directory>");
        result.AppendLine("     [-h  | --help | -?]");
        result.AppendLine("     [-i  | --input  <filename>]");
        result.AppendLine("     [-si | --split-input <splitchar>]");
        result.AppendLine("     [-o  | --output <filename>]");
        result.AppendLine("     [-m  | --mod    <filename.grif | directory>]");
        result.AppendLine();
        result.AppendLine("There may be multiple -m/--mod parameters.");
        return result.ToString();
    }

    /// <summary>
    /// Parse command line parameters.
    /// </summary>
    private static int ParseParameters(string[] args, ref Grod baseGrod)
    {
        if (args.Length == 0)
        {
            OutputText(Syntax());
            return 1;
        }
        try
        {
            int index = 0;
            while (index < args.Length)
            {
                if (args[index].StartsWith('-'))
                {
                    if (index + 1 >= args.Length)
                    {
                        OutputText($"Argument must have a value: {args[index]}\\n\\n");
                        OutputText(Syntax());
                        return 2;
                    }
                    if (args[index].Equals("-h", OIC) ||
                        args[index].Equals("--help", OIC) ||
                        args[index].Equals("-?"))
                    {
                        OutputText(Syntax());
                        return 2;
                    }
                    else if (args[index].Equals("-i", OIC) ||
                        args[index].Equals("--input", OIC))
                    {
                        index++;
                        var inputFilename = args[index++];
                        if (!File.Exists(inputFilename))
                        {
                            OutputText($"Input file not found: {inputFilename}\\n\\n");
                            OutputText(Syntax());
                            return 2;
                        }
                        inputFilenames.Add(inputFilename);
                    }
                    else if (args[index].Equals("-si", OIC) ||
                        args[index].Equals("--split-input", OIC))
                    {
                        index++;
                        splitInput = args[index++];
                    }
                    else if (args[index].Equals("-o", OIC) ||
                        args[index].Equals("--output", OIC))
                    {
                        index++;
                        var tempFilename = args[index++];
                        try
                        {
                            var path = Path.GetDirectoryName(tempFilename);
                            if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            // check if file can be created
                            var outStream = File.CreateText(tempFilename);
                            outStream.Close();
                            outputFilename = tempFilename;
                        }
                        catch (Exception)
                        {
                            OutputText($"Error creating output file: {tempFilename}\\n\\n");
                            OutputText(Syntax());
                            return 2;
                        }
                    }
                    else if (args[index].Equals("-m", OIC) ||
                        args[index].Equals("--mod", OIC))
                    {
                        index++;
                        var modFilename = args[index++];
                        var grod = IO.OpenFile(modFilename); // to check if valid
                        if (grod == null)
                        {
                            OutputText($"Error opening mod file: {modFilename}\\n\\n");
                            OutputText(Syntax());
                            return 2;
                        }
                        if (baseGrod == null || baseGrod.Count(false) == 0)
                        {
                            baseGrod = grod;
                        }
                        else
                        {
                            grod.Parent = baseGrod;
                            baseGrod = grod;
                        }
                    }
                    else
                    {
                        OutputText($"Unknown argument: {args[index++]}\\n\\n");
                        OutputText(Syntax());
                        return 2;
                    }
                }
                else
                {
                    var filename = args[index++];
                    var grod = IO.OpenFile(filename);
                    if (grod == null)
                    {
                        OutputText($"Error opening file: {filename}\\n\\n");
                        OutputText(Syntax());
                        return 2;
                    }
                    if (baseGrod == null || baseGrod.Count(false) == 0)
                    {
                        baseGrod = grod;
                    }
                    else
                    {
                        grod.Parent = baseGrod;
                        baseGrod = grod;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            OutputText($"Error processing parameters: {ex.Message}\\n\\n");
        }
        if (baseGrod == null || baseGrod.Count(false) == 0)
        {
            OutputText(Syntax());
            return 1;
        }
        return 0;
    }

    /// <summary>
    /// Handle input event.
    /// </summary>
    private static void Input(object sender)
    {
        OutputText(((IFGame)sender).Prompt() ?? "");
        string? input;
        if (inputQueue.Count > 0)
        {
            input = inputQueue.Dequeue();
            Console.WriteLine(input);
        }
        else
        {
            input = Console.ReadLine();
        }
        if (input != null)
        {
            if (uppercaseInput)
            {
                input = input.ToUpper();
            }
            OutputTextLog(input + Environment.NewLine);
            var message = new GrifMessage(MessageType.Text, input);
            ((IFGame)sender).InputMessages.Enqueue(message);
            OutputText(((IFGame)sender).AfterPrompt() ?? "");
        }
    }

    /// <summary>
    /// Handle output event.
    /// </summary>
    private static void Output(object sender, GrifMessage e)
    {
        if (e.Type == MessageType.Text)
        {
            OutputText(e.Value);
            return;
        }
        if (e.Type == MessageType.Error)
        {
            OutputText(NL_CHAR);
            OutputText("### ERROR: ");
            OutputText(e.Value);
            return;
        }
        if (e.Value.Equals(OUTCHANNEL_SLEEP, OIC))
        {
            if (!long.TryParse(e.ExtraValue, out long value))
            {
                throw new Exception($"Invalid sleep duration: {e.ExtraValue}");
            }
            Thread.Sleep((int)value);
            return;
        }
    }

    /// <summary>
    /// Output text with word wrapping and special character handling.
    /// </summary>
    private static void OutputText(string text)
    {
        if (text.Contains(SPACE_CHAR))
        {
            text = text.Replace(SPACE_CHAR, " ");
        }
        if (text.Contains('\r') || text.Contains('\n'))
        {
            text = text.Replace("\r", "").Replace("\n", NL_CHAR);
        }
        if (text.Contains(TAB_CHAR) || text.Contains('\t'))
        {
            text = text.Replace(TAB_CHAR, tabChars).Replace("\t", tabChars);
        }
        while (text.Contains(NL_CHAR))
        {
            var index = text.IndexOf(NL_CHAR);
            var before = text[..index];
            text = text[(index + 2)..];
            var lines = IO.Wordwrap(before, outputCount, maxOutputWidth);
            foreach (var line in lines)
            {
                Console.WriteLine(line);
                OutputTextLog(line + Environment.NewLine);
                outputCount = 0;
            }
        }
        if (!string.IsNullOrEmpty(text))
        {
            var lines = IO.Wordwrap(text, outputCount, maxOutputWidth);
            // WriteLine all but last line
            for (int i = 0; i < lines.Count - 1; i++)
            {
                var line = lines[i];
                Console.WriteLine(line);
                OutputTextLog(line + Environment.NewLine);
                outputCount = 0;
            }
            // Write last line with no NL
            if (lines.Count > 0)
            {
                var line = lines[^1];
                Console.Write(line);
                OutputTextLog(line);
                outputCount = line.Length;
            }
        }
    }

    /// <summary>
    /// Output text to log file if specified.
    /// </summary>
    private static void OutputTextLog(string text)
    {
        if (outputFilename == null)
        {
            return;
        }
        try
        {
            using var outStream = File.AppendText(outputFilename);
            outStream.Write(text);
            outStream.Flush();
        }
        catch (Exception)
        {
            // ignore file write errors
        }
    }

    #endregion
}
