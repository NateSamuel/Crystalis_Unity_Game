//Full class is student creation
using UnityEngine;
using System.Collections.Generic;

//level utility for enemy placement due to dealing with differing heights for spells/ movement / pathfinding etc.
public static class LevelUtility
{
    //converts position of texture to the world position
    public static Vector3 LevelPositionToWorldPosition(Vector2 pixelPos, Texture2D heightTexture, int scale)
    {
        int texX = Mathf.Clamp((int)pixelPos.x, 0, heightTexture.width - 1);
        int texY = Mathf.Clamp((int)pixelPos.y, 0, heightTexture.height - 1);

        Color pixel = heightTexture.GetPixel(texX, texY);
        float yHeight = 0f;

        if (ColorsApproximatelyEqual(pixel, LayoutColorMap.RoomLevel(1)))
            yHeight = 12f;
        else if (ColorsApproximatelyEqual(pixel, LayoutColorMap.RoomLevel(2)))
            yHeight = 24f;

        float worldX = (pixelPos.x - 1) * scale;
        float worldZ = (pixelPos.y - 1) * scale;

        return new Vector3(worldX, yHeight, worldZ);
    }
    //converts world positon back to texture pixel position
    public static Vector2 WorldPositionToTexturePixel(Vector3 worldPos, int scale)
    {
        float x = worldPos.x / scale + 1;
        float y = worldPos.z / scale + 1;
        return new Vector2(x, y);
    }
    //If input colors are roughly equal, returns true
    public static bool ColorsApproximatelyEqual(Color a, Color b, float tolerance = 0.05f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance;
    }

    //if one color is within one of the valid colors
    public static bool IsValidRoomColor(Color color, Color[] validColors)
    {
        foreach (var valid in validColors)
        {
            if (ColorsApproximatelyEqual(color, valid))
                return true;
        }
        return false;
    }

    //
    public static Vector2 PickRandomValidPixelOnTexture(Texture2D texture, Color[] validColors, int maxAttempts = 1000)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            int x = Random.Range(0, texture.width);
            int y = Random.Range(0, texture.height);

            Color pixelColor = texture.GetPixel(x, y);
            if (IsValidRoomColor(pixelColor, validColors))
                return new Vector2(x, y);
        }

        Debug.LogWarning("Failed to find valid patrol pixel on texture.");
        return Vector2.zero;
    }

    // Assigns the patrol points for the enemy on the level texture and then converts them to the world position
    public static Vector3[] AssignPatrolPointsFromLevelTexture(Texture2D texture, Color[] validColors, int scale, int count)
    {
        List<Vector3> points = new List<Vector3>();
        int attempts = 0;
        while (points.Count < count && attempts < count * 10)
        {
            attempts++;
            Vector2 pixel = PickRandomValidPixelOnTexture(texture, validColors);
            if (pixel == Vector2.zero)
            {
                break;
            }
            Vector3 worldPoint = LevelPositionToWorldPosition(pixel, texture, scale);

            if (UnityEngine.AI.NavMesh.SamplePosition(worldPoint, out UnityEngine.AI.NavMeshHit hit, 2f, UnityEngine.AI.NavMesh.AllAreas))
            {
                points.Add(hit.position);
            }
        }
        return points.ToArray();
    }
}