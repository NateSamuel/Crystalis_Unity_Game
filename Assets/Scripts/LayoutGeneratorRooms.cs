using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

//Initial design by Barbara Reichart lecture series, 2024, edits by student
public class LayoutGeneratorRooms : MonoBehaviour
{
    
    [SerializeField] int seed = Environment.TickCount;
    [SerializeField] private RoomLevelLayoutConfiguration levelConfig;
    public RoomLevelLayoutConfiguration LevelConfig => levelConfig;
    [SerializeField] GameObject levelLayoutDisplay;

    [SerializeField] List<Hallway> openDoorways;

    Random random;
    Level level;
    Dictionary<RoomTemplate, int> availableRooms;

    [ContextMenu("Generate Level Layout")]
    public Level GenerateLevel(){
        SharedLevelData.Instance.ResetRandom();
        random = SharedLevelData.Instance.Rand;
        availableRooms = levelConfig.GetAvailableRooms();

        openDoorways = new List<Hallway>();
        level = new Level(levelConfig.Width, levelConfig.Length);

        RoomTemplate startRoomTemplate = availableRooms.Keys.ElementAt(random.Next(0,availableRooms.Count));

        RectInt roomRect = GetStartRoomRect(startRoomTemplate);

        Room room = CreateNewRoom(roomRect, startRoomTemplate, 0);
        List<Hallway> hallways = room.CalculateAllPossibleDoorways(room.Area.width, room.Area.height, levelConfig.DoorDistanceFromEdge);

        hallways.ForEach((h) => h.StartRoom = room);
        hallways.ForEach((h) => openDoorways.Add(h));

        level.AddRoom(room);

        Hallway selectedEntryway = openDoorways[random.Next(openDoorways.Count)];

        AddRooms();
        AddHallwaysToRooms();
        AssignRoomTypes();
        DrawLayout(selectedEntryway, roomRect);

        return level;

    }
    void AssignRoomTypes()
    {
        List<Room> borderRooms = level.Rooms.Where(room => room.Connectedness == 1).ToList();
        if(borderRooms.Count < 2)
        {
            return;
        }
        int startRoomIndex = random.Next(0, borderRooms.Count);
        Room randomStartRoom = borderRooms[startRoomIndex];
        level.playerStartRoom = randomStartRoom;
        randomStartRoom.Type = RoomType.Start;
        borderRooms.Remove(randomStartRoom);

        Room farthestRoom = borderRooms
            .OrderByDescending(room => Vector2.Distance(randomStartRoom.Area.center, room.Area.center))
            .FirstOrDefault();
        farthestRoom.Type = RoomType.Exit;
        borderRooms.Remove(farthestRoom);

        List<Room> treasureRooms = borderRooms.OrderBy(r => random.Next()).Take(3).ToList();
        borderRooms.RemoveAll(room => treasureRooms.Contains(room));
        treasureRooms.ForEach(room => room.Type = RoomType.Treasure);

        List<Room> emptyRooms = level.Rooms.Where(room => room.Type.HasFlag(RoomType.Default)).ToList();

        Room bossRoom = emptyRooms
            .OrderByDescending(room => Vector2.Distance(randomStartRoom.Area.center, room.Area.center))
            .OrderByDescending(room => room.Connectedness)
            .OrderByDescending(room => room.Area.width * room.Area.height)
            .FirstOrDefault();
        //student debug addition
        if (bossRoom != null)
        {
            bossRoom.Type = RoomType.Boss;
            emptyRooms.Remove(bossRoom);
        }
        else
        {
            Debug.LogWarning("No available room to assign as Boss room.");
        }
        emptyRooms = emptyRooms.OrderBy(room => random.Next()).ToList();
        RoomType[] typesToAssign = { RoomType.Prison, RoomType.Library, RoomType.Kitchen };
        List<Room> roomsToAssign = emptyRooms.Take(typesToAssign.Length).ToList();
        for (int i = 0; i < roomsToAssign.Count; i++)
        {
            roomsToAssign[i].Type = typesToAssign[i];
        }

    }


