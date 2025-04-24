using System.Text;
using static GRIFTools.DAGSConstants;

namespace GRIFTools;

public partial class Dags
{
    private void RunOneCommand(string[] tokens, ref int index, StringBuilder result)
    {
        try
        {
            int int1;
            int int2;
            string temp1;
            string temp2;
            bool answer;
            long numericAnswer;
            List<string> list = [];
            List<List<string>> array = [];
            var token = tokens[index++];

            // static value
            if (!token.StartsWith('@'))
            {
                result.Append(token);
                return;
            }

            // tokens without parameters
            if (!token.EndsWith('('))
            {
                switch (token)
                {
                    case IF:
                        // begins an @if block
                        HandleIf(tokens, ref index, result);
                        return;
                    case GETINCHANNEL:
                        // get the next value on the InChannel queue
                        if (InChannel.Count > 0)
                        {
                            var inValue = InChannel.Dequeue();
                            if (inValue.StartsWith('@'))
                            {
                                throw new SystemException($"Invalid value on InChannel: {inValue}");
                            }
                            result.Append(inValue);
                        }
                        return;
                    case NL:
                        result.Append(NL_VALUE);
                        return;
                    case RETURN:
                        // immediately jump to end of script
                        index = tokens.Length;
                        return;
                    default:
                        // run a defined function with no parameters
                        // @func=...
                        var value = Get(token);
                        if (value == "")
                        {
                            throw new SystemException($"Function not found: {token}");
                        }
                        RunScript(value, result);
                        return;
                }
                throw new SystemException($"Unknown script token: {token}");
            }

            // get parameters
            var p = GetParameters(tokens, ref index);

            // parse tokens
            switch (token)
            {
                case ABS:
                    // absolute value
                    CheckParamCount(token, p, 1);
                    int1 = ConvertToInt(p[0]);
                    result.Append(Math.Abs(int1));
                    return;
                case ADD:
                    // add two values
                    CheckParamCount(token, p, 2);
                    int1 = ConvertToInt(p[0]);
                    int2 = ConvertToInt(p[1]);
                    numericAnswer = (long)int1 + (long)int2;
                    if (numericAnswer > int.MaxValue || numericAnswer + 1 < -int.MaxValue)
                    {
                        throw new SystemException($"{ADD}{int1},{int2}): Numeric overflow");
                    }
                    result.Append(numericAnswer);
                    return;
                case ADDLIST:
                    // adds a value to the end of a list
                    CheckParamCount(token, p, 2);
                    if (p[0] == "")
                    {
                        throw new SystemException("List name cannot be blank");
                    }
                    AddListItem(p[0], p[1]);
                    return;
                case ADDTO:
                    // add a value to an existing value
                    CheckParamCount(token, p, 2);
                    int1 = GetInt(p[0]);
                    int2 = ConvertToInt(p[1]);
                    numericAnswer = (long)int1 + (long)int2;
                    if (numericAnswer > int.MaxValue || numericAnswer + 1 < -int.MaxValue)
                    {
                        throw new SystemException($"{ADDTO}[{p[0]}]{int1},{int2}): Numeric overflow");
                    }
                    Set(p[0], numericAnswer.ToString());
                    return;
                case CLEARARRAY:
                    // clears the named array
                    CheckParamCount(token, p, 1);
                    if (p[0] == "")
                    {
                        throw new SystemException("Array name cannot be blank");
                    }
                    ClearArray(p[0]);
                    return;
                case CLEARLIST:
                    // clears the named list
                    CheckParamCount(token, p, 1);
                    if (p[0] == "")
                    {
                        throw new SystemException("List name cannot be blank");
                    }
                    Set(p[0], "");
                    return;
                case COMMENT:
                    // comment for script documentation, do nothing
                    CheckParamCount(token, p, 1);
                    return;
                case CONCAT:
                    // concatenate any number of strings together
                    foreach (string s in p)
                    {
                        result.Append(s);
                    }
                    return;
                case DEBUG:
                    // output the value if system.debug=true
                    CheckParamCount(token, p, 1);
                    if (ConvertToBool(Get(DEBUG_MODE)))
                    {
                        // display values in debug mode
                        result.Append(p[0]);
                        result.Append(NL_VALUE);
                    }
                    return;
                case DIV:
                    // divide two values
                    CheckParamCount(token, p, 2);
                    int1 = ConvertToInt(p[0]);
                    int2 = ConvertToInt(p[1]);
                    if (int2 == 0)
                    {
                        throw new SystemException($"{DIV}{int1},{int2}): Division by zero!");
                    }
                    result.Append(int1 / int2);
                    return;
                case DIVTO:
                    // divide an existing value by a value
                    CheckParamCount(token, p, 2);
                    int1 = GetInt(p[0]);
                    int2 = ConvertToInt(p[1]);
                    if (int2 == 0)
                    {
                        throw new SystemException($"{DIVTO}{p[0]}={int1},{int2}): Division by zero!");
                    }
                    Set(p[0], (int1 / int2).ToString());
                    return;
                case EQ:
                    // are values equal (ignoring case)
                    CheckParamCount(token, p, 2);
                    if (int.TryParse(p[0], out int1) && int.TryParse(p[1], out int2))
                    {
                        answer = int1 == int2;
                    }
                    else
                    {
                        temp1 = p[0].Equals(NULL_VALUE, OIC) ? "" : p[0];
                        temp2 = p[1].Equals(NULL_VALUE, OIC) ? "" : p[1];
                        answer = string.Compare(temp1.ToLowerInvariant(), temp2.ToLowerInvariant(), OIC) == 0;
                    }
                    result.Append(ConvertToBoolString(answer));
                    return;
                case EXEC:
                    // note: use space before "@" to delay execution while building
                    StringBuilder value = new();
                    foreach (string s in p)
                    {
                        temp1 = s;
                        while (temp1.StartsWith('@'))
                        {
                            StringBuilder tempResult = new();
                            RunScript(temp1, tempResult);
                            temp1 = tempResult.ToString();
                        }
                        value.Append(temp1);
                    }
                    result.Append(value);
                    return;
                case EXISTS:
                    // does the key exist with a non-null value?
                    CheckParamCount(token, p, 1);
                    temp1 = Get(p[0]);
                    answer = temp1 != "" && !temp1.Equals(NULL_VALUE, OIC);
                    result.Append(ConvertToBoolString(answer));
                    return;
                case FALSE:
                    // is the value false (or falsey). false if error.
                    CheckParamCount(token, p, 1);
                    try
                    {
                        answer = !ConvertToBool(p[0]);
                    }
                    catch (Exception)
                    {
                        answer = false;
                    }
                    result.Append(ConvertToBoolString(answer));
                    return;
                case FOR:
                    // begins a @for block
                    CheckParamCount(token, p, 3);
                    HandleFor(p, tokens, ref index, result);
                    return;
                case FOREACHKEY:
                    // begins a @foreachkey block
                    CheckParamCountAtLeast(token, p, 2);
                    HandleForEachKey(p, tokens, ref index, result);
                    return;
                case FOREACHLIST:
                    // begins a @foreachlist block
                    CheckParamCount(token, p, 2);
                    HandleForEachList(p, tokens, ref index, result);
                    return;
                case FORMAT:
                    // in p[0], replace "{0}"..."{n}" with p[1]...p[n+1]
                    temp1 = p[0];
                    for (int i = 1; i < p.Count; i++)
                    {
                        temp1 = temp1.Replace($"{{{i - 1}}}", p[i]); // {0} = p[1]
                    }
                    result.Append(temp1);
                    return;
                case GE:
                    // is first value greater or equal to second
                    CheckParamCount(token, p, 2);
                    if (int.TryParse(p[0], out int1) && int.TryParse(p[1], out int2))
                    {
                        answer = int1 >= int2;
                    }
                    else
                    {
                        temp1 = p[0].Equals(NULL_VALUE, OIC) ? "" : p[0];
                        temp2 = p[1].Equals(NULL_VALUE, OIC) ? "" : p[1];
                        answer = string.Compare(temp1.ToLowerInvariant(), temp2.ToLowerInvariant(), OIC) >= 0;
                    }
                    result.Append(ConvertToBoolString(answer));
                    return;
                case GET:
                    // get a raw value
                    CheckParamCount(token, p, 1);
                    temp1 = Get(p[0]);
                    result.Append(temp1);
                    return;
                case GETARRAY:
                    // get a name,y,x value
                    CheckParamCount(token, p, 3);
                    if (p[0] == "")
                    {
                        throw new SystemException("Array name cannot be blank");
                    }
                    if (!int.TryParse(p[1], out int1) || !int.TryParse(p[2], out int2) || int1 < 0 || int2 < 0)
                    {
                        throw new SystemException($"Invalid (y,x) for array: ({p[1]},{p[2]})");
                    }
                    result.Append(GetArrayItem(p[0], int1, int2));
                    return;
                case GETLIST:
                    // get a name,x value
                    CheckParamCount(token, p, 2);
                    if (p[0] == "")
                    {
                        throw new SystemException("List name cannot be blank");
                    }
                    if (!int.TryParse(p[1], out int1) || int1 < 0)
                    {
                        throw new SystemException($"Invalid (x) for list: {p[1]}");
                    }
                    result.Append(GetListItem(p[0], p[1]));
                    return;
                case GETVALUE:
                    // get a value with script processing
                    CheckParamCount(token, p, 1);
                    temp1 = Get(p[0]);
                    while (temp1.StartsWith('@'))
                    {
                        StringBuilder tempResult = new();
                        RunScript(temp1, tempResult);
                        temp1 = tempResult.ToString();
                    }
                    result.Append(temp1);
                    return;
                case GOLABEL:
                    // move to the statment after @label(value)
                    CheckParamCount(token, p, 1);
                    for (int i = 0; i < tokens.Length - 1; i++)
                    {
                        if (tokens[i] == LABEL && tokens[i + 1] == p[0] && tokens[i + 2] == ")")
                        {
                            index = i + 3;
                        }
                    }
                    return;
                case GT:
                    // is first value greater than second
                    CheckParamCount(token, p, 2);
                    if (int.TryParse(p[0], out int1) && int.TryParse(p[1], out int2))
                    {
                        answer = int1 > int2;
                    }
                    else
                    {
                        temp1 = p[0].Equals(NULL_VALUE, OIC) ? "" : p[0];
                        temp2 = p[1].Equals(NULL_VALUE, OIC) ? "" : p[1];
                        answer = string.Compare(temp1.ToLowerInvariant(), temp2.ToLowerInvariant(), OIC) > 0;
                    }
                    result.Append(ConvertToBoolString(answer));
                    return;
                case INSERTATLIST:
                    // inserts a value into list name at x
                    CheckParamCount(token, p, 3);
                    if (p[0] == "")
                    {
                        throw new SystemException("List name cannot be blank");
                    }
                    if (!int.TryParse(p[1], out int1) || int1 < 0)
                    {
                        throw new SystemException($"Invalid (x) for list: {p[1]}");
                    }
                    InsertAtListItem(p[0], p[1], p[2]);
                    return;
                case ISBOOL:
                    // is value true or false?
                    CheckParamCount(token, p, 1);
                    try
                    {
                        _ = ConvertToBool(p[0]);
                        answer = true;
                    }
                    catch (Exception)
                    {
                        answer = false;
                    }
                    result.Append(ConvertToBoolString(answer));
                    return;
                case ISNUMBER:
                    // is the value a number
                    CheckParamCount(token, p, 1);
                    answer = p[0] != "" && int.TryParse(p[0], out _);
                    result.Append(ConvertToBoolString(answer));
                    return;
                case ISSCRIPT:
                    // is the raw value a script (starts with '@')
                    CheckParamCount(token, p, 1);
                    temp1 = Get(p[0]);
                    answer = temp1.StartsWith('@');
                    result.Append(ConvertToBoolString(answer));
                    return;
                case LABEL:
                    // label for golabel
                    CheckParamCount(token, p, 1);
                    return;
                case LE:
                    // is first value less or equal to second
                    CheckParamCount(token, p, 2);
                    if (int.TryParse(p[0], out int1) && int.TryParse(p[1], out int2))
                    {
                        answer = int1 <= int2;
                    }
                    else
                    {
                        temp1 = p[0].Equals(NULL_VALUE, OIC) ? "" : p[0];
                        temp2 = p[1].Equals(NULL_VALUE, OIC) ? "" : p[1];
                        answer = string.Compare(temp1.ToLowerInvariant(), temp2.ToLowerInvariant(), OIC) <= 0;
                    }
                    result.Append(ConvertToBoolString(answer));
                    return;
                case LISTLENGTH:
                    // get the length of the named list
                    CheckParamCount(token, p, 1);
                    if (p[0] == "")
                    {
                        throw new SystemException("List name cannot be blank");
                    }
                    result.Append(GetListLength(p[0]));
                    return;
                case LOWER:
                    // lowercase value
                    CheckParamCount(token, p, 1);
                    result.Append(p[0].ToLowerInvariant());
                    return;
                case LT:
                    // is first value less than second
                    CheckParamCount(token, p, 2);
                    if (int.TryParse(p[0], out int1) && int.TryParse(p[1], out int2))
                    {
                        answer = int1 < int2;
                    }
                    else
                    {
                        temp1 = p[0].Equals(NULL_VALUE, OIC) ? "" : p[0];
                        temp2 = p[1].Equals(NULL_VALUE, OIC) ? "" : p[1];
                        answer = string.Compare(temp1.ToLowerInvariant(), temp2.ToLowerInvariant(), OIC) < 0;
                    }
                    result.Append(ConvertToBoolString(answer));
                    return;
                case MOD:
                    // get modulus (remainder)
                    CheckParamCount(token, p, 2);
                    int1 = ConvertToInt(p[0]);
                    int2 = ConvertToInt(p[1]);
                    if (int2 == 0)
                    {
                        throw new SystemException($"{MOD}{int1},{int2}): Mod by zero!");
                    }
                    result.Append(int1 % int2);
                    return;
                case MODTO:
                    // modulus an existing value by a value
                    CheckParamCount(token, p, 2);
                    int1 = GetInt(p[0]);
                    int2 = ConvertToInt(p[1]);
                    if (int2 == 0)
                    {
                        throw new SystemException($"{MODTO}{p[0]}={int1},{int2}): Mod by zero!");
                    }
                    Set(p[0], (int1 % int2).ToString());
                    return;
                case MSG:
                    // print a message
                    CheckParamCount(token, p, 1);
                    temp1 = Get(p[0]);
                    if (temp1 == "") return;
                    while (temp1.StartsWith('@'))
                    {
                        StringBuilder tempResult = new();
                        RunScript(temp1, tempResult);
                        temp1 = tempResult.ToString();
                    }
                    result.Append(temp1);
                    result.Append(NL_VALUE);
                    return;
                case MUL:
                    // multiply two values
                    CheckParamCount(token, p, 2);
                    int1 = ConvertToInt(p[0]);
                    int2 = ConvertToInt(p[1]);
                    numericAnswer = (long)int1 * (long)int2;
                    if (numericAnswer > int.MaxValue || numericAnswer + 1 < -int.MaxValue)
                    {
                        throw new SystemException($"{MUL}{int1},{int2}): Numeric overflow");
                    }
                    result.Append(numericAnswer);
                    return;
                case MULTO:
                    // multiply an existing value by a value
                    CheckParamCount(token, p, 2);
                    int1 = GetInt(p[0]);
                    int2 = ConvertToInt(p[1]);
                    numericAnswer = (long)int1 * (long)int2;
                    if (numericAnswer > int.MaxValue || numericAnswer + 1 < -int.MaxValue)
                    {
                        throw new SystemException($"{MULTO}[{p[0]}]{int1},{int2}): Numeric overflow");
                    }
                    Set(p[0], numericAnswer.ToString());
                    return;
                case NE:
                    // are values not equal (ignoring case)
                    CheckParamCount(token, p, 2);
                    if (int.TryParse(p[0], out int1) && int.TryParse(p[1], out int2))
                    {
                        answer = int1 != int2;
                    }
                    else
                    {
                        temp1 = p[0].Equals(NULL_VALUE, OIC) ? "" : p[0];
                        temp2 = p[1].Equals(NULL_VALUE, OIC) ? "" : p[1];
                        answer = string.Compare(temp1.ToLowerInvariant(), temp2.ToLowerInvariant(), OIC) != 0;
                    }
                    result.Append(ConvertToBoolString(answer));
                    return;
                case NEG:
                    // negate a value
                    CheckParamCount(token, p, 1);
                    int1 = ConvertToInt(p[0]);
                    numericAnswer = -(long)int1;
                    if (numericAnswer > int.MaxValue || numericAnswer + 1 < -int.MaxValue)
                    {
                        throw new SystemException($"{NEG}{int1}): Numeric overflow");
                    }
                    result.Append(numericAnswer);
                    return;
                case NEGTO:
                    // negate an existing value
                    CheckParamCount(token, p, 1);
                    int1 = GetInt(p[0]);
                    numericAnswer = -(long)int1;
                    if (numericAnswer > int.MaxValue || numericAnswer + 1 < -int.MaxValue)
                    {
                        throw new SystemException($"{NEGTO}[{p[0]}]{int1}): Numeric overflow");
                    }
                    Set(p[0], numericAnswer.ToString());
                    return;
                case NULL:
                    // is the value null or empty or NULL_VALUE
                    CheckParamCount(token, p, 1);
                    answer = p[0] == "" || p[0].Equals(NULL_VALUE, OIC);
                    result.Append(ConvertToBoolString(answer));
                    return;
                case RAND:
                    // is random 0-99 less than percent value (1-100)
                    CheckParamCount(token, p, 1);
                    int1 = ConvertToInt(p[0]);
                    int2 = _random.Next(100);
                    answer = int2 < int1;
                    result.Append(ConvertToBoolString(answer));
                    return;
                case REMOVEATLIST:
                    // remove value at x in the named list
                    CheckParamCount(token, p, 2);
                    if (p[0] == "")
                    {
                        throw new SystemException("List name cannot be blank");
                    }
                    if (!int.TryParse(p[1], out int1) || int1 < 0)
                    {
                        throw new SystemException($"Invalid (x) for list: {p[1]}");
                    }
                    RemoveAtListItem(p[0], p[1]);
                    return;
                case REPLACE:
                    // in value0, replace value1 with value2
                    CheckParamCount(token, p, 3);
                    result.Append(p[0].Replace(p[1], p[2], OIC));
                    return;
                case RND:
                    // return random number less than value
                    CheckParamCount(token, p, 1);
                    int1 = ConvertToInt(p[0]);
                    result.Append(_random.Next(int1));
                    return;
                case SCRIPT:
                    // run a script
                    CheckParamCount(token, p, 1);
                    RunScript(Get(p[0]), result);
                    return;
                case SET:
                    // set a value
                    CheckParamCount(token, p, 2);
                    if (p[1].StartsWith(' ') && p[1].TrimStart().StartsWith('@'))
                    {
                        // remove leading spaces in script
                        Set(p[0], p[1].TrimStart());
                    }
                    else
                    {
                        Set(p[0], p[1]);
                    }
                    return;
                case SETARRAY:
                    // set a name,y,x,value
                    CheckParamCount(token, p, 4);
                    if (p[0] == "")
                    {
                        throw new SystemException("Array name cannot be blank");
                    }
                    if (!int.TryParse(p[1], out int1) || !int.TryParse(p[2], out int2) || int1 < 0 || int2 < 0)
                    {
                        throw new SystemException($"Invalid (y,x) for array: ({p[1]},{p[2]})");
                    }
                    SetArrayItem(p[0], int1, int2, p[3]);
                    return;
                case SETLIST:
                    // set a name,x,value
                    CheckParamCount(token, p, 3);
                    if (p[0] == "")
                    {
                        throw new SystemException("List name cannot be blank");
                    }
                    if (!int.TryParse(p[1], out int1) || int1 < 0)
                    {
                        throw new SystemException($"Invalid (x) for list: {p[1]}");
                    }
                    SetListItem(p[0], p[1], p[2]);
                    return;
                case SETOUTCHANNEL:
                    // adds the value to the OutChannel queue
                    CheckParamCount(token, p, 1);
                    OutChannel.Enqueue(p[0]);
                    return;
                case SUB:
                    // subtract two values
                    CheckParamCount(token, p, 2);
                    int1 = ConvertToInt(p[0]);
                    int2 = ConvertToInt(p[1]);
                    numericAnswer = (long)int1 - (long)int2;
                    if (numericAnswer > int.MaxValue || numericAnswer + 1 < -int.MaxValue)
                    {
                        throw new SystemException($"{SUB}{int1},{int2}): Numeric overflow");
                    }
                    result.Append(numericAnswer);
                    return;
                case SUBSTRING:
                    // get substring of characters, (value,start[,len])
                    CheckParamCountAtLeast(token, p, 2);
                    temp1 = p[0];
                    int1 = ConvertToInt(p[1]);
                    if (int1 >= 0 && int1 < temp1.Length)
                    {
                        if (p.Count > 2)
                        {
                            int2 = ConvertToInt(p[2]);
                            temp2 = temp1[int1..];
                            if (temp2.Length < int2)
                            {
                                result.Append(temp2);
                            }
                            else
                            {
                                result.Append(temp2[..int2]);
                            }
                        }
                        else
                        {
                            result.Append(temp1[int1..]);
                        }
                    }
                    return;
                case SUBTO:
                    // subtract a value from existing value
                    CheckParamCount(token, p, 2);
                    int1 = GetInt(p[0]);
                    int2 = ConvertToInt(p[1]);
                    numericAnswer = (long)int1 - (long)int2;
                    if (numericAnswer > int.MaxValue || numericAnswer + 1 < -int.MaxValue)
                    {
                        throw new SystemException($"{SUBTO}[{p[0]}]{int1},{int2}): Numeric overflow");
                    }
                    Set(p[0], numericAnswer.ToString());
                    return;
                case SWAP:
                    // swap two values
                    CheckParamCount(token, p, 2);
                    temp1 = Get(p[0]);
                    Set(p[0], Get(p[1]));
                    Set(p[1], temp1);
                    return;
                case TRIM:
                    // trim leading and trailing spaces from string
                    CheckParamCount(token, p, 1);
                    result.Append(p[0].Replace(NL_VALUE, "").Trim());
                    return;
                case TRUE:
                    // is value true (or truthy). false if error.
                    CheckParamCount(token, p, 1);
                    try
                    {
                        answer = ConvertToBool(p[0]);
                    }
                    catch (Exception)
                    {
                        answer = false;
                    }
                    result.Append(ConvertToBoolString(answer));
                    return;
                case UPPER:
                    // uppercase string
                    CheckParamCount(token, p, 1);
                    result.Append(p[0].ToUpperInvariant());
                    return;
                case WRITE:
                    // write out each value (no NL at end)
                    foreach (string s in p)
                    {
                        temp1 = s;
                        while (temp1.StartsWith('@'))
                        {
                            StringBuilder tempResult = new();
                            RunScript(temp1, tempResult);
                            temp1 = tempResult.ToString();
                        }
                        result.Append(temp1);
                    }
                    return;
                case WRITELINE:
                    // write out each value with a NL at the end
                    foreach (string s in p)
                    {
                        temp1 = s;
                        while (temp1.StartsWith('@'))
                        {
                            StringBuilder tempResult = new();
                            RunScript(temp1, tempResult);
                            temp1 = tempResult.ToString();
                        }
                        result.Append(temp1);
                    }
                    result.Append(NL_VALUE);
                    return;
                default:
                    // run a defined function with any number of replaceable parameters
                    // @func(x)=...$x...
                    // @func(x,y)=...$x...$y...
                    var dict = GetByPrefix(token);
                    if (dict.Count == 0)
                    {
                        throw new SystemException($"Function not found: {token}");
                    }
                    if (dict.Count > 1)
                    {
                        throw new SystemException($"Duplicate functions found: {token}");
                    }
                    var tempKey = dict.Keys.First();
                    var tempValue = dict[tempKey];
                    var tempParams = tempKey[token.Length..^1].Split(',');
                    int pIndex = 0;
                    foreach (string s in tempParams)
                    {
                        tempValue = tempValue?.Replace($"${s}", p[pIndex++]) ?? "";
                    }
                    RunScript(tempValue ?? "", result);
                    return;
            }
            throw new SystemException($"Unknown script token: {token}");
        }
        catch (Exception)
        {
            throw;
        }
    }

