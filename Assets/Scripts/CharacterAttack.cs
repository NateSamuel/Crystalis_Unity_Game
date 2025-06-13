using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    
    public float attackRange = 2f;
    public int damage = 10;
    public LayerMask enemyLayer;
    public Transform attackPoint;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }
    }

    void Attack()
    {
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
                health.EnemyDamageTaken(damage);
            }
        }
    }
}
