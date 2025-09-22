//Full class is student creation
//Minimal updates from base class
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class TutorialBossEnemyAI : TutorialBaseEnemyAI
{
    public GameObject teleportEffectPrefab;
    public Transform castPointFloor;

    private TutorialEnemyTeleportation teleportation;
    private Coroutine teleportRoutine;
    private bool isTeleporting = false;

    public override void Initialize()
    {
        base.Initialize();

        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        teleportation = new TutorialEnemyTeleportation(
            transform,
            castPointFloor,
            levelHeightTexture,
            playerTransform,
            teleportEffectPrefab,
            agent,
            animator,
            this,
            GetComponent<TutorialEnemyAttack>()
        );

        teleportation.OnTeleportFinished += OnTeleportComplete;
    }
    //Uses random weightings for the boss for when the ability works
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
            attackScript?.EnemyAOEAttack();
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