using UnityEngine;

public class RangedBlast : MonoBehaviour
{
    public enum CasterType { Player, Enemy }

    public float speed = 1f;
    public float lifeTime = 3f;
    public float damage = 40f;
    public CasterType caster;
    bool isAbleToHit = true;

    private Vector3 moveDirection;

    public void Launch(Vector3 direction, CasterType casterType, float damageAmount, bool isAbleToDamage)
    {
        moveDirection = direction;
        caster = casterType;
        damage = damageAmount;
        isAbleToHit = isAbleToDamage;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (caster == CasterType.Player)
        {
            if (other.CompareTag("Enemy1") || other.CompareTag("Boss1"))
            {
                EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
                if (enemyHealth != null && isAbleToHit == true)
                {
                    enemyHealth.EnemyDamageTaken(Mathf.RoundToInt(damage));
                }
                Destroy(gameObject);
            }
        }
        else if (caster == CasterType.Enemy)
        {
            if (other.CompareTag("Player"))
            {
                CharacterHealth characterHealth = other.GetComponent<CharacterHealth>();
                if (characterHealth != null)
                {
                    characterHealth.CharacterDamageTaken(Mathf.RoundToInt(damage));
                }
                Destroy(gameObject);
            }
        }
    }
}
