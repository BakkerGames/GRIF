# DAGS Syntax

DAGS scripts consist of functions starting with "@" with zero or more parameters defined for each. Parameter values can be strings or other functions returning strings. Strings are anything not starting with "@", and don't need quotes around them if they have no spaces or special symbols. Some functions, such as arithmetic functions, expect the string parameters to be numbers or functions returning numbers.

DAGS uses a dictionary of (key, value) pairs. Both the keys and values are strings.

Keys cannot be null, "", or contain only whitespace, and should not have leading or trailing spaces. Values which are null or "null" or undefined are returned as "". Integer values which would be "" are returned as "0". Text values containing spaces and some special characters need to be surrounded by double quotes. Quotes within quoted strings must be escaped `\"`.

Keys and values as function parameters can be text values or strings built out of other functions.

Some functions take keys and operate directly on the dictionary, while other take values and operate on those values. Be careful of this! `@true(key)` doesn't check the value but is always false (the string "key" isn't truthy), while `@true(@get(key))` gets the correct answer from the value.

At times it might be necessary to add quotes around script values so they don't execute immediately. `@set(key,value)` is one such situation, when the value is to be stored as a script and not the answer. The value will need to be surrounded by quotes and internal quotes escaped.

Scripts are processed by Dags.RunScript(script, result), with `result` a StringBuilder parameter that will return all output.

There is an "InChannel" queue and an "OutChannel" queue which are used for passing string values between DAGS and the calling program. Strings placed on the queues would be handled by the other end as appropriate.


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

>Sets the value for "key" in the dictionary to "value". If "value" is a script, it stores the final result. If "value" is a script but is quoted, the script is stored directly.

@swap(key1,key2)

>Swaps the values stored in "key1" and "key2".


## Numeric Statements

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

>Returns the value for "key" from the dictionary.

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

>Executes the code in the "foreachkey" block multiple times, by replacing "$token" (the token with a leading "$") anywhere in it. It loops through all the keys in the dictionary which start with "prefix" and optionally end with "suffix". The value replaced for "$token" is the remaining part of the key after the prefix and the optional suffix are removed. Tokens can be anything, but should have no spaces or special characters. Nesting is allow if the tokens are different. Note that the order of keys returned is not deterministic.

@endforeachkey

>Marks the end of the "@foreachkey" loop.


## ForEachList Loop

@foreachlist(token,name)

>Executes the code in the "foreachlist" block multiple times, by replacing "$token" (the token with a leading "$") anywhere in it with all the comma-separated values in the list "name". The script in the block will be executed once per value. Tokens can be anything, but should have no spaces or special characters. Nesting is allowed if the tokens are different.

>Note: Any values within the list which are "" or "null" will be ignored.

@endforeachlist

>Marks the end of the "@foreachlist" loop.


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

These commands allow a two-dimensional array of values to be stored as a group. They are sparse arrays with unspecified values returned as "". Arrays use keys containing the array name, a colon, and the row number, as "name:0". The row values contain zero or more items separated by commas. The rows can be different lengths and rows can be missing.

Note that the array values are referenced by row (y) first and then column (x), both starting at 0. Negative indexes throw an error.

@cleararray(name)

>Clears all values from the array "name".

@getarray(name,y,x)

>Gets the value at position "y,x" (starting at 0,0) for the array "name". If either "y" or "x" is beyond the edge of the stored values, "" is returned.

@setarray(name,y,x,value)

>Sets the value at position "y,x" (starting at 0,0) for the array "name". If either "y" or "x" is beyond the edge of the stored values, missing values will be set to "" as needed before adding "value". Commas are not permitted within values.


## InChannel/OutChannel Commands

>InChannel and OutChannel commands are a way for the DAGS scripts to communicate with the outside calling program and receive information back. They use queues to stack commands for processing in order. The calling program looks at the In channel, handles all commands, and possibly passes back answers. This allows the calling program to handle file I/O and operating system commands so DAGS doesn't have to.

>Any DAGS command can be set in the Out channel, and the calling program would run it by calling DAGS. Typically this is used for running scripts.

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
    @set(temp.yorn,@getinchannel)
    @if @true(@get(temp.yorn)) @then
        @setoutchannel("#RESTART;")
    @elseif @false(@get(temp.yorn)) @then
        @msg(message.ok)
    @else
        @msg(message.yorn_error)
        @setoutchannel("#ASK;")
        @setoutchannel("@script(script.restart))")
    @endif
```

@setoutchannel(value)

>Adds the value to the OutChannel queue. The calling program would be looking for these values and know how to process them. Values should be quoted, and if "value" is a DAGS command it must be quoted.

@getinchannel

>Gets the next value from the InChannel queue and returns it. The script would use or process that value as appropriate.


## Public Interface

new Dags(IDictionary<string, string?> dictionary)

>Create a new DAGS object connected to an existing dictionary.

RunScript(string script, StringBuilder result)

>Runs the specified script and return any answers in result.

Queue InChannel

>Queue for receiving information from the calling program.

Queue OutChannel

>Queue for sending information back to the calling program.

bool ValidateDictionary(StringBuilder result)

>Checks that the dictionary is valid for use with DAGS. Returns all errors found. Includes checking all scripts with ValidateScript().

bool ValidateScript(string script, StringBuilder result)

>Checks that the script has correct syntax. Returns all errors found.

bool ValidateSyntax(string script, StringBuilder result)

>Checks that the script has correct syntax, but doesn't check function names. Returns all errors found.

string PrettyScript(string script)

>Returns the script with line splitting and indenting for more readable code. Useful for exporting or for an editor program.

string Help()

>Returns a syntax listing of all DAGS commands.

string Syntax()

>Returns the full DAGS_SYNTAX.md file for DAGS (this file).


## Adding new functions to DAGS

Functions in DAGS are either built-in, such as those above, or are entries in the dictionary having keys starting with "@". You can add as many new functions to DAGS as you need. They are very useful when functionality is needed in several places, instead of repeating the same code each time.

Function names must be unique. They can't duplicate any of the built-in function names.

A function with no parameters would have a "key" of the desired function name: "@myfunc". The "value" would contain a script to be run whenever that function was called. The "value" can also be a simple text value to be returned whenever the function is called.

A function with parameters would have a "key" with the specified name and would list the parameters in parentheses separated by commas: "@myfunc(x,y,z)". The "value" would be a script with replaceable parameter values such as "$x", "$y", "$z" somewhere in it. Any number of parameters may be specified, but at least one. The parameter names can be anything desired but should have no spaces or special characters, or conflict with other parameters from @for(), @foreachkey(), or @foreachlist() statements within the script.

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