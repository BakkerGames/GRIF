# GRIF - Game Runner for Interactive Fiction

> [!NOTE]
> GRIF is still in active development, so expect some features to change as development continues.

GRIF is a game runner for playing Interactive Fiction games from the console. It is a simple engine which loads and runs game data and logic from external files.

It is designed to be as flexible as possible while doing as little as possible. GRIF handles player input, text output, file loading and saving, and simple parsing. Everything else is handled by scripts in the game data files.

The game data may be stored in one or many files. There may even be modification files for adding addtional features to the game. These data and modification files are plain text, either in JSON format or a more readable GRIF format.

GRIF uses the DAGS scripting language, which was developed specifically for Interactive Fiction text games.

GRIF is written in C# .NET 8 in Visual Studio 2022. The full source is available at [GRIF](https://github.com/BakkerGames/GRIF). GRIF compiles to various Windows and Linux executables, which are available at [Releases](https://github.com/BakkerGames/GRIF/releases). It does not require any additional libraries beyond the standard C# System library. The TestGRIFTools project with unit tests does require some NuGet packages but is not necessary to compile or run GRIF.

Source and executables are available at the [Interactive Fiction Archive] (https://www.ifarchive.org/indexes/if-archive/programming/grif/). Games using the GRIF interpreter can be found at [Games] (https://www.ifarchive.org/indexes/if-archive/games/grif/).

Check out the included "Cloak Of Darkness" and "Tic Tac Toe" demo games to see some of the features of GRIF.

Syntax:

```
grif <filename.grif | directory>
     [-i | --input  <filename>]
     [-o | --output <filename>]
     [-m | --mod    <filename.grif | directory>]

There may be multiple "-m" or "--mod" parameters.
```