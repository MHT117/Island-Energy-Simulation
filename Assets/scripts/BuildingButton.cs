using UnityEngine;
using UnityEngine.UI;

// ← add this so the compiler knows about your consumer SO
//using YourNamespaceForSOs; // e.g. if your SOs live in the global namespace you can omit this

[RequireComponent(typeof(Button))]
public class BuildingButton : MonoBehaviour
{
    // still a generic ScriptableObject slot
    public ScriptableObject data;

    void Awake()
    {
        var btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        // route to the correct StartPlacing overload
        if (data is EnergySourceSO es)
        {
            PlacementController.Instance.StartPlacing(es);
        }
        else if (data is ConsumerBuildingSO cs)
        {
            PlacementController.Instance.StartPlacing(cs);
        }
        else
        {
            Debug.LogError($"Unsupported button data type: {data.GetType()}");
        }
    }
}
