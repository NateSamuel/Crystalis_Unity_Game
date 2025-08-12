using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    public float enemyHealthTotal = 100f;
    private float enemyHealthCurrent;
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

        enemyHealthCurrent = enemyHealthTotal;
        if (healthSlider != null)
        {
            healthSlider.maxValue = enemyHealthTotal;
            healthSlider.value = enemyHealthCurrent;
        }
    }

    public void EnemyDamageTaken(float damageAmount)
    {
        enemyHealthCurrent -= damageAmount;
        if (healthSlider != null)
        {
            healthSlider.value = enemyHealthCurrent;
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
