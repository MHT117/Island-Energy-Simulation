using UnityEngine;

[CreateAssetMenu(menuName = "Energy/Climate Event")]
public class ClimateEventSO : ScriptableObject
{
    public string displayName;
    [TextArea] public string description;
    public Sprite icon;                 // 64×64 sprite for toast
    [Header("Negative or positive deltas")]
    public float dSec;   // e.g. -1  weakens Security
    public float dEq;
    public float dSus;
    public float dAda;
}
