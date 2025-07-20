using UnityEngine;
using System.Collections.Generic;

public class EventScheduler : MonoBehaviour
{
    [Tooltip("Pool of possible events")]
    public List<ClimateEventSO> events = new();

    [Tooltip("How many in-game days between events")]
    [Min(1)] public int daysBetween = 3;

    [Header("UI")]
    public EventToastUI toastPrefab;     // assign in Inspector
    EventToastUI toastInstance;

    void Start()
    {
        TimeSystem.I.OnNewDay += MaybeFireEvent;

        // Use the new API instead:
        var canvas = Object.FindFirstObjectByType<Canvas>();
        toastInstance = Instantiate(toastPrefab, canvas.transform);
    }

    void MaybeFireEvent(int dayNumber)
    {
        // no events on day 1
        if (dayNumber == 1) return;
        // only every N days
        if (dayNumber % daysBetween != 0) return;
        if (events.Count == 0) return;

        // pick a random event from your list
        ClimateEventSO ev = events[Random.Range(0, events.Count)];

        // apply its resilience deltas
        ResilienceManager.I.AddScores(ev.dSec, ev.dEq, ev.dSus, ev.dAda);

        // show the toast!
        toastInstance.Show(ev);
    }
}
