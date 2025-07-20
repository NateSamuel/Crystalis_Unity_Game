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
    [SerializeField] private Texture2D levelHeightTexture;
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

    //student creation
    public void ParentHallwaysToNavMeshSurface()
    {
        GameObject[] hallways = GameObject.FindGameObjectsWithTag("Hallway"); 

        foreach (var hallway in hallways)
        {
            hallway.transform.SetParent(navMeshSurface.transform);
        }
    }
    private bool ColorsApproximatelyEqual(Color a, Color b, float tolerance = 0.05f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
            Mathf.Abs(a.g - b.g) < tolerance &&
            Mathf.Abs(a.b - b.b) < tolerance;
    }
}
