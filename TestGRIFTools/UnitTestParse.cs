using GRIFTools;
using GRIFTools.Parse;

namespace TestGRIFTools;

public class UnitTestParse
{
    [SetUp]
    public void Setup()
    {
        var grod = new Grod();
        grod.Set("command.examine.cloak", "@writeline(\"Looking at cloak...\")");
        grod.Set("command.look", "@writeline(\"Looking...\")");
        grod.Set("command.look.at.cloak", "@script(command.examine.cloak)");
        grod.Set("command.put.*.on.*", "@writeline(\"Put something on something\")");
        grod.Set("command.put.on.cloak", "@script(command.wear.cloak)");
        grod.Set("command.take.cloak", "@writeline(\"Taking cloak...\")");
        grod.Set("command.take.*", "@writeline(\"Taking something...\")");
        grod.Set("command.take.cloak.from.hook", "@writeline(\"Taking cloak from hook...\")");
        grod.Set("command.take.cloak.from.*", "@writeline(\"Taking cloak from something...\")");
        grod.Set("command.wear.cloak", "@writeline(\"Putting on cloak...\")");
        grod.Set("noun.cloak", "cloak,cape");
        grod.Set("noun.hook", "hook,hanger,coathook");
        grod.Set("noun.table", "table");
        grod.Set("preposition.at", "at");
        grod.Set("preposition.from", "from,off");
        grod.Set("preposition.on", "on,onto");
        grod.Set("verb.examine", "examine");
        grod.Set("verb.look", "look");
        grod.Set("verb.put", "put,place,insert");
        grod.Set("verb.take", "take,get");
        grod.Set("verb.wear", "wear,don,equip");
        Parse.Init(grod);
    }

    [Test]
    public void Test_Null()
    {
        var result = Parse.ParseInput("");
        Assert.That(result.Error, Is.EqualTo(""));
        Assert.That(result.Verb, Is.EqualTo(""));
        Assert.That(result.VerbWord, Is.EqualTo(""));
        Assert.That(result.Noun, Is.EqualTo(""));
        Assert.That(result.NounWord, Is.EqualTo(""));
        Assert.That(result.Preposition, Is.EqualTo(""));
        Assert.That(result.PrepositionWord, Is.EqualTo(""));
        Assert.That(result.Object, Is.EqualTo(""));
        Assert.That(result.ObjectWord, Is.EqualTo(""));
        Assert.That(result.CommandKey, Is.EqualTo(""));
    }

    [Test]
    public void Test_Look()
    {
        var result = Parse.ParseInput("look");
        Assert.Multiple(() =>
        {
            Assert.That(result.Error, Is.EqualTo(""));
            Assert.That(result.Verb, Is.EqualTo("look"));
            Assert.That(result.VerbWord, Is.EqualTo("look"));
            Assert.That(result.Noun, Is.EqualTo(""));
            Assert.That(result.CommandKey, Is.EqualTo("command.look"));
        });
    }

    [Test]
    public void Test_Take_Cloak()
    {
        var result = Parse.ParseInput("take cloak");
        Assert.Multiple(() =>
        {
            Assert.That(result.Error, Is.EqualTo(""));
            Assert.That(result.Verb, Is.EqualTo("take"));
            Assert.That(result.VerbWord, Is.EqualTo("take"));
            Assert.That(result.Noun, Is.EqualTo("cloak"));
            Assert.That(result.NounWord, Is.EqualTo("cloak"));
            Assert.That(result.CommandKey, Is.EqualTo("command.take.cloak"));
        });
    }

    [Test]
    public void Test_Take_Cloak_Alias()
    {
        var result = Parse.ParseInput("get cape");
        Assert.Multiple(() =>
        {
            Assert.That(result.Error, Is.EqualTo(""));
            Assert.That(result.Verb, Is.EqualTo("take"));
            Assert.That(result.VerbWord, Is.EqualTo("get"));
            Assert.That(result.Noun, Is.EqualTo("cloak"));
            Assert.That(result.NounWord, Is.EqualTo("cape"));
            Assert.That(result.CommandKey, Is.EqualTo("command.take.cloak"));
        });
    }

    [Test]
    public void Test_Take_Cloak_From_Hook()
    {
        var result = Parse.ParseInput("take cloak from hook");
        Assert.Multiple(() =>
        {
            Assert.That(result.Error, Is.EqualTo(""));
            Assert.That(result.Verb, Is.EqualTo("take"));
            Assert.That(result.VerbWord, Is.EqualTo("take"));
            Assert.That(result.Noun, Is.EqualTo("cloak"));
            Assert.That(result.NounWord, Is.EqualTo("cloak"));
            Assert.That(result.Preposition, Is.EqualTo("from"));
            Assert.That(result.PrepositionWord, Is.EqualTo("from"));
            Assert.That(result.Object, Is.EqualTo("hook"));
            Assert.That(result.ObjectWord, Is.EqualTo("hook"));
            Assert.That(result.CommandKey, Is.EqualTo("command.take.cloak.from.hook"));
        });
    }

