using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [Header("UI refs")]
    [SerializeField] TextMeshProUGUI speechText;
    [SerializeField] Image islaImg;
    [SerializeField] Sprite idleSprite, talkSprite, pointSprite;

    List<string> lines = new()
    {
        "These buttons build POWER PLANTS. They generate electricity.",
        "These buttons build HOUSES & SHOPS. They CONSUME electricity.",
        "Right-click to SELECT a building, left-click on the island to PLACE it.",
        "Need to demolish? Left-click a placed building to sell it for half cost.",
        "Watch SUPPLY vs. DEMAND and the four STAKEHOLDER bars on the right.",
        "Goal: Reach Day 30 with every stakeholder at 70% or higher!",
        "Good luck – the island is in your hands!"
    };
    int index = 0;

    void Start()
    {
        ShowLine();
        GameManager.I.tutorialActive = true;   // block all input
    }

    void Update()
    {
        // inside TutorialController.Update()
        if (Input.GetMouseButtonDown(0))
        {
            index++;
            if (index >= lines.Count)
            {
                // final slide → dismiss tutorial
                GameManager.I.tutorialActive = false;
                Time.timeScale = 1f;            // restore time
                Destroy(gameObject);
            }
            else
                ShowLine();
        }

    }

    void ShowLine()
    {
        speechText.text = lines[index];
        // switch Isla’s expression
        if (index < 2) islaImg.sprite = pointSprite;
        else islaImg.sprite = talkSprite;
    }
}
