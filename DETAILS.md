# GRIF Details


## Syntax

```
GRIF
    [filename[.grif] | directorypath]
    [(-i | --input) inputfilename]
    [(-o | --output) logfilename]
    [(-m | --mod) filename[.grif] | directorypath]
```

If no parameters are given, GRIF will load all the `*.grif` files from the current directory. If there are more than one (such as for a large game), it will combine them all in memory. If a directory path is given, it will do the same with all the `*.grif` files in that directory.

The input filename would contain a list of commands to execute. These only work as long as there are no random events, but are handy for walkthroughs and debugging.

The output filename will store a log of the entire game session. It will be cleared before starting the game.

The modification file(s) specified will be loaded after the base data files and will overwrite any values loaded so far. This is a great way to handle `Localization` (see below) or patches/modifications to the base game.


## Sample games

The information mentioned below can be seen in the "Cloak of Darkness" sample game. The "CloakOfDarkness.grif" file contains the entire set of values and scripts to run the game using GRIF. "Cloak of Darkness" is a "Hello world!" starting point for Interactive Fiction engines. Take a look at the code there, and try running it using GRIF. Enjoy!

Another sample game "TicTacToe.grif" is included to show features of GRIF for something other than Interactive Fiction.


## Data files

GRIF data files are simple text files with a ".grif" extension. They hold lines with keys and value, each surrounded by double-quotes and separated by a colon. Any special characters within double-quotes must be escaped by a backslash `\`.

There are two formats supported for GRIF files: JSON format and GRIF format.

The `JSON` format has `{` and `}` at the beginning and end of the file, with each line separated by commas. The last line should not end with a comma. This is less human-readable as scripts cannot have any formatting.

The `GRIF` format is a more relaxed text format, where keys have no leading or trailing whitespace, and all following lines are indented with spaces or tabs and combine into a single value. If there are any leading or trailing spaces in values, use `\s` to indicate the first leading or last trailing space so it won't get trimmed.


Examples:

```
{
    "key1": "value1",
    "key2": "value2",
    "key3": "@if @eq(@get(key1),value1) @then @write(\"Value found!\") @else @write(\"Not found\") @endif",
    ...
}
```

```
key1
    value1
key2
    \s  This value has leading and trailing spaces.  \s
key3
    @if @eq(@get(key1),value1) @then
        @write("Value found!")
    @else
        @write("Not found")
    @endif
...
```

In the `JSON` format, whitespace outside the quotes around keys and values doesn't matter, nor does whitespace within scripts in either format. The order of keys doesn't matter, but keeping them alphabetical can be helpful.

Any special characters within the keys or values must be escaped. For a double-quote, it would be `\"`. A backslash itself would be `\\`. Newline `\n`, carrige-return `\r, and tab `\t` are allowed. Any other non-ASCII characters (chars 0-31 and 127+) must be escaped 4-digit hexadecimal values, `\u####`.

For scripts, where the value starts with a `@`, there can be formatting in the values using newlines, tabs, and spaces. GRIF data files allow this formatting for making scripts easier to read. They will be handled properly while loading. Note that the value still starts and ends with double quotes and internal quotes are escaped.

```
scriptkey
    @comment("This is a comment.")
    @if @true(@get(testvalue)) @then
        @somecommand(value)
    @endif
...
```

Functions can be added to a game data file if the key starts with `@`. These functions can be used in other scripts as shortcuts, instead of duplicating the same code in several places. There are several styles:

```
@func1
    This is a text string used wherever the key is found.
@func2
    @comment("script to be run")
    @write(@func1)
    @dosomething
    @dootherthing(value)
@func3(x)
    @comment("replace $x with parameter x")
    @write("I see $x here.\n")
@func4(x,y,z)
    You have made $x moves and scored $y out of $z points.
```

There may be more than one parameter with any (simple) names desired. `@func4(x,y,z)` replaces `$x`, `$y`, and `$z` in the value with the provided parameters at those positions.

See the DAGS documentation for a full detailed listing of script commands and syntax.


## Save files

GRIF can handle saving and restoring game state data from a save file. It is controlled by the game data file scripts and may not be enabled for all games.

