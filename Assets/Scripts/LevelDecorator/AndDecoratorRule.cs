using System;
using UnityEngine;

//Design by Barbara Reichart lecture series, 2024 and student alterations added

[Serializable]
[CreateAssetMenu(fileName = "AndDecoratorRule", menuName = "Custom/Procedural Generation/AndDecoratorRule")]
public class AndDecoratorRule : BaseDecoratorRule
{
    [SerializeField] BaseDecoratorRule[] childRules;
    // Checks if all child decorator rules can be applied to the room.
    // Returns false when one rule is not applicable and true only if all are valid.
    internal override bool CanBeApplied(TileType[,] levelDecorated, Room room)
    {
        foreach (BaseDecoratorRule rule in childRules)
        {
            if (!rule.CanBeApplied(levelDecorated, room))
            {
                return false;
            }
        }
        return true;
    }
    //Goes through all child rules and if it’s valid, applies it
    //student creation
    internal override void Apply(TileType[,] levelDecorated, Room room, Transform parent)
    {
        // If any child rule cannot be applied, return
        foreach (var rule in childRules)
        {
            if (rule == null)
            {
                return;
            }
            if (!rule.CanBeApplied(levelDecorated, room))
            {
                return;
            }
        }
        // All child rules are valid — apply them
        foreach (var rule in childRules)
        {
            try
            {
                rule.Apply(levelDecorated, room, parent);
            }
            catch (Exception e)
            {
                Debug.LogError($"[AndDecoratorRule] Error applying rule {rule.name}: {e.Message}");
            }
        }
    }
}