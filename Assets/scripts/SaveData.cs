using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PromptAnswer
{
    public int day;
    public string question;
    public string response;
}

[Serializable]
public class BuildingRecord
{
    public string soName;    // the name of the ScriptableObject asset
    public int x, y;         // grid-cell coordinates
}

[Serializable]
public class SaveData
{
    public int day;
    public int minute;
    public int money;

    public float rSec, rEq, rSus, rAda;

    public int rngState;

    // what’s currently built on the map
    public List<BuildingRecord> buildings = new List<BuildingRecord>();

    //  NEW: the player’s responses to day-10/20/30 prompts
    public List<PromptAnswer> answers = new List<PromptAnswer>();
}
