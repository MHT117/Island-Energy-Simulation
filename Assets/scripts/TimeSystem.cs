using UnityEngine;
using System;

public class TimeSystem : MonoBehaviour
{
    public static TimeSystem I { get; private set; }

    [Header("Tick settings")]
    [Tooltip("Real-time seconds that equal 1 game minute")]
    public float realSecondsPerGameMinute = 1f;

    public int MinutesPerDay = 1440;          // 24 h × 60
    public int Day { get; private set; } = 1;
    public int MinuteOfDay { get; private set; } = 0;   // 0–1439

    public event Action<int, int> OnMinuteTick;   // (day, minute)
    public event Action<int> OnNewDay;       // (day)

    float accumulator;

    void Awake() => I = this;

    void Update()
    {
        accumulator += Time.deltaTime;
        if (accumulator < realSecondsPerGameMinute) return;
        accumulator -= realSecondsPerGameMinute;

        MinuteOfDay++;
        if (MinuteOfDay >= MinutesPerDay)
        {
            MinuteOfDay = 0;
            Day++;
            OnNewDay?.Invoke(Day);
        }
        OnMinuteTick?.Invoke(Day, MinuteOfDay);
    }
}
