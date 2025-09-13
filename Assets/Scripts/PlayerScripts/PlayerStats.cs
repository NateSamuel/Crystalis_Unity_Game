using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlayerStats
{
    public string enemyType;

    [Header("Core Stats")]
    public Stat health;

    [Header("Damage Stats")]
    public List<Stat> damageStats = new List<Stat>();

    [Header("Abilities")]
    public List<AbilityStat> abilities = new List<AbilityStat>();

    // Scale stats per level
    public void ScaleStats(int level)
    {
        // Health uses exponential scaling
        health.scaledValue = health.baseValue * Mathf.Pow(1f + health.growthRate, level - 1);

        // Damage + Abilities use linear multiplicative scaling
        foreach (var stat in damageStats)
            stat.Scale(level);

        foreach (var ability in abilities)
            ability.Scale(level);
    }

    public float GetAbilityChance(string abilityName)
    {
        var ability = abilities.Find(a => a.abilityName == abilityName);
        return ability != null ? ability.scaledChance : 0f;
    }

    // Optional reset
    public void ResetStats()
    {
        health.scaledValue = health.baseValue;
        foreach (var stat in damageStats)
            stat.scaledValue = stat.baseValue;
        foreach (var ability in abilities)
            ability.scaledChance = ability.baseChance;
    }
}