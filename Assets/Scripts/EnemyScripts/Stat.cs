//Full class is student creation
using UnityEngine;

[System.Serializable]
//Stat class for enemy attacks
public class Stat
{
    public string statName;

    [Header("Values")]
    public float baseValue;
    public float scaledValue;

    [Tooltip("Growth per level (0.1 = +10% per level)")]
    public float growthRate = 0.1f;

    //Scales based on level
    public void Scale(int level)
    {
        float multiplier = 1f + (growthRate * (level - 1));
        scaledValue = baseValue * multiplier;
    }
}