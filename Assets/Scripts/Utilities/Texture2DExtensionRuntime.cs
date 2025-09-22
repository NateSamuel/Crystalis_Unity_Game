using UnityEngine;
using System.IO;

public static class Texture2DExtensionRuntime
{
    /// <summary>
    /// Saves a Texture2D as a PNG at runtime in the persistent data path.
    /// Works in builds, including WebGL (file is saved in memory/persistent path).
    /// </summary>
    /// <param name="texture">The Texture2D to save.</param>
    /// <param name="fileName">The file name without extension.</param>
    public static void SaveToDiskRuntime(this Texture2D texture, string fileName)
    {
        byte[] bytes = texture.EncodeToPNG();
        string path = Path.Combine(Application.persistentDataPath, fileName + ".png");
        File.WriteAllBytes(path, bytes);
        Debug.Log("Saved texture to: " + path);
    }
}