    [Test]
    public void Test_Take_Cloak_From_Hook_Alias()
    {
        var result = Parse.ParseInput("get cape off coathook");
        Assert.Multiple(() =>
        {
            Assert.That(result.Error, Is.EqualTo(""));
            Assert.That(result.Verb, Is.EqualTo("take"));
            Assert.That(result.VerbWord, Is.EqualTo("get"));
            Assert.That(result.Noun, Is.EqualTo("cloak"));
            Assert.That(result.NounWord, Is.EqualTo("cape"));
            Assert.That(result.Preposition, Is.EqualTo("from"));
            Assert.That(result.PrepositionWord, Is.EqualTo("off"));
            Assert.That(result.Object, Is.EqualTo("hook"));
            Assert.That(result.ObjectWord, Is.EqualTo("coathook"));
            Assert.That(result.CommandKey, Is.EqualTo("command.take.cloak.from.hook"));
        });
    }

    [Test]
    public void Test_Look_At_Cloak()
    {
        var result = Parse.ParseInput("look at cloak");
        Assert.Multiple(() =>
        {
            Assert.That(result.Error, Is.EqualTo(""));
            Assert.That(result.Verb, Is.EqualTo("look"));
            Assert.That(result.VerbWord, Is.EqualTo("look"));
            Assert.That(result.Noun, Is.EqualTo(""));
            Assert.That(result.NounWord, Is.EqualTo(""));
            Assert.That(result.Preposition, Is.EqualTo("at"));
            Assert.That(result.PrepositionWord, Is.EqualTo("at"));
            Assert.That(result.Object, Is.EqualTo("cloak"));
            Assert.That(result.ObjectWord, Is.EqualTo("cloak"));
            Assert.That(result.CommandKey, Is.EqualTo("command.look.at.cloak"));
        });
    }

    [Test]
    public void Test_Verb_Unknown()
    {
        var result = Parse.ParseInput("eat");
        Assert.That(result.Error, Is.EqualTo("I don't understand that."));
    }

    [Test]
    public void Test_Noun_Unknown()
    {
        var result = Parse.ParseInput("take food");
        Assert.That(result.Error, Is.EqualTo("I don't understand \"take food\"."));
    }

    [Test]
    public void Test_Noun_Wildcard()
    {
        var result = Parse.ParseInput("put cloak on table");
        Assert.Multiple(() =>
        {
            Assert.That(result.Error, Is.EqualTo(""));
            Assert.That(result.Verb, Is.EqualTo("put"));
            Assert.That(result.VerbWord, Is.EqualTo("put"));
            Assert.That(result.Noun, Is.EqualTo("*"));
            Assert.That(result.NounWord, Is.EqualTo("cloak"));
            Assert.That(result.Preposition, Is.EqualTo("on"));
            Assert.That(result.PrepositionWord, Is.EqualTo("on"));
            Assert.That(result.Object, Is.EqualTo("*"));
            Assert.That(result.ObjectWord, Is.EqualTo("table"));
            Assert.That(result.CommandKey, Is.EqualTo("command.put.*.on.*"));
        });
    }

    [Test]
    public void Test_Object_Wildcard()
    {
        var result = Parse.ParseInput("take cloak from table");
        Assert.Multiple(() =>
        {
            Assert.That(result.Error, Is.EqualTo(""));
            Assert.That(result.Verb, Is.EqualTo("take"));
            Assert.That(result.VerbWord, Is.EqualTo("take"));
            Assert.That(result.Noun, Is.EqualTo("cloak"));
            Assert.That(result.NounWord, Is.EqualTo("cloak"));
            Assert.That(result.Preposition, Is.EqualTo("from"));
            Assert.That(result.PrepositionWord, Is.EqualTo("from"));
            Assert.That(result.Object, Is.EqualTo("*"));
            Assert.That(result.ObjectWord, Is.EqualTo("table"));
            Assert.That(result.CommandKey, Is.EqualTo("command.take.cloak.from.*"));
        });
    }

    [Test]
    public void Test_Put_On_Cloak()
    {
        var result = Parse.ParseInput("put on cloak");
        Assert.Multiple(() =>
        {
            Assert.That(result.Error, Is.EqualTo(""));
            Assert.That(result.Verb, Is.EqualTo("put"));
            Assert.That(result.VerbWord, Is.EqualTo("put"));
            Assert.That(result.Noun, Is.EqualTo(""));
            Assert.That(result.NounWord, Is.EqualTo(""));
            Assert.That(result.Preposition, Is.EqualTo("on"));
            Assert.That(result.PrepositionWord, Is.EqualTo("on"));
            Assert.That(result.Object, Is.EqualTo("cloak"));
            Assert.That(result.ObjectWord, Is.EqualTo("cloak"));
            Assert.That(result.CommandKey, Is.EqualTo("command.put.on.cloak"));
        });
    }
}
