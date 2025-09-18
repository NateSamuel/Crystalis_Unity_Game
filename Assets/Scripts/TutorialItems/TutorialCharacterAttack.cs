using System;
using UnityEngine;
using System.Collections;

/// This deals with the attack abilities for the player character, including melee, ranged,
/// AoE, power-ups, and special effects like freeze or force field.
public class TutorialCharacterAttack : MonoBehaviour
{
    public static event Action OnPunchHit;
    public static event Action OnAOEHit;
    public static event Action OnPowerUpWithPunch;
    public static event Action OnFreezeWithRangedBlast;
    public static event Action OnCrystalStab;

    Animator animator;
    public float attackRange = 5f;
    public LayerMask enemyLayer;
    public Transform attackPoint;
    public GameObject spellEffectPrefab;
    public GameObject powerUpEffectPrefab;
    public GameObject freezeEffectPrefab;
    public GameObject forceFieldEffectPrefab;
    public GameObject projectilePrefab;
    public Transform castPoint;
    public Transform castPointFloor;
    private Coroutine activePowerup;
    private Coroutine activeDamageOverTime;
    private float rangedBlastDamage;
    public float normalDamageModifier = 1f;
    public float increasedDamageModifier = 1.5f;
    private float currentDamageModifier;
    private bool isTargeting = false;
    private bool isAbleToDamage = true;
    private bool rangedBlastAvailable = false;

    public CharacterLevelUps levelUp;
    
    // Initializes the Animator component and sets the default damage modifier.
    void Awake()
    {
        animator = GetComponent<Animator>();
        currentDamageModifier = normalDamageModifier;

    }

    void Update()
    {
        if (isTargeting && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Vector3 direction = ray.direction;
            direction.y = 0f;
            direction.Normalize();

            if (direction != Vector3.zero)
            {
                FireProjectile(direction);
                animator.SetTrigger("rangedBlastTrigger");
            }

            isTargeting = false;
        }
    }

    public void IsAbleToDamageEnemy()
    {
        isAbleToDamage = true;
    }

    public void IsNotAbleToDamageEnemy()
    {
        isAbleToDamage = false;
    }
    private void FireProjectile(Vector3 direction)
    {
        Vector3 spawnPosition = transform.position + Vector3.up;
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.LookRotation(direction));
        RangedBlast magic = projectile.GetComponent<RangedBlast>();

