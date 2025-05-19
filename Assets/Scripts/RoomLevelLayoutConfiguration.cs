using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Room Level Layout", menuName = "Custom/Procedural Generation/RoomLevelLayoutConfiguration")]
public class RoomLevelLayoutConfiguration : ScriptableObject
{
    [SerializeField] int width = 64;
    [SerializeField] int length = 64;

    [SerializeField] RoomTemplate[] roomTemplates;
    [SerializeField] int doorDistanceFromEdge = 1;

    [SerializeField] int minHallwayLength = 3;
    [SerializeField] int maxHallwayLength = 5;

    [SerializeField] int maxRoomCount = 10;
    [SerializeField] int minRoomDistance = 1;

    public int Width{ get => width; }
    public int Length{ get => length; }

    public RoomTemplate[] RoomTemplates { get => roomTemplates; }
    public int DoorDistanceFromEdge{ get => doorDistanceFromEdge ; }
    public int MinHallwayLength{ get => minHallwayLength; }
    public int MaxHallwayLength{ get => maxHallwayLength; }
    public int MaxRoomCount{ get => maxRoomCount; }
    public int MinRoomDistance{ get => minRoomDistance; }

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
