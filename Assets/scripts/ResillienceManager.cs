using UnityEngine;
using System;

public class ResilienceManager : MonoBehaviour
{
    public static ResilienceManager I { get; private set; }

    // Running totals
    public float R_Sec { get; private set; }
    public float R_Eq { get; private set; }
    public float R_Sus { get; private set; }
    public float R_Ada { get; private set; }

    public event Action OnScoresChanged;

    void Awake() => I = this;

    public void AddScores(float dSec, float dEq, float dSus, float dAda)
    {
        R_Sec += dSec;
        R_Eq += dEq;
        R_Sus += dSus;
        R_Ada += dAda;
        OnScoresChanged?.Invoke();
    }

    public void ForceSet(float sec, float eq, float sus, float ada)
    {
        R_Sec = sec;
        R_Eq = eq;
        R_Sus = sus;
        R_Ada = ada;
        OnScoresChanged?.Invoke();
    }
}
