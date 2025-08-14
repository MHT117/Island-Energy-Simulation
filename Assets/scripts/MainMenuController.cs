using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] Button continueBtn;

    void Start()
    {
        // only enable “Continue” if any save exists
        bool hasSave = SaveSystem.HasSave(1)
                    || SaveSystem.HasSave(2)
                    || SaveSystem.HasSave(3);
        continueBtn.interactable = hasSave;
        if (AudioManager.I) AudioManager.I.PlayMusic(AudioManager.I.menuMusic, 1f);
    }

    // called by the Start button
    public void BtnStart()
    {
        SaveSystem.DeleteAllSaves();    // fresh start
        SceneManager.LoadScene(1);      // index of your game scene
    }

    public void BtnContinue()
    {
        SceneManager.LoadScene(1);      // game scene auto-loads latest save
    }

    public void BtnCredits() => SceneManager.LoadScene("Credits");


    public void BtnQuit()
    {
        Application.Quit();
    }
}
