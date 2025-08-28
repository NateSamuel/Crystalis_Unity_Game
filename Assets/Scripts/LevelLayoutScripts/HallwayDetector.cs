// using UnityEngine;

// //student creation
// public class HallwayDetector : MonoBehaviour
// {
//     public Texture2D levelTexture;
//     public HallwayPrefabMap hallwayPrefabMap;
//     public Transform parent;
//     public float tileScale = 1f;

//     private static readonly Vector2Int[] directions = {
//         Vector2Int.up,
//         Vector2Int.down,
//         Vector2Int.left,
//         Vector2Int.right
//     };

//     /// Detects red-colored pixels in the level texture 
//     /// Implements the hallway prefabs - this is worked out based on adjacent pixel colours and surrounding room colours.
//     public void DetectHallway()
//     {
//         for (int y = 0; y < levelTexture.height; y++) {
//             for (int x = 0; x < levelTexture.width; x++) {
//                 Color pixel = levelTexture.GetPixel(x, y);

//                 if (ColorsApproximatelyEqual(pixel, Color.red)) {

//                     int? baseLevel = null;

//                     // Search nearby for a room colour (white, magenta, green)
//                     for (int dy = -1; dy <= 1 && baseLevel == null; dy++) {
//                         for (int dx = -1; dx <= 1 && baseLevel == null; dx++) {
//                             Vector2Int neighbor = new Vector2Int(x + dx, y + dy);
//                             if (!InBounds(neighbor)) continue;

//                             Color nearbyColor = levelTexture.GetPixel(neighbor.x, neighbor.y);

//                             for (int level = 0; level <= 2; level++) {
//                                 if (ColorsApproximatelyEqual(nearbyColor, LayoutColorMap.RoomLevel(level))) {
//                                     baseLevel = level;
//                                     break;
//                                 }
//                             }
//                         }
//                     }

//                     foreach (Vector2Int offset in directions) {
//                         Vector2Int neighbor = new Vector2Int(x, y) + offset;
//                         if (!InBounds(neighbor)) continue;

//                         Color neighborColor = levelTexture.GetPixel(neighbor.x, neighbor.y);

//                         foreach (var kvp in LayoutColorMap.GetAllLevelDeltaColors()) {
//                             int delta = kvp.Key;
//                             Color deltaColor = kvp.Value;

//                             if (ColorsApproximatelyEqual(neighborColor, deltaColor)) {

//                                 Direction dir = OffsetToDirection(offset);
//                                 bool needsFlip = (delta == -1 || delta == -2);
//                                 if (needsFlip) {
//                                     dir = ReverseDirection(dir);
//                                 }

//                                 GameObject prefab = hallwayPrefabMap.GetPrefab(delta, dir);
//                                 if (prefab != null) {

//                                     float verticalHeightPerLevel = 12f;
//                                     float yOffset = baseLevel.HasValue ? baseLevel.Value * verticalHeightPerLevel : 0f;
//                                     Quaternion rot = DirectionToRotation(dir);

//                                     Vector3 pos = new Vector3((x - 0.5f) * tileScale, yOffset, (y - 0.5f) * tileScale);

//                                     // Apply flip back offset if needed
//                                     if (needsFlip) {
//                                         Vector3 backOffset = rot * (-Vector3.forward) * 6 * tileScale;
//                                         pos += backOffset;
//                                     }

//                                     var obj = Instantiate(prefab, pos, rot);
                                    
//                                     Debug.DrawRay(pos, Vector3.up * 5, Color.red, 10f);
//                                 } else {
//                                 }
//                             }
//                         }
//                     }
//                 }
//             }
//         }
//     }

//     // Checks if the point is within the level texture edge points
//     bool InBounds(Vector2Int p) =>
//         p.x >= 0 && p.y >= 0 && p.x < levelTexture.width && p.y < levelTexture.height;

//     // Compares two colours approximately using a tolerance value.
//     bool ColorsApproximatelyEqual(Color a, Color b, float tolerance = 0.01f) =>
//         Mathf.Abs(a.r - b.r) < tolerance &&
//         Mathf.Abs(a.g - b.g) < tolerance &&
//         Mathf.Abs(a.b - b.b) < tolerance;

//     // Converts a vector direction to a Direction value.
//     Direction OffsetToDirection(Vector2Int offset)
//     {
//         if (offset == Vector2Int.up) return Direction.North;
//         if (offset == Vector2Int.down) return Direction.South;
//         if (offset == Vector2Int.right) return Direction.East;
//         if (offset == Vector2Int.left) return Direction.West;
//         throw new System.Exception("Invalid direction offset");
//     }

//     // Changes the direction to rotation coordinates
//     Quaternion DirectionToRotation(Direction dir)
//     {
//         switch (dir) {
//             case Direction.North: return Quaternion.identity;
//             case Direction.East:  return Quaternion.Euler(0, 90, 0);
//             case Direction.South: return Quaternion.Euler(0, 180, 0);
//             case Direction.West:  return Quaternion.Euler(0, 270, 0);
//             default: return Quaternion.identity;
//         }
//     }
//     // Returns the opposite of the input direction.
//     Direction ReverseDirection(Direction dir)
//     {
//         switch (dir) {
//             case Direction.North: return Direction.South;
//             case Direction.South: return Direction.North;
//             case Direction.East:  return Direction.West;
//             case Direction.West:  return Direction.East;
//             default: return dir;
//         }
//     }
// }


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

    public void DetectHallway()
    {
        for (int y = 0; y < levelTexture.height; y++) {
            for (int x = 0; x < levelTexture.width; x++) {
                if (!ColorsApproximatelyEqual(levelTexture.GetPixel(x, y), Color.red)) continue;

                int? baseLevel = FindBaseLevel(x, y);
                if (baseLevel == null) continue;

                foreach (HallwayDirection dir in directions) {
                    Vector2Int neighbor = new Vector2Int(x, y) + dir.GetOffset();
                    if (!InBounds(neighbor)) continue;

                    Color neighborColor = levelTexture.GetPixel(neighbor.x, neighbor.y);

                    foreach (var kvp in LayoutColorMap.GetAllLevelDeltaColors()) {
                        int delta = kvp.Key;
                        Color deltaColor = kvp.Value;

                        if (!ColorsApproximatelyEqual(neighborColor, deltaColor)) continue;

                        HallwayDirection hallwayDir = dir;
                        bool needsFlip = (delta == -1 || delta == -2);
                        if (needsFlip) {
                            hallwayDir = hallwayDir.GetOppositeDirection();
                        }

                        GameObject prefab = hallwayPrefabMap.GetPrefab(delta, hallwayDir.ToDirection());
                        if (prefab == null) continue;

                        float verticalHeightPerLevel = 12f;
                        float yOffset = baseLevel.Value * verticalHeightPerLevel;
                        Quaternion rotation = hallwayDir.GetRotation();
                        Vector3 position = new Vector3((x - 0.5f) * tileScale, yOffset, (y - 0.5f) * tileScale);

                        if (needsFlip) {
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

    private bool InBounds(Vector2Int p) =>
        p.x >= 0 && p.y >= 0 && p.x < levelTexture.width && p.y < levelTexture.height;

    private bool ColorsApproximatelyEqual(Color a, Color b, float tolerance = 0.01f) =>
        Mathf.Abs(a.r - b.r) < tolerance &&
        Mathf.Abs(a.g - b.g) < tolerance &&
        Mathf.Abs(a.b - b.b) < tolerance;
}