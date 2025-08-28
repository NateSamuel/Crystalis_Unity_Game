using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flags = System.FlagsAttribute;
//Design by Barbara Reichart lecture series, 2024

// Enum representing different types of rooms using flag attributes for bitwise operations
[Flags]
public enum RoomType
{
    Default = 1,
    Start = 1 << 1,
    Exit = 1 << 2,
    Boss = 1 << 3,
    Treasure = 1 << 4,
}

// A room created in the level that has information about the doorway positions for the hallways, the type of room it is
public class Room
{
    List<Hallway> hallways;
    RectInt area;
    public RectInt Area {get {return area;}}
    public Texture2D LayoutTexture { get; }
    public RoomType Type { get; set;} = RoomType.Default;
    public int Connectedness => hallways.Count;

    //student work adding the vertical level
    public int VerticalLevel { get; set; } = 0;

    public Room(RectInt area, int verticalLevel)
    {
        this.area = area;
        this.VerticalLevel = verticalLevel;
        hallways = new List<Hallway>();
    }
    internal Room(int x, int y, Texture2D layoutTexture, int verticalLevel)
    {
        area = new RectInt(x, y, layoutTexture.width, layoutTexture.height);
        LayoutTexture = layoutTexture;
        hallways = new List<Hallway>();
        VerticalLevel = verticalLevel;
    }

    // Finds all available doorway positions for both room types
    public List <Hallway> CalculateAllPossibleDoorways(int width, int length, int minDistanceFromEdge) {
        if (LayoutTexture == null)
        {
            return CalculateAllPossibleDoorwaysForRectangularRooms(width, length, minDistanceFromEdge);
        }
        else
        {
            return CalculateAllPossibleDoorwayPositions(LayoutTexture);
        }
    }

    // Finds doorway positions for rectangular rooms
    public List <Hallway> CalculateAllPossibleDoorwaysForRectangularRooms(int width, int length, int minDistanceFromEdge) {
        List<Hallway> hallwayCandidates = new List<Hallway>();

        int top = length - 1;
        int minX = minDistanceFromEdge;
        int maxX = width - minDistanceFromEdge;

        for (int x = minX; x < maxX; x++) {
            hallwayCandidates.Add(new Hallway(HallwayDirection.Bottom, new Vector2Int(x, 0)));
            hallwayCandidates.Add(new Hallway(HallwayDirection.Top, new Vector2Int(x, top)));

        }

        int right = width - 1;
        int minY = minDistanceFromEdge;
        int maxY = length - minDistanceFromEdge;

        for (int y = minY; y < maxY; y++){
            hallwayCandidates.Add(new Hallway(HallwayDirection.Left, new Vector2Int(0,y)));
            hallwayCandidates.Add(new Hallway(HallwayDirection.Right, new Vector2Int(right,y)));
        }

        return hallwayCandidates;
    }

    // Finds doorway positions for premade room designs
    List<Hallway> CalculateAllPossibleDoorwayPositions(Texture2D layoutTexture) {
        List<Hallway> possibleHallwayPositions = new List<Hallway>();

        int width = layoutTexture.width;
        int height = layoutTexture.height;

        for(int y = 0; y < height; y++)
        {
            for (int x= 0; x < width; x++)
            {
                Color pixelColor = layoutTexture.GetPixel(x,y);
                HallwayDirection direction = GetHallwayDirection(pixelColor);
                if (direction != HallwayDirection.Undefined)
                {
                    Hallway hallway = new Hallway(direction, new Vector2Int(x, y));
                    possibleHallwayPositions.Add(hallway);
                }
            }
        }
        return possibleHallwayPositions;

    }
    //assigns the hallway direction from the pixel color in the premade room design
    HallwayDirection GetHallwayDirection(Color color)
    {
        Dictionary<Color, HallwayDirection> colorToDirectionMap = HallwayDirectionExtension.GetColorToDirectionMap();
        return colorToDirectionMap.TryGetValue(color, out HallwayDirection direction) ? direction : HallwayDirection.Undefined;
    }

    //add a new hallway to this room
    public void AddHallway(Hallway selectedHallway)
    {
        hallways.Add(selectedHallway);
    }

}
