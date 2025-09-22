using NUnit.Framework;
using UnityEngine;

public class MarchingSquaresTests
{
    // A fake ILevel implementation for testing purposes
    private class FakeLevel : ILevel
    {
        private bool[,] blocked;
        private bool[,] hallwayEdges;

        public FakeLevel(int width, int length)
        {
            Width = width;
            Length = length;
            blocked = new bool[width, length];
            hallwayEdges = new bool[width, length];
        }

        public int Width { get; }
        public int Length { get; }

        public bool IsBlocked(int x, int y) => blocked[x, y];
        public bool IsHallwayEdge(int x, int y) => hallwayEdges[x, y];
        public int Floor(int x, int y) => 0;

        // Marks a tile as blocked and marks a tile as a hallway edge
        public void SetBlocked(int x, int y, bool value) => blocked[x, y] = value;
        public void SetHallwayEdge(int x, int y, bool value) => hallwayEdges[x, y] = value;
    }

    private MarchingSquares marchingSquares;

    // Creates new MarchingSquares
    [SetUp]
    public void Setup()
    {
        marchingSquares = new GameObject().AddComponent<MarchingSquares>();
    }

    // Checks that CalculateTileIndex returns 15 when the 2x2 tiles are blocked
    [Test]
    public void CalculateTileIndexReturnsCorrectIndex_AllBlocked()
    {
        var level = new FakeLevel(2, 2);
        for (int x = 0; x <= 1; x++)
            for (int y = 0; y <= 1; y++)
                level.SetBlocked(x, y, true);

        int index = marchingSquares.InvokeCalculateTileIndex(level, 0, 0);
        Assert.AreEqual(15, index);
    }

    // Checks that CalculateTileIndex returns the correct index for a mixed blocked/unblocked 2x2 tile set
    [Test]
    public void CalculateTileIndexReturnsCorrectIndex_SomeBlocked()
    {
        var level = new FakeLevel(2, 2);
        level.SetBlocked(0, 0, false); // bottomLeft
        level.SetBlocked(1, 0, true);  // bottomRight
        level.SetBlocked(0, 1, true);  // topLeft
        level.SetBlocked(1, 1, false); // topRight

        int index = marchingSquares.InvokeCalculateTileIndex(level, 0, 0);
        Assert.AreEqual(9, index);
    }

    // Checks that CalculateTileIndex returns 0 when any of the tiles are near a hallway edge
    [Test]
    public void CalculateTileIndexReturnsZero_WhenNearHallwayEdge()
    {
        var level = new FakeLevel(2, 2);
        level.SetHallwayEdge(0, 0, true);

        int index = marchingSquares.InvokeCalculateTileIndex(level, 0, 0);
        Assert.AreEqual(0, index);
    }
}

// Extension to call CalculateTileIndex
public static class MarchingSquaresTestExtensions
{
    // Invokes the CalculateTileIndex on MarchingSquares
    public static int InvokeCalculateTileIndex(this MarchingSquares ms, ILevel level, int x, int y)
    {
        var method = typeof(MarchingSquares).GetMethod("CalculateTileIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (int)method.Invoke(ms, new object[] { level, x, y });
    }
}