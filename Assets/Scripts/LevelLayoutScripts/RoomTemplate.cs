using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

//Design by Barbara Reichart lecture series, 2024
//A field that can be edited to make the size of the rectangular rooms larger/smaller

[Serializable]
public class RoomTemplate
{
    [SerializeField] string name;
    [SerializeField] int numberOfRooms;
    [SerializeField] int roomWidthMin = 3;
    [SerializeField] int roomWidthMax = 5;
    [SerializeField] int roomLengthMin = 3;
    [SerializeField] int roomLengthMax = 5;
    [SerializeField] Texture2D layoutTexture;

    public int NumberOfRooms{ get => numberOfRooms; }
    public int RoomWidthMin{ get => roomWidthMin; }
    public int RoomWidthMax{ get => roomWidthMax; }
    public int RoomLengthMin{ get => roomLengthMin; }
    public int RoomLengthMax{ get => roomLengthMax; }
    public Texture2D LayoutTexture => layoutTexture;

    public RectInt GenerateRoomCandidateRect (Random random) {
        if (layoutTexture != null) {
            return new RectInt { width = layoutTexture.width, height = layoutTexture.height};
        }
        RectInt roomCandidateRect = new RectInt {
            width = random.Next(roomWidthMin, roomWidthMax),
            height = random.Next(roomLengthMin, roomLengthMax)
        };
        return roomCandidateRect;
    }
}
