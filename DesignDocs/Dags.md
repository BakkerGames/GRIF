# DAGS Syntax

DAGS is a simple scripting language which is tied directly to a GROD in-memory database. Many of the functions read or write values in the database.

DAGS scripts consist of functions starting with "@" with zero or more parameters defined. Parameter values can be strings or other functions returning strings. Strings are anything not starting with "@", and don't need quotes around them unless they have spaces or special symbols (including "@"). Some functions, such as arithmetic functions, expect the string parameters to be numbers or functions returning numbers.

Some functions take keys and operate directly on the database, while other take values and operate on those values. Be careful of this! `@true(key)` doesn't check the value but is always false (the string "key" isn't true), while `@true(@get(key))` gets the correct answer from the value stored in `key` in the database.

At times it might be necessary to add quotes around values so they don't execute immediately. Quotes are always the double-quote value `"`. The value will need to be surrounded by quotes and internal quotes escaped as `\"`.

Scripts are processed by Dags.Process(grod, script), returning a list of GrifMessage results. These can be displayed to the user if their type = Text or may be commands to the calling program if their type = OutChannel.


## Constants

null

true

false


## Statements

@comment("comment text")

>Used for commenting code. Quotes are recommended and are necessary if there are spaces or special characters. If there is a value "system.debug" set to "true", comments will be output and not ignored, and can even be scripts for showing values.

@debug(value)

>If `system.debug` is `true`, display the value if text, or execute the value and display the results if a script. If `system.debug` is false or doesn't exists, these are ignored. Text values should be quoted.

@exec("script")

>Executes the script specified by "script". "script" should be quoted.

@golabel(value)

>Moves the script pointer to the statement after @label(value).

@label(value)

>Value to be used by @golabel(value) statement.

@script(key)

>Runs the script stored in "key".

@set(key,value)

>Sets the value for "key" in the database to "value". If "value" is a script, it stores the final result. If "value" is a script but is quoted, the script is stored directly.

@swap(key1,key2)

>Swaps the values stored in "key1" and "key2".


## Numeric Statements

Numeric statements operate on 64-bit integer values stored in the database.

@addto(key,value)

>Adds integer "value" to that stored in "key". Stores the answer back into "key".

@subto(key,value)

>Subtracts integer "value" from that stored in "key". Stores the answer back into "key".

@multo(key,value)

>Multiples integer "value" with that stored in "key". Stores the answer back into "key".

@divto(key,value)

>Divides integer stored in "key" by integer "value" and discards the remainder. Stores the answer back into "key".

@modto(key,value)

>Divides integer stored in "key" by integer "value". Stores the remainder back into "key".

@negto(key)

>Negates the integer stored in "key". Stores it back into "key".


## Output Statements

@msg(key)

>Writes the processed value stored in "key" plus "\n" (two separate characters) to result.

@nl

>Writes "\n" (two separate characters) to result.

@write(value[,value...])

>Writes all the values concatinated to result.

@writeline(value[,value...])

>Writes all the values concatinated to result, followed by "\n" (two separate characters).


## Functions

@abs(value)

>Returns the absolute value of integer "value".

@add(value1,value2)

>Returns the integer answer from adding "value1" and "value2".

@concat(value[,value...])

>Concatenates all specified values into a single string.

@div(value1,value2)

>Returns the integer answer from dividing "value1" by "value2".

@format(value,v0[,v1...])

>Replaces tokens "{0}", "{1}"... in value with v0, v1... and returns the result.

@get(key)

>Returns the value for "key" from the database.

@getvalue(key)

>Returns the processed value for "key", running it as a script if necessary.

@lower(value)

>Returns "value" changed to all lowercase.

@mod(value1,value2)

>Returns the integer remainder from dividing "value1" by "value2".

@mul(value1,value2)

>Returns the integer answer from multiplying "value1" and "value2".

@neg(value1)

>Negates the integer answer for "value1".

@replace(value,old,new)

>Returns "value" with all occurances of "old" changed to "new".

@return

>Immediately exit the current script.

