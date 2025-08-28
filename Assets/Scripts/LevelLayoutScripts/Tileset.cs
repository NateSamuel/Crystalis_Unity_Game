using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = System.Random;

// //Design by Barbara Reichart lecture series, 2024
// //creates a tileset menu which assigns floor and wall tiles. This will link to information from the leveltexture to create the 3d level
// [CreateAssetMenu(fileName = "Tileset", menuName = "Custom/Procedural Generation/Tileset")]
// public class Tileset : ScriptableObject
// {
//     [SerializeField]
//     Color wallColor;
//     [SerializeField]
//     TileVariant[] tiles = new TileVariant[16];

//     public Color WallColor => wallColor;

//     public GameObject GetTile(int tileIndex)
//     {
//         if (tileIndex >= tiles.Length)
//         {
//             return null;
//         }
//         return tiles[tileIndex].GetRandomTile();
//     }
// }

[CreateAssetMenu(fileName = "Tileset", menuName = "Custom/Procedural Generation/Tileset")]
public class Tileset : ScriptableObject
{
    [Serializable]
    public class TileVariant
    {
        [SerializeField] GameObject[] variants = new GameObject[0];
        [SerializeField] float[] weights = new float[0];

        public GameObject GetRandomTile() {
            Random random = SharedLevelData.Instance.Rand;

            if (variants.Length == 0) return null;
            if (weights.Length != variants.Length) {
                int index = random.Next(0, variants.Length);
                return variants[index];
            }

            float totalWeight = 0;
            foreach (var w in weights) totalWeight += w;

            float r = (float)(random.NextDouble() * totalWeight);
            float cumulative = 0;

            for (int i = 0; i < weights.Length; i++) {
                cumulative += weights[i];
                if (r <= cumulative) return variants[i];
            }

            return variants[variants.Length - 1];
        }
    }

    [SerializeField] Color wallColor;
    [SerializeField] TileVariant[] tiles = new TileVariant[16];

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