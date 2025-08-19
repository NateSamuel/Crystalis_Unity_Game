using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BossEnemyAI : BaseEnemyAI
{
    public GameObject teleportEffectPrefab;
    public Transform castPointFloor;

    private EnemyTeleportation teleportation;
    private Coroutine teleportRoutine;
    private bool isTeleporting = false;

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
    }

    protected override void UseAbility()
    {
        float roll = Random.Range(0f, 1f);

        if (roll < 0.3f && !isTeleporting)
        {
            animator.SetTrigger("TeleportSpell");
            teleportation.StartTeleport(true);
            isTeleporting = true;
        }
        else if (roll < 0.4f)
        {
            attackScript?.ForceFieldAbility();
        }
        else if (roll < 0.5f)
        {
            attackScript?.EnemyAOEAttack(10f);
        }
        else
        {
            base.UseAbility();
        }
    }

    public void OnTeleportComplete()
    {
        isTeleporting = false;
    }
}