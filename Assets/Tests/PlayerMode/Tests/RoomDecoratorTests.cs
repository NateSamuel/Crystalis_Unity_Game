using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class RoomDecoratorTests
{
    // Checks that InitializeDecoratorArray converts a texture into a TileType array
    [Test]
    public void InitializeDecoratorArraySetsWallsAndFloorsCorrectly()
    {
        var decorator = new GameObject().AddComponent<RoomDecorator>();

        var tex = new Texture2D(2, 2);
        tex.SetPixel(0, 0, Color.black);
        tex.SetPixel(1, 0, Color.white);
        tex.SetPixel(0, 1, Color.white);
        tex.SetPixel(1, 1, Color.white);
        tex.Apply();

        var textureField = typeof(RoomDecorator).GetField("levelTexture",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        textureField.SetValue(decorator, tex);

        var method = typeof(RoomDecorator).GetMethod("InitializeDecoratorArray",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var result = (TileType[,])method.Invoke(decorator, null);

        Assert.AreEqual(TileType.Wall, result[0, 0]);
        Assert.AreEqual(TileType.Floor, result[1, 0]);
        Assert.AreEqual(TileType.Floor, result[0, 1]);
        Assert.AreEqual(TileType.Floor, result[1, 1]);
    }

    // Checks that GenerateTextureFromTileType converts a TileType array back into a Texture2D
    [Test]
    public void GenerateTextureFromTileTypeCreatesCorrectTexture()
    {
        var decorator = new GameObject().AddComponent<RoomDecorator>();

        var decoratedTex = new Texture2D(2, 2);
        var decoratedField = typeof(RoomDecorator).GetField("decoratedTexture",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        decoratedField.SetValue(decorator, decoratedTex);

        TileType[,] tileArray = new TileType[2, 2]
        {
            { TileType.Wall, TileType.Floor },
            { TileType.Floor, TileType.Floor }
        };

        var method = typeof(RoomDecorator).GetMethod("GenerateTextureFromTileType",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        method.Invoke(decorator, new object[] { tileArray });

        var resultTex = (Texture2D)decoratedField.GetValue(decorator);

        Assert.AreEqual(resultTex.GetPixel(0, 0), TileType.Wall.GetColor());
        Assert.AreEqual(resultTex.GetPixel(1, 0), TileType.Floor.GetColor());
    }
}