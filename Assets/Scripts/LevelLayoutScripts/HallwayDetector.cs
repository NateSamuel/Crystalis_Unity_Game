//student creation
using UnityEngine;

/// Detects hallway positions based on a level texture and places corresponding prefabs.
public class HallwayDetector : MonoBehaviour
{
    public Texture2D levelTexture;
    public HallwayPrefabMap hallwayPrefabMap;
    public Transform parent;
    public float tileScale = 1f;

    private static readonly HallwayDirection[] directions = {
        HallwayDirection.Top,
        HallwayDirection.Bottom,
        HallwayDirection.Left,
        HallwayDirection.Right
    };
    // Detects red-colored pixels in the level texture
    // Finds if direction next to red pixel is black, (if so that is the correct direction)
    // Flips direction of hallway if direction of delta is -1 or -2
    // Implements the hallway prefabs
    public void DetectHallway()
    {
        for (int y = 0; y < levelTexture.height; y++)
        {
            for (int x = 0; x < levelTexture.width; x++)
            {
                if (!ColorsApproximatelyEqual(levelTexture.GetPixel(x, y), Color.red)) continue;

                int? baseLevel = FindBaseLevel(x, y);
                if (baseLevel == null) continue;

                foreach (HallwayDirection dir in directions)
                {
                    Vector2Int neighbor = new Vector2Int(x, y) + dir.GetOffset();
                    if (!InBounds(neighbor)) continue;

                    Color neighborColor = levelTexture.GetPixel(neighbor.x, neighbor.y);

                    foreach (var kvp in LayoutColorMap.GetAllLevelDeltaColors())
                    {
                        int delta = kvp.Key;
                        Color deltaColor = kvp.Value;

                        if (!ColorsApproximatelyEqual(neighborColor, deltaColor)) continue;

                        HallwayDirection hallwayDir = dir;
                        bool needsFlip = (delta == -1 || delta == -2);
                        if (needsFlip)
                        {
                            hallwayDir = hallwayDir.GetOppositeDirection();
                        }

                        GameObject prefab = hallwayPrefabMap.GetPrefab(delta, hallwayDir.ToDirection());
                        if (prefab == null) continue;

                        float verticalHeightPerLevel = 12f;
                        float yOffset = baseLevel.Value * verticalHeightPerLevel;
                        Quaternion rotation = hallwayDir.GetRotation();
                        Vector3 position = new Vector3((x - 0.5f) * tileScale, yOffset, (y - 0.5f) * tileScale);

                        if (needsFlip)
                        {
                            Vector3 backOffset = rotation * (-Vector3.forward) * 6 * tileScale;
                            position += backOffset;
                        }

                        Instantiate(prefab, position, rotation, parent);
                        Debug.DrawRay(position, Vector3.up * 5, Color.red, 10f);
                    }
                }
            }
        }
    }
    // Search nearby for a room colour (white, magenta, green) and returns the equivalent level for it
    private int? FindBaseLevel(int x, int y)
    {
        for (int dy = -1; dy <= 1; dy++) {
            for (int dx = -1; dx <= 1; dx++) {
                Vector2Int neighbor = new Vector2Int(x + dx, y + dy);
                if (!InBounds(neighbor)) continue;

                Color nearbyColor = levelTexture.GetPixel(neighbor.x, neighbor.y);
                for (int level = 0; level <= 2; level++) {
                    if (ColorsApproximatelyEqual(nearbyColor, LayoutColorMap.RoomLevel(level))) {
                        return level;
                    }
                }
            }
        }
        return null;
    }
    // Checks if the point is within the level texture edge points
    private bool InBounds(Vector2Int p) =>
        p.x >= 0 && p.y >= 0 && p.x < levelTexture.width && p.y < levelTexture.height;
    
    //Checks if the two colours inputted are approximately equal
    private bool ColorsApproximatelyEqual(Color a, Color b, float tolerance = 0.01f) =>
        Mathf.Abs(a.r - b.r) < tolerance &&
        Mathf.Abs(a.g - b.g) < tolerance &&
        Mathf.Abs(a.b - b.b) < tolerance;
}