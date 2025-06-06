﻿using GRIFTools;
using GRIFTools.Parse;
using System.Text;
using static GRIF.Constants;

namespace GRIF;

internal class Program
{
    static void Main(string[] args)
    {
        try
        {
            // Path for data files, default to current directory
            var dataPath = ".";
            var modFilenames = new List<string>();
            var allowMeta = false;

            // Nothing specified
            if (args.Length == 0)
            {
                UserIO.Output(SystemData.Syntax());
                return;
            }

            // Parse command-line arguments
            int argIndex = 0;
            while (argIndex < args.Length)
            {
                var argValue = args[argIndex++];
                var value = argValue;
                if (value.StartsWith('/'))
                {
                    if (value.Length == 2)
                    {
                        value = "-" + value[1..];
                    }
                    else
                    {
                        value = "--" + value[1..];
                    }
                }
                if (value.Equals("--input", OIC) || value.Equals("-i", OIC))
                {
                    var inputFilename = args[argIndex++];
                    if (!File.Exists(inputFilename))
                    {
                        UserIO.Output("Input file not found: " + inputFilename);
                        UserIO.Output(DAGSConstants.NL_VALUE + DAGSConstants.NL_VALUE);
                        UserIO.Output(SystemData.Syntax());
                        return;
                    }
                    // Input commands
                    UserIO.InitInputFile(inputFilename);
                }
                else if (value.Equals("--output", OIC) || value.Equals("-o", OIC))
                {
                    // Output to a log file
                    UserIO.InitLogFile(args[argIndex++]);
                }
                else if (value.Equals("--mod", OIC) || value.Equals("-m", OIC))
                {
                    // Modiification files
                    modFilenames.Add(args[argIndex++]);
                }
                else if (value.Equals("--meta", OIC))
                {
                    // allows special commands starting with "#".
                    allowMeta = true;
                }
                else if (value.StartsWith('-'))
                {
                    UserIO.Output("Unknown parameter: " + argValue);
                    UserIO.Output(DAGSConstants.NL_VALUE + DAGSConstants.NL_VALUE);
                    UserIO.Output(SystemData.Syntax());
                    return;
                }
                else
                {
                    // Data filename or path to all data files
                    dataPath = value;
                }
            }

            if (string.IsNullOrWhiteSpace(dataPath))
            {
                UserIO.Output(SystemData.Syntax());
                return;
            }

            // Prepare engines
            Grod grod = new()
            {
                UseOverlay = false
            };
            Dags dags = new(grod);

            // Load data
            if (File.Exists(dataPath + DATA_EXTENSION))
            {
                GrodDataIO.LoadDataFromFile(dataPath + DATA_EXTENSION, grod);
            }
            else if (File.Exists(dataPath))
            {
                GrodDataIO.LoadDataFromFile(dataPath, grod);
            }
            else if (Directory.Exists(dataPath))
            {
                var files = Directory.GetFiles(dataPath, "*" + DATA_EXTENSION);
                if (files.Length > 0)
                {
                    foreach (string filename in files)
                    {
                        GrodDataIO.LoadDataFromFile(filename, grod);
                    }
                }
            }
            else
            {
                try
                {
                    var pathName = Path.GetDirectoryName(dataPath) ?? "";
                    var fileName = Path.GetFileName(dataPath);
                    var files = Directory.GetFiles(pathName, fileName);
                    if (files.Length > 0)
                    {
                        foreach (string filename in files)
                        {
                            GrodDataIO.LoadDataFromFile(filename, grod);
                        }
                    }
                    else
                    {
                        UserIO.Output("File or directory not found: " + dataPath);
                        UserIO.Output(DAGSConstants.NL_VALUE + DAGSConstants.NL_VALUE);
                        UserIO.Output(SystemData.Syntax());
                        return;
                    }
                }
                catch (Exception)
                {
                    UserIO.Output("File or directory not found: " + dataPath);
                    UserIO.Output(DAGSConstants.NL_VALUE + DAGSConstants.NL_VALUE);
                    UserIO.Output(SystemData.Syntax());
                    return;
                }
            }

            // Check if anything was loaded
            if (grod.Count() == 0)
            {
                UserIO.Output(SystemData.Syntax());
                return;
            }

            // Load the modification file(s) afterwards so base values will be overwritten
            foreach (string modFile in modFilenames)
            {
                if (File.Exists(modFile + DATA_EXTENSION))
                {
                    GrodDataIO.LoadDataFromFile(modFile + DATA_EXTENSION, grod);
                }
                else if (File.Exists(modFile))
                {
                    GrodDataIO.LoadDataFromFile(modFile, grod);
                }
                else if (Directory.Exists(modFile))
                {
                    var files = Directory.GetFiles(modFile, "*" + DATA_EXTENSION);
                    if (files.Length > 0)
                    {
                        foreach (string filename in files)
                        {
                            GrodDataIO.LoadDataFromFile(filename, grod);
                        }
                    }
                }
                else
                {
                    UserIO.Output("Modification file or directory not found: " + modFile);
                    UserIO.Output(DAGSConstants.NL_VALUE + DAGSConstants.NL_VALUE);
                    UserIO.Output(SystemData.Syntax());
                    return;
                }
            }

            // Validate data
            StringBuilder result = new();
            if (!dags.ValidateDictionary(result))
            {
                UserIO.Output(result);
                return;
            }

            // Initialize static classes
            SystemData.Init(grod, dags);
            DagsIO.Init(grod, dags);
            Metadata.Init(grod, dags);
            Parse.Init(grod);

            // Make sure necessary values exist
            if (!SystemData.SystemValidate(result))
            {
                UserIO.Output(result);
                return;
            }

            // Initialize game variables
            grod.UseOverlay = true;
            ParseResult parseResult = new();
            DagsIO.GameOver = false;

            // Introduction
            result.Clear();
            dags.RunScript(SystemData.Intro(), result);
            UserIO.Output(result);
            DagsIO.CheckOutChannel();

            // Game loop
            while (!DagsIO.GameOver)
            {
                // Run all background scripts
                result.Clear();
                parseResult.Clear();
                DagsIO.RunBackgroundScripts(result);
                UserIO.Output(result);
                DagsIO.CheckOutChannel();
                if (DagsIO.GameOver) break;

                do
                {
                    // Prompt
                    string input;
                    UserIO.Output(SystemData.Prompt());
                    input = UserIO.GetInput();
                    UserIO.Output(SystemData.AfterPrompt());

                    // Check for meta commands
                    if (allowMeta && input.StartsWith('#'))
                    {
                        result.Clear();
                        Metadata.CheckMetaCommand(input, result);
                        UserIO.Output(result);
                        DagsIO.CheckOutChannel();
                        if (DagsIO.GameOver) break;
                        parseResult.CommandKey = "";
                        continue;
                    }

                    // Parse input
                    parseResult = Parse.ParseInput(input);
                    UserIO.Output(parseResult.Error, true);

                } while (parseResult.CommandKey == "");
                if (DagsIO.GameOver) break;

                // Run the script
                result.Clear();
                dags.RunScript($"@script({parseResult.CommandKey})", result);
                UserIO.Output(result);
                DagsIO.CheckOutChannel();
            }
        }
        catch (Exception ex)
        {
            UserIO.Output(ex.Message);
        }
#if DEBUG
        Console.WriteLine();
        Console.Write("Press enter to continue...");
        Console.ReadLine();
#endif
    }
}
