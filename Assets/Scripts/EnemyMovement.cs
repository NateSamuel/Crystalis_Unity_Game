using UnityEngine;
using UnityEngine.AI;
using System.Collections;

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

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        if (agent == null)
        {
            return;
        }

        if (!agent.isOnNavMesh)
        {
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
                }
            }

            // Set first destination
            agent.SetDestination(patrolPoints[0].position);
        }

        isInitialized = true;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        attackScript = GetComponent<EnemyAttack>();
        attackScript?.SetDead(false);
        
    }

    void Update()
    {
        if (isDead || !isInitialized || patrolPoints == null || patrolPoints.Length == 0 || agent.pathPending || !canMove)
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
    public void Stun(float duration)
    {
        if (isDead) return;
        StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        canMove = false;
        agent.isStopped = true;

        if (attackScript != null)
        {
            attackScript.DontAttackPlayer();
            attackScript.IsNotAbleToHitPlayer();
        }

        if (animator != null)
            animator.SetTrigger("Stunned");

        yield return new WaitForSeconds(duration);

        canMove = true;
        agent.isStopped = false;
    }
    public void CharacterDodges(float duration)
    {
        if (isDead) return;
        StartCoroutine(DodgeCoroutine(duration));
    }

    private IEnumerator DodgeCoroutine(float duration)
    {

        if (attackScript != null)
        {
            attackScript.IsNotAbleToDamagePlayer();
        }

        yield return new WaitForSeconds(duration);

        attackScript.IsAbleToDamagePlayer();
    }
}