using UnityEngine;

public class SavePanelController : MonoBehaviour
{
    public void ClickSlot(int slot)
    {
        SaveSystem.SaveGame(slot);
        gameObject.SetActive(false);
    }
}
