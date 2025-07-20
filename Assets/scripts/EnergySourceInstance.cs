using UnityEngine;

public class EnergySourceInstance : MonoBehaviour
{
    public EnergySourceSO data; // Assign this from the Inspector

    public float CurrentOutputMW()
    {
        // Get today’s weather
        var w = WeatherSystem.I.Current;
        float factor = 1f;

        // Wind turbines use the wind factor, solar uses the sun factor
        if (data.displayName.Contains("Wind"))
            factor = w.wind;
        else if (data.displayName.Contains("Solar"))
            factor = w.sun;
        // (Hydro, Biomass, etc. stay at factor = 1)

        return data.baseOutputMW * factor;
    }

}

