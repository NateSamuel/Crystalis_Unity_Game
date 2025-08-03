using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    public int enemyHealthTotal = 100;
    private int enemyHealthCurrent;
    private CharacterTreasure charTreasureScript;
    public Slider healthSlider;
    private Transform playerTransform;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            charTreasureScript = playerTransform.GetComponent<CharacterTreasure>();
        }
        else
        {
            Debug.LogWarning("Player not found! Make sure Player GameObject is tagged 'Player'.");
        }

        enemyHealthCurrent = enemyHealthTotal;
        if (healthSlider != null)
        {
            healthSlider.maxValue = enemyHealthTotal;
            healthSlider.value = enemyHealthCurrent;
        }
    }

    public void EnemyDamageTaken(int damageAmount)
    {
        enemyHealthCurrent -= damageAmount;
        if (healthSlider != null)
        {
            healthSlider.value = enemyHealthCurrent;
            Debug.Log("Enemy health" + enemyHealthCurrent);
        }

        if (enemyHealthCurrent < 0)
        {
            EnemyDie();
        }
    }
    void EnemyDie()
    {
        GetComponent<EnemyMovement>()?.DisableEnemy();
        GetComponent<EnemyAttack>()?.SetDead(true);

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.ResetPath();
            agent.isStopped = true;
            agent.enabled = false;
        }

        transform.position = new Vector3(0, -1000, 0);
        enemyHealthCurrent = enemyHealthTotal;

        if (healthSlider != null)
        {
            healthSlider.value = enemyHealthCurrent;
        }
        charTreasureScript?.ApplyTreasure(2);
    }
}
