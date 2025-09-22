//Full class is student creation
using UnityEngine;
using System.Collections.Generic;

// Stores a list of enemy stat templates
// Creates a dictionary lookup for enemyType
// Has methods to find stats for the specific enemy type, and scales the enemy stats based on the level.
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
            enemy.ResetStats();

            enemy.ScaleStats(currentLevel);
        }
    }
}