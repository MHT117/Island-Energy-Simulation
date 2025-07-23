using UnityEngine;
using UnityEngine.UI;

public class LoadPanelController : MonoBehaviour
{
    [SerializeField] Button[] slotButtons;

    void OnEnable()
    {
        for (int i = 0; i < slotButtons.Length; i++)
            slotButtons[i].interactable = SaveSystem.HasSave(i + 1);
    }

    public void ClickSlot(int slot)
    {
        if (SaveSystem.LoadGame(slot))
        {
            Time.timeScale = 1;
            GameObject.Find("PauseMenuPanel").SetActive(false);
        }
    }
}