        if (magic != null)
        {
            magic.Launch(direction, RangedBlast.CasterType.Player, rangedBlastDamage, isAbleToDamage);
            if (rangedBlastAvailable)
            {
                OnFreezeWithRangedBlast?.Invoke();
            }
        }
    }
    public void RangedBlastAttack(float spellDamage)
    {
        rangedBlastDamage = spellDamage;
        isTargeting = true;
    }

    public void Attack(float hitAmount)
    {

        animator.SetTrigger("punch");

        Transform bestTarget = GetBestEnemyTarget();

        if (bestTarget != null)
        {
            EnemyHealth health = bestTarget.GetComponent<EnemyHealth>();

            if (health != null && isAbleToDamage)
            {
                health.EnemyDamageTaken(Mathf.RoundToInt(hitAmount * currentDamageModifier));
                OnPunchHit?.Invoke();
                if (currentDamageModifier == increasedDamageModifier)
                {
                    OnPowerUpWithPunch?.Invoke();
                }
            }

        }
    }
    public void AOEAttack(float spellDamage)
    {
        animator.SetTrigger("aoeAttackTrigger");
        if (spellEffectPrefab != null && castPoint != null)
        {
            GameObject effect = Instantiate(spellEffectPrefab, castPoint.position, castPoint.rotation);

            Destroy(effect, 1f);
        }

        if (attackPoint == null)
        {
            return;
        }

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health != null && isAbleToDamage == true)
            {
                health.EnemyDamageTaken(Mathf.RoundToInt(spellDamage * currentDamageModifier));
                OnAOEHit?.Invoke();
            }
        }
    }

    public void PowerUpAbility(float strengthIncrease)
    {
        if (activePowerup != null)
        {
            StopCoroutine(activePowerup);
        }
        activePowerup = StartCoroutine(PowerUpRoutine(strengthIncrease));

    }

    private IEnumerator PowerUpRoutine(float strengthIncrease)
    {
        if (powerUpEffectPrefab != null && castPoint != null)
        {
            GameObject effect = Instantiate(powerUpEffectPrefab, castPoint.position, castPoint.rotation);
            Destroy(effect, 1f);
        }
        currentDamageModifier = strengthIncrease;

        yield return new WaitForSeconds(5f);

        currentDamageModifier = normalDamageModifier;
    }

    public void FreezeAbility(float freezeLengthModified)
    {
        animator.SetTrigger("freezeTrigger");
        if (freezeEffectPrefab != null && castPointFloor != null)
        {
            GameObject effect = Instantiate(freezeEffectPrefab, castPointFloor.position, castPointFloor.rotation);
            Destroy(effect, 1f);
        }
        Transform bestTarget = GetBestEnemyTarget();

        if (bestTarget != null)
        {
            TutorialBaseEnemyAI enemy = bestTarget.GetComponent<TutorialBaseEnemyAI>();
            if (enemy != null)
            {
                enemy.Stun(freezeLengthModified);
            }
        }
        StartCoroutine(FreezeCooldown(freezeLengthModified));
    }

    private IEnumerator FreezeCooldown(float freezeLengthModified)
    {
        rangedBlastAvailable = true;

        yield return new WaitForSeconds(freezeLengthModified);

        rangedBlastAvailable = false;
    }

    public void ForceFieldAbility(float fieldFieldLengthModified)
    {
        animator.SetTrigger("aoeAttackTrigger");
        if (forceFieldEffectPrefab != null && castPointFloor != null)
        {
            GameObject effect = Instantiate(forceFieldEffectPrefab, castPoint.position, castPoint.rotation);
            Destroy(effect, fieldFieldLengthModified);
        }
        Transform bestTarget = GetBestEnemyTarget();

        if (bestTarget != null)
        {
            TutorialBaseEnemyAI enemy = bestTarget.GetComponent<TutorialBaseEnemyAI>();
            if (enemy != null)
            {
                enemy.BlockedByForceField(fieldFieldLengthModified);
            }
        }
    }
    public void CrystalStabAttack(float spellDamage)
    {
        animator.SetTrigger("punch");

        Transform bestTarget = GetBestEnemyTarget();

        if (activeDamageOverTime != null)
        {
            StopCoroutine(activeDamageOverTime);
        }
        activeDamageOverTime = StartCoroutine(CrystalStabAttackRoutine(bestTarget, spellDamage));
    }

    private IEnumerator CrystalStabAttackRoutine(Transform bestTarget, float spellDamage)
    {
        if (bestTarget != null)
        {
            OnCrystalStab?.Invoke();
            EnemyHealth health = bestTarget.GetComponent<EnemyHealth>();
            if (health != null && isAbleToDamage == true)
            {
                int ticks = 5;
                for (int i = 0; i < ticks; i++)
                {
                    health.EnemyDamageTaken(Mathf.RoundToInt(spellDamage * currentDamageModifier));
                    yield return new WaitForSeconds(1f);
                }
            }
        }

        activeDamageOverTime = null;
    }


    private Transform GetBestEnemyTarget()
    {
        if (attackPoint == null)
        {
            return null;
        }

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);

        Transform bestTarget = null;
        float closestScore = float.MaxValue;

        foreach (Collider enemyCollider in hitEnemies)
        {
            Transform enemy = enemyCollider.transform;

            Vector3 toEnemy = (enemy.position - transform.position).normalized;
            float frontFactor = Vector3.Dot(transform.forward, toEnemy);

            if (frontFactor > 0.3f)
            {
                float distance = Vector3.Distance(transform.position, enemy.position);
                float score = distance - frontFactor * 2f;

                if (score < closestScore)
                {
                    closestScore = score;
                    bestTarget = enemy;
                }
            }
        }
        return bestTarget;
    }

}
