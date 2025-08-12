using UnityEngine;

public class RangedBlast : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;
    public float damage = 40f;

    private Vector3 moveDirection;

    public void Launch(Vector3 direction)
    {
        moveDirection = direction;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {

        if (((1 << other.gameObject.layer) & LayerMask.GetMask("Enemy1")) != 0)
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.EnemyDamageTaken(Mathf.RoundToInt(damage));
            }

            Destroy(gameObject);
        }
    }
}