using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//student edits
public static class LayoutColorMap {

    public static Color DefaultHallway = Color.gray;

    static Dictionary<int, Color> levelDeltaColors = new Dictionary<int, Color>
    {
        { -2, new Color(0.3f, 0.0f, 0.3f) }, // down 2
        { -1, Color.red }, // down 1
        {  0, Color.blue },                 // flat
        {  1, Color.yellow },               // up 1
        {  2, Color.cyan },                 // up 2
    };

    public static Color GetHallwayColorByLevelDelta(int levelDelta)
    {
        return levelDeltaColors.TryGetValue(levelDelta, out var color) ? color : DefaultHallway;
    }

    public static Color RoomLevel(int level) {
        switch(level) {
            case 0: return Color.white;
            case 1: return new Color(0.7f, 0.7f, 0.7f);
            case 2: return new Color(0.4f, 0.4f, 0.4f);
            default: return Color.gray;
        }
    }
}