using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Student design overrides previous Barbara Reichart lecture series, 2024 base concept

public enum HallwayDirection { Undefined, Left, Right, Top, Bottom };

// Extension class providing utility functions for HallwayDirection
public static class HallwayDirectionExtension
{

    private static Color yellow = new Color(1, 1, 0, 1);
    private static Color magenta = new Color(1, 0, 1, 1);
    private static Color cyan = new Color(0, 1, 1, 1);
    private static Color green = new Color(0, 1, 0, 1);

    // Map each HallwayDirection to a unique Color
    private static readonly Dictionary<HallwayDirection, Color> DirectionToColorMap = new Dictionary<HallwayDirection, Color> {
        {HallwayDirection.Left, yellow},
        {HallwayDirection.Right, magenta},
        {HallwayDirection.Top, cyan},
        {HallwayDirection.Bottom, green},
    };

    /// Gets the Color associated with a given HallwayDirection.
    /// Returns gray if the direction is undefined or not mapped.
    public static Color GetColor(this HallwayDirection direction)
    {
        return DirectionToColorMap.TryGetValue(direction, out Color color) ? color : Color.gray;
    }

    /// Returns a dictionary that maps Colors back to their corresponding HallwayDirection.
    /// Useful for reverse lookups when detecting directions from texture colors.
    public static Dictionary<Color, HallwayDirection> GetColorToDirectionMap()
    {
        return DirectionToColorMap.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
    }

    public static Quaternion GetRotation(this HallwayDirection dir) => dir switch
    {
        HallwayDirection.Top => Quaternion.identity,
        HallwayDirection.Right => Quaternion.Euler(0, 90, 0),
        HallwayDirection.Bottom => Quaternion.Euler(0, 180, 0),
        HallwayDirection.Left => Quaternion.Euler(0, 270, 0),
        _ => Quaternion.identity,
    };

    public static Vector2Int GetOffset(this HallwayDirection dir) => dir switch
    {
        HallwayDirection.Top => Vector2Int.up,
        HallwayDirection.Bottom => Vector2Int.down,
        HallwayDirection.Left => Vector2Int.left,
        HallwayDirection.Right => Vector2Int.right,
        _ => Vector2Int.zero,
    };

    public static HallwayDirection GetOppositeDirection(this HallwayDirection dir) => dir switch
    {
        HallwayDirection.Top => HallwayDirection.Bottom,
        HallwayDirection.Bottom => HallwayDirection.Top,
        HallwayDirection.Left => HallwayDirection.Right,
        HallwayDirection.Right => HallwayDirection.Left,
        _ => HallwayDirection.Undefined,
    };
    public static Direction ToDirection(this HallwayDirection dir)
    {
        return dir switch {
            HallwayDirection.Top => Direction.North,
            HallwayDirection.Bottom => Direction.South,
            HallwayDirection.Left => Direction.West,
            HallwayDirection.Right => Direction.East,
            _ => throw new System.Exception("Invalid hallway direction for conversion")
        };
    }

}
