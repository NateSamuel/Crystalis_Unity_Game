using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Design by Barbara Reichart lecture series, 2024
public class Level
{
    int width;
    int length;
    List<Room> rooms;
    List<Hallway> hallways;

    public int Width => width;
    public int Length => length;

    public Room[] Rooms => rooms.ToArray();
    public Hallway[] Hallways => hallways.ToArray();
    public Room playerStartRoom { get; set; }


    public Level (int width, int length){
        this.width = width;
        this.length = length;
        rooms = new List<Room> ();
        hallways = new List<Hallway> ();
    }

    public void AddRoom(Room newRoom) => rooms.Add(newRoom);
    public void AddHallway(Hallway hallway) => hallways.Add(hallway);

}