    void AddHallwaysToRooms()
    {
        foreach (Room room in level.Rooms)
        {
            Hallway[] hallwaysStartingAtRoom = Array.FindAll(level.Hallways, hallway => hallway.StartRoom == room);
            Array.ForEach(hallwaysStartingAtRoom, hallway => room.AddHallway(hallway));
            Hallway[] hallwaysEndingAtRoom = Array.FindAll(level.Hallways, hallway => hallway.EndRoom == room);
            Array.ForEach(hallwaysEndingAtRoom, hallway => room.AddHallway(hallway));
        }
    }

    [ContextMenu("Generate new Seed")]
    public void GenerateNewSeed() {
        //seed = Environment.TickCount;
        SharedLevelData.Instance.GenerateSeed();

    }

    [ContextMenu("Generate new Seed and Level")]
    public void GenerateNewSeedAndLevel() {
        GenerateNewSeed();
        GenerateLevel();
    }
    RectInt GetStartRoomRect(RoomTemplate roomTemplate){

        RectInt roomSize = roomTemplate.GenerateRoomCandidateRect(random);

        int roomWidth = roomSize.width;
        int availableWidthX = level.Width/2 - roomWidth;
        int randomX = random.Next(0,availableWidthX);
        int roomX = randomX + (level.Width/4);

        int roomLength = roomSize.height;
        int availableLengthY = level.Length/2 - roomLength;
        int randomY = random.Next(0,availableLengthY);
        int roomY = randomY + (level.Length/4);

        return new RectInt(roomX, roomY, roomWidth, roomLength);
    }
    //student creation
    Texture2D TintTexture(Texture2D original, Color tint)
    {
        Texture2D tinted = new Texture2D(original.width, original.height);

        for (int y = 0; y < original.height; y++)
        {
            for (int x = 0; x < original.width; x++)
            {
                Color pixel = original.GetPixel(x, y);

                // Step 1: Make anything not black pure white (preserve alpha)
                if (pixel.r > 0.01f || pixel.g > 0.01f || pixel.b > 0.01f)
                {
                    pixel = new Color(1f, 1f, 1f, pixel.a);
                }

                // Step 2: Apply tint only to white areas
                Color tintedPixel = pixel * tint;
                tintedPixel.a = pixel.a;

                tinted.SetPixel(x, y, tintedPixel);
            }
        }

        tinted.Apply();
        return tinted;
    }
    void DrawLayout(Hallway selectedEntryway = null, RectInt roomCandidateRect = new RectInt(), bool isDebug = false){
        var renderer = levelLayoutDisplay.GetComponent<Renderer>();
        var layoutTexture = (Texture2D) renderer.sharedMaterial.mainTexture;

        layoutTexture.Reinitialize(level.Width, level.Length);
        int scale = SharedLevelData.Instance.Scale;
        levelLayoutDisplay.transform.localScale = new Vector3(level.Width * scale, level.Length * scale, 1);
        float xPos = level.Width * scale / 2.0f - scale;
        float zPos = level.Length * scale / 2.0f - scale;
        levelLayoutDisplay.transform.position = new Vector3(xPos, 100f, zPos);
        layoutTexture.FillWithColor(Color.black);

        //student creation
        foreach (Hallway hallway in level.Hallways)
        {
            int levelDelta = hallway.LevelDelta;
            Color hallwayColor = LayoutColorMap.GetHallwayColorByLevelDelta(levelDelta);
            layoutTexture.DrawLine(hallway.StartPositionAbsolute, hallway.EndPositionAbsolute, 2, hallwayColor);
        }
        foreach (Room room in level.Rooms) {
            if (room.LayoutTexture != null) {
                Texture2D tinted = TintTexture(room.LayoutTexture, LayoutColorMap.RoomLevel(room.VerticalLevel));
                layoutTexture.DrawTexture(tinted, room.Area);
            } else {
                layoutTexture.DrawRectangle(room.Area, LayoutColorMap.RoomLevel(room.VerticalLevel));
            }
            Debug.Log(room.Area + " " + room.Connectedness + " " + room.Type);
        }
        //Array.ForEach(level.Hallways, hallway => layoutTexture.DrawLine(hallway.StartPositionAbsolute, hallway.EndPositionAbsolute, 2, Color.blue));
        
        //layoutTexture.ConvertToBlackAndWhite();
        if (isDebug) {
            layoutTexture.DrawRectangle(roomCandidateRect, Color.blue);
            openDoorways.ForEach(hallway => layoutTexture.SetPixel(hallway.StartPositionAbsolute.x, hallway.StartPositionAbsolute.y, hallway.StartDirection.GetColor()));
        }
        
        if(isDebug && selectedEntryway != null)
        {
            layoutTexture.SetPixel(selectedEntryway.StartPositionAbsolute.x, selectedEntryway.StartPositionAbsolute.y, Color.red);
        }

        layoutTexture.SaveAsset();
    }

