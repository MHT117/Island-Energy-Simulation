using UnityEngine;

[CreateAssetMenu(menuName = "Energy/Stakeholder")]
public class StakeholderSO : ScriptableObject
{
    public string displayName;
    public Sprite icon;           // optional portrait

    [Header("Weight (0–1) each)")]
    [Range(0f, 1f)] public float wSec = 0.25f;
    [Range(0f, 1f)] public float wEq = 0.25f;
    [Range(0f, 1f)] public float wSus = 0.25f;
    [Range(0f, 1f)] public float wAda = 0.25f;

    [Header("Thresholds")]
    [Range(0, 100)] public int loseBelow = 30;
    [Range(0, 100)] public int winAt = 70;

    [HideInInspector] public float satisfaction; // runtime only
}
