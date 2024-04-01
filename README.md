# GRIF - Game Runner for Interactive Fiction

GRIF is a game runner for playing Interactive Fiction games from the console. It is designed so that the game data and logic are stored in external `*.grif` data files, which GRIF loads and executes.

GRIF handles player input, text output, file loading and saving, and parsing. Everything else is done by the game scripts.

The game data may be stored in one or many data files. There may also be extra modification files to add addtional features to the game. Game data my be localized into different written languages by translating the text strings and storing in modification files, one per language. These files are in JSON-formatted plain text.

GRIF uses the DAGS scripting language, which was developed specifically for Interactive Fiction text games.

GRIF is written in C# .NET 8. The full source is available, including the sub-projects GRIFData, DAGS, and GROD. It comples to various Windows and Linux binaries.

See the GRIF/DETAILS.md file for complete information on using GRIF, as well as the various README.md files in the sub-projects. DAGS/SYNTAX.md contains the full specification for DAGS scripts.

Check out the "Cloak Of Darkness" demo game which shows some of the features of GRIF and DAGS.

Syntax:

```
grif <filename.grif|directory>
     [-i|--input  <filename>]
     [-o|--output <filename>]
     [-m|--mod    <filename.grif|directory>]

If nothing is specified, GRIF will load all the "*.grif" files in the current directory.

There may be more than one -m|--mod parameter.
```