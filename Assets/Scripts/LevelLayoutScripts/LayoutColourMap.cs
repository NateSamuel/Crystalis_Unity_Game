using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//student creation
public static class LayoutColorMap {

    public static Color DefaultHallway = Color.gray;
    //assigns colors to numbers for each different hallway situation
    static Dictionary<int, Color> levelDeltaColors = new Dictionary<int, Color>
    {
        { -2, Color.blue },                 // down 2
        { -1, new Color(0.2f, 0.6f, 0.7f) },  // down 1
        {  0, Color.gray },                 // flat
        {  1, Color.yellow },               // up 1
        {  2, Color.cyan },                 // up 2
    };

    public static Color GetHallwayColorByLevelDelta(int levelDelta)
    {
        return levelDeltaColors.TryGetValue(levelDelta, out var color) ? color : DefaultHallway;
    }
    //assigns colors to numbers for each different rooms height
    public static Color RoomLevel(int level) {
        switch(level) {
            case 0: return Color.white;
            case 1: return Color.magenta;
            case 2: return Color.green;
            default: return Color.white;
        }
    }
    public static Dictionary<int, Color> GetAllLevelDeltaColors() => levelDeltaColors;
}