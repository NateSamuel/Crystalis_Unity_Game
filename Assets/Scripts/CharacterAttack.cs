using UnityEngine;
using System.Collections;

public class CharacterAttack : MonoBehaviour
{
    Animator animator;
    public float attackRange = 5f;
    public float damage = 10f;
    public float dotDamage = 5f;
    public LayerMask enemyLayer;
    public Transform attackPoint;
    public GameObject spellEffectPrefab;
    public GameObject powerUpEffectPrefab;
    public GameObject freezeEffectPrefab;
    public Transform castPoint;
    public Transform castPointFloor;
    private Coroutine activePowerup;
    private Coroutine activeDamageOverTime;
    public float normalDamageModifier = 1f;
    public float increasedDamageModifier = 1.5f;
    private float currentDamageModifier;
    public GameObject projectilePrefab;
    private bool isTargeting = false;

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

    private void FireProjectile(Vector3 direction)
    {
        Vector3 spawnPosition = transform.position + Vector3.up;
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.LookRotation(direction));
        RangedBlast magic = projectile.GetComponent<RangedBlast>();

        if (magic != null)
        {
            magic.Launch(direction);
        }
    }
    public void RangedBlastAttack(float spellDamage)
    {
        isTargeting = true;
    }

    public void Attack()
    {
        animator.SetTrigger("punch");

        Transform bestTarget = GetBestEnemyTarget();

        if (bestTarget != null)
        {
            EnemyHealth health = bestTarget.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.EnemyDamageTaken(Mathf.RoundToInt(damage * currentDamageModifier));
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
            if (health != null)
            {
                health.EnemyDamageTaken(Mathf.RoundToInt(spellDamage * currentDamageModifier));
            }
        }
    }

    public void PowerUpAbility()
    {
        if (activePowerup != null)
        {
            StopCoroutine(activePowerup);
        }
        activePowerup = StartCoroutine(PowerUpRoutine());
    }

    private IEnumerator PowerUpRoutine()
    {
        if (powerUpEffectPrefab != null && castPoint != null)
        {
            GameObject effect = Instantiate(powerUpEffectPrefab, castPoint.position, castPoint.rotation);
            Destroy(effect, 1f);
        }
        currentDamageModifier = increasedDamageModifier;

        yield return new WaitForSeconds(5f);

        currentDamageModifier = normalDamageModifier;
    }

    public void FreezeAbility()
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
            EnemyMovement enemy = bestTarget.GetComponent<EnemyMovement>();
            if (enemy != null)
            {
                enemy.Stun(5f);
            }
        }
    }

    public void DodgeAbility()
    {
        Transform bestTarget = GetBestEnemyTarget();

        if (bestTarget != null)
        {
            EnemyMovement enemy = bestTarget.GetComponent<EnemyMovement>();
            if (enemy != null)
            {
                enemy.CharacterDodges(3f);
            }
        }
    }
    public void CrystalStabAttack() 
    {
        animator.SetTrigger("punch");

        Transform bestTarget = GetBestEnemyTarget();

        if (activeDamageOverTime != null)
        {
            StopCoroutine(activeDamageOverTime);
        }
        activeDamageOverTime = StartCoroutine(CrystalStabAttackRoutine(bestTarget));
    }

    private IEnumerator CrystalStabAttackRoutine(Transform bestTarget)
    {
        if (bestTarget != null)
        {
            EnemyHealth health = bestTarget.GetComponent<EnemyHealth>();
            if (health != null)
            {
                int ticks = 5;
                for (int i = 0; i < ticks; i++)
                {
                    health.EnemyDamageTaken(Mathf.RoundToInt(dotDamage * currentDamageModifier));
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
