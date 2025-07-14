using UnityEngine;
using Unity.AI.Navigation;
//This pages' design is by both student and Barbara Reichart lecture series, 2024
public class LevelBuilder : MonoBehaviour
{
    [SerializeField] LayoutGeneratorRooms layoutGeneratorRooms;
    [SerializeField] MarchingSquares marchingSquares;
    [SerializeField] HallwayDetector hallwayDetector;
    [SerializeField] NavMeshSurface navMeshSurface;
    [SerializeField] RoomDecorator roomDecorator;
    //Design by Barbara Reichart lecture series, 2024
    void Start()
    {
        //Design by Student
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
    //Design by Student and Barbara Reichart lecture series, 2024
    [ContextMenu("Generate")]
    public void Generate()
    {
        //Design by Student
        Level level = null;
        int attempts = 0;
        int maxAttempts = 5;
        int maxRooms = layoutGeneratorRooms.LevelConfig.MaxRoomCount;

        // Tries to regenerate until the room count requirement is met
        do
        {
            level = layoutGeneratorRooms.GenerateLevel();
            attempts++;

        } while ((level.Rooms == null || level.Rooms.Length < maxRooms) && attempts < maxAttempts);

        if (level == null || level.Rooms.Length < layoutGeneratorRooms.LevelConfig.MaxRoomCount)
        {
            Debug.LogWarning("Level generation failed after " + attempts + " attempts.");
            return;
        }

        layoutGeneratorRooms.LevelConfig.MaxRoomCount = Mathf.Min(
            layoutGeneratorRooms.LevelConfig.MaxRoomCount + 1,
            6
        );

        hallwayDetector.DetectHallway();

        //Design by Barbara Reichart lecture series, 2024
        marchingSquares.CreateLevelGeometry();
        roomDecorator.PlaceItems(level);
        ParentHallwaysToNavMeshSurface();
        navMeshSurface.BuildNavMesh();

        Room startRoom = level.playerStartRoom;
        Vector2 roomCenter = startRoom.Area.center;
        Vector3 playerPosition = LevelPositionToWorldPosition(roomCenter);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        UnityEngine.AI.NavMeshAgent playerNavMeshAgent = player.GetComponent<UnityEngine.AI.NavMeshAgent>();

        if (playerNavMeshAgent == null)
        {
            player.transform.position = playerPosition;
        }
        else
        {
            playerNavMeshAgent.Warp(playerPosition);
        }
    }
    //Design by Barbara Reichart lecture series, 2024
    Vector3 LevelPositionToWorldPosition(Vector2 levelPosition)
    {
        int scale = SharedLevelData.Instance.Scale;
        return new Vector3((levelPosition.x-1) * scale, 0, (levelPosition.y-1) * scale);
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
}
