using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PromptManager : MonoBehaviour
{
    public static PromptManager I { get; private set; }

    [SerializeField] GameObject promptPrefab;   // assign the PromptPanel prefab here
    [SerializeField] Canvas uiCanvas;           //  main UI canvas

    // day  question mapping
    readonly Dictionary<int, string> scheduler = new()
    {
        {10, "Which power plants and buildings have you chosen so far?" },
        {20, "Did you make any changes to your layout after Day 10? Why?" },
        {30, "Were you able to keep the island balanced? Hard decisions?" }
    };

    public List<PromptAnswer> Answers { get; private set; } = new();

    GameObject activePanel;

    void Awake() => I = this;

    void Start()
    {
        // subscribe to new‐day event
        TimeSystem.I.OnNewDay += CheckPrompt;
    }

    void CheckPrompt(int day)
    {
        // already showing or not a prompt day or already answered?
        if (activePanel != null) return;
        if (!scheduler.ContainsKey(day)) return;
        if (Answers.Exists(a => a.day == day)) return;

        CreatePanel(day, scheduler[day]);
    }

    void CreatePanel(int day, string question)
    {
        // block all inputs
        GameManager.I.tutorialActive = true;

        // ← pause the time here
        Time.timeScale = 0f;

        // instantiate panel under the UI canvas
        activePanel = Instantiate(promptPrefab, uiCanvas.transform);

        // set question text
        var qTxt = activePanel.transform.Find("QuestionTxt")
                  .GetComponent<TextMeshProUGUI>();
        qTxt.text = question;

        // wire OK button
        var okBtn = activePanel.transform.Find("OkButton")
                  .GetComponent<Button>();
        okBtn.onClick.AddListener(() =>
        {
            // read answer
            var resp = activePanel.transform.Find("AnswerField")
                      .GetComponent<TMP_InputField>().text;

            Answers.Add(new PromptAnswer
            {
                day = day,
                question = question,
                response = resp
            });

            // autosave
            SaveSystem.SaveGame(1);

            Destroy(activePanel);
            activePanel = null;

            // ← resume time here
            Time.timeScale = 1f;

            // unlock inputs
            GameManager.I.tutorialActive = false;
        });
    }


    // called by LoadGame to restore saved answers
    public void SetLoadedAnswers(List<PromptAnswer> list)
    {
        if (list != null) Answers = list;
    }
}
