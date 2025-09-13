using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LevelUpAbilities
{
    public string name;
    public float originalStatAmount;
    public float currentStatAmount;
    public float baseMultiplier = 0.1f;
    public int levelUpIncreaseMultiplier = 1;

}