@rnd(value)

>Returns a random integer from 0 to "value" minus 1.

@sub(value1,value2)

>Returns the integer answer from subtracting "value2" from "value1".

@substring(value,start) or @substring(value,start,len)

>Returns the substring starting at position "start". If "len" is specified, return up to "len" characters. Returns "" if out of range.

@trim(value)

>Trims leading and trailing spaces from "value".

@upper(value)

>Returns "value" changed to all uppercase.


## If Block

@if

>Starts an "@if" block. Followed by condition statements until "@then".

@then

>End of conditions, begin running statements if conditions were true. Required after "@if" and after each "@elseif".

@elseif

>Starts another "@if" block if the first one was not true. Followed by conditions, "@then", and statements. Multiple "@elseif" blocks may be chained one after another. Optional.

@else

>Final statements to be processed if all previous conditions were false. Optional.

@endif

>Ends the "@if" block. Required.


## If Conditions

Conditions return string "true"/"false" values, or values that are "truthy" or "falsey".

"true", "t", "on", "yes", "y", "1", and "-1" are truthy.

"false", "f", "off", "no", "n", "0", "null", and "" are falsey. Undefined values are also falsey.

Conditions are processed strictly left-to-right with no parenthesis used for grouping. They are connected with "@and" and "@or", which short-circuit when possible. "@not" preceeding the condition will reverse the answer. It is recommended that "@and" and "@or" not be mixed in the same "@if" condition.

Any functions which returns truthy or falsey values may be defined and used as "@if" conditions.

@eq(value1,value2)

>Checks if the two values are equal. Compares as integers if both convert to integers, otherwise compares as strings (ignoring case).

@exists(key)

>Returns true if "key" exists and the value is not "" or "null".

@false(value)
@isfalse(value)

>Returns true if "value" is falsey. Returns false if the value is truthy or isn't boolean.

@ge(value1,value2)

>Checks if "value1" is greater than or equal to "value2". Compares as integers if both convert to integers, otherwise compares as strings (ignoring case).

@gt(value1,value2)

>Checks if "value1" is greater than "value2". Compares as integers if both convert to integers, otherwise compares as strings (ignoring case).

@isbool(value)

>Returns true if "value" is truthy or falsey.

@isnumber(value)

>Returns true if "value" is an integer number.

@isscript(key)

>Checks if the value for "key" starts with "@". Does not run the script.

@le(value1,value2)

>Checks if "value1" is less than or equal to "value2". Compares as integers if both convert to integers, otherwise compares as strings (ignoring case).

@lt(value1,value2)

>Checks if "value1" is less than "value2". Compares as integers if both convert to integers, otherwise compares as strings (ignoring case).

@ne(value1,value2)

>Checks if the two values are not equal. Compares as integers if both convert to integers, otherwise compares as strings (ignoring case).

@null(value)

>Returns true if "value" is "" or "null".

@rand(value)

>Checks if a random integer 0-99 is less than integer "value" 1-100. Shortened version of "@lt(@rnd(100),value)".

@true(value)
@istrue(value)

>Returns true if "value" is truthy. Returns false if the value is falsey or isn't boolean.


## Condition Connectors/Modifiers

@and

>Stops processing if the condition so far is false and jumps over the "@then" statements to "@elseif", "@else", or "@endif". Otherwise continues.

@or

>Stops processing if the condition so far is true and jumps to the "@then" statements. Otherwise continues.

@not

>Reverses the answer on the next condition.


## For Loop

@for(token,start,end)

>Executes the code in the "for" block multiple times, by replacing "$token" (the token with a leading "$") anywhere in it with the numeric values from "start" to "end" (inclusive). Tokens can be anything, such as "i", "x", "y", "token". Nesting is allow if the tokens are different.

@endfor

>Marks the end of the "@for" loop.


## ForEachKey Loop

@foreachkey(token,prefix) or @foreachkey(token,prefix,suffix)