GRIF uses GROD (Game Resource Overlay Dictionary) to hold information about the current state. The overlay dictionary will contain everything which changed since the game started. It is exported to the save file when saving, and cleared and filled from the save file when restoring. It is cleared if the game is being restarted to the beginning.

The directory used for saving is in `Documents` for the current user, in `Documents\GRIF\<gamename>`, and the default save file is `save.dat`.

See `InChannel and OutChannel Queues` for information on handling saving, restoring, and restarting in the game data files.


## Parsing input

GRIF has a simple parser. It can handle the following patterns. Optional items are in square brackets, and there can be multiple adjectives.

```
verb
verb [article] [adjective...] noun
verb [article] [adjective...] noun preposition [article] [adjective...] object
```

It looks up matches using key prefixes in the dictionary. It can understand any items starting with one of these prefixes. The "object" value uses the "noun." key prefix.

```
"verb.", "noun.", "adjective.", "preposition."
```

These contain one or more comma separated words, such as "GO,RUN,WALK,CLIMB" for the key "verb.go". When matching, case is ignored.

Articles are stored in the list `articles`, which contains `a,an,the` and perhaps others. These are mostly ignored when parsing, except there can only be one before each set of adjectives and a noun.

The answers will be stored into "input." values, with "input.verb" and "input.noun" being the second half of the key values found, and "input.verbword" and "input.nounword" being the actual words matched. "input.preposition", "input.prepositionword", "input.object", and "input.objectword" are filled for the last pattern.

When matching succeeds, a CommandKey is returned. This is the script key for the proper command to be run. It is stored in the dictionary as "command.verb" or "command.verb.noun" using the verb and noun identified above. It must exist for this routine to succeed.

If a noun is entered but the command is not found, three special commands will be checked to see if they are in the dictionary for this verb. Avoid using more than one of these for each verb.

```
"command.<verb>.#" will match any noun which is an integer number.
"command.<verb>.*" will match any noun in the dictionary.
"command.<verb>.?" will match anything entered as a noun, even if not in the dictionary.
```

"command.verb.#" is useful for combination locks, pacing off steps before digging, etc., where any number might be entered, but most are invalid. There should be commands for valid ones, like "command.pace.30" or "command.left.15". Or the command with "#" could check the entered number sent in "input.nounword".

"command.verb.\*" is useful for generic handling, as "command.take.\*" displaying "I don't see that here."

"command.verb.?" is good for unknown words, or filenames for saving, or anything undefined. Good for examining items, when the noun is not defined but might be mentioned in the room text. "command.examine.?" could display "There is nothing special about the " and the noun.

When "#" or "?" special commands are matched, "input.noun" will contain "?" or "#" and "input.nounword" will contain what was entered.


## Data used by GRIF

GRIF expects that there will be certain keys and values in the data file so it can run. They will have special prefixes to indicate their purpose. Here are the ones GRIF expects:

`verb.???`

One line for each verb used in the game. The value contains all synonyms separated by commas. Note that having directions as verbs allows them to be used directly. Some older games use numbers as the verb "name", like `verb.10`, and may limit each synonym to a small number of characters.

```
verb.go
    go,walk,run
verb.climb
    climb,ascend
verb.take
    take,get,grab
verb.drop
    drop,throw
verb.north
    north,n
verb.south
    south,s
```

`noun.???`

One line for each noun used in the game. The value contains all synonyms separated by commas. Note that having directions as nouns allows them to be used with a verb, as in `go north` or `climb up` (but does increase the number of commands needed). Some older games use numbers as noun "names".

```
noun.north
    north,n
noun.south
    south,s
noun.cup
    cup,glass,goblet
```

`adjective.???`

The second part of `adjective.???` is the noun for which the adjectives apply, and the value is a list of valid adjectives. The adjectives within descriptive text should be included for each noun. For example:

```
adjective.ball
    blue,rubber,large
adjective.vase
    crystal,fragile
```

`command.<verb>`, `command.<verb>.<noun>`, `command.<verb>.<noun>.<preposition>.<noun>`

