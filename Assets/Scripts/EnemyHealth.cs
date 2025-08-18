using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public float enemyHealthTotal = 100f;
    private float enemyHealthCurrent;
    public EnemyTrackerForObjectives tracker;
    private CharacterTreasure charTreasureScript;
    public Slider healthSlider;
    private Transform playerTransform;
    private Animator animator;
    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
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
        if (enemyHealthCurrent > damageAmount)
        {
            animator.SetTrigger("HitReaction");
            enemyHealthCurrent -= damageAmount;
        }
        else
        {
            enemyHealthCurrent = 0f;
            EnemyDie();
        }

        if (healthSlider != null)
        {
            healthSlider.value = enemyHealthCurrent;
        }

    }
    void EnemyDie()
    {

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Standing React Death Forward") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Dying Backwards"))
        {
            GetComponent<EnemyAttack>()?.SetDead(true);
            GetComponent<EnemyMovement>()?.DisableEnemy();
            animator.SetTrigger("DeathAnimTrigger");
            StartCoroutine(WaitForDeathAnimationThenContinue());
        }

    }

    IEnumerator WaitForDeathAnimationThenContinue()
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Standing React Death Forward") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Dying Backwards"))
        {
            yield return null;
        }
        
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.7f)
        {
            yield return null;
        }

        MoveEnemyAfterDeath();
        charTreasureScript?.ApplyTreasure(6);
    }

    void MoveEnemyAfterDeath()
    {
        
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
        gameObject.SetActive(false);
        tracker?.SetEnemyActive(gameObject, false);
    }
}
