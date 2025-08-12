using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    public Transform playerTransform;
    public float rotationSpeed = 0.5f;
    private bool isAttacking = false;
    private bool isAbleToHit = false;
    private bool isAbleToDamage = true;
    public float toggleInterval = 1f;
    private float toggleTimer;
    private bool isRotating = true;
    public bool facingThePlayer = false;
    public float facingAngleThreshold = 5f;
    public float hitPlayerCooldown = 2f;
    private Animator animator;
    private bool hasPunched = false;
    private CharacterHealth characterHealth;
    private bool isDead = false;

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
        if (isDead) return;
        if(isAbleToHit)
        {
            HitPlayer();
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
    public void IsAbleToDamagePlayer()
    {
        isAbleToDamage = true;
    }
    public void IsNotAbleToDamagePlayer()
    {
        isAbleToDamage = false;
    }
    public void HitPlayer()
    {
        if(!hasPunched && isAbleToHit)
        {
            animator.SetTrigger("Punch");
            hasPunched = true;
            StartCoroutine(ResetPunchCooldown());
            if(isAbleToDamage)
            {
                characterHealth.CharacterDamageTaken(10);
            }
        }
    }

    IEnumerator ResetPunchCooldown()
    {
        yield return new WaitForSeconds(2f);
        hasPunched = false;
    }
    public void SetDead(bool value)
    {
        isDead = value;
        isAbleToHit = false;
        isAttacking = false;
    }
}
