using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

//Design by Barbara Reichart lecture series, 2024
[Serializable]
[CreateAssetMenu(fileName = "OrDecoratorRule", menuName = "Custom/Procedural Generation/OrDecoratorRule")]
public class OrDecoratorRule : BaseDecoratorRule
{
    [SerializeField] BaseDecoratorRule[] childRules;
    internal override bool CanBeApplied(TileType[,] levelDecorated, Room room)
    {
        foreach (BaseDecoratorRule rule in childRules)
        {
            if (rule.CanBeApplied(levelDecorated, room))
            {
                return true;
            }
        }
        return false;
    }
    internal override void Apply(TileType[,] levelDecorated, Room room, Transform parent)
    {
        List<BaseDecoratorRule> applicableChildRules = new List<BaseDecoratorRule>();
        
        foreach (BaseDecoratorRule rule in childRules)
        {
            if(rule.CanBeApplied(levelDecorated, room))
            {
                applicableChildRules.Add(rule);
            }
        }
        Random random = SharedLevelData.Instance.Rand;
        int selectedRuleIndex = random.Next(0, applicableChildRules.Count);
        BaseDecoratorRule selectedRule = applicableChildRules[selectedRuleIndex];
        selectedRule.Apply(levelDecorated, room, parent);
    }
}
