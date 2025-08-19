using UnityEngine;
using UnityEngine.AI;
using System.Collections;

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

    public virtual void Initialize()
    {
        levelTextureScale = SharedLevelData.Instance.Scale;
        isDead = false;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        attackScript = GetComponent<EnemyAttack>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        playerTransform = playerObj?.transform;

        canMove = true;

        validRoomColors = new Color[] {
            LayoutColorMap.RoomLevel(0),
            LayoutColorMap.RoomLevel(1),
            LayoutColorMap.RoomLevel(2)
        };
        if (levelHeightTexture == null)
        {
            Debug.LogError($"{name} - levelHeightTexture is NULL");
        }
        Debug.Log($"{name} - levelTextureScale: {levelTextureScale}");
        Debug.Log($"agent == null? {agent == null}");
        Debug.Log($"patrolPoints == null? {patrolPoints == null}");
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;

            if (levelHeightTexture != null)
            {
                patrolPoints = LevelUtility.AssignPatrolPointsFromLevelTexture(levelHeightTexture, validRoomColors, levelTextureScale, patrolPoints.Length);
                Debug.Log($"{name} patrol points assigned: {patrolPoints?.Length}");
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

    protected void PlayerChaseLogic()
    {
        if (playerTransform == null)
        {
            agent.isStopped = false;
            attackScript?.DontAttackPlayer();
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
                attackScript?.DontAttackPlayer();
                attackScript?.IsNotAbleToHitPlayer();
            }
            else
            {
                agent.isStopped = true;
                attackScript?.AttackPlayer();

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
            attackScript?.DontAttackPlayer();
            attackScript?.IsNotAbleToHitPlayer();
        }
    }

    protected virtual void UseAbility()
    {
        attackScript?.IsAbleToHitPlayer();
    }

    protected void UpdateAnimation()
    {
        if (animator == null || agent == null || !agent.isOnNavMesh) return;
        Vector3 vel = agent.velocity; vel.y = 0;
        float forwardSpeed = vel.magnitude / agent.speed;
        animator.SetFloat("forwardSpeed", forwardSpeed, 0.1f, Time.deltaTime);
    }

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

    public void DisableEnemy()
    {
        isDead = true;
        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

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