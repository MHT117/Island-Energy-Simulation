using UnityEngine;
using System.IO;
using System.Collections.Generic;

public static class SaveSystem
{
    //---------- Overloads for “quick” single-slot save/load (buttons use these) ----------
    public static void SaveGame() => SaveGame(1);
    public static bool LoadGame() => LoadGame(1);

    //---------- Helpers for slot file naming ----------
    static string PathForSlot(int slot) =>
        Path.Combine(Application.persistentDataPath, $"islandSave_slot{slot}.json");

  /*  public static bool HasSave(int slot) =>
        File.Exists(PathForSlot(slot));

    public static void DeleteSave(int slot)
    {
        string path = PathForSlot(slot);
        if (File.Exists(path))
            File.Delete(path);
    }
  */
    //---------- Slot-based save ----------
    public static void SaveGame(int slot)
    {
        // 1) Gather up all the primitives
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

        // 2) Record every EnergySourceInstance
        var sources = Object.FindObjectsByType<EnergySourceInstance>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None
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

        // 3) Record every ConsumerInstance
        var consumers = Object.FindObjectsByType<ConsumerInstance>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None
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

        // 4) Serialize and write out
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(PathForSlot(slot), json);
        Debug.Log($"Game saved to {PathForSlot(slot)}");
    }

    //---------- Slot-based load ----------
    public static bool LoadGame(int slot)
    {
        string path = PathForSlot(slot);
        if (!File.Exists(path))
            return false;

        // 1) Read and deserialize
        string json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<SaveData>(json);

        // 2) Clear existing buildings
        var allGOs = Object.FindObjectsByType<GameObject>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None
        );
        foreach (var go in allGOs)
        {
            if (go.layer == LayerMask.NameToLayer("Buildings"))
                Object.Destroy(go);
        }

        // 3) Restore RNG
        Random.InitState(data.rngState);

        // 4) Restore time & money
        TimeSystem.I.Day = data.day;
        TimeSystem.I.MinuteOfDay = data.minute;
        GameManager.I.SetMoney(data.money);

        // 5) Restore resilience
        ResilienceManager.I.ForceSet(
            data.rSec, data.rEq, data.rSus, data.rAda
        );

        // 6) Recreate buildings
        var grid = Object.FindAnyObjectByType<Grid>();
        foreach (var rec in data.buildings)
        {
            var so = Resources.Load<ScriptableObject>(rec.soName);
            var cell = new Vector3Int(rec.x, rec.y, 0);
            var worldPos = grid.CellToWorld(cell) + Vector3.one * 0.5f;
            PlacementController.I.InstantiateFromSO(so, worldPos, true);
        }

        Debug.Log($"Game loaded from {path}");
        return true;
    }

    //  only a single HasSave method
    public static bool HasSave(int slot)
        => File.Exists(PathForSlot(slot));

    // Delete all slots
    public static void DeleteAllSaves()
    {
        for (int i = 1; i <= 3; i++)
            if (HasSave(i))
                File.Delete(PathForSlot(i));
    }

    // Try loading slots 1 to 3; return true if any load succeeds
    public static bool LoadLatest()
    {
        for (int i = 1; i <= 3; i++)
            if (LoadGame(i))
                return true;
        return false;
    }
}
