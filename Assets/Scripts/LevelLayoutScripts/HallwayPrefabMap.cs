using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//student creation

//each of the entries has these information cells added to them
[System.Serializable]
public class HallwayPrefabSetter {
    public int levelDelta;
    public Direction direction;
    public GameObject prefab;
}

public enum Direction {
    North,
    South,
    East,
    West
}

//An asset menu which sets the hallway prefabs for each direction and what type of hallway it is I.e. standard hallway, or 1 level change steps, or 2 level change steps
[CreateAssetMenu(fileName = "HallwayPrefabMap", menuName = "Custom/Procedural Generation/HallwayPrefabMap")]
public class HallwayPrefabMap : ScriptableObject {
    public List<HallwayPrefabSetter> entries;

    private Dictionary<(int, Direction), GameObject> _lookup;

    public GameObject GetPrefab(int levelDelta, Direction direction) {
        if (_lookup == null) {
            _lookup = new();
            foreach (var entry in entries) {
                _lookup[(entry.levelDelta, entry.direction)] = entry.prefab;
            }
        }
        return _lookup.TryGetValue((levelDelta, direction), out var prefab) ? prefab : null;
    }
}