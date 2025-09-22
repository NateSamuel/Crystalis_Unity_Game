// This class calls creation of the layout, and moves character, enemies and hallways to navmesh
//Mostly student creation, a couple of methods partially designed by Barbara Reichhart 2024
using UnityEngine;
using Unity.AI.Navigation;
using System.Collections;
using System.Collections.Generic;

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

    // Master lists — persist across levels
    private List<GameObject> allEnemies = new List<GameObject>();
    private List<GameObject> allBosses = new List<GameObject>();
    private List<GameObject> allPrisoners = new List<GameObject>();

    public static List<GameObject> ActiveEnemies { get; private set; } = new List<GameObject>();
    public static List<GameObject> InactiveEnemies { get; private set; } = new List<GameObject>();
    public static List<GameObject> ActiveBosses { get; private set; } = new List<GameObject>();
    public static List<GameObject> InactiveBosses { get; private set; } = new List<GameObject>();
    public static List<GameObject> ActivePrisoners { get; private set; } = new List<GameObject>();
    public static List<GameObject> InactivePrisoners { get; private set; } = new List<GameObject>();

    public EnemyTrackerForObjectives tracker;
    private void Awake()
    {
        // Populate master lists once — includes inactive units
        allEnemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy1"));
        allBosses.AddRange(GameObject.FindGameObjectsWithTag("Boss1"));
        allPrisoners.AddRange(GameObject.FindGameObjectsWithTag("Prisoner"));
        DeactivateAllUnits();
    }

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

        if (level == null || level.Rooms.Length < maxRooms) yield break;

        layoutGeneratorRooms.LevelConfig.MaxRoomCount = Mathf.Min(maxRooms + 1, layoutGeneratorRooms.LevelConfig.FinalMaxRoomCount);

        hallwayDetector.DetectHallway();
        marchingSquares.CreateLevelGeometry();

        yield return StartCoroutine(PlaceDecorationsDelayed(level));
    }

    //Delays placement of decorations for runtime
    private IEnumerator PlaceDecorationsDelayed(Level level)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.05f);

        roomDecorator.PlaceItems(level);
        ParentHallwaysToNavMeshSurface();
        navMeshSurface.BuildNavMesh();

        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f);

        // Place enemies
        PlaceUnitsOnNavMesh<RuffianEnemyAI>(
            allEnemies, "Enemy1Pos", ActiveEnemies, InactiveEnemies,
            tracker.RegisterEnemy, tracker.SetEnemyActive,
            logic => logic?.Initialize(), logic => logic?.DisableEnemy(),
            levelEnemyCount
        );

        // Place bosses
        PlaceUnitsOnNavMesh<BossEnemyAI>(
            allBosses, "Boss1Pos", ActiveBosses, InactiveBosses,
            tracker.RegisterBoss, tracker.SetBossActive,
            logic => logic?.Initialize(), logic => logic?.DisableEnemy()
        );

        // Place prisoners separately
        PlacePrisonersOnNavMesh();

        MovePlayerToStart(level);
        totalPrisoners?.FindPrisoners();
    }

    //Designed by student
    //Tries to position enemies from a master list onto NavMesh spawn points
    //Activates and initializes them if placed successfully, or deactivates/hides them if not.
    private void PlaceUnitsOnNavMesh<T>(
        List<GameObject> masterList,
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
        if (positions.Length == 0)
        {
            Debug.LogWarning($"[LevelBuilder] No spawn positions found for tag {positionTag}.");
            return;
        }

        activeList.Clear();
        inactiveList.Clear();

        int spawnCount = unitCountLimit > 0 ? Mathf.Min(unitCountLimit, masterList.Count, positions.Length) : Mathf.Min(masterList.Count, positions.Length);

        for (int i = 0; i < masterList.Count; i++)
        {
            GameObject unit = masterList[i];
            T logic = unit.GetComponent<T>();
            var agent = unit.GetComponent<UnityEngine.AI.NavMeshAgent>();

            bool placed = false;

            if (i < spawnCount)
            {
                Vector3 targetPos = positions[i].transform.position;

                UnityEngine.AI.NavMeshHit hit;
                bool found = UnityEngine.AI.NavMesh.SamplePosition(targetPos, out hit, 2f, UnityEngine.AI.NavMesh.AllAreas)
                            || UnityEngine.AI.NavMesh.SamplePosition(targetPos, out hit, 6f, UnityEngine.AI.NavMesh.AllAreas);

                if (!found)
                {
                    Vector3[] offsets = { new Vector3(0, -4f, 0), new Vector3(0, 4f, 0), new Vector3(2f, 0, 0), new Vector3(-2f, 0, 0) };
                    foreach (var off in offsets)
                    {
                        if (UnityEngine.AI.NavMesh.SamplePosition(targetPos + off, out hit, 6f, UnityEngine.AI.NavMesh.AllAreas))
                        {
                            found = true;
                            break;
                        }
                    }
                }

                if (found)
                {
                    if (agent != null)
                    {
                        agent.enabled = false;
                        unit.transform.position = hit.position;
                        agent.enabled = true;
                        agent.Warp(hit.position);
                    }
                    else
                    {
                        unit.transform.position = hit.position;
                    }

                    unit.SetActive(true);
                    initializeUnitLogic?.Invoke(logic);
                    registerUnit?.Invoke(unit);

                    activeList.Add(unit);
                    setUnitActive?.Invoke(unit, true);

                    placed = true;
                }
            }

            if (!placed)
            {
                unit.SetActive(false);
                if (agent != null) agent.enabled = false;
                disableUnitLogic?.Invoke(logic);
                unit.transform.position = new Vector3(0, -1000, 0);

                inactiveList.Add(unit);
                setUnitActive?.Invoke(unit, false);
            }
        }
    }
    //Designed by student
    //Tries to position active prisoners onto NavMesh spawn points
    //Deactivates/hides them if not possible
    private void PlacePrisonersOnNavMesh()
    {
        GameObject[] positions = GameObject.FindGameObjectsWithTag("PrisonerPos");
        if (positions.Length == 0)
        {
            return;
        }

        ActivePrisoners.Clear();
        InactivePrisoners.Clear();

        int spawnCount = Mathf.Min(allPrisoners.Count, positions.Length);

        for (int i = 0; i < allPrisoners.Count; i++)
        {
            GameObject prisoner = allPrisoners[i];
            var agent = prisoner.GetComponent<UnityEngine.AI.NavMeshAgent>();

            bool placed = false;

            if (i < spawnCount)
            {
                Vector3 targetPos = positions[i].transform.position;

                UnityEngine.AI.NavMeshHit hit;
                bool found = UnityEngine.AI.NavMesh.SamplePosition(targetPos, out hit, 2f, UnityEngine.AI.NavMesh.AllAreas)
                            || UnityEngine.AI.NavMesh.SamplePosition(targetPos, out hit, 6f, UnityEngine.AI.NavMesh.AllAreas);

                if (!found)
                {
                    Vector3[] offsets = { new Vector3(0, -4f, 0), new Vector3(0, 4f, 0), new Vector3(2f, 0, 0), new Vector3(-2f, 0, 0) };
                    foreach (var off in offsets)
                    {
                        if (UnityEngine.AI.NavMesh.SamplePosition(targetPos + off, out hit, 6f, UnityEngine.AI.NavMesh.AllAreas))
                        {
                            found = true;
                            break;
                        }
                    }
                }

                if (found)
                {
                    if (agent != null)
                    {
                        agent.enabled = false;
                        prisoner.transform.position = hit.position;
                        agent.enabled = true;
                        agent.Warp(hit.position);
                    }
                    else
                    {
                        prisoner.transform.position = hit.position;
                    }

                    prisoner.SetActive(true);
                    ActivePrisoners.Add(prisoner);
                    placed = true;
                }
            }

            if (!placed)
            {
                prisoner.SetActive(false);
                if (agent != null) agent.enabled = false;
                prisoner.transform.position = new Vector3(0, -1000, 0);
                InactivePrisoners.Add(prisoner);
            }
        }
    }

    //Design by student
    //Deactivates all enemies/prsoners and moves them when level restarts/starts
    public void DeactivateAllUnits()
    {
        // Handles Enemies
        EnemyHealth[] oldEnemies = FindObjectsOfType<EnemyHealth>();
        foreach (EnemyHealth enemy in oldEnemies)
        {
            var go = enemy.gameObject;
            var agent = go.GetComponent<UnityEngine.AI.NavMeshAgent>();
            go.SetActive(false);
            if (agent != null) agent.enabled = false;
            go.transform.position = new Vector3(0, -1000, 0);
        }

        // Handles Prisoners
        GameObject[] oldPrisoners = GameObject.FindGameObjectsWithTag("Prisoner");
        foreach (var prisoner in oldPrisoners)
        {
            var agent = prisoner.GetComponent<UnityEngine.AI.NavMeshAgent>();
            prisoner.SetActive(false);
            if (agent != null) agent.enabled = false;
            prisoner.transform.position = new Vector3(0, -1000, 0);
        }
    }

    private void ResetUnit<T>(GameObject unit, System.Action<T> disableUnitLogic, T logic, List<GameObject> inactiveList, System.Action<GameObject, bool> setUnitActive)
        where T : MonoBehaviour
    {
        var agent = unit.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null) agent.enabled = false;
        unit.transform.position = new Vector3(0, -1000, 0);
        unit.SetActive(true);
        disableUnitLogic?.Invoke(logic);
        inactiveList.Add(unit);
        setUnitActive?.Invoke(unit, false);

    }

    //Design by Barbara Reichart lecture series, 2024
    // Moves the player to the start room.
    private void MovePlayerToStart(Level level)
    {
        Room startRoom = level.playerStartRoom;
        Vector2 roomCenter = startRoom.Area.center;
        Vector3 playerPosition = LevelPositionToWorldPosition(roomCenter);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        var agent = player.GetComponent<UnityEngine.AI.NavMeshAgent>();

        if (agent != null) agent.Warp(playerPosition);
        else player.transform.position = playerPosition;
    }

    //Design by Student
    // Add all hallway tagged objects to the navmeshsurface
    public void ParentHallwaysToNavMeshSurface()
    {
        foreach (var hallway in GameObject.FindGameObjectsWithTag("Hallway"))
        {
            hallway.transform.SetParent(navMeshSurface.transform);
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

        if (ColorsApproximatelyEqual(pixel, LayoutColorMap.RoomLevel(1))) yHeight = 12f;
        else if (ColorsApproximatelyEqual(pixel, LayoutColorMap.RoomLevel(2))) yHeight = 24f;

        return new Vector3((levelPosition.x - 1) * scale, yHeight, (levelPosition.y - 1) * scale);
    }

    //Design by Student
    // Compares two colors to check if they are almost equal.
    private bool ColorsApproximatelyEqual(Color a, Color b, float tolerance = 0.05f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance;
    }
}