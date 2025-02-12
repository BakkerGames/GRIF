using System.Reflection;
using System.Text;
using static GRIFTools.DAGSConstants;

namespace GRIFTools;

public partial class Dags
{
    /// <summary>
    /// Quick reference list of DAGS commands.
    /// </summary>
    public static string Help()
    {
        StringBuilder result = new();

        result.AppendLine("Statements:");
        result.AppendLine($"   {COMMENT}x)");
        result.AppendLine($"   {EXEC}script)");
        result.AppendLine($"   {GOLABEL}x)");
        result.AppendLine($"   {LABEL}x)");
        result.AppendLine($"   {RETURN}");
        result.AppendLine($"   {SCRIPT}key)");
        result.AppendLine($"   {SET}key,value)");
        result.AppendLine($"   {SWAP}key1,key2)");
        result.AppendLine();

        result.AppendLine("Numeric statements:");
        result.AppendLine($"   {ADDTO}key,x)");
        result.AppendLine($"   {DIVTO}key,x)");
        result.AppendLine($"   {MODTO}key,x)");
        result.AppendLine($"   {MULTO}key,x)");
        result.AppendLine($"   {NEGTO}key)");
        result.AppendLine($"   {SUBTO}key,x)");
        result.AppendLine();

        result.AppendLine("Output statements:");
        result.AppendLine($"   {NL}");
        result.AppendLine($"   {MSG}key)");
        result.AppendLine($"   {WRITE}value[,value,...])");
        result.AppendLine($"   {WRITELINE}value[,value,...])");
        result.AppendLine();

        result.AppendLine("Functions:");
        result.AppendLine($"   {ABS}x)");
        result.AppendLine($"   {ADD}x,y)");
        result.AppendLine($"   {CONCAT}value[,value,...])");
        result.AppendLine($"   {DIV}x,y)");
        result.AppendLine($"   {FORMAT}value,v0[,v1,...])");
        result.AppendLine($"   {GET}key)");
        result.AppendLine($"   {GETVALUE}key)");
        result.AppendLine($"   {LOWER}x)");
        result.AppendLine($"   {MOD}x,y)");
        result.AppendLine($"   {MUL}x,y)");
        result.AppendLine($"   {NEG}x)");
        result.AppendLine($"   {REPLACE}value,old,new)");
        result.AppendLine($"   {RND}x)");
        result.AppendLine($"   {SUB}x,y)");
        result.AppendLine($"   {SUBSTRING}value,start[,len])");
        result.AppendLine($"   {TRIM}x)");
        result.AppendLine($"   {UPPER}x)");
        result.AppendLine();

        result.AppendLine("If statement:");
        result.AppendLine($"   {IF} ... {THEN}");
        result.AppendLine($"      ...");
        result.AppendLine($"   [{ELSEIF} ... {THEN}]");
        result.AppendLine($"      [...]");
        result.AppendLine($"   [{ELSE}]");
        result.AppendLine($"      [...]");
        result.AppendLine($"   {ENDIF}");
        result.AppendLine();

        result.AppendLine("If conditions:");
        result.AppendLine($"   {EQ}x,y)");
        result.AppendLine($"   {EXISTS}key)");
        result.AppendLine($"   {FALSE}x)");
        result.AppendLine($"   {GE}x,y)");
        result.AppendLine($"   {GT}x,y)");
        result.AppendLine($"   {ISBOOL}x)");
        result.AppendLine($"   {ISNUMBER}x)");
        result.AppendLine($"   {ISSCRIPT}key)");
        result.AppendLine($"   {LE}x,y)");
        result.AppendLine($"   {LT}x,y)");
        result.AppendLine($"   {NE}x,y)");
        result.AppendLine($"   {NULL}x)");
        result.AppendLine($"   {RAND}x)");
        result.AppendLine($"   {TRUE}x)");
        result.AppendLine();

        result.AppendLine("Condition connectors/modifiers:");
        result.AppendLine($"   {AND}");
        result.AppendLine($"   {OR}");
        result.AppendLine($"   {NOT}");
        result.AppendLine();

        result.AppendLine("For loop:");
        result.AppendLine($"   {FOR}token,start,end)");
        result.AppendLine("        ...$token...");
        result.AppendLine($"   {ENDFOR}");
        result.AppendLine();

        result.AppendLine("ForEachKey loop:");
        result.AppendLine($"   {FOREACHKEY}token,prefix[,suffix])");
        result.AppendLine("        ...$token...");
        result.AppendLine($"   {ENDFOREACHKEY}");
        result.AppendLine();

        result.AppendLine("ForEachList loop:");
        result.AppendLine($"   {FOREACHLIST}token,name)");
        result.AppendLine("        ...$token...");
        result.AppendLine($"   {ENDFOREACHLIST}");
        result.AppendLine();

        result.AppendLine("List statements/functions:");
        result.AppendLine($"   {ADDLIST}name,value)");
        result.AppendLine($"   {CLEARLIST}name)");
        result.AppendLine($"   {GETLIST}name,pos)");
        result.AppendLine($"   {INSERTATLIST}name,pos,value)");
        result.AppendLine($"   {LISTLENGTH}name)");
        result.AppendLine($"   {REMOVEATLIST}name,pos)");
        result.AppendLine($"   {SETLIST}name,pos,value)");
        result.AppendLine();

        result.AppendLine("Array statements/functions:");
        result.AppendLine($"   {CLEARARRAY}name)");
        result.AppendLine($"   {GETARRAY}name,y,x)");
        result.AppendLine($"   {SETARRAY}name,y,x,value)");
        result.AppendLine();

        result.AppendLine("In/Out Channel commands:");
        result.AppendLine($"   {GETINCHANNEL}");
        result.AppendLine($"   {SETOUTCHANNEL}value)");

        return result.ToString();
    }

    /// <summary>
    /// Full syntax reference document for DAGS commands.
    /// </summary>
    public static string Syntax()
    {
        return GetResourceText("DAGS_SYNTAX.md");
    }

    #region Private

    private static string GetResourceText(string resourceName)
    {
        var result = "";
        try
        {
            var _assembly = Assembly.GetExecutingAssembly();
            if (_assembly != null)
            {
                var stream = _assembly?.GetManifestResourceStream(resourceName);
                if (stream != null)
                {
                    var _textStreamReader = new StreamReader(stream);
                    result = _textStreamReader.ReadToEnd();
                }
            }
            return result;
        }
        catch
        {
            return "";
        }
    }

    #endregion
}
