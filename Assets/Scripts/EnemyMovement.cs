using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private bool isInitialized = false;
    private bool canMove = true;

    public Transform[] patrolPoints;
    private int currentPointIndex = 0;

    public Transform playerTransform;
    public float detectionRange = 20f;
    public float attackRange = 5f;
    private bool isDead = false;

    private EnemyAttack attackScript;
    private Animator animator;

    public void Initialize()
    {
        isDead = false;
        Debug.Log($"{name}: Initialize() called!");

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        if (agent == null)
        {
            Debug.LogError($"{name}: No NavMeshAgent found.");
            return;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogError($"{name}: Agent is not on NavMesh at position {transform.position}");
            return;
        }

        agent.speed = 3.5f;
        agent.acceleration = 8f;
        agent.stoppingDistance = 3f;
        
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                var point = patrolPoints[i];
                NavMeshHit hit;
                if (NavMesh.SamplePosition(point.position, out hit, 2f, NavMesh.AllAreas))
                {
                    patrolPoints[i].position = hit.position;
                    Debug.Log($"Patrol point {i} snapped to NavMesh at {hit.position}");
                }
                else
                {
                    Debug.LogError($"Patrol point {i} is not near a NavMesh surface.");
                }
            }

            // Set first destination
            agent.SetDestination(patrolPoints[0].position);
        }
        else
        {
            Debug.LogError($"{name}: No patrol points assigned!");
        }

        isInitialized = true;
        Debug.Log($" {name} initialized and on NavMesh.");

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player not found! Make sure Player GameObject is tagged 'Player'.");
        }
        attackScript = GetComponent<EnemyAttack>();
        attackScript?.SetDead(false);
        
    }

    void Update()
    {
        if (isDead || !isInitialized || patrolPoints == null || patrolPoints.Length == 0 || agent.pathPending)
            return;

        if (!agent.hasPath || agent.remainingDistance < 0.5f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPointIndex].position);
        }

        if (playerTransform != null)
        {

            Vector3 enemyPosFlat = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            Vector3 playerPosFlat = new Vector3(playerTransform.position.x, playerTransform.position.y, playerTransform.position.z);

            float distanceToPlayer = Vector3.Distance(enemyPosFlat, playerPosFlat);

            if (distanceToPlayer <= detectionRange)
            {
                if (distanceToPlayer > agent.stoppingDistance)
                {
                    agent.isStopped = false;
                    agent.SetDestination(playerTransform.position);
                }
                else
                {
                    agent.isStopped = true;
                }

                attackScript?.AttackPlayer();

                if (distanceToPlayer <= attackRange)
                {
                    attackScript?.IsAbleToHitPlayer();
                }
                else
                {
                    attackScript?.IsNotAbleToHitPlayer();
                }
            }
            else
            {
                agent.isStopped = false;
                attackScript?.DontAttackPlayer();
                attackScript?.IsNotAbleToHitPlayer();
            }
        }
        if (animator != null)
        {
            Vector3 velocity = agent.velocity;
            velocity.y = 0f;

            float forwardSpeed = velocity.magnitude / agent.speed;
            animator.SetFloat("forwardSpeed", forwardSpeed, 0.1f, Time.deltaTime);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = canMove ? Color.green : Color.red;
        Gizmos.DrawSphere(transform.position + Vector3.up * 1f, 0.3f);

        if (patrolPoints != null)
        {
            Gizmos.color = Color.blue;
            foreach (var point in patrolPoints)
            {
                if (point != null)
                    Gizmos.DrawSphere(point.position, 0.2f);
            }
        }
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
}