using GRIFTools;
using static GRIFTools.GrodEnums;

namespace TestGRIFTools;

public class UnitTestGrod
{
    [Test]
    public void Test_NullKey()
    {
        Grod g = new()
        {
            UseOverlay = false
        };
        try
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            g.Set(null, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Fail();
        }
        catch (Exception)
        {
            Assert.Pass();
        }
    }

    [Test]
    public void Test_EmptyKey()
    {
        Grod g = new()
        {
            UseOverlay = false
        };
        try
        {
            g.Set("", "empty");
            Assert.Fail();
        }
        catch (Exception)
        {
            Assert.Pass();
        }
    }

    [Test]
    public void Test_WhitespaceKey()
    {
        Grod g = new()
        {
            UseOverlay = false
        };
        try
        {
            g.Set("   \t\r\n  ", "whitespace");
            Assert.Fail();
        }
        catch (Exception)
        {
            Assert.Pass();
        }
    }

    [Test]
    public void Test_NotFound()
    {
        Grod g = new()
        {
            UseOverlay = false
        };
        Assert.That(g.Get("k"), Is.EqualTo(""));
    }

    [Test]
    public void Test_SingleValue()
    {
        Grod g = new()
        {
            UseOverlay = false
        };
        g.Set("k", "v");
        Assert.That(g.Get("k"), Is.EqualTo("v"));
    }

    [Test]
    public void Test_Add()
    {
        Grod g = new()
        {
            UseOverlay = false
        };
        g.Set("k", "v");
        Assert.That(g.Get("k"), Is.EqualTo("v"));
    }

    [Test]
    public void Test_AddTwice()
    {
        Grod g = new()
        {
            UseOverlay = false
        };
        g.Set("k", "v");
        g.Set("k", "vvv");
        Assert.That(g.Get("k"), Is.EqualTo("vvv"));
    }

    [Test]
    public void Test_NullValue()
    {
        Grod g = new()
        {
            UseOverlay = false
        };
        g.Set("k", null);
        Assert.That(g.Get("k"), Is.EqualTo(""));
    }

    [Test]
    public void Test_CaseInsensitiveKeys()
    {
        Grod g = new()
        {
            UseOverlay = false
        };
        g.Set("k", "v");
        Assert.That(g.Get("K"), Is.EqualTo("v"));
    }

    [Test]
    public void Test_TrimmedKeysSet()
    {
        Grod g = new()
        {
            UseOverlay = false
        };
        g.Set("    k   ", "v");
        Assert.That(g.Get("k"), Is.EqualTo("v"));
    }

    [Test]
    public void Test_TrimmedKeysGet()
    {
        Grod g = new()
        {
            UseOverlay = false
        };
        g.Set("k", "v");
        Assert.That(g.Get("   k   "), Is.EqualTo("v"));
    }

    [Test]
    public void Test_OverlayGetOverlayBaseThru()
    {
        Grod g = new()
        {
            UseOverlay = false
        };
        g.Set("k", "v");
        g.UseOverlay = true;
        Assert.That(g.Get("k"), Is.EqualTo("v"));
    }

    [Test]
    public void Test_OverlayGetOverlayValue()
    {
        Grod g = new()
        {
            UseOverlay = false
        };
        g.Set("k", "v");
        g.UseOverlay = true;
        g.Set("k", "value");
        Assert.That(g.Get("k"), Is.EqualTo("value"));
    }

    [Test]
    public void Test_OverlayGetOverlayBackToBase()
    {
        Grod g = new()
        {
            UseOverlay = false
        };
        g.Set("k", "v");
        g.UseOverlay = true;
        g.Set("k", "value");
        g.UseOverlay = false;
        Assert.That(g.Get("k"), Is.EqualTo("v"));
    }

    [Test]
    public void Test_GetKeys()
    {
        Grod g = new()
        {
            UseOverlay = false
        };
        g.Set("a", "1");
        g.Set("b", "2");
        g.Set("c", "3");
        var answer = "";
        foreach (string s in g.Keys())
        {
            answer += s;
            answer += g.Get(s);
        }
        // test separately, order is not guaranteed
        Assert.That(answer.Contains("a1") && answer.Contains("b2") && answer.Contains("c3"), Is.True);
    }

    [Test]
    public void Test_GetKeysBaseAndOverlay()
    {
        Grod g = new()
        {
            UseOverlay = false
        };
        g.Set("a", "1");
        g.Set("b", "2");
        g.UseOverlay = true;
        g.Set("c", "3");
        var answer = "";
        foreach (string s in g.Keys())
        {
            answer += s;
            answer += g.Get(s);
        }
        // test separately, order is not guaranteed
        Assert.That(answer.Contains("a1") && answer.Contains("b2") && answer.Contains("c3"), Is.True);
    }

