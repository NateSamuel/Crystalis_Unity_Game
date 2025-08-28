using System.Collections.Generic;
using UnityEngine;

//Dictionary use to draw colours for specific info in the layout texture for the decorated elements that are to be added
// - Design by Barbara Reichart lecture series, 2024
public enum TileType { Noop, Empty, Floor, BlockedFloor, Wall, DecoratedWall, Item }

public static class TileTypeExtension
{
    private static readonly Dictionary<TileType, Color> DirectionToColorMap = new Dictionary<TileType, Color>
    {
        { TileType.Wall, Color.black },
        { TileType.Floor, Color.white },
        { TileType.Item, Color.blue },
        { TileType.BlockedFloor, Color.red },
        { TileType.DecoratedWall, Color.yellow }
    };

    public static Color GetColor(this TileType tileType)
    {
        return DirectionToColorMap.TryGetValue(tileType, out Color color) ? color : Color.gray;
    }

}
