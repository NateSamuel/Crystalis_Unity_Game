using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    public Transform playerTransform;
    public float rotationSpeed = 0.5f;
    private bool isAttacking = false;
    private bool isAbleToHit = false;
    public float toggleInterval = 1f;
    private float toggleTimer;
    private bool isRotating = true;
    public bool facingThePlayer = false;
    public float facingAngleThreshold = 5f;
    public float hitPlayerCooldown = 2f;
    private Animator animator;
    private bool hasPunched = false;
    private CharacterHealth characterHealth;
    
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
        toggleTimer = toggleInterval;
        animator = GetComponent<Animator>();
        characterHealth = player.GetComponent<CharacterHealth>();
        
    }

    
    void Update()
    {
        
        if(isAttacking)
        {
            toggleTimer -= Time.deltaTime;
            if (toggleTimer <= 0f)
            {
                isRotating = !isRotating;
                toggleTimer = toggleInterval;
            }

            if (isRotating && playerTransform != null)
            {
                Vector3 direction = playerTransform.position - transform.position;
                direction.y = 0f;

                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                    float angle = Quaternion.Angle(transform.rotation, targetRotation);
                    facingThePlayer = angle < facingAngleThreshold;
                }
            }
            else{

                facingThePlayer = false;
            }
            if(facingThePlayer)
            {
                HitPlayer();
            }
        }

    }
    public void AttackPlayer()
    {
        isAttacking = true;
    }
    public void DontAttackPlayer()
    {
        isAttacking = false;
    }

    public void IsAbleToHitPlayer()
    {
        isAbleToHit = true;
    }
    public void IsNotAbleToHitPlayer()
    {
        isAbleToHit = false;
    }
    public void HitPlayer()
    {
        if(!hasPunched && isAbleToHit)
        {
            animator.SetTrigger("Punch");
            hasPunched = true;
            StartCoroutine(ResetPunchCooldown());
            characterHealth.CharacterDamageTaken(10);
        }
    }

    IEnumerator ResetPunchCooldown()
    {
        yield return new WaitForSeconds(2f);
        hasPunched = false;
    }
}
