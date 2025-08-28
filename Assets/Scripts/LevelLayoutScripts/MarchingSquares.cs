using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Student design overrides previous Barbara Reichart lecture series, 2024 base concept

public class MarchingSquares : MonoBehaviour
{
    [SerializeField] Texture2D levelTexture;
    [SerializeField] GameObject generatedLevel;
    [SerializeField] Tileset tileset;

    //Destroys previous level, and uses the marching squares algorithm to assign the specific tile types
    //from the newly generated level texture based on the surrounding pixels and vertical level assignments
    [ContextMenu("Create Level Geometry")]
    public void CreateLevelGeometry()
    {
        generatedLevel.transform.DestroyAllChildren();
        int scale = SharedLevelData.Instance.Scale;
        Vector3 scaleVector = new Vector3(scale, scale, scale);
        TextureBasedLevel level = new TextureBasedLevel(levelTexture);

        for (int y = 0; y < level.Length - 1; y++)
        {
            for (int x = 0; x < level.Width - 1; x++)
            {
                int tileIndex = CalculateTileIndex(level, x, y);
                GameObject prefab = tileset.GetTile(tileIndex);
                if (prefab == null) { continue; }

                //student creation
                
                int[] levels = new int[]
                {
                    level.GetVerticalLevel(x, y),
                    level.GetVerticalLevel(x + 1, y),
                    level.GetVerticalLevel(x, y + 1),
                    level.GetVerticalLevel(x + 1, y + 1)
                };

                int highestLevel = Mathf.Max(levels[0], levels[1], levels[2], levels[3]);
                float heightOffset = highestLevel * 12f;

                GameObject tile = Instantiate(prefab, generatedLevel.transform);
                tile.transform.localScale = scaleVector;
                tile.transform.position = new Vector3(x * scale, heightOffset, y * scale);
                tile.name = $"x{x}y{y}_tileIndex{tileIndex}_level{highestLevel}";
            }
        }
    }

    //student creation/edit
    // Calculates the tile index for a pixel set (2x2) on the level texture using the Marching Squares pattern.
    // Returns a number that will represent the specific prefab, i.e. a floor prefab or a wall prefab that is in the correct rotation for that position.
    // If the tile is near a hallway edge, a 0 is returned which represents a plain tile so the hallways aren't blocked off by walls

    int CalculateTileIndex(ILevel level, int x, int y)
    {
        bool nearHallway = 
            level.IsHallwayEdge(x, y + 1) ||
            level.IsHallwayEdge(x + 1, y + 1) ||
            level.IsHallwayEdge(x, y) ||
            level.IsHallwayEdge(x + 1, y);

        if (nearHallway)
        {
            return 0;
        }

        
        int topLeft = level.IsBlocked(x, y + 1) ? 1 : 0;
        int topRight = level.IsBlocked(x + 1, y + 1) ? 1 : 0;
        int bottomLeft = level.IsBlocked(x, y) ? 1 : 0;
        int bottomRight = level.IsBlocked(x + 1, y) ? 1 : 0;
        int tileIndex = topLeft + topRight * 2 + bottomLeft * 4 + bottomRight * 8;

        return tileIndex;
    }
}
