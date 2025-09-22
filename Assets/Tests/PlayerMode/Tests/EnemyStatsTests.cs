using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[TestFixture]
public class EnemyStatsTests
{
    // Checks that scaling all stats at a given level updates health, damage, and ability
    [Test]
    public void ScaleStats_ScalesAllStatsCorrectly()
    {
        var stats = new EnemyStats
        {
            health = new Stat { baseValue = 100, growthRate = 0.1f },
            damageStats = new List<Stat> { new Stat { baseValue = 10, growthRate = 0.2f } },
            abilities = new List<AbilityStat> { new AbilityStat { abilityName = "Fireball", baseChance = 0.5f, growthRate = 0.1f } }
        };

        stats.ScaleStats(3);

        Assert.AreEqual(121f, stats.health.scaledValue, 0.01f);

        Assert.AreEqual(0.6f, stats.abilities[0].scaledChance, 0.01f);
    }

    // Checks that GetAbilityChance gives the right scaled chance for an ability, and 0 for a fake ability.
    [Test]
    public void GetAbilityChanceReturnsCorrectValue()
    {
        var ability = new AbilityStat { abilityName = "Ice", baseChance = 0.3f, growthRate = 0.1f };
        ability.Scale(2);

        var stats = new EnemyStats
        {
            abilities = new List<AbilityStat> { ability }
        };

        Assert.AreEqual(0.33f, stats.GetAbilityChance("Ice"), 0.01f);
        Assert.AreEqual(0f, stats.GetAbilityChance("Fire"), 0.01f); // Fake
    }

    // Checks that ResetStats resets stats back to their base value.
    [Test]
    public void ChecksResetStatsResetsBaseValues()
    {
        var health = new Stat { baseValue = 100, scaledValue = 150 };
        var damage = new Stat { baseValue = 10, scaledValue = 20 };
        var ability = new AbilityStat { abilityName = "Lightning", baseChance = 0.4f, scaledChance = 0.6f };

        var stats = new EnemyStats
        {
            health = health,
            damageStats = new List<Stat> { damage },
            abilities = new List<AbilityStat> { ability }
        };

        stats.ResetStats();

        Assert.AreEqual(100f, stats.health.scaledValue);
        Assert.AreEqual(10f, stats.damageStats[0].scaledValue);
        Assert.AreEqual(0.4f, stats.abilities[0].scaledChance);
    }
}