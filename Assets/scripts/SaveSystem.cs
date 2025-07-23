using UnityEngine;
using System.IO;
using System.Collections.Generic;

public static class SaveSystem
{
    // at top of SaveSystem class, under PathForSlot:
    public static void SaveGame() => SaveGame(1);
    public static bool LoadGame() => LoadGame(1);

    // helper to name each slot file
    static string PathForSlot(int slot) =>
        Path.Combine(Application.persistentDataPath, $"islandSave_slot{slot}.json");

    public static bool HasSave(int slot) =>
        File.Exists(PathForSlot(slot));

    public static void DeleteSave(int slot)
    {
        var path = PathForSlot(slot);
        if (File.Exists(path)) File.Delete(path);
    }

    public static void SaveGame(int slot)
    {
        var data = new SaveData
        {
            day = TimeSystem.I.Day,
            minute = TimeSystem.I.MinuteOfDay,
            money = GameManager.I.Money,
            rSec = ResilienceManager.I.R_Sec,
            rEq = ResilienceManager.I.R_Eq,
            rSus = ResilienceManager.I.R_Sus,
            rAda = ResilienceManager.I.R_Ada,
            rngState = Random.state.GetHashCode()
        };

        // record all energy sources
        var sources = Object.FindObjectsByType<EnergySourceInstance>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None
        );
        foreach (var src in sources)
        {
            data.buildings.Add(new BuildingRecord
            {
                soName = src.data.name,
                x = src.CellX,
                y = src.CellY
            });
        }

        // record all consumers
        var consumers = Object.FindObjectsByType<ConsumerInstance>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None
        );
        foreach (var con in consumers)
        {
            data.buildings.Add(new BuildingRecord
            {
                soName = con.data.name,
                x = con.CellX,
                y = con.CellY
            });
        }

        var json = JsonUtility.ToJson(data, true);
        File.WriteAllText(PathForSlot(slot), json);
        Debug.Log($"Game saved to {PathForSlot(slot)}");
    }

    public static bool LoadGame(int slot)
    {
        var path = PathForSlot(slot);
        if (!File.Exists(path)) return false;

        var data = JsonUtility.FromJson<SaveData>(
            File.ReadAllText(path)
        );

        // clear existing buildings
        var allGOs = Object.FindObjectsByType<GameObject>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None
        );
        foreach (var go in allGOs)
        {
            if (go.layer == LayerMask.NameToLayer("Buildings"))
                Object.Destroy(go);
        }

        // restore RNG
        Random.InitState(data.rngState);

        // restore time & money
        TimeSystem.I.Day = data.day;
        TimeSystem.I.MinuteOfDay = data.minute;
        GameManager.I.SetMoney(data.money);

        // restore resilience
        ResilienceManager.I.ForceSet(
            data.rSec, data.rEq, data.rSus, data.rAda
        );

        // recreate buildings
        var grid = Object.FindAnyObjectByType<Grid>();
        foreach (var rec in data.buildings)
        {
            var so = Resources.Load<ScriptableObject>(rec.soName);
            var cell = new Vector3Int(rec.x, rec.y, 0);
            var worldPos = grid.CellToWorld(cell) + Vector3.one * 0.5f;
            PlacementController.I.InstantiateFromSO(so, worldPos, true);
        }

        Debug.Log($"Loaded save from {path}");
        return true;
    }
}
