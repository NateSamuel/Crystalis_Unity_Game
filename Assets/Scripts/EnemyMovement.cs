using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private bool isInitialized = false;
    private bool canMove = true;

    public Vector3[] patrolPoints;
    private int currentPointIndex = 0;

    public Transform playerTransform;
    public float detectionRange = 20f;
    public float attackRange = 5f;
    private bool isDead = false;
    private Coroutine teleportRoutine;
    private bool isTeleporting = false;
    private EnemyAttack attackScript;
    private Animator animator;
    public Transform castPointFloor;
    public Texture2D levelHeightTexture;
    private Color[] validRoomColors;
    public GameObject teleportEffectPrefab;
    private float globalAbilityCooldown = 2f;
    private float lastAbilityTime = -Mathf.Infinity;

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

        validRoomColors = new Color[] {
            LayoutColorMap.RoomLevel(0),
            LayoutColorMap.RoomLevel(1),
            LayoutColorMap.RoomLevel(2)
        };

        AssignPatrolPointsFromLevelTexture();

        agent.speed = 3.5f;
        agent.acceleration = 8f;
        agent.stoppingDistance = 3f;

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[0]);
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

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPointIndex]);
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
                    if (Time.time - lastAbilityTime >= globalAbilityCooldown)
                    {
                        TryRandomAbility();
                        lastAbilityTime = Time.time;
                    }
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
                if (isTeleporting)
                {
                    isTeleporting = false;
                }
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

    private void TryRandomAbility()
    {
        float roll = Random.Range(0f, 1f);

        if (roll < 0.2f)
        {
            animator.SetTrigger("TeleportSpell");

            if (teleportEffectPrefab != null && castPointFloor != null)
            {
                GameObject effect = Instantiate(teleportEffectPrefab, castPointFloor.position, castPointFloor.rotation);
                Destroy(effect, 1f);
            }

            StartCoroutine(TeleportRoutineAfterAnimation());
            isTeleporting = true;
        }
        else if (roll >= 0.2f && roll < 0.3f)
        {
            attackScript?.ForceFieldAbility();
        }
        else if (roll >= 0.3f && roll < 0.4f)
        {
            attackScript?.EnemyAOEAttack(10f);
        }
        else
        {
            attackScript?.IsAbleToHitPlayer();
        }
    }

    void AssignPatrolPointsFromLevelTexture()
    {
        if (patrolPoints == null || patrolPoints.Length == 0 || levelHeightTexture == null)
        {
            return;
        }
        patrolPoints = new Vector3[4]; 
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            Vector2 randomPixel = PickRandomValidPixelOnTexture();
            Vector3 worldPos = LevelPositionToWorldPosition(randomPixel);

            if (NavMesh.SamplePosition(worldPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                patrolPoints[i] = hit.position;
            }
            else
            {
                patrolPoints[i] = worldPos;
            }
        }
    }

    Vector2 PickRandomValidPixelOnTexture()
    {
        int width = levelHeightTexture.width;
        int height = levelHeightTexture.height;

        int maxAttempts = 1000;
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            int x = Random.Range(1, width - 1);
            int y = Random.Range(1, height - 1);

            Color centerColor = levelHeightTexture.GetPixel(x, y);
            if (!IsValidRoomColor(centerColor)) continue;

            bool allNeighborsValid = true;
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;

                    Color neighborColor = levelHeightTexture.GetPixel(x + dx, y + dy);
                    if (!IsValidRoomColor(neighborColor))
                    {
                        allNeighborsValid = false;
                        break;
                    }
                }
                if (!allNeighborsValid) break;
            }

            if (allNeighborsValid)
            {
                return new Vector2(x, y);
            }
        }

        return new Vector2(0, 0);
    }

    bool IsValidRoomColor(Color color)
    {
        foreach (var validColor in validRoomColors)
        {
            if (ColorsApproximatelyEqual(color, validColor))
                return true;
        }
        return false;
    }

    bool ColorsApproximatelyEqual(Color a, Color b, float tolerance = 0.05f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance;
    }

    Vector3 LevelPositionToWorldPosition(Vector2 pixelPos)
    {
        int scale = SharedLevelData.Instance.Scale;

        int texX = Mathf.Clamp((int)pixelPos.x, 0, levelHeightTexture.width - 1);
        int texY = Mathf.Clamp((int)pixelPos.y, 0, levelHeightTexture.height - 1);

        Color pixel = levelHeightTexture.GetPixel(texX, texY);

        float yHeight = 0f;

        if (ColorsApproximatelyEqual(pixel, LayoutColorMap.RoomLevel(1)))
            yHeight = 12f;
        else if (ColorsApproximatelyEqual(pixel, LayoutColorMap.RoomLevel(2)))
            yHeight = 24f;

        float worldX = (pixelPos.x - 1) * scale;
        float worldZ = (pixelPos.y - 1) * scale;

        return new Vector3(worldX, yHeight, worldZ);
    }

    bool SurroundedByBlackPixels(int x, int y, int radius)
    {
        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                int nx = x + dx;
                int ny = y + dy;

                if (nx >= 0 && nx < levelHeightTexture.width &&
                    ny >= 0 && ny < levelHeightTexture.height)
                {
                    Color c = levelHeightTexture.GetPixel(nx, ny);
                    if (!ColorsApproximatelyEqual(c, Color.black))
                        return false;
                }
            }
        }
        return true;
    }

    Vector2 WorldPositionToTexturePixel(Vector3 worldPos)
    {
        int scale = SharedLevelData.Instance.Scale;
        float x = worldPos.x / scale + 1;
        float y = worldPos.z / scale + 1;

        return new Vector2(x, y);
    }

    private IEnumerator TeleportRoutineAfterAnimation()
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Standing 2H Cast Spell 01"))
        {
            yield return null;
        }
        
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.25f)
        {
            yield return null;
        }

        if (!isDead && isInitialized)
        {
            TeleportEnemyWithinSameRoom();
        }
    }

    public void TeleportEnemyWithinSameRoom()
    {
        Vector3 currentPos = transform.position;
        Vector2 currentPixel = WorldPositionToTexturePixel(currentPos);

        Color currentRoomColor = levelHeightTexture.GetPixel((int)currentPixel.x, (int)currentPixel.y);

        int attempts = 1000;
        for (int i = 0; i < attempts; i++)
        {
            int x = Random.Range(0, levelHeightTexture.width);
            int y = Random.Range(0, levelHeightTexture.height);

            Color candidateColor = levelHeightTexture.GetPixel(x, y);

            if (ColorsApproximatelyEqual(candidateColor, currentRoomColor))
            {
                if (!SurroundedByBlackPixels(x, y, 2))
                {
                    Vector3 worldTarget = LevelPositionToWorldPosition(new Vector2(x, y));

                    float distToPlayer = Vector3.Distance(worldTarget, playerTransform.position);
                    if (distToPlayer >= 7f && distToPlayer <= detectionRange)
                    {
                        if (NavMesh.SamplePosition(worldTarget, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                        {
                            transform.position = hit.position;
                            //attackScript?.ShootSpellAtPlayer();
                            attackScript?.ShootTripleSpellAtPlayer();
                            return;
                        }
                    }
                }
            }
        }
        isTeleporting = false;
    }

    void OnDrawGizmos()
    {
        if (patrolPoints != null)
        {
            Gizmos.color = Color.red;
            foreach (var point in patrolPoints)
            {
                Gizmos.DrawSphere(point, 0.2f);
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