//Full class is student creation
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections;
using TMPro;

//deals with the enemies health, death, and damage taken
public class EnemyHealth : MonoBehaviour
{
    public float enemyHealthCurrent;
    public TMP_Text healthText;

    public EnemyTrackerForObjectives tracker;
    private CharacterTreasure charTreasureScript;
    public Slider healthSlider;
    private Transform playerTransform;
    private Animator animator;
    private bool hitAnimationAvailable = true;
    protected EnemyAttack attackScript;

    [SerializeField] public EnemyStats enemyStats;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        attackScript = GetComponent<EnemyAttack>();

        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            charTreasureScript = playerTransform.GetComponent<CharacterTreasure>();
        }

        var difficultyManager = FindAnyObjectByType<EnemyDifficultyIncrease>();
        if (difficultyManager != null)
        {
            enemyStats = difficultyManager.GetStatsForType(gameObject.tag);
            if (enemyStats != null)
            {
                if (enemyStats.health.scaledValue == 0)
                {
                    enemyStats.ScaleStats(1);
                }

                InitHealthFromStats();
                UpdateHealthUIText();
            }
        }
    }
    //Initiates enemy health from the EnemyStats
    private void InitHealthFromStats()
    {
        if (enemyStats == null) return;
        enemyHealthCurrent = enemyStats.health.scaledValue;
        if (healthSlider != null)
        {
            healthSlider.maxValue = enemyHealthCurrent;
            healthSlider.value = enemyHealthCurrent;
        }
    }
    //Updates health text on enemy prefab
    private void UpdateHealthUIText()
    {
        if (healthText != null)
            healthText.text = $"{Mathf.CeilToInt(enemyHealthCurrent)}";
    }

    //Enemy being hit animation is available if enemy is not using another animation first
    public void isHitAnimationAvailable()
    {
        if ((animator.GetCurrentAnimatorStateInfo(0).IsName("Standing React Death Forward") || animator.GetCurrentAnimatorStateInfo(0).IsName("Dying Backwards")) ||
            (animator.GetCurrentAnimatorStateInfo(0).IsName("Punching") || animator.GetCurrentAnimatorStateInfo(0).IsName("Cross Punch")) ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Standing 2H Cast Spell 01") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Standing 2H Magic Attack 04"))
        {
            hitAnimationAvailable = false;
        }
        else
        {
            hitAnimationAvailable = true;
        }
    }

    //Resets health on death
    public void ResetHealth()
    {
        InitHealthFromStats();
        UpdateHealthUIText();
    }
    //When player hits enemy, this updates the enemy health based on damage taken
    public void EnemyDamageTaken(float damageAmount)
    {
        if (enemyHealthCurrent > damageAmount)
        {
            if (hitAnimationAvailable)
            {
                animator.SetTrigger("HitReaction");
            }
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
        if (healthText != null)
        {
            healthText.text = $"{Mathf.CeilToInt(enemyHealthCurrent)}";
        }

    }
    //Enemy death routine starts with animations and then MoveEnemyAfterDeath is called. Player gets crsystals following enemy death as reward
    void EnemyDie()
    {
        attackScript?.StopAOEAttack();
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Standing React Death Forward") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Dying Backwards"))
        {
            GetComponent<EnemyAttack>()?.SetDead(true);
            GetComponent<BaseEnemyAI>()?.DisableEnemy();
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
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
        {
            yield return null;
        }

        MoveEnemyAfterDeath();
        charTreasureScript?.ApplyTreasure(6);
    }
    //Enemy is moved off screen, health updated and disable (i.e. reset) after death
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

        InitHealthFromStats();
        UpdateHealthUIText();

        animator.Rebind();
        animator.Update(0f);
        gameObject.SetActive(false);
        tracker?.SetEnemyActive(gameObject, false);
    }
    
}
