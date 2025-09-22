//Full class is student creation
using UnityEngine;
using System.Collections.Generic;
//Stats for an enemy, so it would have health, and a list of damage stats, and ability stats
[System.Serializable]
public class EnemyStats
{
    public string enemyType;

    [Header("Core Stats")]
    public Stat health;

    [Header("Damage Stats")]
    public List<Stat> damageStats = new List<Stat>();

    [Header("Abilities")]
    public List<AbilityStat> abilities = new List<AbilityStat>();

    //these are scaled based on the level number
    public void ScaleStats(int level)
    {
        health.scaledValue = health.baseValue * Mathf.Pow(1f + health.growthRate, level - 1);

        foreach (var stat in damageStats)
            stat.Scale(level);

        foreach (var ability in abilities)
            ability.Scale(level);
    }

    //gets the chance that the ability will be used
    public float GetAbilityChance(string abilityName)
    {
        var ability = abilities.Find(a => a.abilityName == abilityName);
        return ability != null ? ability.scaledChance : 0f;
    }

    //stats are able to be reset i.e. if the player retrys level
    public void ResetStats()
    {
        health.scaledValue = health.baseValue;
        foreach (var stat in damageStats)
            stat.scaledValue = stat.baseValue;
        foreach (var ability in abilities)
            ability.scaledChance = ability.baseChance;
    }
}
