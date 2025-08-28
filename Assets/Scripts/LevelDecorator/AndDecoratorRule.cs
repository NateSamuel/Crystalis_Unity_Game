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
    //Goes through all child rules and if it’s valid, applies it, while skipping or logging warnings/errors for invalid, null, or failing rules
    //student creation
    internal override void Apply(TileType[,] levelDecorated, Room room, Transform parent)
    {
        foreach (var rule in childRules)
        {
            if (rule == null)
            {
                Debug.LogWarning("[AndDecoratorRule] Encountered null child rule — skipping.");
                continue;
            }

            if (!rule.CanBeApplied(levelDecorated, room))
            {
                Debug.LogWarning($"[AndDecoratorRule] Rule {rule.name} cannot be applied in room: — skipping.");
                continue;
            }

            try
            {
                rule.Apply(levelDecorated, room, parent);
            }
            catch (Exception e)
            {
                Debug.LogError($"[AndDecoratorRule] Error applying rule {rule.name} in room: {e.Message}");
            }
        }
    }
}
