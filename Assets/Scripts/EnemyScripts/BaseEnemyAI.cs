//Full class is student creation
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

//Deals with main enemy AI info such as movement and if forcefields and stuns are happening
public class BaseEnemyAI : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected Animator animator;
    protected EnemyAttack attackScript;
    protected Transform playerTransform;
    private bool isInitialized = false;
    protected bool isDead = false;
    protected bool canMove = true;
    protected float globalAbilityCooldown = 2f;
    protected float lastAbilityTime = -Mathf.Infinity;

    public float detectionRange = 20f;
    public float attackRange = 5f;
    public Vector3[] patrolPoints;
    protected int currentPatrolIndex;

    [SerializeField] protected Texture2D levelHeightTexture;
    protected Color[] validRoomColors;
    protected int levelTextureScale;

    //Initialises enemy movement and animator items.
    public virtual void Initialize()
    {
        levelTextureScale = SharedLevelData.Instance.Scale;
        isDead = false;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        attackScript = GetComponent<EnemyAttack>();
        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
            animator.ResetTrigger("HitReaction");
            animator.ResetTrigger("Punch");
            animator.ResetTrigger("TeleportSpell");
            animator.ResetTrigger("DeathAnimTrigger");
            animator.ResetTrigger("RangedSpell");
            animator.SetFloat("forwardSpeed", 0f);
        }
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        playerTransform = playerObj?.transform;

        canMove = true;

        validRoomColors = new Color[] {
            LayoutColorMap.RoomLevel(0),
            LayoutColorMap.RoomLevel(1),
            LayoutColorMap.RoomLevel(2)
        };

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;

            if (levelHeightTexture != null)
            {
                patrolPoints = LevelUtility.AssignPatrolPointsFromLevelTexture(levelHeightTexture, validRoomColors, levelTextureScale, patrolPoints.Length);
                if (patrolPoints.Length > 0)
                    agent.SetDestination(patrolPoints[0]);
            }
        }
        isInitialized = true;
        attackScript?.SetDead(false);
    }

    protected virtual void Update()
    {
        if (isDead || !isInitialized || patrolPoints == null || patrolPoints.Length == 0 || agent.pathPending || !canMove)
            return;

        PatrolLogic();
        PlayerChaseLogic();
        UpdateAnimation();
    }

    //Walks around the different points chosen using Navmesh aent SetDestination to move from each point
    protected void PatrolLogic()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolIndex]);
            agent.isStopped = false;
        }
    }

    //Chases the player if within detection range using navmesh. If within attack range, UseABility is called
    protected void PlayerChaseLogic()
    {
        if (playerTransform == null)
        {
            agent.isStopped = false;
            attackScript?.IsNotAbleToHitPlayer();
            return;
        }

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= detectionRange)
        {
            if (distance > attackRange)
            {
                agent.isStopped = false;
                agent.SetDestination(playerTransform.position);
                attackScript?.IsNotAbleToHitPlayer();
            }
            else
            {
                agent.isStopped = true;

                if (Time.time - lastAbilityTime >= globalAbilityCooldown)
                {
                    UseAbility();
                    lastAbilityTime = Time.time;
                }
            }
        }
        else
        {
            agent.isStopped = false;
            attackScript?.IsNotAbleToHitPlayer();
        }
    }
    //When ability is able to be used attack script is called
    protected virtual void UseAbility()
    {
        attackScript?.IsAbleToHitPlayer();
    }

    //Update animation for general movement
    protected void UpdateAnimation()
    {
        if (animator == null || agent == null || !agent.isOnNavMesh) return;
        Vector3 vel = agent.velocity; vel.y = 0;
        float forwardSpeed = vel.magnitude / agent.speed;
        animator.SetFloat("forwardSpeed", forwardSpeed, 0.1f, Time.deltaTime);
    }

    //If agent is stunned with the freeze from the player, it can no longer move for a specfic duration
    public void Stun(float duration)
    {
        if (isDead) return;
        StartCoroutine(StunCoroutine(duration));
    }

    IEnumerator StunCoroutine(float duration)
    {
        canMove = false;
        agent.isStopped = true;
        animator?.SetTrigger("Stunned");
        yield return new WaitForSeconds(duration);
        canMove = true;
        agent.isStopped = false;
    }

    //If enemy dies then sets isDead and isStopped to true and resets path
    public void DisableEnemy()
    {
        isDead = true;
        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    //If player has forcefield up, enemy cannot damage them for a specific period of time.
    public void BlockedByForceField(float duration)
    {
        if (isDead) return;
        StartCoroutine(ForceFieldCoroutine(duration));
    }

    private IEnumerator ForceFieldCoroutine(float duration)
    {
        if (attackScript != null)
        {
            attackScript.IsNotAbleToDamagePlayer();
        }

        yield return new WaitForSeconds(duration);

        attackScript.IsAbleToDamagePlayer();
    }
}