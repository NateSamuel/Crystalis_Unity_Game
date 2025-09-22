using NUnit.Framework;
using UnityEngine;

public class StatTests
{
    // Checks that scaling a stat at level 1 results in the base value
    [Test]
    public void ScaleLevel1EqualsBaseValue()
    {
        var stat = new Stat { baseValue = 10f, growthRate = 0.1f };
        stat.Scale(1);
        Assert.AreEqual(10f, stat.scaledValue, 0.0001f);
    }

    // Checks that scaling a stat at level 5 increases the value based on growth rate
    [Test]
    public void ScaleLevel5IncreasesCorrectly()
    {
        var stat = new Stat { baseValue = 10f, growthRate = 0.1f };
        stat.Scale(5);
        Assert.AreEqual(14f, stat.scaledValue, 0.0001f);
    }

    // Checks that a stat with zero growth rate will always returns the base value of level
    [Test]
    public void ScaleZeroGrowthRateAlwaysBaseValue()
    {
        var stat = new Stat { baseValue = 20f, growthRate = 0f };
        stat.Scale(10);
        Assert.AreEqual(20f, stat.scaledValue, 0.0001f);
    }

    // Checks that a stat with '-' growth rate goes down when scaled
    [Test]
    public void ScaleNegativeGrowthRateDecreasesValue()
    {
        var stat = new Stat { baseValue = 10f, growthRate = -0.1f };
        stat.Scale(3);
        Assert.AreEqual(8f, stat.scaledValue, 0.0001f);
    }

}