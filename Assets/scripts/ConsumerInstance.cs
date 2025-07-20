using UnityEngine;

public class ConsumerInstance : MonoBehaviour
{
    public ConsumerBuildingSO data;

    public int CellX { get; private set; }
    public int CellY { get; private set; }

    public void SetCell(int x, int y)
    {
        CellX = x;
        CellY = y;
    }

    // Called each minute to get demand
    public float CurrentDemandKWh(int minuteOfDay)
    {
        int hour = minuteOfDay / 60;
        float mul = data.hourlyMultiplier.Evaluate(hour);
        return data.baseDemand * mul;
    }
}
