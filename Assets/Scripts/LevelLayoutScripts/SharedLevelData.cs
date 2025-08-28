using UnityEngine;
using System;
using Random = System.Random;

//Design by Barbara Reichart lecture series, 2024
[ExecuteAlways]
[DisallowMultipleComponent]
public class SharedLevelData : MonoBehaviour
{
    public static SharedLevelData Instance { get; private set; }
    [SerializeField] int scale = 1;
    [SerializeField] int seed = Environment.TickCount;

    Random random;
    public int Scale => scale;
    public Random Rand => random;

    //creates random seed based on time
    [ContextMenu("Generate New Seed")]
    public void GenerateSeed()
    {
        seed = Environment.TickCount;
        random = new Random(seed);
    }
    //makes sure only one instance is active of this object
    private void OnEnable()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            enabled = false;
        }
        Debug.Log(Instance.GetInstanceID());
        random = new Random(seed);
    }

    //resets the random number generator
    public void ResetRandom()
    {
        random = new Random(seed);
    }
}
