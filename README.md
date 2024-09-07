# GRIF - Game Runner for Interactive Fiction

> [!NOTE]
> GRIF and GRIFTools are still in active development, so expect certain features to change or disappear as development continues.

GRIF is a game runner for playing Interactive Fiction games from the console. It is a simple engine which loads and runs game data and logic from external files.

It is designed to be as flexible as possible while doing as little as possible. GRIF handles player input, text output, file loading and saving, and simple parsing. Everything else is handled by scripts in the game data files.

The game data may be stored in one or many files. There may even be modification files for adding addtional features to the game. These data and modification files are plain text, either in JSON format or a more readable GRIF format.

GRIF uses the DAGS scripting language, which was developed specifically for Interactive Fiction text games.

GRIF is written in C# .NET 8. The full source is available, plus the required [GRIFTools](https://github.com/BakkerGames/GRIFTools) library containing the GROD, DAGS, and GameData projects. GRIF compiles to various Windows and Linux binaries.

Check out the "Cloak Of Darkness" and "Tic Tac Toe" demo games to see some of the features of GRIF.

Syntax:

```
grif <filename.grif | directory>
     [-i | --input  <filename>]
     [-o | --output <filename>]
     [-m | --mod    <filename.grif | directory>]

There may be multiple "-m" or "--mod" parameters.
```