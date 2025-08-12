using UnityEngine;
using System;

public class TimeSystem : MonoBehaviour
{
    public static TimeSystem I { get; private set; }

    [Header("Tick settings")]
    [Tooltip("Real-time seconds that equal 1 game minute")]
    public float realSecondsPerGameMinute = 1f;

    public int MinutesPerDay = 1440;          // 24 h × 60
    public int Day { get; set; }           // <-- was private set
    public int MinuteOfDay { get; set; }   // <-- was private set

    public event Action<int, int> OnMinuteTick;   // (day, minute)
    public event Action<int> OnNewDay;       // (day)

    float accumulator;

    void Awake() => I = this;

    void Update()
    {
        if (GameManager.I != null && (GameManager.I.tutorialActive || GameManager.I.inPlanningPhase))
            return;

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
