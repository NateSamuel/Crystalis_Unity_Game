//Full class is student creation
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
//Specifically deals with Boss abilities
public class BossEnemyAI : BaseEnemyAI
{
    public GameObject teleportEffectPrefab;
    public Transform castPointFloor;

    private EnemyTeleportation teleportation;
    private Coroutine teleportRoutine;
    private bool isTeleporting = false;

    private EnemyHealth enemyHealthScript;
    public override void Initialize()
    {
        base.Initialize();

        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        teleportation = new EnemyTeleportation(
            transform,
            castPointFloor,
            levelHeightTexture,
            playerTransform,
            teleportEffectPrefab,
            agent,
            animator,
            this,
            GetComponent<EnemyAttack>()
        );

        teleportation.OnTeleportFinished += OnTeleportComplete;
        enemyHealthScript = GetComponent<EnemyHealth>();
    }
    //Uses random weightings for the boss for when the ability works. 
    //These weightings change depending on the enemies health making it more dynamic and more urgent the lower the health the enemy has.
    //The lower the health the higher the chance that the ranged blast is a triple spell instead of just a single spell.

    protected override void UseAbility()
    {
        float roll = Random.Range(0f, 1f);
        if (enemyHealthScript.enemyHealthCurrent < 30)
        {
            if (roll < 0.35f && !isTeleporting)
            {

                animator.SetTrigger("TeleportSpell");
                isTeleporting = true;
                teleportation.StartTeleport(true);
            }
            else if (roll < 0.45f)
            {
                attackScript?.ForceFieldAbility();
            }
            else if (roll < 0.8f)
            {
                attackScript?.EnemyAOEAttack();
            }
            else
            {
                base.UseAbility();
            }
        }
        else if (enemyHealthScript.enemyHealthCurrent < 60)
        {
            if (roll < 0.15f && !isTeleporting)
            {
                animator.SetTrigger("TeleportSpell");
                isTeleporting = true;
                teleportation.StartTeleport(true);

            }
            else if (roll < 0.3f && !isTeleporting)
            {
                animator.SetTrigger("TeleportSpell");
                isTeleporting = true;
                teleportation.StartTeleport(false);

            }
            else if (roll < 0.45f)
            {
                attackScript?.ForceFieldAbility();
            }
            else if (roll < 0.7f)
            {
                attackScript?.EnemyAOEAttack();
            }
            else
            {
                base.UseAbility();
            }
        }
        else
        {
            if (roll < 0.25f && !isTeleporting)
            {
                animator.SetTrigger("TeleportSpell");
                isTeleporting = true;
                teleportation.StartTeleport(false);

            }
            else if (roll < 0.35f)
            {
                attackScript?.ForceFieldAbility();
            }
            else if (roll < 0.5f)
            {
                attackScript?.EnemyAOEAttack();
            }
            else
            {
                base.UseAbility();
            }
        }
    }

    public void OnTeleportComplete()
    {
        isTeleporting = false;
    }
}