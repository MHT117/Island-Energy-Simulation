using UnityEngine;

[CreateAssetMenu(menuName = "Energy/Consumer Data")]
public class ConsumerBuildingSO : ScriptableObject
{
    public GameObject prefab;          // ← ADD THIS
    public string displayName;
    public Sprite sprite;
    public int buildCost = 50;
    public int maintenancePerDay = 2;   // NEW

    [Tooltip("Base demand in kWh per game minute")]
    public float baseDemand = 0.8f;
    [Tooltip("24-hour curve multiplier (x-axis = 0-23)")]
    public AnimationCurve hourlyMultiplier =
        AnimationCurve.Constant(0, 24, 1);
}
