using UnityEngine;
//BaseDecoratorRule is a class for creating modular decoration rules that can be applied to rooms during level generation.
//Design by Barbara Reichart lecture series, 2024
public abstract class BaseDecoratorRule : ScriptableObject
{
    [SerializeField, EnumFlags] RoomType roomTypes;
    public RoomType RoomTypes => roomTypes;
    //Each subclass must define whether it can apply the rule to the given room
    internal abstract bool CanBeApplied(TileType[,] levelDecorated, Room room);

    //Each subclass must implement how to apply the decoration
    internal abstract void Apply(TileType[,] levelDecorated, Room room, Transform parent);
}
