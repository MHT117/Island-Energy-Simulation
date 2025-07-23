using UnityEngine;
using System.Collections.Generic;

public class PowerManager : MonoBehaviour
{
    public static PowerManager I { get; private set; }

    [Header("Blackout rule")]
    [Tooltip("How many consecutive game-minutes of deficit before blackout.")]
    public int deficitMinutesToFail = 5;

    public float supplyMW { get; private set; }
    public float demandMW { get; private set; }
    public float surplusMW => supplyMW - demandMW;

    int deficitStreak = 0;
    readonly List<EnergySourceInstance> producers = new();
    readonly List<ConsumerInstance> consumers = new();

    public IReadOnlyList<EnergySourceInstance> Producers => producers;
    public event System.Action<int, int> OnMinuteTick;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        TimeSystem.I.OnMinuteTick += OnMinute;
    }

    void OnMinute(int day, int minuteOfDay)
    {
        // relay to UI
        OnMinuteTick?.Invoke(day, minuteOfDay);

        // Sum producers
        supplyMW = 0f;
        foreach (var p in producers)
            supplyMW += p.CurrentOutputMW();

        // Sum consumers (kWh/min → MW)
        demandMW = 0f;
        foreach (var c in consumers)
            demandMW += c.CurrentDemandKWh(minuteOfDay) / 1000f;

        // Blackout?
        if (surplusMW < 0f) deficitStreak++;
        else deficitStreak = 0;

        if (deficitStreak >= deficitMinutesToFail)
            GameManager.I.InstanceBlackout();
    }

    public void RegisterSource(EnergySourceInstance e) => producers.Add(e);
    public void RegisterConsumer(ConsumerInstance c) => consumers.Add(c);

    // used by PlacementController to enforce max per-type
    public int CountType(EnergySourceSO es)
    {
        int c = 0;
        foreach (var p in producers)
            if (p.data == es) c++;
        return c;
    }
}
