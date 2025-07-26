using UnityEngine;

public class SavePanelController : MonoBehaviour
{
    public void ClickSlot(int slot)
    {
        Debug.Log($"[SavePanel] ClickSlot({slot})");
        SaveSystem.SaveGame(slot);
        gameObject.SetActive(false);
    }
}
