using UnityEngine;

//Student design overrides previous Barbara Reichart lecture series, 2024 base concept

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
    // Checks whether a tile at the given coordinates is blocked, based on its color and returns true or false.
    // Colours are blocked if they are hallways or the background, rooms are not blocked.
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
    //student creation
    //Checks whether the tile at the given coordinates is a hallway edge based on the level texture pixel colour.
    //Returns true if so.
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
    //student creation
    // Compares two colors to check if they are almost equal.
    bool ColorsApproximatelyEqual(Color a, Color b, float tolerance = 0.01f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
            Mathf.Abs(a.g - b.g) < tolerance &&
            Mathf.Abs(a.b - b.b) < tolerance;
    }

    //student creation
    // Gets the vertical height of a tile based on the pixel color in the texture.
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
