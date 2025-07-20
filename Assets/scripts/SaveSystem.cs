using UnityEngine;
using System.IO;
using System.Collections.Generic;

public static class SaveSystem
{
    static string path => Path.Combine(Application.persistentDataPath, "islandSave.json");

    public static void SaveGame()
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

        foreach (var src in Object.FindObjectsByType<EnergySourceInstance>(
                    FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            data.buildings.Add(new BuildingRecord
            {
                soName = src.data.name,
                x = src.CellX,
                y = src.CellY
            });
        }

        foreach (var con in Object.FindObjectsByType<ConsumerInstance>(
                    FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            data.buildings.Add(new BuildingRecord
            {
                soName = con.data.name,
                x = con.CellX,
                y = con.CellY
            });
        }

        File.WriteAllText(path, JsonUtility.ToJson(data, true));
        Debug.Log($"Game saved to {path}");
    }

    public static bool LoadGame()
    {
        if (!File.Exists(path)) return false;
        var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));

        // --- clear existing buildings ---
        foreach (var go in Object.FindObjectsByType<GameObject>(
                    FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (go.layer == LayerMask.NameToLayer("Buildings"))
                Object.Destroy(go);
        }

        // RNG
        Random.InitState(data.rngState);

        // Time & money
        TimeSystem.I.Day = data.day;
        TimeSystem.I.MinuteOfDay = data.minute;
        GameManager.I.SetMoney(data.money);

        // Resilience
        ResilienceManager.I.ForceSet(
            data.rSec, data.rEq, data.rSus, data.rAda);

        // Recreate buildings
        var grid = Object.FindAnyObjectByType<Grid>();
        foreach (var rec in data.buildings)
        {
            var so = Resources.Load<ScriptableObject>(rec.soName);
            var cell = new Vector3Int(rec.x, rec.y, 0);
            var worldPos = grid.CellToWorld(cell) + new Vector3(0.5f, 0.5f);
            PlacementController.I.InstantiateFromSO(so, worldPos, register: true);
        }

        Debug.Log("Save loaded.");
        return true;
    }

    public static void DeleteSave()
    {
        if (File.Exists(path)) File.Delete(path);
    }
}