    [Test]
    public void Test_RemoveKey()
    {
        Grod g = new()
        {
            UseOverlay = false
        };
        g.Set("a", "1");
        g.Set("b", "2");
        g.Set("c", "3");
        g.Remove("b");
        var answer = "";
        foreach (string s in g.Keys())
        {
            answer += s;
            answer += g.Get(s);
        }
        // test separately, order is not guaranteed
        Assert.That(answer.Contains("a1") && !answer.Contains("b2") && answer.Contains("c3"), Is.True);
    }

    [Test]
    public void Test_ContainsKey()
    {
        Grod g = new()
        {
            UseOverlay = false
        };
        g.Set("a", "1");
        g.Set("b", "2");
        g.Set("c", "3");
        Assert.That(g.ContainsKey("a") && g.ContainsKey("b") && g.ContainsKey("c"), Is.True);
    }

    [Test]
    public void Test_ContainsKeyAfterRemove()
    {
        Grod g = new()
        {
            UseOverlay = false
        };
        g.Set("a", "1");
        g.Set("b", "2");
        g.Set("c", "3");
        g.Remove("b");
        Assert.That(g.ContainsKey("a") && !g.ContainsKey("b") && g.ContainsKey("c"), Is.True);
    }

    [Test]
    public void Test_ContainsKeyAny()
    {
        Grod g = new()
        {
            UseOverlay = false
        };
        g.Set("a", "1");
        g.UseOverlay = true;
        g.Set("b", "2");
        Assert.That(g.ContainsKey("a") && g.ContainsKey("b"), Is.True);
    }

    [Test]
    public void Test_MissingKeyNotFound()
    {
        Grod g = new()
        {
            UseOverlay = false
        };
        Assert.That(g.ContainsKey("a"), Is.False);
    }

    [Test]
    public void Test_OverlayContainsKeyInBase()
    {
        Grod g = new()
        {
            UseOverlay = false
        };
        g.Set("a", "1");
        g.UseOverlay = true;
        Assert.That(g.ContainsKey("a"), Is.True);
    }

    [Test]
    public void Test_ModifyValue()
    {
        Grod g = new()
        {
            UseOverlay = false
        };
        g.Set("a", "1");
        g.Set("a", "123456");
        var answer = g.Get("a");
        Assert.That(answer, Is.EqualTo("123456"));
    }

    [Test]
    public void Test_Clear()
    {
        Grod g = new()
        {
            UseOverlay = false
        };
        g.Set("a", "1");
        g.Set("b", "2");
        g.Set("c", "3");
        g.Clear();
        var answer = g.Count();
        Assert.That(answer, Is.Zero);
    }

    [Test]
    public void Test_ClearOnlyOverlay()
    {
        Grod g = new()
        {
            UseOverlay = false
        };
        g.Set("a", "1");
        g.Set("b", "2");
        g.Set("c", "3");
        g.UseOverlay = true;
        g.Set("aaa", "111");
        g.Set("bbb", "222");
        g.Set("ccc", "333");
        var answerBeforeClear = g.Count();
        g.Clear(WhichData.Overlay);
        var answerAfterClearOverlay = g.Count();
        g.UseOverlay = false;
        var answerAfterClearBase = g.Count();
        Assert.That(answerBeforeClear == 6 && answerAfterClearOverlay == 3 & answerAfterClearBase == 3, Is.True);
    }

    [Test]
    public void Test_ClearOnlyBase()
    {
        Grod g = new()
        {
            UseOverlay = false
        };
        g.Set("a", "1");
        g.Set("b", "2");
        g.Set("c", "3");
        g.UseOverlay = true;
        g.Set("aaa", "111");
        g.Set("bbb", "222");
        g.Set("ccc", "333");
        var answerBeforeClear = g.Count();
        g.UseOverlay = false;
        g.Clear(WhichData.Base);
        var answerAfterClearBase = g.Count();
        g.UseOverlay = true;
        var answerAfterClearOverlay = g.Count();
        Assert.That(answerBeforeClear == 6 && answerAfterClearBase == 0 && answerAfterClearOverlay == 3, Is.True);
    }

    [Test]
    public void Test_Keys()
    {
        Grod g = new()
        {
            UseOverlay = false
        };
        g.Set("a", "1");
        g.Set("b", "2");
        g.Set("c", "3");
        var keys = g.Keys();
        Assert.That(keys.Contains("a") && keys.Contains("b") && keys.Contains("c"), Is.True);
    }

    [Test]
    public void Test_KeysOverlay()
    {
        Grod g = new()
        {
            UseOverlay = false
        };
        g.Set("a", "1");
        g.Set("b", "2");
        g.Set("c", "3");
        g.UseOverlay = true;
        g.Set("bbb", "222");
        var keys = g.Keys(WhichData.Overlay);
        Assert.That(!keys.Contains("a") && !keys.Contains("b") && !keys.Contains("c") && keys.Contains("bbb"), Is.True);
    }
}
