using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuildingRecord
{
    public string soName;  // the name of the ScriptableObject asset
    public int x, y;       // grid cell coords
}

[Serializable]
public class SaveData
{
    public int day;
    public int minute;
    public int money;

    public float rSec, rEq, rSus, rAda;

    public int rngState;

    public List<BuildingRecord> buildings = new();
}
