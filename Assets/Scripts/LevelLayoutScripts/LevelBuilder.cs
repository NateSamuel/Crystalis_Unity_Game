using UnityEngine;
using Unity.AI.Navigation;
using System.Collections;
using System.Collections.Generic;

//Overall: This class calls creation of the layout, and moves character, enemies and hallways to navmesh

public class LevelBuilder : MonoBehaviour
{
    [SerializeField] LayoutGeneratorRooms layoutGeneratorRooms;
    [SerializeField] MarchingSquares marchingSquares;
    [SerializeField] HallwayDetector hallwayDetector;
    [SerializeField] NavMeshSurface navMeshSurface;
    [SerializeField] RoomDecorator roomDecorator;
    [SerializeField] private Texture2D levelHeightTexture;
    [SerializeField] private int levelEnemyCount = 5;

    private TotalPrisoners totalPrisoners;

    public static List<GameObject> ActiveEnemies { get; private set; } = new List<GameObject>();
    public static List<GameObject> InactiveEnemies { get; private set; } = new List<GameObject>();

    public static List<GameObject> ActiveBosses { get; private set; } = new List<GameObject>();
    public static List<GameObject> InactiveBosses { get; private set; } = new List<GameObject>();

    public static List<GameObject> ActivePrisoners { get; private set; } = new List<GameObject>();
    public static List<GameObject> InactivePrisoners { get; private set; } = new List<GameObject>();
    public EnemyTrackerForObjectives tracker;

    //Design by Student
    void Start()
    {
        layoutGeneratorRooms.LevelConfig.ResetMaxRoomCount();
        GenerateRandom();
        totalPrisoners = FindAnyObjectByType<TotalPrisoners>();
    }

    //Design by Barbara Reichart lecture series, 2024
    // Generates a new random seed and starts level generation.
    [ContextMenu("Generate Random")]
    public void GenerateRandom()
    {
        SharedLevelData.Instance.GenerateSeed();
        Generate();
    }
    //Design by Student
    // Starts the level generation coroutine.
    [ContextMenu("Generate")]
    public void Generate()
    {
        StartCoroutine(GenerateLevelRoutine());
    }
    //Design by Student
    // Tries to create a level layout and then builds it, with the navmesh added. Enemies, bosses, and the player are placed.
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

        layoutGeneratorRooms.LevelConfig.MaxRoomCount = Mathf.Min(maxRooms + 1, layoutGeneratorRooms.LevelConfig.FinalMaxRoomCount);

        hallwayDetector.DetectHallway();
        marchingSquares.CreateLevelGeometry();
        roomDecorator.PlaceItems(level);

