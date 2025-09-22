
//Initial design by Barbara Reichart lecture series, 2024, student additions for height placement, bug fixes and converting it to be able to be used at runtime
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[CreateAssetMenu(fileName = "PatternDecoratorRule", menuName = "Custom/Procedural Generation/PatternDecoratorRule")]
public class PatternMatchingDecoratorRule : BaseDecoratorRule
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private float prefabRotation = 0f;
    [SerializeField] public Array2DWrapper<TileType> placement;
    [SerializeField] private Array2DWrapper<TileType> fill;
    [SerializeField] private bool centerHorizontally = false;
    [SerializeField] private bool centerVertically = false;
    [SerializeField] private Texture2D levelHeightTexture;

    //Clone for runtime
    public PatternMatchingDecoratorRule CloneForRuntime()
    {
        var clone = ScriptableObject.CreateInstance<PatternMatchingDecoratorRule>();
        clone.prefab = prefab;
        clone.prefabRotation = prefabRotation;
        clone.centerHorizontally = centerHorizontally;
        clone.centerVertically = centerVertically;
        clone.levelHeightTexture = levelHeightTexture;
        clone.placement = placement?.DeepCopy() ?? new Array2DWrapper<TileType>(1, 1);
        clone.fill = fill?.DeepCopy() ?? new Array2DWrapper<TileType>(1, 1);
        clone.name = name + "_RuntimeClone";
        return clone;
    }
    // Checks if the rule can be applied to the given room by searching for pattern occurrences.
    // Returns true if at least one available position is found.
    internal override bool CanBeApplied(TileType[,] levelDecorated, Room room)
    {
        if (placement == null || fill == null || prefab == null || levelHeightTexture == null)
        {
            Debug.LogWarning($"[PatternMatchingDecoratorRule] {name} missing fields!");
            return false;
        }
        return FindOccurrences(levelDecorated, room).Length > 0;
    }

    // Applies the decorator rule by placing a prefab at a randomly chosen valid pattern match position.
    // Updates the tile data, instantiates the decoration prefab, rotates and positions it based on layout height.
    internal override void Apply(TileType[,] levelDecorated, Room room, Transform parent)
    {

        if (prefab == null || placement == null || fill == null || levelHeightTexture == null)
        {
            return;
        }

        if (parent == null)
        {
            GameObject runtimeParent = new GameObject($"Decorations_{name}");
            parent = runtimeParent.transform;
        }

        // Find all valid positions
        Vector2Int[] occurrences = FindOccurrences(levelDecorated, room);
        if (occurrences.Length == 0)
        {
            return;
        }

        Random random = SharedLevelData.Instance.Rand;

        // Rattempt multiple positions before failing
        int maxAttempts = occurrences.Length * 2;
        int attempts = 0;

        List<Vector2Int> remaining = new List<Vector2Int>(occurrences);

        while (attempts < maxAttempts && remaining.Count > 0)
        {
            int index = random.Next(remaining.Count);
            Vector2Int pos = remaining[index];
            remaining.RemoveAt(index);
            attempts++;

            try
            {
                // Fill tiles
                for (int y = 0; y < placement.Height; y++)
                    for (int x = 0; x < placement.Width; x++)
                        if (!TileType.Noop.Equals(fill[x, y]))
                            levelDecorated[pos.x + x, pos.y + y] = fill[x, y];

                // Instantiate prefab
                GameObject decoration = GameObject.Instantiate(prefab, parent);
                decoration.transform.eulerAngles += new Vector3(0, prefabRotation, 0);

                // Find world position
                int centerX = Mathf.RoundToInt(pos.x + placement.Width / 2f);
                int centerY = Mathf.RoundToInt(pos.y + placement.Height / 2f);
                Color pixel = levelHeightTexture.GetPixel(centerX, centerY);
                if (IsBlack(pixel)) pixel = FindNearestNonBlackPixel(centerX, centerY, levelHeightTexture);

                float yHeight = ColorsApproximatelyEqual(pixel, LayoutColorMap.RoomLevel(1)) ? 6f :
                                ColorsApproximatelyEqual(pixel, LayoutColorMap.RoomLevel(2)) ? 12f : 0f;

                int scale = SharedLevelData.Instance.Scale;
                Vector3 worldPos = (new Vector3(centerX, yHeight, centerY) + new Vector3(-1, 0, -1)) * scale;

                decoration.transform.position = worldPos;
                decoration.transform.localScale = Vector3.one * scale;
                decoration.SetActive(true);

                break;
            }
            catch
            {
                continue;
            }
        }
    }
    
    // Finds all available positions within the room where the pattern can be applied.
    // Adds in horizontal/vertical centering if requested and checks pattern match at each position.
    private Vector2Int[] FindOccurrences(TileType[,] levelDecorated, Room room)
    {
        var occurrences = new System.Collections.Generic.List<Vector2Int>();
        int centerX = room.Area.position.x + room.Area.width / 2 - placement.Width / 2;
        int centerY = room.Area.position.y + room.Area.height / 2 - placement.Height / 2;

        for (int y = room.Area.position.y - 1; y <= room.Area.position.y + room.Area.height + 1 - placement.Height; y++)
        {
            for (int x = room.Area.position.x - 1; x <= room.Area.position.x + room.Area.width + 1 - placement.Width; x++)
            {
                if (centerHorizontally && x != centerX) continue;
                if (centerVertically && y != centerY) continue;
                if (IsPatternAtPosition(levelDecorated, placement, x, y))
                    occurrences.Add(new Vector2Int(x, y));
            }
        }

        return occurrences.ToArray();
    }
    // Checks if the pattern matches exactly at a start position in the level grid.
    // Ignores 'Noop' (don't-care) tiles in the pattern during comparison.
    private bool IsPatternAtPosition(TileType[,] levelDecorated, Array2DWrapper<TileType> pattern, int startX, int startY)
    {
        for (int y = 0; y < pattern.Height; y++)
            for (int x = 0; x < pattern.Width; x++)
                if (!TileType.Noop.Equals(pattern[x, y]) && levelDecorated[startX + x, startY + y] != pattern[x, y])
                    return false;
        return true;
    }
    // Compares two colors with a small tolerance, returning true if they are approximately equal.
    // Used to detect height levels based on pixel color in the levelHeightTexture.
    //student creation
    private bool ColorsApproximatelyEqual(Color a, Color b, float tolerance = 0.05f)
        => Mathf.Abs(a.r - b.r) < tolerance &&
           Mathf.Abs(a.g - b.g) < tolerance &&
           Mathf.Abs(a.b - b.b) < tolerance;

    // Searches in a radius around (x, y) to find the nearest non-black pixel
    private bool IsBlack(Color color, float threshold = 0.05f)
        => color.r < threshold && color.g < threshold && color.b < threshold;

    private Color FindNearestNonBlackPixel(int x, int y, Texture2D texture, int maxRadius = 5)
    {
        for (int radius = 1; radius <= maxRadius; radius++)
        {
            for (int offsetY = -radius; offsetY <= radius; offsetY++)
            {
                for (int offsetX = -radius; offsetX <= radius; offsetX++)
                {
                    if (offsetX == 0 && offsetY == 0) continue;
                    int nx = x + offsetX;
                    int ny = y + offsetY;
                    if (nx >= 0 && nx < texture.width && ny >= 0 && ny < texture.height)
                    {
                        Color neighbor = texture.GetPixel(nx, ny);
                        if (!IsBlack(neighbor)) return neighbor;
                    }
                }
            }
        }
        return Color.black;
    }
}