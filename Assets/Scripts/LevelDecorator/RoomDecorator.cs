using UnityEngine;
using Random = System.Random;

public class RoomDecorator : MonoBehaviour
{
    [SerializeField] GameObject parent;
    [SerializeField] LayoutGeneratorRooms layoutGenerator;
    [SerializeField] Texture2D levelTexture;

    Random random;

    [ContextMenu("Place Items")]
    public void PlaceItemsFromMenu()
    {
        SharedLevelData.Instance.ResetRandom();
        Level level = layoutGenerator.GenerateLevel();
        PlaceItems(level);
    }
    public void PlaceItems(Level level)
    {
        random = SharedLevelData.Instance.Rand;
        Transform decorationsTransform = parent.transform.Find("Decorations");

        if(decorationsTransform == null)
        {
            GameObject decorationsGameObject = new GameObject("Decorations");
            decorationsTransform = decorationsGameObject.transform;
            decorationsTransform.SetParent(parent.transform);
        }
        else {
            decorationsTransform.DestroyAllChildren();
        }

        TileType[,] levelDecorated = InitializeDecoratorArray();

        //GameObject testGameObject = new GameObject("Test Child");
        //testGameObject.transform.SetParent(decorationsTransform);
    }
    private TileType[,] InitializeDecoratorArray()
    {
        TileType[,] levelDecorated = new TileType[levelTexture.width, levelTexture.height];
        for (int y = 0; y < levelTexture.height; y++)
        {
            for (int x = 0; x < levelTexture.width; x++)
            {
                Color pixelColor = levelTexture.GetPixel(x,y);
                if (pixelColor == Color.black)
                {
                    levelDecorated[x,y] = TileType.Wall;
                }else{
                    levelDecorated[x,y] = TileType.Floor;
                }
            }
        }
        return levelDecorated;
    }
}
