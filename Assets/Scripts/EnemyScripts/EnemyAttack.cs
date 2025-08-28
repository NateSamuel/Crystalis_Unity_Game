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
    public bool facingThePlayer = false;
    public float facingAngleThreshold = 5f;
    public float hitPlayerCooldown = 2f;
    private Animator animator;
    private bool hasPunched = false;
    private CharacterHealth characterHealth;
    private CharacterAttack characterAttack;
    private bool isDead = false;
    public GameObject spellPrefab;
    public GameObject forceFieldEffectPrefab;
    public GameObject aoeSpellEffectPrefab;
    public float spellSpeed = 10f;
    public Transform spellSpawnPoint;
    public Transform castPoint;
    public float spellDamage = 30f;
    private BaseEnemyAI movement;
    private EnemyHealth enemyHealth;
    public float aoeRadius = 5f;
    private Coroutine aoeCoroutine;
    private bool isAOEActive = false;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
        toggleTimer = toggleInterval;
        animator = GetComponent<Animator>();
        characterHealth = player.GetComponent<CharacterHealth>();
        characterAttack = player.GetComponent<CharacterAttack>();
        movement = GetComponent<BaseEnemyAI>();
        enemyHealth = GetComponent<EnemyHealth>();
    }

    void Update()
    {
        if (isDead) return;
        if(isAbleToHit)
        {
            HitPlayer();
            isAbleToHit = false;
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
        if (isAbleToHit && characterHealth.characterHealthCurrent > 0 && enemyHealth.enemyHealthCurrent > 0)
        {
            animator.SetTrigger("Punch");

            if (isAbleToDamage)
            {
                characterHealth.CharacterDamageTaken(10);
            }
        }
    }

    public void SetDead(bool value)
    {
        isDead = value;
        isAbleToHit = false;
        isAttacking = false;
    }

    public void ShootSpellAtPlayer()
    {
        if(characterHealth.characterHealthCurrent > 0 && enemyHealth.enemyHealthCurrent > 0)
        {
            StartCoroutine(ShootSpellCoroutine(1f));
        }
    }

    private IEnumerator ShootSpellCoroutine(float delay)
    {
        if (movement != null)
        {
            movement.enabled = false;
        }

        yield return new WaitForSeconds(delay);

        if (spellPrefab == null || playerTransform == null || spellSpawnPoint == null)
        {
            if (movement != null) movement.enabled = true;
            yield break;
        }

        GameObject spell = Instantiate(spellPrefab, spellSpawnPoint.position, Quaternion.identity);

        Vector3 direction = (playerTransform.position - spellSpawnPoint.position).normalized;
        spell.transform.forward = direction;

        RangedBlast rangedBlast = spell.GetComponent<RangedBlast>();
        if (rangedBlast != null)
        {
            rangedBlast.Launch(direction, RangedBlast.CasterType.Enemy, spellDamage, isAbleToDamage);
        }
        if (movement != null)
        {
            movement.enabled = true;
        }
    }

    public void ShootTripleSpellAtPlayer()
    {
        if(characterHealth.characterHealthCurrent > 0 && enemyHealth.enemyHealthCurrent > 0)
        {
            StartCoroutine(ShootTripleSpellCoroutine(1f));
        }
    }

    private IEnumerator ShootTripleSpellCoroutine(float delay)
    {
        if (movement != null)
        {
            movement.enabled = false;
        }

        yield return new WaitForSeconds(delay);

        if (spellPrefab == null || playerTransform == null || spellSpawnPoint == null)
        {
            if (movement != null) movement.enabled = true;
            yield break;
        }

        Vector3 direction = (playerTransform.position - spellSpawnPoint.position).normalized;

        FireSpell(direction);

        FireSpell(RotateDirection(direction, 30f));
        FireSpell(RotateDirection(direction, -30f));

        if (movement != null)
        {
            movement.enabled = true;
        }
    }

    private void FireSpell(Vector3 direction)
    {
        GameObject spell = Instantiate(spellPrefab, spellSpawnPoint.position, Quaternion.LookRotation(direction));
        RangedBlast rangedBlast = spell.GetComponent<RangedBlast>();

        if (rangedBlast != null)
        {
            rangedBlast.Launch(direction, RangedBlast.CasterType.Enemy, spellDamage, isAbleToDamage);
        }
    }

    private Vector3 RotateDirection(Vector3 direction, float angleDegrees)
    {
        Quaternion rotation = Quaternion.Euler(0, angleDegrees, 0);
        return rotation * direction;
    }

    public void ForceFieldAbility()
    {

        if (forceFieldEffectPrefab != null && castPoint != null && characterHealth.characterHealthCurrent > 0 && enemyHealth.enemyHealthCurrent > 0)
        {
            animator.SetTrigger("TeleportSpell");
            GameObject effect = Instantiate(forceFieldEffectPrefab, castPoint.position, castPoint.rotation);
            Destroy(effect, 1f);
        }
        StartCoroutine(ForceFieldCoroutine(3f));
    }
    private IEnumerator ForceFieldCoroutine(float duration)
    {

        if (characterAttack != null)
        {
            characterAttack.IsNotAbleToDamageEnemy();
        }

        yield return new WaitForSeconds(duration);

        characterAttack.IsAbleToDamageEnemy();
    }

    public void EnemyAOEAttack(float spellDamage)
    {
        if (!isAOEActive && aoeSpellEffectPrefab != null && spellSpawnPoint != null && characterHealth.characterHealthCurrent > 0 && enemyHealth.enemyHealthCurrent > 0)
        {
            animator.SetTrigger("TeleportSpell");
            GameObject effect = Instantiate(aoeSpellEffectPrefab, spellSpawnPoint.position, spellSpawnPoint.rotation);
            effect.transform.localScale *= 2f;

            Destroy(effect, 6f);

            isAOEActive = true;
            aoeCoroutine = StartCoroutine(AOEDamageRoutine(spellSpawnPoint.position, spellDamage));
        }
    }

    private IEnumerator AOEDamageRoutine(Vector3 center, float spellDamage)
    {
        int ticks = 6;
        float tickRate = 1f;

        if (movement != null)
            movement.enabled = false;

        for (int i = 0; i < ticks; i++)
        {
            // Exit early if dead or deactivated
            if (enemyHealth.enemyHealthCurrent <= 0 || !gameObject.activeInHierarchy)
                break;

            if (playerTransform != null)
            {
                float distance = Vector3.Distance(center, playerTransform.position);
                if (distance <= aoeRadius)
                {
                    CharacterHealth playerHealth = playerTransform.GetComponent<CharacterHealth>();
                    if (playerHealth != null && isAbleToDamage && characterHealth.characterHealthCurrent > 0)
                    {
                        playerHealth.CharacterDamageTaken(Mathf.RoundToInt(spellDamage));
                    }
                }
            }

            yield return new WaitForSeconds(tickRate);
        }

        if (movement != null)
            movement.enabled = true;

        isAOEActive = false;
    }
    public void StopAOEAttack()
    {
        if (aoeCoroutine != null)
        {
            StopCoroutine(aoeCoroutine);
            aoeCoroutine = null;
        }

        if (movement != null)
            movement.enabled = true;

        isAOEActive = false;
    }
}
