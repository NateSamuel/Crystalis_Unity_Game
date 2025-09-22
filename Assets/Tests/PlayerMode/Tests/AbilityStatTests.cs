using NUnit.Framework;
using UnityEngine;

public class AbilityStatTests
{
    AbilityStat ability;

    // creates new AbilityStat with values
    [SetUp]
    public void Setup()
    {
        ability = new AbilityStat
        {
            abilityName = "TestAbility",
            baseChance = 0.5f,
            growthRate = 0.2f
        };
    }

    // Checks that scaling at level 1 keeps the chance equal to the base chance.
    [Test]
    public void Scale_Level1ScaledChanceEqualsBaseChance()
    {
        ability.Scale(1);
        Assert.AreEqual(0.5f, ability.scaledChance);
    }

    // Checks that scaling at level 2 increases the chance with the growth rate.
    [Test]
    public void Scale_Level2ScaledChanceIncreased()
    {
        ability.Scale(2);
        Assert.AreEqual(0.6f, ability.scaledChance);
    }

    // Checks that scaling at level 5 creates the correct scaled chance
    [Test]
    public void Scale_Level5ScaledChanceCorrect()
    {
        ability.Scale(5);
        Assert.AreEqual(0.9f, ability.scaledChance);
    }
}