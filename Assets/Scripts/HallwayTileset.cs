// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

//Student Creation

// [CreateAssetMenu(fileName = "HallwayTileset", menuName = "Custom/Procedural Generation/HallwayTileset")]
// public class HallwayTileset : ScriptableObject
// {
//     [System.Serializable]
//     public class HallwayVariant
//     {
//         public int levelDelta;
//         public Direction direction;
//         public GameObject prefab;
//     }

//     public List<HallwayVariant> hallwayVariants;

//     public GameObject GetHallwayTile(int levelDelta, Direction direction)
//     {
//         foreach (var variant in hallwayVariants)
//         {
//             if (variant.levelDelta == levelDelta && variant.direction == direction)
//             {
//                 return variant.prefab;
//             }
//         }

//         return null;
//     }
// }