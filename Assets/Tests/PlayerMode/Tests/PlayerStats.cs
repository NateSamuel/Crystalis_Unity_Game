using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PlayerStatsTests
{
    private PlayerStats player;

    // Creates a PlayerStats with values
    [SetUp]
    public void Setup()
    {
        player = new PlayerStats
        {
            health = new Stat { baseValue = 10, growthRate = 0.1f },
            damageStats = new List<Stat> { new Stat { baseValue = 5, growthRate = 0.2f } },
            abilities = new List<AbilityStat> { new AbilityStat { abilityName = "Fireball", baseChance = 0.2f } }
        };
    }

    // Checks that ScaleStats scales health, damage, and ability stats for a level
    [Test]
    public void ScaleStatsScalesCorrectly()
    {
        player.ScaleStats(2);

        Assert.AreEqual(10 * Mathf.Pow(1 + 0.1f, 1), player.health.scaledValue, 0.001f);
        Assert.AreEqual(5 * (1 + 0.2f * 1), player.damageStats[0].scaledValue, 0.001f);
        Assert.AreEqual(0.2f * (1 + player.abilities[0].growthRate * 1), player.abilities[0].scaledChance, 0.001f);
    }

    // Checks that GetAbilityChance returns the scaled chance for an ability, and 0 for a fake ability
    [Test]
    public void GetAbilityChanceReturnsCorrectValue()
    {
        player.abilities[0].scaledChance = 0.5f;
        Assert.AreEqual(0.5f, player.GetAbilityChance("Fireball"));
        Assert.AreEqual(0f, player.GetAbilityChance("IceBlast"));
    }

    // Tests that ResetStats resets all health, damage, and ability stats to their base values
    [Test]
    public void ResetStatsResetsAllValues()
    {
        player.health.scaledValue = 50;
        player.damageStats[0].scaledValue = 20;
        player.abilities[0].scaledChance = 0.8f;

        player.ResetStats();

        Assert.AreEqual(10, player.health.scaledValue);
        Assert.AreEqual(5, player.damageStats[0].scaledValue);
        Assert.AreEqual(0.2f, player.abilities[0].scaledChance);
    }
}