One for each allowed verb, verb/noun, and verb/noun/preposition/noun combination allowed during the game. These would have scripts to be run when the command is entered.

The noun part can be replaced with "\*" to mean any noun which is not specifically covered in another command. (See "input.noun" for info on processing these.)

The noun part can be replaced with "#" to mean any number. Some commands need a number but there could be many possibilities. The value in "input.noun" will be "#" and "input.nounword" will contain the entered number. Note that if you have `command.<verb>.#` it will supercede all other commands for a verb with numeric values.

The noun part can also be replaced with "?" to mean any word, known or unknown. Some commands need some value, such as "SAVE filename". The value in "input.noun" will be "?" and "input.nounword" will contain the value. Note that if you have `command.verb.?` it will supercede all other commands for that verb with a noun.

```
command.north
    @moveplayer(north)
command.go.north
    @moveplayer(north)
command.take.book
    @takethebook
command.take.*
    @takesomething
command.pace.#
    @pacebeforedigging
command.save.?
    @savetofilename
```

`background.???`

These are scripts which are run once per game loop and handle background tasks. All background scripts are run each time, so they may wish to save and check values to know whether to run them or not. For instance, a script to show the beginning introduction message only once might be look like this:

```
value.introdone
    false
...
background.intro
    @if @false(@get(value.introdone)) @then
        @msg(message.intromessage)
        @set(value.introdone,true)
        @look
    @endif
```

`system.???`

There are several values which the GRIF program expects, and they are prefixed by `system.`. The system prefix can be used in other scripts for other reasons. They are usually static configuration values.

For all of these, `system.gamename` is the only critical one, as GRIF has no other way to know what game it is running! An error will be thrown if the game data file doesn't have this one.

The value for `system.wordsize` is required if the vocabulary does not consist of whole words, but only the first few letters of each word. It is used to compare the beginning of the words typed by the player during parsing. Older games might only have the first 3 or 5 letters significant. If the vocabulary does have whole words, use "0" or don't set it to anything.

These values may be changed however the game developer desires.

```
system.gamename
    MyGame
system.intro
    Welcome to MyGame!\n
system.wordsize
    0
system.prompt
    \n>
system.after_prompt
    \n
system.output_width
    80
system.dont_understand
    I don't understand "{0}".
system.do_what_with
    Do what with the {0}?
```

`system.intro` is a script or text which will be run once at the start of the game.

`system.prompt` is shown each time the player can enter something. If it is "", no prompt is shown. It can also be a script that changes the prompt as appropriate.

`system.after_prompt` is shown after the player entered something. For instance, `\n` will add a blank line.

`system.output_width` controls the width of text messages if automatic word wrapping should done. It always breaks at a space so whole words are shown. Setting it to "0" turns off any automatic wrapping. This isn't necessary if all the long messages and descriptions have `\n` at the proper line breaks, or if word wrapping doesn't matter.

`system.dont_understand` is used whenever the parser can't understand what has been typed. If a word doesn't match any verb or noun in the data file, this message will be used, replacing the "{0}" with the unknown value.

`system.do_what_with` is used by the parser if the player enters a noun only, or uses a recognized noun but an unknown verb. It will replace "{0}" in the value with the noun entered.


## Input values

The prefix `input.` is used by GRIF to send information typed by the player over to the game scripts so it can be used as needed. These values are temporary and change each time the player's input is parsed. They are not usually stored in the initial game data but appear as the game is run.

`input.verb`, `input.verbword`, `input.noun`, `input.nounword`, `input.preposition`, `input.prepositionword`, `input.object`, `input.objectword`

The "verb" and "noun" entered by the player are sent to the DAGS engine in the values for `input.verb` and `input.noun`. These could then be used in a script. So `command.take.*` would be run and know that `@get(input.noun)` has the item name the player wanted to take. These are the general names, as in `noun.cup` above, even if the player entered "take goblet". The specific words the player used, such as "goblet", are sent in `input.verbword` and `input.nounword`, in case those are necessary for messages.

There is also a queue GRIF can use to send information to the scripts. See "InChannel and OutChannel Queues".


