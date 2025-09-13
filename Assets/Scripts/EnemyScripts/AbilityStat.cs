using UnityEngine;

[System.Serializable]
public class AbilityStat
{
    public string abilityName;

    [Header("Values")]
    public float baseChance;
    public float scaledChance;

    [Tooltip("Growth per level (0.1 = +10% per level)")]
    public float growthRate = 0.1f;

    /// <summary>
    /// True multiplicative scaling (linear).
    /// Example: base=0.2 (20%), growthRate=0.05 → Level 2=0.21, Level 3=0.22...
    /// </summary>
    public void Scale(int level)
    {
        float multiplier = 1f + (growthRate * (level - 1));
        scaledChance = baseChance * multiplier;
    }
}