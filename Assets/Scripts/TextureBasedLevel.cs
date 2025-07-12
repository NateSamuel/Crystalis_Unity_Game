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

    public bool IsBlocked(int x, int y)
    {
        if (x < 0 || y < 0 || x >= levelTexture.width || y >= levelTexture.height)
        {
            return true;
        }
        Color pixel = levelTexture.GetPixel(x, y);

        return Color.black.Equals(pixel) ? true : false;
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