    private List<string> GetParameters(string[] tokens, ref int index)
    {
        try
        {
            var resultList = new List<string>();
            while (tokens[index] != ")")
            {
                var value = GetOneValue(tokens, ref index);
                resultList.Add(value);
                if (tokens[index] == ",")
                {
                    index++;
                    if (tokens[index] == ")")
                    {
                        resultList.Add("");
                    }
                }
            }
            index++;
            return resultList;
        }
        catch (Exception)
        {
            throw;
        }
    }

    private string GetOneValue(string[] tokens, ref int index)
    {
        try
        {
            if (tokens[index].StartsWith('@'))
            {
                StringBuilder newResult = new();
                RunOneCommand(tokens, ref index, newResult);
                return newResult.ToString();
            }
            if (tokens[index].StartsWith('"') && tokens[index].EndsWith('"'))
            {
                return tokens[index++][1..^1].Replace("\\\"", "\"");
            }
            if (tokens[index] == ",")
            {
                return "";
            }
            return tokens[index++];
        }
        catch (Exception)
        {
            throw;
        }
    }

    private void HandleIf(string[] tokens, ref int index, StringBuilder result)
    {
        // @if <conditions> @then ... [@elseif <conditions> @then ...] [<repeat>] [@else ...] @endif
        if (CheckConditions(tokens, ref index))
        {
            while (!tokens[index].Equals(ELSE, OIC) &&
                   !tokens[index].Equals(ELSEIF, OIC) &&
                   !tokens[index].Equals(ENDIF, OIC))
            {
                RunOneCommand(tokens, ref index, result);
            }
            SkipPastEndif(tokens, ref index);
        }
        else
        {
            SkipToElse(tokens, ref index);
            if (tokens[index].Equals(ELSEIF, OIC))
            {
                index++;
                HandleIf(tokens, ref index, result);
                return;
            }
            if (tokens[index].Equals(ELSE, OIC))
            {
                index++;
                while (!tokens[index].Equals(ELSE, OIC) &&
                       !tokens[index].Equals(ELSEIF, OIC) &&
                       !tokens[index].Equals(ENDIF, OIC))
                {
                    RunOneCommand(tokens, ref index, result);
                }
            }
            SkipPastEndif(tokens, ref index);
        }
    }

