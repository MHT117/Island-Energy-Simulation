using UnityEngine;
using System;
using System.Collections.Generic;

public class StakeholderManager : MonoBehaviour
{
    public static StakeholderManager I { get; private set; }

    [Header("Stakeholders (populate these in the Inspector)")]
    public List<StakeholderSO> stakeholders = new();

    [Header("UI")]
    public StakeholderBarUI barPrefab;   // drag your StakeholderBar_Template prefab here
    public Transform barParent;     // drag your StakeholderPanel transform here

    // Fires whenever one stakeholder's satisfaction changes
    public event Action<StakeholderSO> OnSatisfactionChanged;

    void Awake()
    {
        // singleton setup
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
    }

    void Start()
    {
        // 1) Instantiate one bar per stakeholder
        foreach (var st in stakeholders)
        {
            var bar = Instantiate(barPrefab, barParent);
            bar.data = st;
            bar.Refresh();

            // subscribe so this specific bar updates when its SO changes
            OnSatisfactionChanged += changed =>
            {
                if (changed == st)
                    bar.Refresh();
            };
        }

        // 2) initial calculation & hook future resilience changes
        RecalculateAll();
        ResilienceManager.I.OnScoresChanged += RecalculateAll;

        // 3) hook win check on each new day tick
        TimeSystem.I.OnNewDay += CheckWinCondition;
    }

    /// <summary>
    /// Recomputes every stakeholder’s satisfaction based on the four Resilience scores,
    /// fires UI updates, and triggers end-game if any fall below their lose threshold.
    /// </summary>
    void RecalculateAll()
    {
        foreach (var st in stakeholders)
        {
            float total =
                  ResilienceManager.I.R_Sec * st.wSec
                + ResilienceManager.I.R_Eq * st.wEq
                + ResilienceManager.I.R_Sus * st.wSus
                + ResilienceManager.I.R_Ada * st.wAda;

            st.satisfaction = Mathf.Clamp(total * 10f, 0f, 100f);
            OnSatisfactionChanged?.Invoke(st);

            if (TimeSystem.I.Day >= 30 && st.satisfaction < st.loseBelow)
            {
                GameManager.I.TriggerEndGame(false);
            }
        }
    }

    /// <summary>
    /// Returns true if every stakeholder is at or above their win threshold.
    /// </summary>
    bool AllHappy()
    {
        foreach (var st in stakeholders)
            if (st.satisfaction < st.winAt)
                return false;
        return true;
    }

    /// <summary>
    /// Called each new day: if we've reached day 30+, and everyone is happy, trigger a win.
    /// </summary>
    void CheckWinCondition(int dayNumber)
    {
        const int WIN_DAY = 30;      // you can adjust this
        if (dayNumber < WIN_DAY) return;

        if (AllHappy())
            GameManager.I.TriggerEndGame(true);
    }
}
