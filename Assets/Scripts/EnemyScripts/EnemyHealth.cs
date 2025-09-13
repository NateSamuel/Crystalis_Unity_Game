using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections;
using TMPro;

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

        // 🔑 Automatically assign stats based on tag
        var difficultyManager = FindAnyObjectByType<EnemyDifficultyIncrease>();
        if (difficultyManager != null)
        {
            enemyStats = difficultyManager.GetStatsForType(gameObject.tag);
            if (enemyStats != null)
            {
                // Force scale now if it hasn't been done yet
                if (enemyStats.health.scaledValue == 0)
                {
                    enemyStats.ScaleStats(1); // or whatever multiplier your level uses
                }

                InitHealthFromStats();
                UpdateHealthUIText();
            }
        }
        
    }

    // void OnEnable()
    // {
    //     StartCoroutine(InitAfterDifficultyReady());
    // }

    // private IEnumerator InitAfterDifficultyReady()
    // {
    //     // Wait until the difficulty manager exists and the stats are scaled
    //     var difficultyManager = FindObjectOfType<EnemyDifficultyIncrease>();
    //     while (difficultyManager == null || difficultyManager.GetStatsForType(gameObject.tag)?.health.scaledValue == 0)
    //         yield return null; // wait a frame

    //     enemyStats = difficultyManager.GetStatsForType(gameObject.tag);
    //     InitHealthFromStats();
    //     UpdateHealthUIText();
    // }

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

    private void UpdateHealthUIText()
    {
        if (healthText != null)
            healthText.text = $"{Mathf.CeilToInt(enemyHealthCurrent)}";
    }

    public void isHitAnimationAvailable()
    {
        if ((animator.GetCurrentAnimatorStateInfo(0).IsName("Standing React Death Forward") || animator.GetCurrentAnimatorStateInfo(0).IsName("Dying Backwards"))||
            (animator.GetCurrentAnimatorStateInfo(0).IsName("Punching") || animator.GetCurrentAnimatorStateInfo(0).IsName("Cross Punch"))||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Standing 2H Cast Spell 01") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Standing 2H Magic Attack 04"))
        {
            hitAnimationAvailable = false;
        }
        else{
            hitAnimationAvailable = true;
        }
    }

    public void ResetHealth()
    {
        InitHealthFromStats();
        UpdateHealthUIText();
    }

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