    private bool CheckConditions(string[] tokens, ref int index)
    {
        try
        {
            var answer = CheckOneCondition(tokens, ref index);
            while (!tokens[index].Equals(THEN, OIC))
            {
                if (tokens[index].Equals(AND, OIC))
                {
                    index++;
                    if (!answer)
                    {
                        while (!tokens[index].Equals(THEN, OIC))
                            index++;
                        break;
                    }
                }
                else if (tokens[index].Equals(OR, OIC))
                {
                    index++;
                    if (answer)
                    {
                        while (!tokens[index].Equals(THEN, OIC))
                            index++;
                        break;
                    }
                }
                else
                {
                    throw new SystemException($"Expected @AND or @OR but found: {tokens[index]}");
                }
                answer = CheckOneCondition(tokens, ref index);
            }
            index++; // skip @then
            return answer;
        }
        catch (Exception)
        {
            throw;
        }
    }

    private bool CheckOneCondition(string[] tokens, ref int index)
    {
        var notFlag = false;
        if (tokens[index].Equals(NOT, OIC))
        {
            notFlag = true;
            index++;
        }
        bool answer;
        try
        {
            answer = ConvertToBool(GetOneValue(tokens, ref index));
        }
        catch (Exception)
        {
            answer = false;
        }
        if (notFlag) answer = !answer;
        return answer;
    }

