//Full class is student creation
using UnityEngine;
//stats for enemy abilities. These increase the higher the level is.
[System.Serializable]
public class AbilityStat
{
    public string abilityName;

    [Header("Values")]
    public float baseChance;
    public float scaledChance;

    [Tooltip("Growth per level (0.1 = +10% per level)")]
    public float growthRate = 0.1f;

    //scales the ability up based on the level
    public void Scale(int level)
    {
        float multiplier = 1f + (growthRate * (level - 1));
        scaledChance = baseChance * multiplier;
    }
}