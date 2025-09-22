using NUnit.Framework;
using UnityEngine;

public class TextureBasedLevelTests
{
    // Checks that the constructor sets Width and Length from the texture
    [Test]
    public void ConstructorSetsWidthAndLength()
    {
        Texture2D tex = new Texture2D(5, 10);
        var level = new TextureBasedLevel(tex);

        Assert.AreEqual(5, level.Width);
        Assert.AreEqual(10, level.Length);
    }

    // Checks that IsBlocked is true for blocked colors and coordinates that are out of bounds
    [Test]
    public void IsBlockedReturnsTrueForBlockedColors()
    {
        Texture2D tex = new Texture2D(2, 2);
        tex.SetPixel(0, 0, Color.black);
        tex.SetPixel(1, 0, Color.white);
        tex.Apply();

        var level = new TextureBasedLevel(tex);

        Assert.IsTrue(level.IsBlocked(0, 0));
        Assert.IsFalse(level.IsBlocked(1, 0));
        Assert.IsTrue(level.IsBlocked(-1, 0));
        Assert.IsTrue(level.IsBlocked(0, 2));
    }

    // Checks that IsHallwayEdge picks up on hallway edge colors
    [Test]
    public void IsHallwayEdgeReturnsTrueForHallwayColors()
    {
        Texture2D tex = new Texture2D(1, 2);
        tex.SetPixel(0, 0, Color.red);
        tex.SetPixel(0, 1, Color.blue);
        tex.Apply();

        var level = new TextureBasedLevel(tex);

        Assert.IsTrue(level.IsHallwayEdge(0, 0));
        Assert.IsFalse(level.IsHallwayEdge(0, 1));
    }
}