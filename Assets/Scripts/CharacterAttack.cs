using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    Animator animator;
    public float attackRange = 5f;
    public int damage = 10;
    public LayerMask enemyLayer;
    public Transform attackPoint;
    public GameObject spellEffectPrefab;
    public Transform castPoint;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Attack()
    {
        animator.SetTrigger("punch");

        if (attackPoint == null)
        {
            Debug.LogError("attackPoint is not assigned!");
            return;
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

        if (bestTarget != null)
        {
            EnemyHealth health = bestTarget.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.EnemyDamageTaken(damage);
                Debug.Log("Hit: " + bestTarget.name);
            }
        }
        else
        {
            Debug.Log("No valid enemy in front.");
        }
    }
    public void AOEAttack(int spellDamage)
    {
        if (spellEffectPrefab != null && castPoint != null)
        {
            GameObject effect = Instantiate(spellEffectPrefab, castPoint.position, castPoint.rotation);

            Destroy(effect, 1f);
        }

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);
        if (attackPoint == null)
        {
            Debug.LogError("attackPoint is not assigned!");
            return;
        }

        foreach (Collider enemy in hitEnemies)
        {
            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.EnemyDamageTaken(spellDamage);
            }
        }
    }
}