## Other prefixes and functions

It can be useful to have a standard for keys so that they begin with a prefix indicating their purpose. These are only referenced by scripts, not by GRIF. Obviously these are suggestions and you can use your own as you wish.

`value.???`

This is a catch-all prefix for any game data which is needed from the beginning of the game to the end. It can hold things such as your current score, the room you are in, the number of points for getting a treasure, whether your lamp is on or off, and so on. These are the values which change as the game is played. For instance, `value.room` would hold the current room name/number the player is in.

`message.???`

This is used for text messages which will be displayed at some point in the game. Some games have numbers for messages, so `message.1008` might mean "OK". Putting them all under the same prefix makes them easy to find. The `@msg()` command in DAGS is designed for these, so you would only need to call `@msg(message.found_a_treasure)` to output that message and automatically add a following newline.

`script.???`

Useful for storing common scripts, such as anything used in more than one place. `@script(script.name)` would run that script.

`room.???.???`

This helps to organize rooms and room properties. The first "???" would be the room name/number, and the second is the property name. `room.foyer.shortdesc` and `room.foyer.longdesc` are easy to understand that way. The `@look` function (see below) would handle which one to show, using `room.???.visited`. There might be a need for `room.???.afterlook` to hold special scripts which would run after looking in the room (which would also handled by the `@look` script).

`room.???.exit.???`

This holds all the exit locations for the room. The first "???" would be the room name/number, and the second is the direction such as "north". It would have the name of another room, or a script to run instead of moving that way. The movement scripts would control this.

`item.???.???`

Same as rooms, it is simple to store items by name and property. Items would also have ".longdesc" and ".shortdesc" (or maybe ".invdesc"). The most important property is ".location", where the item is at the current time. It would be a room name/number, or "inventory" (or "-1") for being carried by the player, or even the name/number of another container item.

`@look`

The most common action is being able to look at the description and contents of a room. A function `@look` is handy to show the proper description and list all the items visible in the room. The format of this function is different for almost every game, which is why making it a callable function makes the most sense instead of building it into GRIF. It can be used in several places, such as after moving to a new room, when entering the command "look", or after restoring a saved game.

`@dark`

This is a very useful way to put all the dark logic in one place. This one returns "true" or "false" and is called at the very beginning of `@look` (and other places). `@if @dark @then ... @else ... @endif`

`@room`

To avoid extra typing, use a function `@room` to return `@get(value.room)` so it is easily checked in scripts.

`@inv`

This holds the value which means an item is in your inventory. Some games use "inventory", some "player", some "-1". It can also be a script instead of a value for games with multiple inventories.

`@carry(x)`, `@here(x)`, `@herecarry(x)`

These are for determining the location of an item. Are you carrying it, is it here in the room, or is it either one? These return "true" or "false". They compare `@get(item.$x.location)` to `@inv` and/or `@room`.

`@take(x)`, `@drop(x)`

These functions are shortcuts for moving an object into inventory or putting it into the room. These are very common tasks, so having these defined will save a lot of typing. `@take(x)` should probably check if your inventory is full first.

`goto(x)`

Another handy function for moving the player to a specified room. It might have logic for incrementing the number of moves and running `@look` afterwards.


## InChannel and OutChannel Queues

Scripts often need to send commands to GRIF or to get information from the player or the outside world. There are two queues used for these purposes, called InChannel and OutChannel, for sending data into and out of DAGS. Queues are lists of information in a first-in, first-out format.

OutChannel is used by game scripts to send information to GRIF. These are specific values which GRIF will look for as special commands, or values starting with `@` indicating DAGS scripts to be run.

Game scripts can add values to the queue using `@setoutchannel("value")`. In each game loop, OutChannel is checked after the background scripts run, and again after the parsed command is run.

InChannel is used by GRIF to send information back to the scripts, such as when a question is asked and the player has to answer. The scripts can use `@getinchannel` to get the next value from the queue.

Here are the OutChannel values GRIF expects. They must be exactly as specified, except case doesn't matter.

`#GAMEOVER;` immediately terminates the game. No further processing is done or messages displayed.