        ParentHallwaysToNavMeshSurface();
        navMeshSurface.BuildNavMesh();


        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f);

        // PlaceEnemiesOnNavMesh();
        // PlaceBossesOnNavMesh();
        PlaceUnitsOnNavMesh<RuffianEnemyAI>(
            "Enemy1", 
            "Enemy1Pos",
            ActiveEnemies,
            InactiveEnemies,
            tracker.RegisterEnemy,
            tracker.SetEnemyActive,
            logic => logic?.Initialize(),
            logic => logic?.DisableEnemy(),
            levelEnemyCount
        );

        PlaceUnitsOnNavMesh<BossEnemyAI>(
            "Boss1",
            "Boss1Pos",
            ActiveBosses,
            InactiveBosses,
            tracker.RegisterBoss,
            tracker.SetBossActive,
            logic => logic?.Initialize(),
            logic => logic?.DisableEnemy()
        );
        PlacePrisonersOnNavMesh();

        MovePlayerToStart(level);
        totalPrisoners?.FindPrisoners();
    }
    private void PlaceUnitsOnNavMesh<T>(
        string unitTag, 
        string positionTag, 
        List<GameObject> activeList, 
        List<GameObject> inactiveList,
        System.Action<GameObject> registerUnit,
        System.Action<GameObject, bool> setUnitActive,
        System.Action<T> initializeUnitLogic,
        System.Action<T> disableUnitLogic,
        int unitCountLimit = -1) where T : MonoBehaviour
    {
        GameObject[] positions = GameObject.FindGameObjectsWithTag(positionTag);
        GameObject[] allUnits = Resources.FindObjectsOfTypeAll<GameObject>();
        List<GameObject> units = new List<GameObject>();

        foreach (var go in allUnits)
        {
            if (go.CompareTag(unitTag))
            {
                go.SetActive(true);
                units.Add(go);
            }
        }

        activeList.Clear();
        inactiveList.Clear();

        int spawnCount = unitCountLimit > 0 ? Mathf.Min(unitCountLimit, units.Count, positions.Length) : Mathf.Min(units.Count, positions.Length);

        for (int i = 0; i < units.Count; i++)
        {
            GameObject unit = units[i];
            var agent = unit.GetComponent<UnityEngine.AI.NavMeshAgent>();
            T logic = unit.GetComponent<T>();

            registerUnit?.Invoke(unit);

            if (i < spawnCount)
            {
                GameObject posObj = positions[i];
                Vector2Int gridPos = WorldToGridPosition(posObj.transform.position);
                Vector3 correctedWorldPos = LevelPositionToWorldPosition(gridPos);

                if (UnityEngine.AI.NavMesh.SamplePosition(correctedWorldPos, out UnityEngine.AI.NavMeshHit hit, 2f, UnityEngine.AI.NavMesh.AllAreas))
                {
                    unit.SetActive(true);
                    if (agent != null && !agent.enabled) agent.enabled = true;

                    if (agent?.Warp(hit.position) ?? false)
                    {
                        initializeUnitLogic?.Invoke(logic);
                        activeList.Add(unit);
                        setUnitActive?.Invoke(unit, true);
                    }
                }
            }
            else
            {
                if (agent != null && !agent.enabled) agent.enabled = true;
                agent?.Warp(new Vector3(0, -1000, 0));
                disableUnitLogic?.Invoke(logic);
                unit.SetActive(false);
                inactiveList.Add(unit);
                setUnitActive?.Invoke(unit, false);
            }
        }
    }
    private void PlacePrisonersOnNavMesh()
    {
        GameObject[] prisonerPositions = GameObject.FindGameObjectsWithTag("PrisonerPos");
        GameObject[] allPrisoners = Resources.FindObjectsOfTypeAll<GameObject>();
        List<GameObject> prisoners = new List<GameObject>();

        foreach (var go in allPrisoners)
        {
            if (go.CompareTag("Prisoner"))
            {
                go.SetActive(true);
                prisoners.Add(go);
            }
        }

        int spawnCount = Mathf.Min(prisoners.Count, prisonerPositions.Length);

        for (int i = 0; i < prisoners.Count; i++)
        {
            GameObject prisoner = prisoners[i];
            var agent = prisoner.GetComponent<UnityEngine.AI.NavMeshAgent>();

            if (i < spawnCount)
            {
                GameObject spawnPos = prisonerPositions[i];
                Vector3 targetPos = spawnPos.transform.position;

                if (UnityEngine.AI.NavMesh.SamplePosition(targetPos, out UnityEngine.AI.NavMeshHit hit, 2f, UnityEngine.AI.NavMesh.AllAreas))
                {
                    prisoner.SetActive(true);
                    if (agent != null)
                    {
                        if (!agent.enabled) agent.enabled = true;
                        agent.Warp(hit.position);
                    }
                    else
                    {
                        prisoner.transform.position = hit.position;
                    }
                }
            }
            else
            {
                // move unused prisoners somewhere out of the way
                if (agent != null)
                {
                    if (!agent.enabled) agent.enabled = true;
                    agent.Warp(new Vector3(0, -1000, 0));
                }
                else
                {
                    prisoner.transform.position = new Vector3(0, -1000, 0);
                }
                prisoner.SetActive(false);
            }
        }
    }
    // //Design by Student
    // // Finds and places enemies onto the NavMesh. Deactivates if not used.
    // private void PlaceEnemiesOnNavMesh()
    // {

    //     GameObject[] enemyPositions = GameObject.FindGameObjectsWithTag("Enemy1Pos");
    //     GameObject[] allEnemies = Resources.FindObjectsOfTypeAll<GameObject>();
    //     List<GameObject> enemies = new List<GameObject>();

    //     foreach (var go in allEnemies)
    //     {
    //         if (go.CompareTag("Enemy1"))
    //         {
    //             go.SetActive(true);
    //             enemies.Add(go);
    //         }
    //     }

    //     ActiveEnemies.Clear();
    //     InactiveEnemies.Clear();

    //     int spawnCount = Mathf.Min(levelEnemyCount, enemies.Count, enemyPositions.Length);

    //     for (int i = 0; i < enemies.Count; i++)
    //     {
    //         GameObject enemy = enemies[i];
    //         var agent = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
    //         tracker.RegisterEnemy(enemy);
    //         if (i < spawnCount)
    //         {
    //             GameObject enemyPos = enemyPositions[i];
    //             Vector2Int gridPos = WorldToGridPosition(enemyPos.transform.position);
    //             Vector3 correctedWorldPos = LevelPositionToWorldPosition(gridPos);

    //             if (UnityEngine.AI.NavMesh.SamplePosition(correctedWorldPos, out UnityEngine.AI.NavMeshHit hit, 2f, UnityEngine.AI.NavMesh.AllAreas))
    //             {
    //                 enemy.SetActive(true);
    //                 if (agent != null)
    //                 {
    //                     if (!agent.enabled)
    //                         agent.enabled = true;

    //                     bool warped = agent.Warp(hit.position);

    //                     if (warped)
    //                     {
    //                         enemy.GetComponent<RuffianEnemyAI>()?.Initialize();
    //                         ActiveEnemies.Add(enemy);
    //                         tracker.SetEnemyActive(enemy, true);
    //                     }
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             if (agent != null && !agent.enabled)
    //                 agent.enabled = true;


    //             agent?.Warp(new Vector3(0, -1000, 0));
    //             enemy.GetComponent<RuffianEnemyAI>()?.DisableEnemy();
    //             enemy.SetActive(false);
    //             InactiveEnemies.Add(enemy);
    //             tracker.SetEnemyActive(enemy, false);
    //         }
    //     }
    // }

    // //Design by Student
    // // Finds and places bosses onto the NavMesh. Deactivates if not used.
    // private void PlaceBossesOnNavMesh()
    // {

    //     GameObject[] bossPositions = GameObject.FindGameObjectsWithTag("Boss1Pos");
    //     GameObject[] allBosses = Resources.FindObjectsOfTypeAll<GameObject>();
    //     List<GameObject> bosses = new List<GameObject>();

    //     foreach (var go in allBosses)
    //     {
    //         if (go.CompareTag("Boss1"))
    //         {
    //             go.SetActive(true);
    //             bosses.Add(go);
    //         }
    //     }

    //     ActiveBosses.Clear();
    //     InactiveBosses.Clear();

    //     int bossCount = Mathf.Min(bosses.Count, bossPositions.Length);

    //     for (int i = 0; i < bosses.Count; i++)
    //     {
    //         GameObject boss = bosses[i];
    //         var agent = boss.GetComponent<UnityEngine.AI.NavMeshAgent>();
    //         tracker.RegisterBoss(boss);
    //         if (i < bossCount)
    //         {
    //             GameObject bossPos = bossPositions[i];

    //             Vector2Int gridPos = WorldToGridPosition(bossPos.transform.position);
    //             Vector3 correctedWorldPos = LevelPositionToWorldPosition(gridPos);

    //             if (UnityEngine.AI.NavMesh.SamplePosition(correctedWorldPos, out UnityEngine.AI.NavMeshHit hit, 2f, UnityEngine.AI.NavMesh.AllAreas))
    //             {
    //                 boss.SetActive(true);
    //                 if (agent != null)
    //                 {
    //                     if (!agent.enabled)
    //                         agent.enabled = true;

    //                     bool warped = agent.Warp(hit.position);

    //                     if (warped)
    //                     {
    //                         var logic = boss.GetComponent<BossEnemyAI>();
    //                         logic?.Initialize();

    //                         ActiveBosses.Add(boss);
    //                         tracker.SetBossActive(boss, true);
    //                     }
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             if (agent != null && !agent.enabled)
    //                 agent.enabled = true;

    //             agent?.Warp(new Vector3(0, -1000, 0));
    //             boss.GetComponent<BossEnemyAI>()?.DisableEnemy();
    //             boss.SetActive(false);
    //             InactiveBosses.Add(boss);
    //             tracker.SetBossActive(boss, false);
    //         }
    //     }
    // }

    //Design by Barbara Reichart lecture series, 2024
    // Moves the player to the start room.
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

    //Design by Student
    // Converts a level texture into a world position, using level scale and height.
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
    // Compares two colors to check if they are almost equal.
    private bool ColorsApproximatelyEqual(Color a, Color b, float tolerance = 0.05f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
            Mathf.Abs(a.g - b.g) < tolerance &&
            Mathf.Abs(a.b - b.b) < tolerance;
    }

    //Design by Student
    // Converts a world position into a level texture.
    Vector2Int WorldToGridPosition(Vector3 worldPos)
    {
        int scale = SharedLevelData.Instance.Scale;
        int gridX = Mathf.RoundToInt(worldPos.x / scale);
        int gridY = Mathf.RoundToInt(worldPos.z / scale);
        return new Vector2Int(gridX, gridY);
    }

    //Design by Student
    // Add all hallway tagged objects to the navmeshsurface
    public void ParentHallwaysToNavMeshSurface()
    {
        GameObject[] hallways = GameObject.FindGameObjectsWithTag("Hallway"); 

        foreach (var hallway in hallways)
        {
            hallway.transform.SetParent(navMeshSurface.transform);
        }
    }
}
