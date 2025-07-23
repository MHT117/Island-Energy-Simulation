using UnityEngine;

[CreateAssetMenu(menuName = "Energy/Source Data")]
public class EnergySourceSO : ScriptableObject
{
    [Header("Placement Limit")]
    [Tooltip("0 = unlimited")]
    public int maxPlantsAllowed = 0;

    public GameObject prefab;  // drag the matching prefab here
    public string displayName;
    public Sprite sprite;  
    [Header("Economy")]
    public int buildCost = 100;
    public int maintenancePerDay = 10;
    public int maintenanceDay = 5;
    [Header("Power output")]
    public float baseOutputMW = 1f;   // MW at 100 % weather factor(will add later)


    [Header("Resilience scores (+ve good, -ve bad)")]
    public float sec;   // Security
    public float eq;    // Equity
    public float sus;   // Sustainability
    public float ada;   // Adaptability

}
//