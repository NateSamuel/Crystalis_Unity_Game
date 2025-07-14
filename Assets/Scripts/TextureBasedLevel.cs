using UnityEngine;

//Design by Barbara Reichart lecture series, 2024, edited by student
public class TextureBasedLevel : ILevel
{
    Texture2D levelTexture;

    public TextureBasedLevel(Texture2D levelTexture)
    {
        this.levelTexture = levelTexture;
    }
    public int Width => levelTexture.width;
    public int Length => levelTexture.height;

    public int Floor(int x, int y)
    {
        return 0;
    }
    //student creation/edits
    public bool IsBlocked(int x, int y)
    {
        if (x < 0 || y < 0 || x >= levelTexture.width || y >= levelTexture.height)
        {
            return true;
        }

        Color pixel = levelTexture.GetPixel(x, y);

        Color[] blockedColors = new Color[]
        {
            Color.black,
            Color.blue,
            new Color(0.2f, 0.6f, 0.7f),
            Color.gray,
            Color.yellow,
            Color.cyan
        };

        foreach (Color blocked in blockedColors)
        {
            if (ColorsApproximatelyEqual(pixel, blocked))
                return true;
        }

        return false;
    }
    public bool IsHallwayEdge(int x, int y)
    {
        if (x < 0 || y < 0 || x >= levelTexture.width || y >= levelTexture.height)
        {
            return false;
        }
        Color pixel = levelTexture.GetPixel(x, y);
        
        Color[] hallwayColors = new Color[]
        {
            Color.red,
            new Color(0.87f, 0.63f, 0.87f)
        };
        foreach (Color hallwayColor in hallwayColors)
        {
            if (ColorsApproximatelyEqual(pixel, hallwayColor))
                return true;
        }
        return false;
    }

    bool ColorsApproximatelyEqual(Color a, Color b, float tolerance = 0.01f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
            Mathf.Abs(a.g - b.g) < tolerance &&
            Mathf.Abs(a.b - b.b) < tolerance;
    }

    //student creation
    public int GetVerticalLevel(int x, int y)
    {
        if (x < 0 || y < 0 || x >= levelTexture.width || y >= levelTexture.height)
            return 0;

        Color pixel = levelTexture.GetPixel(x, y);

        if (pixel == LayoutColorMap.RoomLevel(0)) return 0;
        if (pixel == LayoutColorMap.RoomLevel(1)) return 1;
        if (pixel == LayoutColorMap.RoomLevel(2)) return 2;

        return 0;
    }

}
