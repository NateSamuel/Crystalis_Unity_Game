using System.Collections.Generic;
using UnityEngine;

//helper extension method to destory all child objects of a transform - Barbara Reichart lecture series, 2024
public static class MonoBehaviorExtensionMethods
{
    #region Variables

    #endregion

    #region Methods
    public static void DestroyAllChildren(this Transform transform)
    {
        var children = new List<GameObject>();
        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);
        }
        #if UNITY_EDITOR
            children.ForEach(child => GameObject.DestroyImmediate(child));
        #else
            children.ForEach(child => Destroy(transform.GetChild(i).gameObject));
        #endif
    }

    #endregion
}