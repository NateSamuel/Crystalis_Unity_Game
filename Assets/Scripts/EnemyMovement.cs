using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public TileType[,] grid;
    public float moveSpeed = 2f;
    public Vector2Int gridPosition;
    Vector2Int currentTargetTile;
    int patrolDirection = -1;

    public Transform playerTransform;
    public float detectionRange = 20f;
    public float attackRange = 5f;
    private EnemyAttack attackScript;



    void Start()
    {
        gridPosition = WorldToGridPosition(transform.position);
        currentTargetTile = gridPosition + new Vector2Int(patrolDirection, 0);
        attackScript = GetComponent<EnemyAttack>();
        if (attackScript == null)
        {
            Debug.LogWarning("EnemyAttack script not found!");
        }
        
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player not found! Make sure Player GameObject is tagged 'Player'.");
        }

        

    }

    void Update()
    {
        
        Vector3 targetWorldPos = GridToWorldPosition(currentTargetTile);
        transform.position = Vector3.MoveTowards(transform.position, targetWorldPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetWorldPos) < 0.05f)
        {
            transform.position = targetWorldPos;
            gridPosition = currentTargetTile;
            patrolDirection *= -1;
            currentTargetTile = gridPosition + new Vector2Int(patrolDirection, 0);
        }

        
        if (playerTransform != null)
        {
            Vector3 enemyPosFlat = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 playerPosFlat = new Vector3(playerTransform.position.x, 0, playerTransform.position.z);

            float distanceToPlayer = Vector3.Distance(enemyPosFlat, playerPosFlat);
            if (distanceToPlayer <= detectionRange)
            {
                
                attackScript?.AttackPlayer();
                if(distanceToPlayer <= attackRange)
                {
                    attackScript?.IsAbleToHitPlayer();
                }
                else{
                    attackScript?.IsNotAbleToHitPlayer();
                }
            }

            else{
                attackScript?.DontAttackPlayer();
                attackScript?.IsNotAbleToHitPlayer();
            }
        }
    }

    Vector3 GridToWorldPosition(Vector2Int gridPos)
    {
        int scale = SharedLevelData.Instance.Scale;
        return new Vector3(gridPos.x * scale, 0, gridPos.y * scale);
    }

    Vector2Int WorldToGridPosition(Vector3 worldPos)
    {
        int scale = SharedLevelData.Instance.Scale;
        int gridX = Mathf.RoundToInt(worldPos.x / scale);
        int gridY = Mathf.RoundToInt(worldPos.z / scale);
        return new Vector2Int(gridX, gridY);
    }


}