>Executes the code in the "foreachkey" block multiple times, by replacing "$token" (the token with a leading "$") anywhere in it. It loops through all the keys in the database which start with "prefix" and optionally end with "suffix". The value replaced for "$token" is the remaining part of the key after the prefix and the optional suffix are removed. Tokens can be anything, but should have no spaces or special characters. Nesting is allow if the tokens are different. Note that the order of keys returned is not deterministic.

@endforeachkey

>Marks the end of the "@foreachkey" loop.


## ForEachList Loop

@foreachlist(token,name)

>Executes the code in the "foreachlist" block multiple times, by replacing "$token" (the token with a leading "$") anywhere in it with all the comma-separated values in the list "name". The script in the block will be executed once per value. Tokens can be anything, but should have no spaces or special characters. Nesting is allowed if the tokens are different.

>Note: Any values within the list which are "" or "null" will be ignored.

@endforeachlist

>Marks the end of the "@foreachlist" loop.


## While Loop

@while conditions... @do
...statements...
@endwhile

>Executes the code in the "while" block multiple times as long as the condition is true. The condition is checked before each iteration. The "@do" is required.

>The same condition statements as used in "@if" blocks are used here, with @and, @or, and @not.

>Be careful to ensure that the loop will eventually exit, or an infinite loop will occur.


## List Statements/Functions

These commands allow named lists of values to be stored as a single group instead of separate key/value pairs. They can be indexed by number, appended to, and cleared. They are stored as a single string with values separated by commas.

@addlist(name,value)

>Adds value to the end of the list "name".

@clearlist(name)

>Clears the list "name".

@getlist(name,index)

>Gets the value at position "index" (starting at 0) for the list "name". If "index" is beyond the end of the list, "" is returned.

@insertatlist(name,index,value)

>Inserts "value" at position "index" (starting at 0) from the list, shifting all later items. If "index" is past the end of the list, all items from the end up to "index" are filled with "" and then "value" is added.

@listlength(name)

>Returns the number of items in list "name".

@removeatlist(name,index)

>Removes the item at position "index" (starting at 0) from the list, shifting all later items. If "index" is past the end of the list, nothing happens.

@setlist(name,index,value)

>Sets the value at position "index" (starting at 0) for the list "name". If "index" is beyond the end of the list, all items from the end up to "index" are filled with "" and then "value" is added. Commas are not permitted within values.


## Array Statements/Functions

These commands allow a two-dimensional array of values to be stored as a group. They are sparse arrays with unspecified values returned as `null`. Arrays use keys containing the array name, a colon, and the row number, as "name:0". The row values contain zero or more items separated by commas. The rows can be different lengths and rows can be sparsely filled.

Note that the array values are referenced by row (y) first and then column (x), both starting at 0. Negative indexes throw an error.

@cleararray(name)

>Clears all values from the array "name".

@getarray(name,y,x)

>Gets the value at position "y,x" (starting at 0,0) for the array "name". If either "y" or "x" is beyond the edge of the stored values, "" is returned.

@setarray(name,y,x,value)

>Sets the value at position "y,x" (starting at 0,0) for the array "name". If either "y" or "x" is beyond the edge of the stored values, missing values will be set to "" as needed before adding "value". Commas are not permitted within values.


## InChannel/OutChannel Commands

>InChannel and OutChannel commands are a way for the DAGS scripts to communicate with the outside calling program and receive information back. The results returned from DAGS include any OutChannel commands, which the calling program would need to process. The calling program may also add messages of type InChannel, which the DAGS script can retrieve.

>An OutChannel command can either be one of the special values recognized by the calling program (GRIF), or it can be a DAGS script to be executed by the calling program by sending it back into DAGS. An example of this might be asking the user a question and including a script to handle the answer. (See the Cloak of Darkness example in the GRIF documentation.)

>The list of special values recognized by GRIF are:

>"#ASK;"
>"#ENTER;"
>"#EXISTS;"
>"#EXISTSNAME;"
>"#GAMEOVER;"
>"#RESTART;"
>"#RESTORE;"
>"#RESTORENAME;"
>"#SAVE;"
>"#SAVENAME;"