    Hallway SelectHallwayCandidate(RectInt roomCandidateRect, RoomTemplate roomTemplate, Hallway entryway) {

        Room room = CreateNewRoom(roomCandidateRect, roomTemplate, 0, false);
        List<Hallway> candidates = room.CalculateAllPossibleDoorways(room.Area.width, room.Area.height, levelConfig.DoorDistanceFromEdge);
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

    Room ConstructAdjacentRoom(Hallway selectedEntryway) 
    {

        RoomTemplate roomTemplate = availableRooms.Keys.ElementAt(random.Next(0, availableRooms.Count));
        RectInt roomCandidateRect = roomTemplate.GenerateRoomCandidateRect(random);

        Hallway selectedExit = SelectHallwayCandidate(roomCandidateRect, roomTemplate, selectedEntryway);
        if (selectedExit == null && availableRooms.Count > 0)
        {
            for (int r = 0; r < availableRooms.Count; r++)
            {
                roomTemplate = availableRooms.Keys.ElementAt(random.Next(0, availableRooms.Count));
                roomCandidateRect = roomTemplate.GenerateRoomCandidateRect(random);
                selectedExit = SelectHallwayCandidate(roomCandidateRect, roomTemplate, selectedEntryway);

                if (selectedExit != null)
                {
                    break;
                }
            }
        }
        if (selectedExit == null) { return null;}
        int distance = random.Next(levelConfig.MinHallwayLength, levelConfig.MaxHallwayLength + 1);
        Vector2Int roomCandidatePosition = CalculateRoomPosition(selectedEntryway, roomCandidateRect.width, roomCandidateRect.height, distance, selectedExit.StartPosition);
        roomCandidateRect.position = roomCandidatePosition;
        if(!IsRoomCandidateValid(roomCandidateRect))
        {
            return null;
        }
        //student work
        int verticalLevel = random.Next(0, 3);
        Room newRoom = CreateNewRoom(roomCandidateRect, roomTemplate, verticalLevel, true);




        selectedEntryway.EndRoom = newRoom;
        selectedEntryway.EndPosition = selectedExit.StartPosition;
        return newRoom;
    }
    void AddRooms()
    {
        while (openDoorways.Count > 0 && level.Rooms.Length < levelConfig.MaxRoomCount && availableRooms.Count > 0)
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
            List<Hallway> newOpenHallways = newRoom.CalculateAllPossibleDoorways(newRoom.Area.width, newRoom.Area.height, levelConfig.DoorDistanceFromEdge);
            newOpenHallways.ForEach(hallway => hallway.StartRoom = newRoom);

            openDoorways.Remove(SelectedEntryway);
            openDoorways.AddRange(newOpenHallways);

        }
    }
    private void UseUpRoomTemplate(RoomTemplate roomTemplate)
    {
        availableRooms[roomTemplate] -= 1;
        if (availableRooms[roomTemplate] ==0)
        {
            availableRooms.Remove(roomTemplate);
        }
    }

    bool IsRoomCandidateValid(RectInt roomCandidateRect) {
        RectInt levelRect = new RectInt(1, 1, level.Width - 2, level.Length - 2);
        return levelRect.Contains(roomCandidateRect) && !CheckRoomOverlap(roomCandidateRect, level.Rooms, level.Hallways, levelConfig.MinRoomDistance);
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

    Room CreateNewRoom(RectInt roomCandidateRect, RoomTemplate roomTemplate, int verticalLevel, bool useUp = true)
    {
        if (useUp)
        {
            UseUpRoomTemplate(roomTemplate);
        }

        Room room;

        if (roomTemplate.LayoutTexture == null)
        {
            room = new Room(roomCandidateRect, verticalLevel);
        }
        else
        {
            room = new Room(roomCandidateRect.x, roomCandidateRect.y, roomTemplate.LayoutTexture, verticalLevel);
        }

        room.VerticalLevel = verticalLevel;

        return room;
    }

}
