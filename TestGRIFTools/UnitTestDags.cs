using GRIFTools;
using System.Text;
using static GRIFTools.DAGSRoutines;

namespace TestGRIFTools;

public class UnitTestDags
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test_Passing()
    {
        Assert.Pass();
    }

    [Test]
    public void Test_Get()
    {
        Grod data = new();
        Dags dags = new(data);
        var key = "abc";
        var value = "123";
        data.Set(key, value);
        StringBuilder result = new();
        dags.RunScript($"@get({key})", result);
        Assert.That(result.ToString(), Is.EqualTo(value));
    }

    [Test]
    public void Test_Set()
    {
        Grod data = new();
        Dags dags = new(data);
        var key = "abc";
        var value = "123";
        StringBuilder result = new();
        dags.RunScript($"@set({key},{value})", result);
        dags.RunScript($"@get({key})", result);
        Assert.That(result.ToString(), Is.EqualTo(value));
    }

    [Test]
    public void Test_Set_Script()
    {
        Grod data = new();
        Dags dags = new(data);
        var key = "abc";
        var answer = "@comment(\"this is a comment\")";
        var value = "\"" + answer.Replace("\"", "\\\"") + "\"";
        StringBuilder result = new();
        dags.RunScript($"@set({key},{value})", result);
        dags.RunScript($"@get({key})", result);
        Assert.That(result.ToString(), Is.EqualTo(answer));
    }

    [Test]
    public void Test_SetArray()
    {
        Grod data = new();
        Dags dags = new(data);
        var key = "abc";
        var value = "123";
        StringBuilder result = new();
        dags.RunScript($"@setarray({key},2,3,{value})", result);
        dags.RunScript($"@getarray({key},2,3)", result);
        Assert.That(result.ToString(), Is.EqualTo(value));
    }

    [Test]
    public void Test_SetArray_Null()
    {
        Grod data = new();
        Dags dags = new(data);
        var key = "abc";
        var value = "";
        StringBuilder result = new();
        dags.RunScript($"@setarray({key},2,3,{value})", result);
        dags.RunScript($"@getarray({key},2,3)", result);
        Assert.That(result.ToString(), Is.EqualTo(value));
    }

    [Test]
    public void Test_ClearArray()
    {
        Grod data = new();
        Dags dags = new(data);
        var key = "abc";
        var value = "123";
        StringBuilder result = new();
        dags.RunScript($"@setarray({key},2,3,{value})", result);
        dags.RunScript($"@cleararray({key})", result);
        dags.RunScript($"@getarray({key},2,3)", result);
        Assert.That(result.ToString(), Is.EqualTo(""));
    }

    [Test]
    public void Test_SetList()
    {
        Grod data = new();
        Dags dags = new(data);
        var key = "abc";
        var value = "123";
        StringBuilder result = new();
        dags.RunScript($"@setlist({key},1,{value})", result);
        dags.RunScript($"@getlist({key},1)", result);
        Assert.That(result.ToString(), Is.EqualTo(value));
    }

    [Test]
    public void Test_SetList_Null()
    {
        Grod data = new();
        Dags dags = new(data);
        var key = "abc";
        var value = "";
        StringBuilder result = new();
        dags.RunScript($"@setlist({key},1,{value})", result);
        dags.RunScript($"@getlist({key},1)", result);
        Assert.That(result.ToString(), Is.EqualTo(value));
    }

    [Test]
    public void Test_SetList_TabCRLF()
    {
        Grod data = new();
        Dags dags = new(data);
        var key = "abc";
        var value = "abc\t\r\n123";
        StringBuilder result = new();
        dags.RunScript($"@setlist({key},1,\"{value}\")", result);
        dags.RunScript($"@getlist({key},1)", result);
        Assert.That(result.ToString(), Is.EqualTo(value));
    }

    [Test]
    public void Test_InsertAtList()
    {
        Grod data = new();
        Dags dags = new(data);
        var key = "abc";
        var value = "123";
        StringBuilder result = new();
        dags.RunScript($"@addlist({key},0)", result);
        dags.RunScript($"@addlist({key},1)", result);
        dags.RunScript($"@addlist({key},2)", result);
        dags.RunScript($"@addlist({key},3)", result);
        dags.RunScript($"@insertatlist({key},1,{value})", result);
        dags.RunScript($"@getlist({key},1)", result);
        Assert.That(result.ToString(), Is.EqualTo(value));
        result.Clear();
        dags.RunScript($"@getlist({key},4)", result);
        Assert.That(result.ToString(), Is.EqualTo("3"));
    }

    [Test]
    public void Test_RemoveAtList()
    {
        Grod data = new();
        Dags dags = new(data);
        var key = "abc";
        var value = "123";
        StringBuilder result = new();
        dags.RunScript($"@setlist({key},3,{value})", result);
        dags.RunScript($"@removeatlist({key},0)", result);
        dags.RunScript($"@getlist({key},2)", result);
        Assert.That(result.ToString(), Is.EqualTo(value));
    }

    [Test]
    public void Test_Function()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        dags.RunScript("@set(\"@boo\",\"@write(eek!)\")", result);
        dags.RunScript("@boo", result);
        Assert.That(result.ToString(), Is.EqualTo("eek!"));
    }

    [Test]
    public void Test_FunctionParameters()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        dags.RunScript("@set(\"@boo(x)\",\"@write($x)\")", result);
        dags.RunScript("@boo(eek!)", result);
        Assert.That(result.ToString(), Is.EqualTo("eek!"));
    }

    [Test]
    public void Test_ValidateSucceed()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        bool value = dags.ValidateScript("key", "@set(key,value)", result);
        Assert.That(value, Is.EqualTo(true));
    }

    [Test]
    public void Test_ValidateFail()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        bool value = dags.ValidateScript("key", "@blah(key)", result);
        Assert.That(value, Is.EqualTo(false));
    }

    [Test]
    public void Test_ValidateSyntaxSucceed()
    {
        StringBuilder result = new();
        bool value = Dags.ValidateSyntax("key", "@blah(key)", result);
        Assert.That(value, Is.EqualTo(true));
    }

    [Test]
    public void Test_Abs()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@write(@abs(1))", result);
        Assert.That(result.ToString(), Is.EqualTo("1"));
        result.Clear();
        dags.RunScript("@write(@abs(-1))", result);
        Assert.That(result.ToString(), Is.EqualTo("1"));
    }

    [Test]
    public void Test_Add()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@write(@add(1,3))", result);
        Assert.That(result.ToString(), Is.EqualTo("4"));
    }

    [Test]
    public void Test_AddTo()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@set(value,12) @addto(value,7) @write(@get(value))", result);
        Assert.That(result.ToString(), Is.EqualTo("19"));
    }

    [Test]
    public void Test_Comment()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@comment(\"this is a comment\")", result);
        Assert.That(result.ToString(), Is.EqualTo(""));
    }

    [Test]
    public void Test_Concat()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@write(@concat(abc,def,123))", result);
        Assert.That(result.ToString(), Is.EqualTo("abcdef123"));
    }

    [Test]
    public void Test_Debug()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        data.Set("system.debug", "true");
        result.Clear();
        dags.RunScript("@debug(\"this is a comment\")", result);
        Assert.That(result.ToString(), Is.EqualTo("### this is a comment" + DAGSConstants.NL_VALUE));
        result.Clear();
        dags.RunScript("@debug(@add(123,456))", result);
        Assert.That(result.ToString(), Is.EqualTo("### 579" + DAGSConstants.NL_VALUE));
        data.Set("system.debug", "false");
        result.Clear();
        dags.RunScript("@debug(\"this is a comment\")", result);
        Assert.That(result.ToString(), Is.EqualTo(""));
    }

    [Test]
    public void Test_Div()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@write(@div(42,6))", result);
        Assert.That(result.ToString(), Is.EqualTo("7"));
    }

    [Test]
    public void Test_DivTo()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@set(value,12) @divto(value,3) @write(@get(value))", result);
        Assert.That(result.ToString(), Is.EqualTo("4"));
    }

    [Test]
    public void Test_EQ()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@write(@eq(42,6))", result);
        Assert.That(result.ToString(), Is.EqualTo("false"));
        result.Clear();
        dags.RunScript("@write(@eq(42,42))", result);
        Assert.That(result.ToString(), Is.EqualTo("true"));
    }

    [Test]
    public void Test_Exec()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@exec(\"@set(value,23)\") @write(@get(value))", result);
        Assert.That(result.ToString(), Is.EqualTo("23"));
    }

    [Test]
    public void Test_False()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@write(@false(\"\"))", result);
        Assert.That(result.ToString(), Is.EqualTo("true"));
        result.Clear();
        dags.RunScript("@write(@false(0))", result);
        Assert.That(result.ToString(), Is.EqualTo("true"));
        result.Clear();
        dags.RunScript("@write(@false(1))", result);
        Assert.That(result.ToString(), Is.EqualTo("false"));
        result.Clear();
        dags.RunScript("@write(@false(abc))", result);
        Assert.That(result.ToString(), Is.EqualTo("false"));
    }

    [Test]
    public void Test_For()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@for(x,1,3) @write($x) @endfor", result);
        Assert.That(result.ToString(), Is.EqualTo("123"));
    }

    [Test]
    public void Test_ForEachKey()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        dags.RunScript("@set(value.1,100) @set(value.2,200)", result);
        result.Clear();
        dags.RunScript("@foreachkey(x,\"value.\") @write($x) @endforeachkey", result);
        Assert.That(result.ToString(), Is.EqualTo("12"));
        result.Clear();
        dags.RunScript("@foreachkey(x,\"value.\") @get(value.$x) @endforeachkey", result);
        Assert.That(result.ToString(), Is.EqualTo("100200"));
    }

    [Test]
    public void Test_ForEachList()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        dags.RunScript("@setlist(value,1,10)", result);
        dags.RunScript("@setlist(value,2,20)", result);
        dags.RunScript("@setlist(value,3,30)", result);
        result.Clear();
        dags.RunScript("@foreachlist(x,value) @write($x) @endforeachlist", result);
        Assert.That(result.ToString(), Is.EqualTo("102030"));
    }

    [Test]
    public void Test_Format()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@write(@format(\"{0}-{1}-{2}\",1,2,3))", result);
        Assert.That(result.ToString(), Is.EqualTo("1-2-3"));
        result.Clear();
        dags.RunScript("@write(@format(\"{2}-{1}-{0}\",1,2,3))", result);
        Assert.That(result.ToString(), Is.EqualTo("3-2-1"));
        result.Clear();
        dags.RunScript("@write(@format(\"{0}-{1}-{2}\",1,2))", result);
        Assert.That(result.ToString(), Is.EqualTo("1-2-{2}"));
    }

    [Test]
    public void Test_GE()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@write(@ge(42,6))", result);
        Assert.That(result.ToString(), Is.EqualTo("true"));
        result.Clear();
        dags.RunScript("@write(@ge(42,42))", result);
        Assert.That(result.ToString(), Is.EqualTo("true"));
        result.Clear();
        dags.RunScript("@write(@ge(1,42))", result);
        Assert.That(result.ToString(), Is.EqualTo("false"));
    }

    [Test]
    public void Test_GetInChannel()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        dags.InChannel.Enqueue("abc");
        dags.InChannel.Enqueue("123");
        result.Clear();
        dags.RunScript("@write(@getinchannel)", result);
        Assert.That(result.ToString(), Is.EqualTo("abc"));
        result.Clear();
        dags.RunScript("@write(@getinchannel)", result);
        Assert.That(result.ToString(), Is.EqualTo("123"));
        result.Clear();
        dags.RunScript("@write(@getinchannel)", result);
        Assert.That(result.ToString(), Is.EqualTo(""));
    }

    [Test]
    public void Test_GetValue()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        dags.RunScript("@set(v1,\"@get(v2)\") @set(v2,123)", result);
        result.Clear();
        dags.RunScript("@get(v1)", result);
        Assert.That(result.ToString(), Is.EqualTo("@get(v2)"));
        result.Clear();
        dags.RunScript("@write(@getvalue(v1))", result);
        Assert.That(result.ToString(), Is.EqualTo("123"));
    }

    [Test]
    public void Test_GoLabel()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@write(abc) @golabel(1) @write(def) @label(1) @write(xyz)", result);
        Assert.That(result.ToString(), Is.EqualTo("abcxyz"));
    }

    [Test]
    public void Test_GT()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@write(@gt(42,6))", result);
        Assert.That(result.ToString(), Is.EqualTo("true"));
        result.Clear();
        dags.RunScript("@write(@gt(42,42))", result);
        Assert.That(result.ToString(), Is.EqualTo("false"));
        result.Clear();
        dags.RunScript("@write(@gt(1,42))", result);
        Assert.That(result.ToString(), Is.EqualTo("false"));
    }

    [Test]
    public void Test_If()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@if true @then @write(abc) @else @write(def) @endif", result);
        Assert.That(result.ToString(), Is.EqualTo("abc"));
        result.Clear();
        dags.RunScript("@if false @then @write(abc) @else @write(def) @endif", result);
        Assert.That(result.ToString(), Is.EqualTo("def"));
        result.Clear();
        dags.RunScript("@if true @or false @then @write(abc) @else @write(def) @endif", result);
        Assert.That(result.ToString(), Is.EqualTo("abc"));
        result.Clear();
        dags.RunScript("@if true @and false @then @write(abc) @else @write(def) @endif", result);
        Assert.That(result.ToString(), Is.EqualTo("def"));
        result.Clear();
        dags.RunScript("@if null @then @write(abc) @else @write(def) @endif", result);
        Assert.That(result.ToString(), Is.EqualTo("def"));
    }

    [Test]
    public void Test_IsBool()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@write(@isbool(0))", result);
        Assert.That(result.ToString(), Is.EqualTo("true"));
        result.Clear();
        dags.RunScript("@write(@isbool(1))", result);
        Assert.That(result.ToString(), Is.EqualTo("true"));
        result.Clear();
        dags.RunScript("@write(@isbool(notboolean))", result);
        Assert.That(result.ToString(), Is.EqualTo("false"));
    }

    [Test]
    public void Test_Null()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@write(@null(null))", result);
        Assert.That(result.ToString(), Is.EqualTo("true"));
        result.Clear();
        dags.RunScript("@write(@null(abc))", result);
        Assert.That(result.ToString(), Is.EqualTo("false"));
        result.Clear();
        dags.RunScript("@write(@null(@get(value)))", result);
        Assert.That(result.ToString(), Is.EqualTo("true"));
    }

    [Test]
    public void Test_Exists()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@write(@exists(test.value))", result);
        Assert.That(result.ToString(), Is.EqualTo("false"));
        result.Clear();
        dags.RunScript("@set(test.value,null) @write(@exists(test.value))", result);
        Assert.That(result.ToString(), Is.EqualTo("false"));
        result.Clear();
        dags.RunScript("@set(test.value,abc) @write(@exists(test.value))", result);
        Assert.That(result.ToString(), Is.EqualTo("true"));
        result.Clear();
        dags.RunScript("@set(test.value,\"\") @write(@exists(test.value))", result);
        Assert.That(result.ToString(), Is.EqualTo("false"));
    }

    [Test]
    public void Test_IsScript()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@set(test.value,abc) @write(@isscript(test.value))", result);
        Assert.That(result.ToString(), Is.EqualTo("false"));
        result.Clear();
        dags.RunScript("@set(test.value,\"@get(value)\") @write(@isscript(test.value))", result);
        Assert.That(result.ToString(), Is.EqualTo("true"));
    }

    [Test]
    public void Test_LE()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@write(@le(42,6))", result);
        Assert.That(result.ToString(), Is.EqualTo("false"));
        result.Clear();
        dags.RunScript("@write(@le(42,42))", result);
        Assert.That(result.ToString(), Is.EqualTo("true"));
        result.Clear();
        dags.RunScript("@write(@le(1,42))", result);
        Assert.That(result.ToString(), Is.EqualTo("true"));
    }

    [Test]
    public void Test_Lower()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@lower(ABC)", result);
        Assert.That(result.ToString(), Is.EqualTo("abc"));
        result.Clear();
        dags.RunScript("@lower(DEF)", result);
        Assert.That(result.ToString(), Is.EqualTo("def"));
    }

    [Test]
    public void Test_LT()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@write(@lt(42,6))", result);
        Assert.That(result.ToString(), Is.EqualTo("false"));
        result.Clear();
        dags.RunScript("@write(@lt(42,42))", result);
        Assert.That(result.ToString(), Is.EqualTo("false"));
        result.Clear();
        dags.RunScript("@write(@lt(1,42))", result);
        Assert.That(result.ToString(), Is.EqualTo("true"));
    }

    [Test]
    public void Test_Mod()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@write(@mod(13,4))", result);
        Assert.That(result.ToString(), Is.EqualTo("1"));
        result.Clear();
        dags.RunScript("@write(@mod(12,4))", result);
        Assert.That(result.ToString(), Is.EqualTo("0"));
    }

    [Test]
    public void Test_ModTo()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@set(value,13) @modto(value,4) @write(@get(value))", result);
        Assert.That(result.ToString(), Is.EqualTo("1"));
    }

    [Test]
    public void Test_Msg()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@set(value,abcdef) @msg(value)", result);
        Assert.That(result.ToString(), Is.EqualTo("abcdef" + DAGSConstants.NL_VALUE));
    }

    [Test]
    public void Test_Mul()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@write(@mul(3,4))", result);
        Assert.That(result.ToString(), Is.EqualTo("12"));
    }

    [Test]
    public void Test_MulTo()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@set(value,3) @multo(value,4) @write(@get(value))", result);
        Assert.That(result.ToString(), Is.EqualTo("12"));
    }

    [Test]
    public void Test_NE()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@write(@ne(42,6))", result);
        Assert.That(result.ToString(), Is.EqualTo("true"));
        result.Clear();
        dags.RunScript("@write(@ne(42,42))", result);
        Assert.That(result.ToString(), Is.EqualTo("false"));
    }

    [Test]
    public void Test_Neg()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@write(@neg(3))", result);
        Assert.That(result.ToString(), Is.EqualTo("-3"));
    }

    [Test]
    public void Test_NegTo()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@set(value,3) @negto(value) @write(@get(value))", result);
        Assert.That(result.ToString(), Is.EqualTo("-3"));
    }

    [Test]
    public void Test_NL()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@nl", result);
        Assert.That(result.ToString(), Is.EqualTo(DAGSConstants.NL_VALUE));
    }

    [Test]
    public void Test_Not()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@if @not false @then @write(abc) @else @write(def) @endif", result);
        Assert.That(result.ToString(), Is.EqualTo("abc"));
    }

    [Test]
    public void Test_Or()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@if true @or false @then @write(abc) @else @write(def) @endif", result);
        Assert.That(result.ToString(), Is.EqualTo("abc"));
    }

    [Test]
    public void Test_Rand()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@rand(30)", result);
        Assert.That(result.ToString() == "true" || result.ToString() == "false");
    }

    [Test]
    public void Test_Replace()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@write(@replace(abcdef,d,x))", result);
        Assert.That(result.ToString(), Is.EqualTo("abcxef"));
    }

    [Test]
    public void Test_Rnd()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        dags.RunScript("@set(value,@rnd(20))", result);
        result.Clear();
        dags.RunScript("@get(value)", result);
        var r1 = int.Parse(result.ToString());
        Assert.That(r1 >= 0 && r1 < 20);
    }

    [Test]
    public void Test_Script()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        dags.RunScript("@set(script1,\"@write(abc)\")", result);
        result.Clear();
        dags.RunScript("@script(script1)", result);
        Assert.That(result.ToString(), Is.EqualTo("abc"));
    }

    [Test]
    public void Test_SetOutChannel()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@setoutchannel(abc)", result);
        var value = dags.OutChannel.Dequeue() ?? "";
        Assert.That(value, Is.EqualTo("abc"));
    }

    [Test]
    public void Test_Sub()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@write(@sub(1,3))", result);
        Assert.That(result.ToString(), Is.EqualTo("-2"));
    }

    [Test]
    public void Test_Substring()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@write(@substring(abcdef,1,4))", result);
        Assert.That(result.ToString(), Is.EqualTo("bcde"));
    }

    [Test]
    public void Test_SubTo()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@set(value,12) @subto(value,7) @write(@get(value))", result);
        Assert.That(result.ToString(), Is.EqualTo("5"));
    }

    [Test]
    public void Test_Swap()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@set(value1,abc) @set(value2,def) @swap(value1,value2) @write(@get(value1),@get(value2))", result);
        Assert.That(result.ToString(), Is.EqualTo("defabc"));
    }

    [Test]
    public void Test_Trim()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@set(value,\"   abc   \") @write(@trim(@get(value)))", result);
        Assert.That(result.ToString(), Is.EqualTo("abc"));
    }

    [Test]
    public void Test_True()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@write(@true(0))", result);
        Assert.That(result.ToString(), Is.EqualTo("false"));
        result.Clear();
        dags.RunScript("@write(@true(1))", result);
        Assert.That(result.ToString(), Is.EqualTo("true"));
        result.Clear();
        dags.RunScript("@write(@true(abc))", result);
        Assert.That(result.ToString(), Is.EqualTo("false"));
    }

    [Test]
    public void Test_Upper()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        result.Clear();
        dags.RunScript("@write(@upper(abc))", result);
        Assert.That(result.ToString(), Is.EqualTo("ABC"));
    }

    [Test]
    public void Test_Help()
    {
        var helpText = Dags.Help();
        Assert.That(helpText, !Is.EqualTo(null));
    }

    [Test]
    public void Test_PrettyScript()
    {
        var script = "@if @eq(@get(value),0) @then @write(\"zero\") @else @write(\"not zero\") @endif";
        var expected = "@if @eq(@get(value),0) @then\r\n\t@write(\"zero\")\r\n@else\r\n\t@write(\"not zero\")\r\n@endif";
        var actual = PrettyScript(script);
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Test_PrettyScript_Min()
    {
        var script = "@if@eq(@get(value),0)@then@write(\"zero\")@else@write(\"not zero\")@endif";
        var expected = "@if @eq(@get(value),0) @then\r\n\t@write(\"zero\")\r\n@else\r\n\t@write(\"not zero\")\r\n@endif";
        var actual = PrettyScript(script);
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Test_PrettyScript_Same()
    {
        var script = "@write(\"hello \\\"wonderful\\\" world.\")";
        var actual = PrettyScript(script);
        Assert.That(actual, Is.EqualTo(script));
    }

    [Test]
    public void Test_IfThenNoStatements()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        var script = "@if @eq(1,1) @then @endif";
        dags.RunScript(script, result);
        Assert.That(result.ToString(), Is.EqualTo(""));
    }

    [Test]
    public void Test_IfThenElseNoStatements()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        var script = "@if @eq(1,2) @then @write(abc) @else @endif";
        dags.RunScript(script, result);
        Assert.That(result.ToString(), Is.EqualTo(""));
    }

    [Test]
    public void Test_Syntax()
    {
        var helpText = Dags.Syntax();
        Assert.That(helpText, !Is.EqualTo(null));
    }

    [Test]
    public void Test_Return()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        var key = "abc";
        var value1 = "123";
        var value2 = "456";
        dags.RunScript($"@set({key},{value1}) @return @set({key},{value2})", result);
        Assert.That(result.Length, Is.EqualTo(0));
        dags.RunScript($"@get({key})", result);
        Assert.That(result.ToString(), Is.EqualTo(value1));
    }

    [Test]
    public void Test_AddList()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        var key = "abc";
        var value1 = "123";
        var value2 = "456";
        dags.RunScript($"@addlist({key},{value1}) @addlist({key},{value2})", result);
        dags.RunScript($"@get({key})", result);
        Assert.That(result.ToString(), Is.EqualTo(value1 + ',' + value2));
    }

    [Test]
    public void Test_ClearList()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        var key = "abc";
        var value1 = "123";
        var value2 = "456";
        dags.RunScript($"@addlist({key},{value1}) @addlist({key},{value2})", result);
        dags.RunScript($"@clearlist({key})", result);
        dags.RunScript($"@get({key})", result);
        Assert.That(result.ToString(), Is.EqualTo(""));
    }

    [Test]
    public void Test_GetArray()
    {
        Grod data = new();
        Dags dags = new(data);
        var key = "abc";
        var value = "123";
        StringBuilder result = new();
        dags.RunScript($"@setarray({key},2,3,{value})", result);
        dags.RunScript($"@getarray({key},2,3)", result);
        Assert.That(result.ToString(), Is.EqualTo(value));
    }

    [Test]
    public void Test_GetList()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        var key = "abc";
        var value1 = "123";
        var value2 = "456";
        dags.RunScript($"@addlist({key},{value1}) @addlist({key},{value2})", result);
        dags.RunScript($"@getlist({key},0)", result);
        Assert.That(result.ToString(), Is.EqualTo(value1));
        result.Clear();
        dags.RunScript($"@getlist({key},1)", result);
        Assert.That(result.ToString(), Is.EqualTo(value2));
        result.Clear();
        dags.RunScript($"@getlist({key},2)", result);
        Assert.That(result.ToString(), Is.EqualTo(""));
    }

    [Test]
    public void Test_IsNumber()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        var value1 = "123";
        var value2 = "abc";
        var value3 = "";
        dags.RunScript($"@isnumber({value1})", result);
        Assert.That(result.ToString(), Is.EqualTo("true"));
        result.Clear();
        dags.RunScript($"@isnumber({value2})", result);
        Assert.That(result.ToString(), Is.EqualTo("false"));
        result.Clear();
        dags.RunScript($"@isnumber({value3})", result);
        Assert.That(result.ToString()[..5], Is.EqualTo("ERROR"));
    }

    [Test]
    public void Test_ListLength()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        var key = "abc";
        var value1 = "123";
        var value2 = "456";
        dags.RunScript($"@addlist({key},{value1}) @addlist({key},{value2})", result);
        dags.RunScript($"@listlength({key})", result);
        Assert.That(result.ToString(), Is.EqualTo("2"));
    }

    [Test]
    public void Test_Write()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        var value1 = "123";
        dags.RunScript($"@write({value1})", result);
        Assert.That(result.ToString(), Is.EqualTo(value1));
    }

    [Test]
    public void Test_WriteLine()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        var value1 = "123";
        dags.RunScript($"@writeline({value1})", result);
        // @writeline result ends with two characters, '\' and 'n'.
        // This is the expected behavior. See Test_NL().
        Assert.That(result.ToString(), Is.EqualTo(value1 + DAGSConstants.NL_VALUE));
    }

    [Test]
    public void Test_GetBit()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        dags.RunScript($"@getbit(4,2)", result);
        Assert.That(result.ToString(), Is.EqualTo("1"));
        result.Clear();
        dags.RunScript($"@getbit(8,2)", result);
        Assert.That(result.ToString(), Is.EqualTo("0"));
        result.Clear();
        dags.RunScript($"@getbit(1073741824,30)", result);
        Assert.That(result.ToString(), Is.EqualTo("1"));
    }

    [Test]
    public void Test_SetBit()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        dags.RunScript($"@setbit(0,2)", result);
        Assert.That(result.ToString(), Is.EqualTo("4"));
        result.Clear();
        dags.RunScript($"@setbit(0,0)", result);
        Assert.That(result.ToString(), Is.EqualTo("1"));
        result.Clear();
        dags.RunScript($"@setbit(0,30)", result);
        Assert.That(result.ToString(), Is.EqualTo("1073741824"));
    }

    [Test]
    public void Test_ClearBit()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        dags.RunScript($"@clearbit(7,2)", result);
        Assert.That(result.ToString(), Is.EqualTo("3"));
        result.Clear();
        dags.RunScript($"@clearbit(7,0)", result);
        Assert.That(result.ToString(), Is.EqualTo("6"));
        result.Clear();
        dags.RunScript($"@clearbit(1073741824,30)", result);
        Assert.That(result.ToString(), Is.EqualTo("0"));
    }

    [Test]
    public void Test_BitwiseAnd()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        dags.RunScript($"@bitwiseand(7,2)", result);
        Assert.That(result.ToString(), Is.EqualTo("2"));
        result.Clear();
        dags.RunScript($"@bitwiseand(8,2)", result);
        Assert.That(result.ToString(), Is.EqualTo("0"));
    }

    [Test]
    public void Test_BitwiseOr()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        dags.RunScript($"@bitwiseor(7,2)", result);
        Assert.That(result.ToString(), Is.EqualTo("7"));
        result.Clear();
        dags.RunScript($"@bitwiseor(8,2)", result);
        Assert.That(result.ToString(), Is.EqualTo("10"));
    }

    [Test]
    public void Test_BitwiseXor()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        dags.RunScript($"@bitwisexor(7,2)", result);
        Assert.That(result.ToString(), Is.EqualTo("5"));
        result.Clear();
        dags.RunScript($"@bitwisexor(8,7)", result);
        Assert.That(result.ToString(), Is.EqualTo("15"));
    }

    [Test]
    public void Test_ToBinary()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        dags.RunScript($"@tobinary(7)", result);
        Assert.That(result.ToString(), Is.EqualTo("111"));
    }

    [Test]
    public void Test_ToInteger()
    {
        Grod data = new();
        Dags dags = new(data);
        StringBuilder result = new();
        dags.RunScript($"@tointeger(111)", result);
        Assert.That(result.ToString(), Is.EqualTo("7"));
    }
}