    private static void SkipToElse(string[] tokens, ref int index)
    {
        try
        {
            int level = 0;
            while ((!tokens[index].Equals(ELSE, OIC) &&
                    !tokens[index].Equals(ELSEIF, OIC) &&
                    !tokens[index].Equals(ENDIF, OIC))
                    || level > 0)
            {
                if (tokens[index].Equals(IF, OIC))
                {
                    level++;
                    index++;
                }
                else if (tokens[index].Equals(ENDIF, OIC))
                {
                    if (level > 0)
                    {
                        level--;
                        index++;
                    }
                }
                else
                {
                    index++;
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static void SkipPastEndif(string[] tokens, ref int index)
    {
        try
        {
            int level = 0;
            while (!tokens[index].Equals(ENDIF, OIC) || level > 0)
            {
                if (tokens[index].Equals(IF, OIC))
                {
                    level++;
                    index++;
                }
                else if (tokens[index].Equals(ENDIF, OIC))
                {
                    if (level > 0)
                    {
                        level--;
                        index++;
                    }
                }
                else
                {
                    index++;
                }
            }
            index++;
        }
        catch (Exception)
        {
            throw;
        }
    }

    private void HandleFor(List<string> p, string[] tokens, ref int index, StringBuilder result)
    {
        // @for(i,<start>,<end inclusive>)=...$i...@endfor
        var newTokens = new StringBuilder();
        var level = 0;
        do
        {
            var token = tokens[index++];
            if (token == FOR)
            {
                level++;
            }
            else if (token == ENDFOR)
            {
                if (level <= 0)
                {
                    break;
                }
                level--;
            }
            if (newTokens.Length > 0)
            {
                newTokens.Append(' ');
            }
            newTokens.Append(token);
        } while (index < tokens.Length);

        for (int value = int.Parse(p[1]); value <= int.Parse(p[2]); value++)
        {
            var script = newTokens.ToString().Replace($"${p[0]}", value.ToString());
            RunScript(script, result);
        }
    }

    private void HandleForEachKey(List<string> p, string[] tokens, ref int index, StringBuilder result)
    {
        // @foreach(i,prefix,[suffix])=...$i...@endforeach
        var newTokens = new StringBuilder();
        var level = 0;
        do
        {
            var token = tokens[index++];
            if (token == FOREACHKEY)
            {
                level++;
            }
            else if (token == ENDFOREACHKEY)
            {
                if (level <= 0)
                {
                    break;
                }
                level--;
            }
            if (newTokens.Length > 0)
            {
                newTokens.Append(' ');
            }
            newTokens.Append(token);
        } while (index < tokens.Length);

        var keys = Data.Keys().Where(x => x.StartsWith(p[1]));
        foreach (string key in keys)
        {
            var value = key[p[1].Length..];
            if (p.Count > 2)
            {
                if (!value.EndsWith(p[2]))
                    continue;
                value = value[..^p[2].Length];
            }
            var script = newTokens.ToString().Replace($"${p[0]}", value);
            RunScript(script, result);
        }
    }

    private void HandleForEachList(List<string> p, string[] tokens, ref int index, StringBuilder result)
    {
        // @foreachinlist(x,listname)=...$x...@endforeachinlist
        var newTokens = new StringBuilder();
        var level = 0;
        do
        {
            var token = tokens[index++];
            if (token == FOREACHLIST)
            {
                level++;
            }
            else if (token == ENDFOREACHLIST)
            {
                if (level <= 0)
                {
                    break;
                }
                level--;
            }
            if (newTokens.Length > 0)
            {
                newTokens.Append(' ');
            }
            newTokens.Append(token);
        } while (index < tokens.Length);
        // p[1] holds the name of the list
        string list = Get(p[1]);
        if (!string.IsNullOrWhiteSpace(list))
        {
            var items = list.Split(',');
            foreach (string value in items)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var script = newTokens.ToString().Replace($"${p[0]}", value);
                    RunScript(script, result);
                }
            }
        }
    }
}
