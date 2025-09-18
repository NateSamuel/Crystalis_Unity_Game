using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

//Design by Student and Barbara Reichart lecture series, 2024

//Design by Barbara Reichart lecture series, 2024

//Creates parameters for the level that can be altered in the project
[CreateAssetMenu(fileName = "Room Level Layout", menuName = "Custom/Procedural Generation/RoomLevelLayoutConfiguration")]
public class RoomLevelLayoutConfiguration : ScriptableObject
{
    [SerializeField] int width = 128;
    [SerializeField] int length = 128;

    [SerializeField] RoomTemplate[] roomTemplates;
    [SerializeField] int doorDistanceFromEdge = 1;

    [SerializeField] int minHallwayLength = 3;
    [SerializeField] int maxHallwayLength = 5;

    [SerializeField] int maxRoomCount = 4;
    [SerializeField] int initialMaxRoomCount = 4;
    [SerializeField] int finalMaxRoomCount = 9;
    [SerializeField] int minRoomDistance = 1;

    public int Width{ get => width; }
    public int Length{ get => length; }

    public RoomTemplate[] RoomTemplates { get => roomTemplates; }
    public int DoorDistanceFromEdge{ get => doorDistanceFromEdge ; }
    public int MinHallwayLength{ get => minHallwayLength; }
    public int MaxHallwayLength{ get => maxHallwayLength; }

    //Student creation - used to make the levels increase in size the more levels that are generated
    public int MaxRoomCount
    {
        get => maxRoomCount;
        set => maxRoomCount = value;
    }

    public int InitialMaxRoomCount
    {
        get => initialMaxRoomCount;
        set => initialMaxRoomCount = value;
    }

    public int FinalMaxRoomCount
    {
        get => finalMaxRoomCount;
        set => finalMaxRoomCount = value;
    }
    
    public void ResetMaxRoomCount()
    {
        maxRoomCount = initialMaxRoomCount;
    }

    //Design by Barbara Reichart lecture series, 2024
    public int MinRoomDistance{ get => minRoomDistance; }

    //Assigns room templates to a dictionary and how many are available to be used
    public Dictionary<RoomTemplate, int> GetAvailableRooms()
    {
        Dictionary<RoomTemplate, int> availableRooms = new Dictionary<RoomTemplate, int>();
        for(int i = 0; i < roomTemplates.Length; i++) {
            availableRooms.Add(roomTemplates[i], roomTemplates[i].NumberOfRooms);
        }
        availableRooms = availableRooms.Where(kvp => kvp.Value > 0).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        return availableRooms;

    }
}
