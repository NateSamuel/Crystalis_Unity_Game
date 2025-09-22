//Full class is student creation
using UnityEngine;
using System.Collections.Generic;
//class to store info about level ups for abilities like original and current multipliers  how much they increase by
[System.Serializable]
public class LevelUpAbilities
{
    public string name;
    public float originalStatAmount;
    public float currentStatAmount;
    public float baseMultiplier = 0.1f;
    public int levelUpIncreaseMultiplier = 1;

}
