using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = System.Random;

//Design by Barbara Reichart lecture series, 2024

[Serializable]
public class TileVariant
{
    [SerializeField] GameObject[] variants = new GameObject[0];

    public GameObject GetRandomTile() {
        Random random = SharedLevelData.Instance.Rand;
        int randomIndex = random.Next(0, variants.Length);
        return variants[randomIndex];
    }
}
