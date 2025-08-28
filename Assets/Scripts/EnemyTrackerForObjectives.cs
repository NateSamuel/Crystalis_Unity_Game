using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class EnemyTrackerForObjectives : MonoBehaviour
{
    public TextMeshProUGUI enemyStatusText;

    private List<GameObject> enemies = new List<GameObject>();
    private List<GameObject> bosses = new List<GameObject>();
    public int activeEnemies;
    public int activeBosses;
    private TotalPrisoners totalPrisoners;
    void Start()
    {
        totalPrisoners = FindAnyObjectByType<TotalPrisoners>();
    }

    public void RegisterEnemy(GameObject enemy)
    {
        if (!enemies.Contains(enemy))
            enemies.Add(enemy);

        UpdateStatusText();
    }

    public void RegisterBoss(GameObject boss)
    {
        if (!bosses.Contains(boss))
            bosses.Add(boss);

        UpdateStatusText();
    }

    public void SetEnemyActive(GameObject enemy, bool isActive)
    {
        UpdateStatusText();
    }

    public void SetBossActive(GameObject boss, bool isActive)
    {
        UpdateStatusText();
    }

    public void UpdateStatusText()
    {
        activeEnemies = enemies.FindAll(e => e != null && e.activeSelf).Count;
        int inactiveEnemies = enemies.Count - activeEnemies;

        activeBosses = bosses.FindAll(b => b != null && b.activeSelf).Count;
        int inactiveBosses = bosses.Count - activeBosses;

        enemyStatusText.text = $"Roughians left to kill: {activeEnemies}\n" +
                               $"Captains left to kill: {activeBosses}\n" +
                               $"Prisoners left to free: {totalPrisoners.prisonersCount}";
    }
}