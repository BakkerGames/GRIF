# GROD - Game Resource Overlay Dictionary

GROD is a special pair of dictionaries, a base and an overlay, for holding key/value pairs which can be modified from the base value. It is designed for games where progress from the beginning can be saved and restored.

The base dictionary is filled with the starting values at the beginning of a game, and the overlay holds all the changes as the game is played.

Getting a value during the game will return the overlay value if found, or else the base value if found, or "". Setting a value during the game always writes to the overlay. No errors are thrown if the key doesn't exist yet or if it is added multiple times.

When saving the game progress, only the overlay values need to be stored into a save file. When restoring a save file, the overlay would be cleared and loaded with the saved values, returning to the game state at the time of the save.

Set UseOverlay = false to load the base values, and then set UseOverlay = true to play the game.

Keys are strings and cannot be null, "", or only whitespace, but all others are valid. Keys are case-sensitive. Use the Keys() function to get a list of all keys or KeysOverlay() for overlay keys only, such as when saving.

Values are strings. If value is null it is changed to "" but no other modifications are performed. Other data types can be converted to and from string values by the calling program.