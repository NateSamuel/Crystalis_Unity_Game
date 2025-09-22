//Initial design by Barbara Reichart lecture series, 2024, updates by student to make it run at runtime/bug fixes

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

[Serializable]
public class RuleAvailability
{
    public BaseDecoratorRule rule;
    public int maxAvailability;
    public RuleAvailability(RuleAvailability other)
    {
        rule = other.rule;
        maxAvailability = other.maxAvailability;
    }
}

public class RoomDecorator : MonoBehaviour
{
    [SerializeField] private GameObject parent;
    [SerializeField] private LayoutGeneratorRooms layoutGenerator;
    [SerializeField] private Texture2D levelTexture;
    [SerializeField] private Texture2D decoratedTexture;
    [SerializeField] private RuleAvailability[] availableRules;

    private Random random;

    [ContextMenu("Place Items")]
    public void PlaceItemsFromMenu()
    {
        SharedLevelData.Instance.ResetRandom();
        Level level = layoutGenerator.GenerateLevel();
        PlaceItems(level);
    }
    //Places items in each room of a level by repeatedly selecting and trying to apply available decoration rules
    // They are instantiated under a "Decorations" parent and GenerateTextureFromTileType is called
    public void PlaceItems(Level level)
    {
        random = SharedLevelData.Instance.Rand ?? new Random();

        Transform decorationsTransform = parent.transform.Find("Decorations");
        if (decorationsTransform == null)
        {
            GameObject decorationsGameObject = new GameObject("Decorations");
            decorationsTransform = decorationsGameObject.transform;
            decorationsTransform.SetParent(parent.transform);
        }
        else
        {
            decorationsTransform.DestroyAllChildren();
        }

        TileType[,] levelDecorated = InitializeDecoratorArray();

        foreach (Room room in level.Rooms)
        {
            // Copy available rules and filter by room type
            List<RuleAvailability> availableRulesForRoom = CopyRuleAvailability()
                .Where(ra => ra.rule.RoomTypes.HasFlag(room.Type))
                .ToList();

            int currentNumberOfDecorations = 0;
            int maxNumberOfDecorations = room.Area.width * room.Area.height * 4;
            int currentTries = 0;
            int maxTries = 50;

            // Retry for decoration placement
            while (currentNumberOfDecorations < maxNumberOfDecorations &&
                currentTries < maxTries &&
                availableRulesForRoom.Count > 0)
            {
                int selectedRuleIndex = random.Next(availableRulesForRoom.Count);
                RuleAvailability selectedRuleAvailability = availableRulesForRoom[selectedRuleIndex];

                BaseDecoratorRule ruleToApply = selectedRuleAvailability.rule;
                //For runtime
    #if !UNITY_EDITOR
                
                if (ruleToApply is PatternMatchingDecoratorRule pattern)
                {
                    ruleToApply = pattern.CloneForRuntime();
                }
    #endif

                // Check if rule can be applied
                if (ruleToApply.CanBeApplied(levelDecorated, room))
                {
                    try
                    {
                        ruleToApply.Apply(levelDecorated, room, decorationsTransform);
                        currentNumberOfDecorations++;

                        if (selectedRuleAvailability.maxAvailability > 0)
                            selectedRuleAvailability.maxAvailability--;

                        // Remove rule if not possible
                        if (selectedRuleAvailability.maxAvailability == 0)
                            availableRulesForRoom.RemoveAt(selectedRuleIndex);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[RoomDecorator] Error applying rule {ruleToApply.name}: {e.Message}");
                    }
                }

                currentTries++;
            }
        }

        GenerateTextureFromTileType(levelDecorated);
    }

    private TileType[,] InitializeDecoratorArray()
    {
        TileType[,] levelDecorated = new TileType[levelTexture.width, levelTexture.height];
        for (int y = 0; y < levelTexture.height; y++)
            for (int x = 0; x < levelTexture.width; x++)
                levelDecorated[x, y] = levelTexture.GetPixel(x, y) == Color.black
                    ? TileType.Wall
                    : TileType.Floor;

        return levelDecorated;
    }
    //Generates and applies a decorated texture that shows the level after decoration, using the tile type color data, and saves it as an asset.
    private void GenerateTextureFromTileType(TileType[,] tileTypes)
    {
        int width = tileTypes.GetLength(0);
        int height = tileTypes.GetLength(1);
        Color32[] pixels = new Color32[width * height];

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                pixels[x + y * width] = tileTypes[x, y].GetColor();

        decoratedTexture.Reinitialize(width, height);
        decoratedTexture.SetPixels32(pixels);
        decoratedTexture.Apply();
        decoratedTexture.SaveToDiskRuntime("DecoratedMap");
    }

    //Returns a copy of the rule availability list. Makes sure rule counts are separate per room by copying each RuleAvailability object.
    private List<RuleAvailability> CopyRuleAvailability()
    {
        return availableRules.Select(r => new RuleAvailability(r)).ToList();
    }
}