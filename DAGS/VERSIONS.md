# Versions

1.0.0 - Initial Version.

1.0.1 - Fixed PrettyScript().

1.0.2 - Added "@isbool(x)" so scripts can test if values are boolean. Changed "@true" and "@false" to return false if value isn't boolean, instead of throwing errors.

1.1.0 - Breaking change: Made Help() static. Also: Fixed quotes within quoted strings. Added "@isnumber()".

1.1.1 - Fixed RunScript() when called with a text string instead of a script.

1.2.0 - Renamed to DAGS. Added @falsedata(key), @truedata(key), @isbooldata(key), @isnulldata(key), @isnumberdata(key), @isscriptdata(key).

1.2.1 - Added error checking in CheckOneCondition() for non-boolean data.

1.2.2 - Changed RunScript() to return error messages in "result" instead of throwing an error. Fixed some error text.

1.2.3 - Added ReadMe(), License(), Syntax(), and VersionHistory() for retriving that information.

1.2.4 - New DebugLog() command, for tracking each step during script processing.

1.2.5 - Added ValidateSyntax() for checking syntax but not function names. Return more errors during Validate routines. PrettyScript() now calls ValidateSyntax() first.

1.2.6 - Removed thrown errors from PrettyScript() and SplitTokens().

1.2.7 - Null check on resource stream

1.2.8 - Check for leading spaces on script values in @set(key,value)