> Some special commands would be followed by a script to handle the result. The script is called after doing the special command, and that script would use @getinchannel to retrieve the answer. Here is a partial example:

```
command.restart
    @msg(message.askrestart)
    @setoutchannel("#ASK;")
    @setoutchannel("@script(script.restart)")
script.restart
	@set(__yorn,@getinchannel)
	@if @true(@get(__yorn)) @then
		@setoutchannel("#RESTART;")
		@setoutchannel("@msg(message.restarting) @nl")
		@setoutchannel("@script(system.intro)")
	@elseif @false(@get(__yorn)) @then
		@msg(message.ok)
	@else
		@msg(message.yorn_error)
		@setoutchannel("#ASK;")
		@setoutchannel("@script(script.restart))")
	@endif
```

@setoutchannel(value)

>Adds the value to the OutChannel queue. The calling program would be looking for these values and know how to process them. Values should be quoted, and if "value" is a DAGS command it must be quoted. The calling program should also gracefully ignore any unknown OutChannel values as they may be for an alternate calling program, or may be added in future versions.

@getinchannel

>Gets the value from the InChannel and returns it. The script would use or process that value as appropriate. Currently only one InChannel value can be sent at a time.


## Adding new functions to DAGS

Functions in DAGS are either built-in, such as those above, or are entries in the database having keys starting with "@". You can add as many new functions to DAGS as you need. They are very useful when functionality is needed in several places, instead of repeating the same code each time.

Function names must be unique. They can't duplicate any of the built-in function names.

A function with no parameters would have a "key" of the desired function name: "@myfunc". The "value" would contain a script to be run whenever that function was called. The "value" can also be a simple text value to be returned whenever the function is called.

A function with parameters would have a "key" with the specified name and would list the parameters in parentheses separated by commas: "@myfunc(x,y,z)". The "value" would be a script with replaceable parameter values such as "$x", "$y", "$z" somewhere in it. Any number of parameters may be specified, but at least one if parenthesis are used. The parameter names can be anything desired but should have no spaces or special characters, or conflict with other parameters from @for(), @foreachkey(), or @foreachlist() statements within the script.

Functions return values by writing them. All output from the function is the returned value.

If you are adding functions which will be conditions in an `@if` statement, be sure that they return the words `true` or `false` (or other truthy/falsey values).

Examples:

```
@quitmsg
    Are you sure you want to quit?

@score
    @write("You have a score of ",@get(value.score)," out of ",@get(value.maxscore)," points.")
    @nl
    @nl

@moveto(x,y)
    @comment("moves the item to a location")
    @set(item.$x.location,$y)

@unknown(x)
    @write("I don't understand ",$x,".\n")

@isnegative(x)
    @if @lt($x,0) @then
        @write(true)
    @else
        @write(false)
    @endif
```


## GrifLib

GrifLib is the code library for GRIF. It contains the DAGS and GROD classes, along with other support classes such as IO.

It contains a class call IFGame, which is a higher-level interactive fiction engine built on top of DAGS and GROD. This class uses DAGS scripts to define the game logic, and GROD to store the game state. It handles loading and saving games, processing user commands, and managing the game flow. It has another class IFParser for parsing user input into commands.

IFGame has event handling for text input and output, allowing the calling program to customize how text is displayed and how user input is received.

Setting up a basic IFGame is as simple as the following code, although Input and Output event handlers will need to be defined and the file name and game name must be specified:

```csharp
var game = new IFGame();
var grod = IO.OpenFile("<FileName>");
game.Initialize(grod, "<GameName>");
game.InputEvent += Input;
game.OutputEvent += Output;
await game.Intro();
await game.GameLoop();
```

See the GRIF file `Program.cs` for a complete example of using IFGame to run an interactive fiction game. (This is GRIF itself!)

GrifLib is designed to be reusable in other interactive fiction projects, allowing developers to focus on creating the game content rather than the underlying engine. It can be plugged into any C# application needing scripting and base/overlay game state management. It can include resource files for localization, graphics, sound, and other features as needed.
