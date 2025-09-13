// using UnityEngine;

// public class EnemyDifficultyIncrease : MonoBehaviour
// {

//     private float originalPunchDamage = 15f;
//     private float originalSpellDamage = 20f;
//     private float originalTripleSpellDamage = 20f;
//     private float originalForceFieldLength = 2f;
//     private float originalAOESpellDamage = 8f;

//     private float originalBossEnemyHealthTotal = 150f;
//     private float originalRuffianHealthTotal = 100f;

//     private float originalTotalBossAvailableAbilityPercentage = 0.5f;
//     private float originalBossAbilityOnePercentage = 0.3f;
//     private float originalBossAbilityTwoPercentage = 0.4f;
//     private float originalBossAbilityThreePercentage = 0.5f;

//     private float originalRuffianAbilityOnePercentage = 0.1f;

//     public float modifiedPunchDamage = 15f;
//     public float modifiedSpellDamage = 20f;
//     public float modifiedTripleSpellDamage = 20f;
//     public float modifiedForceFieldLength = 2f;
//     public float modifiedAOESpellDamage = 8f;

//     public float modifiedBossEnemyHealthTotal = 150f;
//     public float modifiedRuffianHealthTotal = 100f;

//     private float modifiedTotalBossAvailableAbilityPercentage = 0.5f;
//     public float modifiedBossAbilityOnePercentage = 0.3f;
//     public float modifiedBossAbilityTwoPercentage = 0.4f;
//     public float modifiedBossAbilityThreePercentage = 0.5f;

//     public float modifiedRuffianAbilityOnePercentage = 0.1f;


//     public void UpdateLevelDifficultyInfo(int currentLevel)
//     {
//         modifiedPunchDamage = originalPunchDamage + (originalPunchDamage * (0.1f * (currentLevel-1)));
//         modifiedSpellDamage = originalSpellDamage + (originalSpellDamage * (0.1f * (currentLevel-1)));
//         modifiedTripleSpellDamage = originalTripleSpellDamage + (originalTripleSpellDamage * (0.1f * (currentLevel-1)));
//         modifiedForceFieldLength = originalForceFieldLength + (originalForceFieldLength * (0.1f * (currentLevel-1)));
//         modifiedAOESpellDamage = originalAOESpellDamage + (originalAOESpellDamage * (0.1f * (currentLevel-1)));

//         modifiedBossEnemyHealthTotal = originalBossEnemyHealthTotal + (originalBossEnemyHealthTotal * (0.1f * (currentLevel-1)));
//         modifiedRuffianHealthTotal = originalRuffianHealthTotal + (originalRuffianHealthTotal * (0.1f * (currentLevel-1)));

//         if(modifiedTotalBossAvailableAbilityPercentage < 1f)
//         {
//             modifiedTotalBossAvailableAbilityPercentage = originalTotalBossAvailableAbilityPercentage + (originalTotalBossAvailableAbilityPercentage * (0.05f * (currentLevel-1)));
//             modifiedBossAbilityOnePercentage = originalBossAbilityOnePercentage + (originalBossAbilityOnePercentage * (0.01f * (currentLevel-1)));
//             modifiedBossAbilityTwoPercentage = originalBossAbilityTwoPercentage + (originalBossAbilityTwoPercentage * (0.02f * (currentLevel-1)));
//             modifiedBossAbilityThreePercentage = originalBossAbilityThreePercentage + (originalBossAbilityThreePercentage * (0.05f * (currentLevel-1)));
//         }

//         if(modifiedRuffianAbilityOnePercentage < 1f)
//         {
//             modifiedRuffianAbilityOnePercentage = originalRuffianAbilityOnePercentage + (originalRuffianAbilityOnePercentage * (0.05f * (currentLevel-1)));
//         }
//     }
// }

using UnityEngine;
using System.Collections.Generic;

public class EnemyDifficultyIncrease : MonoBehaviour
{
    [Header("Enemy Types")]
    [SerializeField] private List<EnemyStats> enemies = new List<EnemyStats>();
    private Dictionary<string, EnemyStats> enemyLookup;

    void Awake()
    {
        enemyLookup = new Dictionary<string, EnemyStats>();
        foreach (var e in enemies)
        {
            if (!enemyLookup.ContainsKey(e.enemyType))
                enemyLookup.Add(e.enemyType, e);
        }
    }

    public EnemyStats GetStatsForType(string type)
    {
        return enemyLookup.ContainsKey(type) ? enemyLookup[type] : null;
    }

    public void UpdateLevelDifficultyInfo(int currentLevel)
    {
        foreach (var enemy in enemies)
        {
            // Always reset before scaling
            enemy.ResetStats();

            // Pass the integer level directly
            enemy.ScaleStats(currentLevel);
        }
    }
}