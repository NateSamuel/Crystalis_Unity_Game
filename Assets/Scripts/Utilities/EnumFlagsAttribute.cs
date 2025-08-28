using UnityEngine;

//generic wrapper by others to work with the EnumFlagsAttributeDrawer for visualisation in the editor - Barbara Reichart, 2024
public class EnumFlagsAttribute : PropertyAttribute
{
    [SerializeField] string enumName;

    public EnumFlagsAttribute() { }

    public EnumFlagsAttribute(string name)
    {
        enumName = name;
    }
}
