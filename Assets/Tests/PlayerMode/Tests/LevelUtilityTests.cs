using NUnit.Framework;
using UnityEngine;

public class LevelUtilityTests
{
    private Texture2D testTexture;
    private Color validColor1 = Color.red;
    private Color validColor2 = Color.green;

    // Creates a 3x3 texture with valid colors
    [SetUp]
    public void SetUp()
    {
        testTexture = new Texture2D(3, 3);

        testTexture.SetPixel(0, 0, validColor1);
        testTexture.SetPixel(1, 1, validColor2);
        testTexture.SetPixel(2, 2, Color.blue);
        testTexture.Apply();
    }

    // Checks that IsValidRoomColor returns true for valid colours
    [Test]
    public void IsValidRoomColorReturnsTrueForValidColor()
    {
        Color[] validColors = { validColor1, validColor2 };
        Assert.IsTrue(LevelUtility.IsValidRoomColor(validColor1, validColors));
        Assert.IsTrue(LevelUtility.IsValidRoomColor(validColor2, validColors));
    }

    // Checks that IsValidRoomColor returns false for in valid colours
    [Test]
    public void IsValidRoomColorReturnsFalseForInvalidColor()
    {
        Color[] validColors = { validColor1, validColor2 };
        Assert.IsFalse(LevelUtility.IsValidRoomColor(Color.blue, validColors));
    }

    // Checks that LevelPositionToWorldPosition returns the correct height for a specific pixel
    [Test]
    public void LevelPositionToWorldPositionReturnsCorrectYHeight()
    {
        Vector2 pixelPos = new Vector2(0, 0);
        Vector3 worldPos = LevelUtility.LevelPositionToWorldPosition(pixelPos, testTexture, 10);

        Assert.AreEqual(0f, worldPos.y);
    }

    // Checks that WorldPositionToTexturePixel changes world coordinates to texture pixel coordinates.
    [Test]
    public void WorldPositionToTexturePixelReturnsCorrectPixel()
    {
        Vector3 worldPos = new Vector3(10, 0, 20);
        Vector2 pixelPos = LevelUtility.WorldPositionToTexturePixel(worldPos, 10);

        Assert.AreEqual(2f, pixelPos.x);
        Assert.AreEqual(3f, pixelPos.y);
    }

    // Checks that PickRandomValidPixelOnTexture returns a pixel that matches a correct color.
    [Test]
    public void PickRandomValidPixelOnTextureReturnsValidPixel()
    {
        Color[] validColors = { validColor1, validColor2 };
        Vector2 pixel = LevelUtility.PickRandomValidPixelOnTexture(testTexture, validColors, 100);

        Color returnedColor = testTexture.GetPixel((int)pixel.x, (int)pixel.y);
        Assert.IsTrue(LevelUtility.IsValidRoomColor(returnedColor, validColors));
    }

    // Checks that PickRandomValidPixelOnTexture returns Vector2.zero if no correct texture pixel exists.
    [Test]
    public void PickRandomValidPixelOnTextureReturnsZeroIfNoValidPixels()
    {
        Color[] validColors = { Color.magenta };
        Vector2 pixel = LevelUtility.PickRandomValidPixelOnTexture(testTexture, validColors, 10);

        Assert.AreEqual(Vector2.zero, pixel);
    }
}