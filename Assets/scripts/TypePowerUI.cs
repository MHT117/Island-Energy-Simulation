using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TypePowerUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform listParent;          // Assign the TypePowerPanel Transform
    [SerializeField] TextMeshProUGUI rowPrefab;     // Assign the TypeRow_Template prefab

    readonly Dictionary<string, TextMeshProUGUI> cache = new();
    readonly Dictionary<string, float> totals = new();

    void Start()
    {
        // now that Awake() has run on PM, this is safe
        PowerManager.I.OnMinuteTick += Refresh;
        Refresh(TimeSystem.I.Day, TimeSystem.I.MinuteOfDay);
    }

    void OnDestroy()
    {
        if (PowerManager.I != null)
            PowerManager.I.OnMinuteTick -= Refresh;
    }

    void Refresh(int day, int minute)
    {
        // 1) clear running totals
        totals.Clear();

        // 2) sum output of every producer
        foreach (var src in PowerManager.I.Producers)
        {
            string key = src.data.displayName;
            if (!totals.ContainsKey(key)) totals[key] = 0f;
            totals[key] += src.CurrentOutputMW();
        }

        // 3) update / create UI rows
        foreach (var kv in totals)
        {
            if (!cache.TryGetValue(kv.Key, out var row))
            {
                row = Instantiate(rowPrefab, listParent);
                cache[kv.Key] = row;
            }
            row.text = $"{kv.Key}  :  {kv.Value:0.0} MW";
        }
    }
}
