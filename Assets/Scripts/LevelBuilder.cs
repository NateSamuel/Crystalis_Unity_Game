using UnityEngine;
using Unity.AI.Navigation;
using System.Collections;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField] LayoutGeneratorRooms layoutGeneratorRooms;
    [SerializeField] MarchingSquares marchingSquares;
    [SerializeField] HallwayDetector hallwayDetector;
    [SerializeField] NavMeshSurface navMeshSurface;
    [SerializeField] RoomDecorator roomDecorator;
    [SerializeField] private Texture2D levelHeightTexture;
    [SerializeField] private int levelEnemyCount = 5;
    
    //Design by Student
    void Start()
    {
        layoutGeneratorRooms.LevelConfig.ResetMaxRoomCount();
        GenerateRandom();
    }

    //Design by Barbara Reichart lecture series, 2024
    [ContextMenu("Generate Random")]
    public void GenerateRandom()
    {
        SharedLevelData.Instance.GenerateSeed();
        Generate();
    }
    //Design by Student
    [ContextMenu("Generate")]
    public void Generate()
    {
        StartCoroutine(GenerateLevelRoutine());
    }
    //Design by Student
    private IEnumerator GenerateLevelRoutine()
    {
        Level level = null;
        int attempts = 0;
        int maxAttempts = 5;
        int maxRooms = layoutGeneratorRooms.LevelConfig.MaxRoomCount;

        do
        {
            level = layoutGeneratorRooms.GenerateLevel();
            attempts++;
        } while ((level.Rooms == null || level.Rooms.Length < maxRooms) && attempts < maxAttempts);

        if (level == null || level.Rooms.Length < maxRooms)
        {
            yield break;
        }

        layoutGeneratorRooms.LevelConfig.MaxRoomCount = Mathf.Min(maxRooms + 1, 6);

        hallwayDetector.DetectHallway();
        marchingSquares.CreateLevelGeometry();
        roomDecorator.PlaceItems(level);

        ParentHallwaysToNavMeshSurface();
        navMeshSurface.BuildNavMesh();

        
        yield return new WaitForEndOfFrame(); 
        yield return new WaitForSeconds(0.1f);

        PlaceEnemiesOnNavMesh();
        PlaceBossesOnNavMesh();
        MovePlayerToStart(level);
    }
    //Design by Student
    private void PlaceEnemiesOnNavMesh()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy1");
        GameObject[] enemyPositions = GameObject.FindGameObjectsWithTag("Enemy1Pos");

        int spawnCount = Mathf.Min(levelEnemyCount, enemies.Length, enemyPositions.Length);

        for (int i = 0; i < enemies.Length; i++)
        {
            GameObject enemy = enemies[i];
            var agent = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();

            if (i < spawnCount)
            {
                
                GameObject enemyPos = enemyPositions[i];
                Vector2Int gridPos = WorldToGridPosition(enemyPos.transform.position);
                Vector3 correctedWorldPos = LevelPositionToWorldPosition(gridPos);

                if (UnityEngine.AI.NavMesh.SamplePosition(correctedWorldPos, out UnityEngine.AI.NavMeshHit hit, 2f, UnityEngine.AI.NavMesh.AllAreas))
                {
                    if (agent != null)
                    {
                        if (!agent.enabled)
                            agent.enabled = true;

                        bool warped = agent.Warp(hit.position);

                        if (warped)
                            enemy.GetComponent<EnemyMovement>()?.Initialize();

                    }
                }
            }
            else
            {
                
                if (agent != null && !agent.enabled)
                    agent.enabled = true;

                agent?.Warp(new Vector3(0, -1000, 0));
                enemy.GetComponent<EnemyMovement>()?.DisableEnemy();
            }
        }
    }
    //Design by Student
    private void PlaceBossesOnNavMesh()
    {
        GameObject[] bossPositions = GameObject.FindGameObjectsWithTag("Boss1Pos");
        GameObject[] bosses = GameObject.FindGameObjectsWithTag("Boss1");

        int bossCount = Mathf.Min(bosses.Length, bossPositions.Length);

        for (int i = 0; i < bossCount; i++)
        {
            GameObject boss = bosses[i];
            GameObject bossPos = bossPositions[i];

            Vector2Int gridPos = WorldToGridPosition(bossPos.transform.position);
            Vector3 correctedWorldPos = LevelPositionToWorldPosition(gridPos);

            if (UnityEngine.AI.NavMesh.SamplePosition(correctedWorldPos, out UnityEngine.AI.NavMeshHit hit, 2f, UnityEngine.AI.NavMesh.AllAreas))
            {
                var agent = boss.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (agent != null)
                {
                    if (!agent.enabled)
                        agent.enabled = true;
                    bool warped = agent.Warp(hit.position);

                    if (warped)
                    {
                        var logic = boss.GetComponent<EnemyMovement>();
                        if (logic != null)
                            logic.Initialize();
                    }
                }
            }
        }
    }
    //Design by Barbara Reichart lecture series, 2024
    private void MovePlayerToStart(Level level)
    {
        Room startRoom = level.playerStartRoom;
        Vector2 roomCenter = startRoom.Area.center;
        Vector3 playerPosition = LevelPositionToWorldPosition(roomCenter);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        UnityEngine.AI.NavMeshAgent agent = player.GetComponent<UnityEngine.AI.NavMeshAgent>();

        if (agent != null)
        {
            bool warped = agent.Warp(playerPosition);
        }
        else
        {
            player.transform.position = playerPosition;
        }
    }

    //student creation
    Vector3 LevelPositionToWorldPosition(Vector2 levelPosition)
    {
        int scale = SharedLevelData.Instance.Scale;

        int texX = Mathf.Clamp((int)levelPosition.x, 0, levelHeightTexture.width - 1);
        int texY = Mathf.Clamp((int)levelPosition.y, 0, levelHeightTexture.height - 1);

        Color pixel = levelHeightTexture.GetPixel(texX, texY);

        float yHeight = 0f;

        if (ColorsApproximatelyEqual(pixel, LayoutColorMap.RoomLevel(1)))
            yHeight = 12f;
        else if (ColorsApproximatelyEqual(pixel, LayoutColorMap.RoomLevel(2)))
            yHeight = 24f;

        float worldX = (levelPosition.x - 1) * scale;
        float worldZ = (levelPosition.y - 1) * scale;

        return new Vector3(worldX, yHeight, worldZ);
    }
    //Design by Student
    Vector2Int WorldToGridPosition(Vector3 worldPos)
    {
        int scale = SharedLevelData.Instance.Scale;
        int gridX = Mathf.RoundToInt(worldPos.x / scale);
        int gridY = Mathf.RoundToInt(worldPos.z / scale);
        return new Vector2Int(gridX, gridY);
    }

    //student creation
    public void ParentHallwaysToNavMeshSurface()
    {
        GameObject[] hallways = GameObject.FindGameObjectsWithTag("Hallway"); 

        foreach (var hallway in hallways)
        {
            hallway.transform.SetParent(navMeshSurface.transform);
        }
    }
    //Design by Student
    private bool ColorsApproximatelyEqual(Color a, Color b, float tolerance = 0.05f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
            Mathf.Abs(a.g - b.g) < tolerance &&
            Mathf.Abs(a.b - b.b) < tolerance;
    }
}
