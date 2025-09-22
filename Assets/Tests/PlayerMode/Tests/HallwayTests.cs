using NUnit.Framework;
using UnityEngine;

public class HallwayTests
{
    // Checks that a Hallway is constructed with a starting direction and position
    // Checks rooms are initially null
    [Test]
    public void HallwayCanBeConstructed_WithStartDirectionAndPosition()
    {
        Vector2Int startPos = new Vector2Int(5, 5);
        HallwayDirection direction = HallwayDirection.Top;  

        Hallway hallway = new Hallway(direction, startPos);

        Assert.AreEqual(startPos, hallway.StartPosition);
        Assert.AreEqual(direction, hallway.StartDirection);
        Assert.IsNull(hallway.StartRoom);
        Assert.IsNull(hallway.EndRoom);
    }

    // Checks that StartPosition and EndPosition can be get and set
    [Test]
    public void HallwayCanSetAndGetPositions()
    {
        Vector2Int startPos = new Vector2Int(2, 3);
        Vector2Int endPos = new Vector2Int(7, 8);

        Hallway hallway = new Hallway(HallwayDirection.Top, startPos);

        hallway.EndPosition = endPos;
        hallway.StartPosition = startPos;

        Assert.AreEqual(startPos, hallway.StartPosition);
        Assert.AreEqual(endPos, hallway.EndPosition);
    }

    // Checks that the EndDirection can be get and set correctly.
    [Test]
    public void HallwayCanSetAndGetEndDirection()
    {
        Hallway hallway = new Hallway(HallwayDirection.Top, new Vector2Int(0, 0));

        hallway.EndDirection = HallwayDirection.Bottom;

        Assert.AreEqual(HallwayDirection.Bottom, hallway.EndDirection);
    }
}