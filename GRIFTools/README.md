# GRIFTools

> [!NOTE]
> GRIFTools is still in active development, so expect certain features to change or disappear as development continues.

GRIFTools is a library containing the GROD, DAGS, and GameData classes used by GRIF and similar projects. Putting them together in a single library simplifies development and distribution.

# GROD - Game Resource Overlay Dictionary

GROD is a special pair of dictionaries, a base and an overlay, for holding string key/value pairs which are modified over time. It is designed for games where progress from the beginning can be saved and restored.

The base dictionary is filled with the starting values at the beginning of a game, and the overlay holds all the changes to the base data as the game is played.

Getting a value during the game will return the overlay value if found, or the base value if found, or "". Setting a value during the game always writes to the overlay. No errors are thrown if the key doesn't exist yet or if it is added multiple times.

When saving the game progress, only the overlay values need to be stored into a save file. When restoring a save file, the overlay is cleared and loaded with the saved values, returning to the state at the time of the save.

Set UseOverlay = false to load the base values, and then set UseOverlay = true to play the game.

Keys are strings and cannot be null, "", or only whitespace, but all others are valid. Keys are case-insensitive and trimmed. Use the Keys() function to get a list of all keys or KeysOverlay() for overlay keys only, such as when saving.

Values are strings. If value is null it is changed to "" but no other modifications are performed. Other data types would be converted to and from string values by the calling program.

# DAGS - Data Access Game Scripts

DAGS is a simple scripting engine which directly connects to a GROD key/value dictionary. It has commands to get and set data and manipulate it in various ways. It is interpretive, not compiled, to give it greater flexibility.

DAGS consists of functions starting with "@". They can have no parameters or a list of parameters in `()`. Parameter values can be strings or other functions returning strings. Strings are anything not starting with "@", and don't need quotes around them if they have no spaces or special symbols. Some functions, such as arithmetic functions, expect the string parameters to be numbers or functions returning numbers.

Many of the built-in functions directly access the data from the GROD dictionary. "@get(mykey)" reads the key "mykey" and returns the value. "@set(mykey,myvalue)" sets mykey=myvalue in the dictionary. Other functions return text to the calling program or execute other scripts.

DAGS can be extended by creating new functions and adding them to the dictionary. They are used exactly like the built-in functions.

# DAGS Script Library

This contains the DAGS constants and routines for handling DAGS scripts.

# GameData - Loading and saving GROD data files

GameData is a class to handle the loading and saving of GROD data files.

The format of a GROD data file can either be JSON or GRIF.

JSON files start with `{` and end with `}` and have valid JSON strings for keys and values, with special characters escaped using `\`. They are great for portability but less readable.

GRIF files are less structured. Keys are on lines with no leading whitespace. Values are on the following line(s), each with leading TAB or SPACE character(s). GRIF values can span mulitiple lines, which are joined back together with a single space. Special characters don't need escaping. Script and array values are formatted for easier reading.

The keys for both formats are sorted appropriately as the files are written.
