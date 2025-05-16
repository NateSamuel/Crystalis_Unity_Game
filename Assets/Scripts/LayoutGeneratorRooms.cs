using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class LayoutGeneratorRooms : MonoBehaviour
{
    
    [SerializeField] int seed = Environment.TickCount;
    [SerializeField] int width = 64;
    [SerializeField] int length = 64;
    [SerializeField] int roomWidthMin = 3;
    [SerializeField] int roomWidthMax = 5;
    [SerializeField] int roomLengthMin = 3;
    [SerializeField] int roomLengthMax = 5;

    [SerializeField] int minHallwayLength = 3;
    [SerializeField] int maxHallwayLength = 5;

    [SerializeField] int maxRoomCount = 10;

    [SerializeField] GameObject levelLayoutDisplay;

    [SerializeField] List<Hallway> openDoorways;

    Random random;
    Level level;

    [ContextMenu("Generate Level Layout")]
    public void GenerateLevel(){
        random = new Random(seed);

        openDoorways = new List<Hallway>();
        level = new Level(width, length);

        var roomRect = GetStartRoomRect();
        Debug.Log(roomRect);

        Room room = new Room(roomRect);
        List<Hallway> hallways = room.CalculateAllPossibleDoorways(room.Area.width, room.Area.height, 1);

        hallways.ForEach((h) => h.StartRoom = room);
        hallways.ForEach((h) => openDoorways.Add(h));

        level.AddRoom(room);

        Hallway selectedEntryway = openDoorways[random.Next(openDoorways.Count)];

        // Room secondRoom = ConstructAdjacentRoom(selectedEntryway);

        // level.AddRoom(secondRoom);
        // level.AddHallway(selectedEntryway);
        AddRooms();
        DrawLayout(selectedEntryway, roomRect);

        // Room testRoom1 = new Room(new RectInt(3, 6, 6,10));
        // Room testRoom2 = new Room(new RectInt(15, 4, 10, 12));
        // Hallway testHallway = new Hallway(HallwayDirection.Right, new Vector2Int(6,3), testRoom1);
        // testHallway.EndPosition = new Vector2Int(0, 5);
        // testHallway.EndRoom = testRoom2;

        // level.AddRoom(testRoom1);
        // level.AddRoom(testRoom2);
        // level.AddHallway(testHallway);
        // Debug.Log(HallwayDirection.Left.GetOppositeDirection());


        // DrawLayout(roomRect);
    }

    [ContextMenu("Generate new Seed")]
    public void GenerateNewSeed() {
        seed = Environment.TickCount;

    }

    [ContextMenu("Generate new Seed and Level")]
    public void GenerateNewSeedAndLevel() {
        GenerateNewSeed();
        GenerateLevel();
    }
    RectInt GetStartRoomRect(){
        int roomWidth = random.Next(roomWidthMin,roomWidthMax);
        int availableWidthX = width/2 - roomWidth;
        int randomX = random.Next(0,availableWidthX);
        int roomX = randomX + (width/4);

        int roomLength = random.Next(roomLengthMin,roomLengthMax);
        int availableLengthY = length/2 - roomLength;
        int randomY = random.Next(0,availableLengthY);
        int roomY = randomY + (length/4);

        return new RectInt(roomX, roomY, roomWidth, roomLength);
    }

    void DrawLayout(Hallway selectedEntryway = null, RectInt roomCandidateRect = new RectInt()){
        var renderer = levelLayoutDisplay.GetComponent<Renderer>();
        var layoutTexture = (Texture2D) renderer.sharedMaterial.mainTexture;

        layoutTexture.Reinitialize(width, length);
        levelLayoutDisplay.transform.localScale = new Vector3(width, length, 1);

        layoutTexture.FillWithColor(Color.black);

        Array.ForEach(level.Rooms, room => layoutTexture.DrawRectangle(room.Area, Color.white));
        Array.ForEach(level.Hallways, hallway => layoutTexture.DrawLine(hallway.StartPositionAbsolute, hallway.EndPositionAbsolute, Color.white));

        //if (isDebug) {
            
        //}
        layoutTexture.DrawRectangle(roomCandidateRect, Color.blue);

        openDoorways.ForEach(hallway => layoutTexture.SetPixel(hallway.StartPositionAbsolute.x, hallway.StartPositionAbsolute.y, hallway.StartDirection.GetColor()));
        
        if(selectedEntryway != null)
        {
            layoutTexture.SetPixel(selectedEntryway.StartPositionAbsolute.x, selectedEntryway.StartPositionAbsolute.y, Color.red);
        }

        layoutTexture.SaveAsset();
    }

    Hallway SelectHallwayCandidate(RectInt roomCandidateRect, Hallway entryway) {
        Room room = new Room(roomCandidateRect);
        List<Hallway> candidates = room.CalculateAllPossibleDoorways(room.Area.width, room.Area.height, 1);
        HallwayDirection requiredDirection = entryway.StartDirection.GetOppositeDirection();
        List<Hallway> filteredHallwayCandidates = candidates.Where(hallwayCandidate => hallwayCandidate.StartDirection == requiredDirection).ToList();
        return filteredHallwayCandidates.Count > 0 ? filteredHallwayCandidates[random.Next(filteredHallwayCandidates.Count)] : null;
    }

    Vector2Int CalculateRoomPosition(Hallway entryway, int roomWidth, int roomLength, int distance, Vector2Int endPosition) {
        Vector2Int roomPosition = entryway.StartPositionAbsolute;
        switch (entryway.StartDirection) {
            case HallwayDirection.Left:
                roomPosition.x -= distance + roomWidth;
                roomPosition.y -= endPosition.y;
                break;
            case HallwayDirection.Top:
                roomPosition.x -= endPosition.x;
                roomPosition.y += distance + 1;
                break;
            case HallwayDirection.Right:
                roomPosition.x += distance + 1;
                roomPosition.y -= endPosition.y;
                break;
            case HallwayDirection.Bottom:
                roomPosition.x -= endPosition.x;
                roomPosition.y -= distance + roomLength;
                break;
        }
        return roomPosition;
    }

    Room ConstructAdjacentRoom(Hallway selectedEntryway) {
        RectInt roomCandidateRect = new RectInt {
            width = random.Next(roomWidthMin, roomWidthMax),
            height = random.Next(roomLengthMin, roomLengthMax)
        };
        Hallway selectedExit = SelectHallwayCandidate(roomCandidateRect, selectedEntryway);
        if (selectedExit == null) { return null;}
        int distance = random.Next(minHallwayLength, maxHallwayLength + 1);
        Vector2Int roomCandidatePosition = CalculateRoomPosition(selectedEntryway, roomCandidateRect.width, roomCandidateRect.height, distance, selectedExit.StartPosition);
        roomCandidateRect.position = roomCandidatePosition;
        if(!IsRoomCandidateValid(roomCandidateRect))
        {
            return null;
        }
        Room newRoom = new Room(roomCandidateRect);
        selectedEntryway.EndRoom = newRoom;
        selectedEntryway.EndPosition = selectedExit.StartPosition;
        return newRoom;
    }
    void AddRooms()
    {
        while (openDoorways.Count > 0 && level.Rooms.Length < maxRoomCount)
        {
            Hallway SelectedEntryway = openDoorways[random.Next(0, openDoorways.Count)];
            Room newRoom = ConstructAdjacentRoom(SelectedEntryway);

            if (newRoom == null)
            {
                openDoorways.Remove(SelectedEntryway);
                continue;
            }

            level.AddRoom(newRoom);
            level.AddHallway(SelectedEntryway);

            SelectedEntryway.EndRoom = newRoom;
            List<Hallway> newOpenHallways = newRoom.CalculateAllPossibleDoorways(newRoom.Area.width, newRoom.Area.height, 1);
            newOpenHallways.ForEach(hallway => hallway.StartRoom = newRoom);

            openDoorways.Remove(SelectedEntryway);
            openDoorways.AddRange(newOpenHallways);

        }
    }

    bool IsRoomCandidateValid(RectInt roomCandidateRect) {
        RectInt levelRect = new RectInt(1, 1, width - 2, length - 2);
        return levelRect.Contains(roomCandidateRect) && !CheckRoomOverlap(roomCandidateRect, level.Rooms, level.Hallways, 1);
    }

    bool CheckRoomOverlap(RectInt roomCandidateRect, Room[] rooms, Hallway[] hallways, int minRoomDistance)
    {
        RectInt paddedRoomRect = new RectInt {
            x = roomCandidateRect.x - minRoomDistance,
            y = roomCandidateRect.y - minRoomDistance,
            width = roomCandidateRect.width + 2 * minRoomDistance,
            height = roomCandidateRect.height + 2 * minRoomDistance
        };
        foreach (Room room in rooms) {
            if (paddedRoomRect.Overlaps(room.Area)) {
                return true;
            }
        }
        foreach (Hallway hallway in hallways)
        {
            if (paddedRoomRect.Overlaps(hallway.Area)) {
                return true;
            }
        }
        return false;
    }

}
