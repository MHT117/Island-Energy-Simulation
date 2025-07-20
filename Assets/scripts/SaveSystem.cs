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

        // record every producer
        foreach (var src in Object.FindObjectsOfType<EnergySourceInstance>())
            data.buildings.Add(new BuildingRecord
            {
                soName = src.data.name,
                x = src.CellX,
                y = src.CellY
            });

        // record every consumer
        foreach (var con in Object.FindObjectsOfType<ConsumerInstance>())
            data.buildings.Add(new BuildingRecord
            {
                soName = con.data.name,
                x = con.CellX,
                y = con.CellY
            });

        File.WriteAllText(path, JsonUtility.ToJson(data, true));
        Debug.Log($"Game saved to {path}");
    }

    public static bool LoadGame()
    {
        if (!File.Exists(path)) return false;
        var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));

        // 1) destroy all existing buildings
        foreach (var go in Object.FindObjectsOfType<GameObject>())
            if (go.layer == LayerMask.NameToLayer("Buildings"))
                Object.Destroy(go);

        // 2) restore RNG
        Random.InitState(data.rngState);

        // 3) restore time & money
        TimeSystem.I.Day = data.day;
        TimeSystem.I.MinuteOfDay = data.minute;
        GameManager.I.SetMoney(data.money);

        // 4) restore resilience scores
        ResilienceManager.I.ForceSet(data.rSec, data.rEq, data.rSus, data.rAda);

        // 5) re-instantiate each building
        Grid grid = Object.FindObjectOfType<Grid>();
        foreach (var rec in data.buildings)
        {
            var so = Resources.Load<ScriptableObject>(rec.soName);
            Vector3Int cell = new Vector3Int(rec.x, rec.y, 0);
            Vector3 pos = grid.CellToWorld(cell) + new Vector3(0.5f, 0.5f);
            // we’ll add an overload to PlacementController in a moment
            PlacementController.I.InstantiateFromSO(so, pos, register: true);
        }

        Debug.Log("Save loaded.");
        return true;
    }

    public static void DeleteSave()
    {
        if (File.Exists(path)) File.Delete(path);
    }
}
