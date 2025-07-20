// WeatherSystem.cs
using UnityEngine;

[System.Serializable]
public struct WeatherSnapshot
{
    public float sun;   // 0–1
    public float wind;  // 0–1
}

public class WeatherSystem : MonoBehaviour
{
    public static WeatherSystem I { get; private set; }

    [Header("Daily factor ranges")]
    [Range(0f, 1f)] public float minSun = 0.25f, maxSun = 0.9f;
    [Range(0f, 1f)] public float minWind = 0.15f, maxWind = 0.8f;

    WeatherSnapshot today;

    void Awake() => I = this;

    void Start()
    {
        GenerateNewDay(1);
        TimeSystem.I.OnNewDay += GenerateNewDay;
    }

    void GenerateNewDay(int dayNumber)
    {
        today.sun = Random.Range(minSun, maxSun);
        today.wind = Random.Range(minWind, maxWind);
        Debug.Log($"Day {dayNumber} Sun {today.sun:F2} Wind {today.wind:F2}");
    }

    public WeatherSnapshot Current => today;
}
