# GRIFData - Loading and saving GRIF game data files

This is a simple class library to handle the loading and saving of game data files for GRIF (Game Runner for Interactive Fiction).

The format of a GRIF game data file is a JSON object with keys and values that are strings. The keys cannot be blank, null, or only whitespace and values cannot be null. Both must be quoted and any internal quotes or other special characters must be escaped. Non-ASCII chars must be in the format "\u####" for the hexadecimal character code.

The DAGS scripts and all script commands/functions start with `@`. Anything else is a string value.

The keys in the output file are sorted both alphabetically and numerically. If a key uses "." to separarate sections and has numeric sections, like `room.23.description`, it would be sorted in this case alphabetically for the first and third sections but numerically for the second. A section with the generic value of `*` is always placed first, with `?` and `#` following before other values.

GRIFData uses DAGS (Data Access Game Scripts) for formatting the game scripts and GROD (Game Resource Overlay Dictionary) for holding in-memory text resources. See the [DAGS](https://github.com/BakkerGames/DAGS) and [GROD](https://github.com/BakkerGames/GROD) GitHub sites for information on those.