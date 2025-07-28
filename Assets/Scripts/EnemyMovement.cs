// using UnityEngine;
// using UnityEngine.AI;

// public class EnemyMovement : MonoBehaviour
// {
//     public Vector2Int[] patrolGridPoints;
//     private int currentPatrolIndex = 0;

//     public float moveSpeed = 2f;
//     public Transform playerTransform;
//     public float detectionRange = 20f;
//     public float attackRange = 5f;

//     private EnemyAttack attackScript;
//     private NavMeshAgent agent;

//     void Start()
//     {
//         agent = GetComponent<NavMeshAgent>();

//         if (agent != null)
//         {
//             agent.speed = moveSpeed;

//             // Only set patrol destination don't warp or move enemy now
//             if (patrolGridPoints != null && patrolGridPoints.Length > 0)
//             {
//                 Vector3 firstPatrolPos = GridToWorldPosition(patrolGridPoints[0]);
//                 agent.SetDestination(SnapToNavMesh(firstPatrolPos));
//             }
//         }

//         attackScript = GetComponent<EnemyAttack>();
//         if (attackScript == null)
//             Debug.LogWarning("EnemyAttack script not found!");

//         GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
//         playerTransform = playerObject != null ? playerObject.transform : null;

//         if (playerTransform == null)
//             Debug.LogWarning("Player not found! Make sure Player GameObject is tagged 'Player'.");
//     }

//     void Update()
//     {
//         if (agent == null || !agent.isOnNavMesh)
//             return;

//         if (playerTransform != null)
//         {
//             float distanceToPlayer = Vector3.Distance(
//                 new Vector3(transform.position.x, 0, transform.position.z),
//                 new Vector3(playerTransform.position.x, 0, playerTransform.position.z)
//             );

//             if (distanceToPlayer <= detectionRange)
//             {
//                 agent.SetDestination(playerTransform.position);
//                 attackScript?.AttackPlayer();

//                 if (distanceToPlayer <= attackRange)
//                     attackScript?.IsAbleToHitPlayer();
//                 else
//                     attackScript?.IsNotAbleToHitPlayer();

//                 return;
//             }
//             else
//             {
//                 attackScript?.DontAttackPlayer();
//                 attackScript?.IsNotAbleToHitPlayer();
//             }
//         }

//         if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
//         {
//             currentPatrolIndex = (currentPatrolIndex + 1) % patrolGridPoints.Length;
//             Vector3 nextPatrolPos = GridToWorldPosition(patrolGridPoints[currentPatrolIndex]);
//             agent.SetDestination(SnapToNavMesh(nextPatrolPos));
//         }
//     }

//     Vector3 GridToWorldPosition(Vector2Int gridPos)
//     {
//         int scale = SharedLevelData.Instance.Scale;

//         // Approximate height if you want perfect accuracy, pass in the level height texture or centralize the logic.
//         float yHeight = transform.position.y; // Keep Y constant for patrol
//         return new Vector3(gridPos.x * scale, yHeight, gridPos.y * scale);
//     }

//     Vector3 SnapToNavMesh(Vector3 position)
//     {
//         if (NavMesh.SamplePosition(position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
//             return hit.position;

//         return position;
//     }
// }
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private bool isInitialized = false;
    private bool canMove = true;

    public Transform[] patrolPoints;
    private int currentPointIndex = 0;

    public void Initialize()
    {
        
        Debug.Log($"{name}: 🔁 Initialize() called!");
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(agent.transform.position + transform.forward * 5f);
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
        Debug.Log($"{name}: isOnNavMesh = {agent.isOnNavMesh}, enabled = {agent.enabled}");

        agent.speed = 3.5f;
        agent.acceleration = 8f;

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[currentPointIndex].position);
        }

        isInitialized = true;
        Debug.Log($"✅ {name} initialized and on NavMesh.");
    }

    void Update()
    {
        if (!isInitialized || patrolPoints == null || patrolPoints.Length == 0 || agent.pathPending)
            return;

        if (!agent.hasPath || agent.remainingDistance < 0.5f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPointIndex].position);
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
}