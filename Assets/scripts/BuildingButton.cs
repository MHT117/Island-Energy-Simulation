using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BuildingButton : MonoBehaviour
{
    // assign either an EnergySourceSO or a ConsumerBuildingSO in the Inspector
    public ScriptableObject data;

    void Awake()
    {
        var btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (data is EnergySourceSO es)
        {
            PlacementController.I.StartPlacing(es);
        }
        else if (data is ConsumerBuildingSO cs)
        {
            PlacementController.I.StartPlacing(cs);
        }
        else
        {
            Debug.LogError($"Unsupported data type on BuildingButton: {data.GetType()}");
        }
    }
}
