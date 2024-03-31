using System.Text;
using static DAGS.Constants;

namespace DAGS;

public partial class Dags
{
    private bool _debugLogFlag = false;
    private readonly StringBuilder _debugLogResult = new();

    private string[] _debugTokens = [];
    private int _debugIndex = -1;
    private bool _debugIf = false;
    private bool _debugIfCondition = false;
    private bool _debugIfAnswer = false;

    /// <summary>
    /// Returns the full log data while processing and running the script.
    /// </summary>
    public void DebugLog(string script, StringBuilder result)
    {
        if (string.IsNullOrWhiteSpace(script) || script.Equals(NULL_VALUE, OIC))
        {
            return;
        }
        if (_dict.ContainsKey(script))
        {
            script = Get(script);
        }
        _debugLogFlag = true;
        _debugLogResult.Clear();
        RunScript(script, result);
        result.AppendLine("---");
        result.Append(_debugLogResult);
        _debugLogFlag = false;
    }
    
    /// <summary>
    /// Starts debugging a script at the top level.
    /// </summary>
    public void DebugScript(string script, StringBuilder result)
    {
        if (string.IsNullOrWhiteSpace(script) || script.Equals(NULL_VALUE, OIC))
        {
            return;
        }
        if (_dict.ContainsKey(script))
        {
            script = Get(script);
        }
        _debugTokens = SplitTokens(script);
        _debugIndex = 0;
        _debugIf = false;
        _debugIfCondition = false;
        _debugIfAnswer = false;
        DebugNext(result);
    }

    /// <summary>
    /// Performs the next debug step for the current script.
    /// </summary>
    public void DebugNext(StringBuilder result)
    {
        if (DebugDone())
        {
            result.AppendLine("Debug done.");
            return;
        }
        StringBuilder tempResult = new();
        var saveIndex = _debugIndex;
        var saveIfCond = _debugIfCondition;
        if (_debugIfCondition)
        {
            if (_debugTokens[_debugIndex].Equals(AND, OIC))
            {
                result.AppendLine(AND);
                if (!_debugIfAnswer)
                {
                    result.AppendLine($"False, skipping to {ELSE}");
                    _debugIfCondition = false;
                    SkipToElse(_debugTokens, ref _debugIndex);
                    _debugIndex++;
                }
            }
            else if (_debugTokens[_debugIndex].Equals(OR, OIC))
            {
                result.AppendLine(OR);
                if (_debugIfAnswer)
                {
                    _debugIfCondition = false;
                    result.AppendLine($"True, skipping to {THEN}");
                    while (!_debugTokens[_debugIndex].Equals(THEN, OIC))
                    {
                        _debugIndex++;
                    }
                    _debugIndex++;
                }
            }
            else if (_debugTokens[_debugIndex].Equals(THEN, OIC))
            {
                if (!_debugIfAnswer)
                {
                    result.AppendLine($"False, skipping to {ELSE}");
                    _debugIfCondition = false;
                    SkipToElse(_debugTokens, ref _debugIndex);
                    _debugIndex++;
                }
                else
                {
                    _debugIfCondition = false;
                    _debugIndex++;
                }
            }
            else
            {
                _debugIfAnswer = CheckOneCondition(_debugTokens, ref _debugIndex);
            }
        }
        else if (_debugIf)
        {
            if (_debugTokens[_debugIndex].Equals(ENDIF, OIC))
            {
                _debugIndex++;
                _debugIf = false;
            }
            else if (_debugTokens[_debugIndex].Equals(ELSE, OIC) ||
                     _debugTokens[_debugIndex].Equals(ELSEIF, OIC))
            {
                result.AppendLine($"Skipping to {ENDIF}");
                SkipPastEndif(_debugTokens, ref _debugIndex);
                _debugIf = false;
            }
            else
            {
                RunOneCommand(_debugTokens, ref _debugIndex, tempResult);
            }
        }
        else if (_debugTokens[_debugIndex].Equals(IF, OIC))
        {
            _debugIndex++;
            _debugIf = true;
            _debugIfCondition = true;
            saveIfCond = true;
            _debugIfAnswer = CheckOneCondition(_debugTokens, ref _debugIndex);
        }
        else
        {
            RunOneCommand(_debugTokens, ref _debugIndex, tempResult);
        }
        result.Append($"Index {saveIndex}: ");
        result.AppendLine(string.Join(" ", _debugTokens, saveIndex, _debugIndex - saveIndex));
        result.Append(tempResult);
        if (saveIfCond)
        {
            result.AppendLine($"Answer = {_debugIfAnswer}");
        }
    }

    /// <summary>
    /// Checks if debugging the script has completed.
    /// </summary>
    public bool DebugDone()
    {
        return _debugIndex < 0 || _debugIndex >= _debugTokens.Length;
    }
}