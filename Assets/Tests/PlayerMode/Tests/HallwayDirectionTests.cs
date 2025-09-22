using NUnit.Framework;
using UnityEngine;

public class HallwayDirectionTests
{
    // Checks that each HallwayDirection returns the correct color, and Undefined returns gray.
    [Test]
    public void GetColorReturnsCorrectColor()
    {
        Assert.AreEqual(new Color(1, 1, 0, 1), HallwayDirection.Left.GetColor());
        Assert.AreEqual(new Color(1, 0, 1, 1), HallwayDirection.Right.GetColor());
        Assert.AreEqual(new Color(0, 1, 1, 1), HallwayDirection.Top.GetColor());
        Assert.AreEqual(new Color(0, 1, 0, 1), HallwayDirection.Bottom.GetColor());
        Assert.AreEqual(Color.gray, HallwayDirection.Undefined.GetColor());
    }

    // Checks that the color-to-direction mapping dictionary maps colors back to the HallwayDirection.
    [Test]
    public void GetColorToDirectionMapReturnsCorrectMappings()
    {
        var map = HallwayDirectionExtension.GetColorToDirectionMap();
        Assert.AreEqual(HallwayDirection.Left, map[new Color(1, 1, 0, 1)]);
        Assert.AreEqual(HallwayDirection.Right, map[new Color(1, 0, 1, 1)]);
        Assert.AreEqual(HallwayDirection.Top, map[new Color(0, 1, 1, 1)]);
        Assert.AreEqual(HallwayDirection.Bottom, map[new Color(0, 1, 0, 1)]);
    }

    // Checks that a HallwayDirection returns the right rotation as a Quaternion.
    [Test]
    public void GetRotationReturnsCorrectQuaternion()
    {
        Assert.AreEqual(Quaternion.identity, HallwayDirection.Top.GetRotation());
        Assert.AreEqual(Quaternion.Euler(0, 90, 0), HallwayDirection.Right.GetRotation());
        Assert.AreEqual(Quaternion.Euler(0, 180, 0), HallwayDirection.Bottom.GetRotation());
        Assert.AreEqual(Quaternion.Euler(0, 270, 0), HallwayDirection.Left.GetRotation());
    }

    // Checks that each HallwayDirection returns the right vector, and Undefined returns zero.
    [Test]
    public void GetOffsetReturnsCorrectVector()
    {
        Assert.AreEqual(Vector2Int.up, HallwayDirection.Top.GetOffset());
        Assert.AreEqual(Vector2Int.down, HallwayDirection.Bottom.GetOffset());
        Assert.AreEqual(Vector2Int.left, HallwayDirection.Left.GetOffset());
        Assert.AreEqual(Vector2Int.right, HallwayDirection.Right.GetOffset());
        Assert.AreEqual(Vector2Int.zero, HallwayDirection.Undefined.GetOffset());
    }

    // Checks that each HallwayDirection returns its opposite direction, and Undefined returns Undefined.
    [Test]
    public void GetOppositeDirectionReturnsCorrectDirection()
    {
        Assert.AreEqual(HallwayDirection.Bottom, HallwayDirection.Top.GetOppositeDirection());
        Assert.AreEqual(HallwayDirection.Top, HallwayDirection.Bottom.GetOppositeDirection());
        Assert.AreEqual(HallwayDirection.Right, HallwayDirection.Left.GetOppositeDirection());
        Assert.AreEqual(HallwayDirection.Left, HallwayDirection.Right.GetOppositeDirection());
        Assert.AreEqual(HallwayDirection.Undefined, HallwayDirection.Undefined.GetOppositeDirection());
    }

    // Checks that each HallwayDirection converts to its Direction enum, and throws for Undefined.
    [Test]
    public void ToDirectionReturnsCorrectDirection()
    {
        Assert.AreEqual(Direction.North, HallwayDirection.Top.ToDirection());
        Assert.AreEqual(Direction.South, HallwayDirection.Bottom.ToDirection());
        Assert.AreEqual(Direction.West, HallwayDirection.Left.ToDirection());
        Assert.AreEqual(Direction.East, HallwayDirection.Right.ToDirection());
        Assert.Throws<System.Exception>(() => HallwayDirection.Undefined.ToDirection());
    }
}