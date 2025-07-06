using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Design by Barbara Reichart lecture series, 2024

[CreateAssetMenu(fileName = "Tileset", menuName = "Custom/Procedural Generation/Tileset")]
public class Tileset : ScriptableObject
{
    [SerializeField]
    Color wallColor;
    [SerializeField]
    TileVariant[] tiles = new TileVariant[16];

    public Color WallColor => wallColor;

    public GameObject GetTile(int tileIndex)
    {
        if (tileIndex >= tiles.Length)
        {
            return null;
        }
        return tiles[tileIndex].GetRandomTile();
    }
}
