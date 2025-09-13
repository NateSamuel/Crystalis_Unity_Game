using UnityEngine;

[System.Serializable]
public class Stat
{
    public string statName;

    [Header("Values")]
    public float baseValue;
    public float scaledValue;

    [Tooltip("Growth per level (0.1 = +10% per level)")]
    public float growthRate = 0.1f;

    /// <summary>
    /// True multiplicative scaling (linear).
    /// Example: base=10, growthRate=0.2 → Level 1=10, Level 2=12, Level 3=14...
    /// </summary>
    public void Scale(int level)
    {
        float multiplier = 1f + (growthRate * (level - 1));
        scaledValue = baseValue * multiplier;
    }
}