`#ASK;` will ask the player to enter a value. The messages about this should already have been displayed. The prompt will be shown and the answer pushed to InChannel but not parsed. This is used for Yes/No questions or more detailed ones.

`#ENTER;` will wait for the player to press ENTER. The messages about this should already have been displayed. If they type anything, it is ignored.

`#EXISTS;` will check if the default save file exists. It returns `true` or `false` into the InChannel.

`#EXISTSNAME;` will check if the specified save file name exists. It returns `true` or `false` into the InChannel.

`#SAVE;` will save the current game state to the default save file `save.dat`. See above for path info. It will overwrite the file if it already exists.

`#SAVENAME;` will save the current game state to a filename which will be the next item in the OutChannel queue.

`#RESTORE;` will restore the current game state from the default save file `save.dat`.

`#RESTORENAME;` will restore the current game state from the save file name specified in the next item in the OutChannel queue.

`#RESTART;` will restart the game from the beginning.

Scripts in the OutChannel queue starting with `@` are run directly using DAGS. The above situations may require a script to be run afterwards, such as `#ASK;`. The next OutChannel command would be a script to handle that entered value. Note that the second `@setoutchannel` has to put escaped quotes around the script or else it would run immediately.

```
script.ask_about_intro
    @msg(message.show_intro_yes_or_no)
    @setoutchannel("#ASK;")
    @setoutchannel("@script(script.handle_answer)")
    ...
script.handle_answer
    @set(temp.yorn,@getinchannel)
    @if @true(@get(temp.yorn)) @then ...
```


## Noun/Item mismatch

Occasionally there will be a game which has a mismatch between nouns and item names/numbers. For example, the Scott Adams "Pirate's Adventure" game has four different bottle items all using the noun "BOT". The game logic prevents more than one being available at the same time but it does make it difficult in `command.take.bot` or `command.take.*` to know which one is being referenced.

One way to handle this is to use a special set of data lines, `nounitem.???`. The "???" is the noun name/number, and the value holds a list of all items with this noun. A `@foreachlist` loop checks each value in the list to see if that item is here. The code (this game uses numbers, not names) looks something like this. Do something similar for dropping or any other uses.

```
verb.10
    get,tak
noun.11
    bot,rum
nounitem.11
    7,25,42,49
command.10.*
    @comment("take first item that matches noun")
    @set(temp.found,false)
    @foreachlist(x,@concat("nounitem.",@get(input.noun)))
        @if @here($x) @and @false(@get(temp.found)) @then
            @take($x)
            @set(temp.found,true)
            @write("OK\n")
        @endif
    @endforeachlist
    @if @false(@get(temp.found)) @then
        @write("I DON'T SEE THAT HERE.\n")
    @endif
```


## Localization

Many Interactive Fiction games are written in English, but players with other native languages may wish to play games too. For this reason, everything possible has been done to allow the GRIF game text to be easily changed.

It is possible to have different versions of game data files for different languages. Make separate files for all the descriptions, messages, and vocabulary for each desired language. This is one reason the data files are in simple text (JSON) format, so they can be easily modified. The game scripts and other non-text values don't need to be duplicated in the language files. Make sure to leave the keys unmodified. Also remember that non-ASCII accented characters must be stored as `\u####` unicode values.

Start GRIF with a base data file (with one language, or not) and indicate the language file with the text values for the desired language. Use the `-m | --mod` command line parameter, something like this:

```
GRIF basegame.grif -m spanish.grif
```

The current parser does expect VERB or VERB NOUN commands. The verb words and noun words can be in any languages, but the pattern must be maintained. It also doesn't handle multi-word verbs or nouns (yet). Maybe soon...

When using other languages for vocabulary, `system.wordsize` will probably need to be zero and all verbs and nouns given as full words.


## Meta Commands

There are two meta commands available while running a GRIF game, for debugging purposes. These are only enabled if a command line option `--meta` is added when running. They are very useful for displaying or modifying data and for testing scripts.

`#exec <script>`

This will execute the script specified. It must start with `@`.

`#debug <script>`

This will execute the script specified and display script text and various values as it runs. It must start with `@`.