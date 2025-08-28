using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

//Design by Barbara Reichart lecture series, 2024, student additions for height placement
[Serializable]
[CreateAssetMenu(fileName = "DecoratorRule", menuName = "Custom/Procedural Generation/Pattern Decorator Rule")]
public class PatternMatchingDecoratorRule : BaseDecoratorRule
{
    [SerializeField] GameObject prefab;
    [SerializeField] float prefabRotation = 0;
    [SerializeField] Array2DWrapper<TileType> placement;
    [SerializeField] Array2DWrapper<TileType> fill;
    [SerializeField] bool centerHorizontally = false;
    [SerializeField] bool centerVertically = false;
    [SerializeField] private Texture2D levelHeightTexture;

    // Checks if the rule can be applied to the given room by searching for pattern occurrences.
    // Returns true if at least one available position is found.
    internal override bool CanBeApplied(TileType[,] levelDecorated, Room room)
    {
        var found = FindOccurrences(levelDecorated, room);
        if (found.Length == 0)
        {
            Debug.Log($"[PatternMatchingDecoratorRule] No occurrences found for {name} in room {room}");
            return false;
        }
        return true;
    }

    // Applies the decorator rule by placing a prefab at a randomly chosen valid pattern match position.
    // Updates the tile data, instantiates the decoration prefab, rotates and positions it based on layout height.
    internal override void Apply(TileType[,] levelDecorated, Room room, Transform parent)
    {
        Vector2Int[] occurrences = FindOccurrences(levelDecorated, room);
        if (occurrences.Length == 0) { return; }
        Random random = SharedLevelData.Instance.Rand;
        int occurrenceIndex = random.Next(0, occurrences.Length);
        Vector2Int occurrence = occurrences[occurrenceIndex];
        for (int y = 0; y < placement.Height; y++)
        {
            for (int x = 0; x < placement.Width; x++)
            {
                TileType tileType = fill[x, y];
                if (!TileType.Noop.Equals(tileType))
                {
                    levelDecorated[occurrence.x + x, occurrence.y + y] = tileType;
                }

            }
        }
        GameObject decoration = Instantiate(prefab, parent.transform);

        Vector3 currentRotation = decoration.transform.eulerAngles;
        decoration.transform.eulerAngles = currentRotation + new Vector3(0, prefabRotation, 0);

        //student creation
        int centerX = Mathf.RoundToInt(occurrence.x + placement.Width / 2.0f);
        int centerY = Mathf.RoundToInt(occurrence.y + placement.Height / 2.0f);
        Color pixel = levelHeightTexture.GetPixel(centerX, centerY);
        if (IsBlack(pixel))
        {
            pixel = FindNearestNonBlackPixel(centerX, centerY, levelHeightTexture);
        }

        float yHeight = 0f;
        if (ColorsApproximatelyEqual(pixel, LayoutColorMap.RoomLevel(1))) yHeight = 6f;
        else if (ColorsApproximatelyEqual(pixel, LayoutColorMap.RoomLevel(2))) yHeight = 12f;

        Vector3 center = new Vector3(centerX, yHeight, centerY);
        int scale = SharedLevelData.Instance.Scale;
        decoration.transform.position = (center + new Vector3(-1, 0, -1)) * scale;

        decoration.transform.localScale = Vector3.one * scale;

    }
    // Finds all available positions within the room where the pattern can be applied.
    // Adds in horizontal/vertical centering if requested and checks pattern match at each position.
    private Vector2Int[] FindOccurrences(TileType[,] levelDecorated, Room room)
    {
        List<Vector2Int> occurrences = new List<Vector2Int>();
        int centerX = room.Area.position.x + room.Area.width / 2 - placement.Width / 2;
        int centerY = room.Area.position.y + room.Area.height / 2 - placement.Height / 2;
        for (int y = room.Area.position.y - 1; y < room.Area.position.y + room.Area.height + 2 - placement.Height; y++)
        {
            for (int x = room.Area.position.x - 1; x < room.Area.position.x + room.Area.width + 2 - placement.Width; x++)
            {
                if (centerHorizontally && x != centerX)
                {
                    continue;
                }
                if (centerVertically && y != centerY)
                {
                    continue;
                }
                if (IsPatternAtPosition(levelDecorated, placement, x, y))
                {
                    occurrences.Add(new Vector2Int(x, y));
                }
            }
        }
        return occurrences.ToArray();
    }
    // Checks if the pattern matches exactly at a start position in the level grid.
    // Ignores 'Noop' (don't-care) tiles in the pattern during comparison.
    bool IsPatternAtPosition(TileType[,] levelDecorated, Array2DWrapper<TileType> pattern, int startX, int startY)
    {
        for (int y = 0; y < pattern.Height; y++)
        {
            for (int x = 0; x < pattern.Width; x++)
            {
                if (!TileType.Noop.Equals(pattern[x, y]) && levelDecorated[startX + x, startY + y] != pattern[x, y])
                {
                    return false;
                }
            }
        }
        return true;
    }
    // Compares two colors with a small tolerance, returning true if they are approximately equal.
    // Used to detect height levels based on pixel color in the levelHeightTexture.
    //student creation
    private bool ColorsApproximatelyEqual(Color a, Color b, float tolerance = 0.05f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
            Mathf.Abs(a.g - b.g) < tolerance &&
            Mathf.Abs(a.b - b.b) < tolerance;
    }
    // Checks if a color is approximately black
    private bool IsBlack(Color color, float threshold = 0.05f)
    {
        return color.r < threshold && color.g < threshold && color.b < threshold;
    }

    // Searches in a radius around (x, y) to find the nearest non-black pixel
    private Color FindNearestNonBlackPixel(int x, int y, Texture2D texture, int maxRadius = 5)
    {
        for (int radius = 1; radius <= maxRadius; radius++)
        {
            for (int offsetY = -radius; offsetY <= radius; offsetY++)
            {
                for (int offsetX = -radius; offsetX <= radius; offsetX++)
                {
                    if (offsetX == 0 && offsetY == 0) continue; // Skip center

                    int nx = x + offsetX;
                    int ny = y + offsetY;

                    if (nx >= 0 && nx < texture.width && ny >= 0 && ny < texture.height)
                    {
                        Color neighbor = texture.GetPixel(nx, ny);
                        if (!IsBlack(neighbor))
                        {
                            return neighbor;
                        }
                    }
                }
            }
        }

        return Color.black;
    }
}
