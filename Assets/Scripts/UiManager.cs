using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [Header("GamePanels")]
    [SerializeField] GameObject loadPanel;
    [SerializeField] GameObject inGamePanel;
    [SerializeField] GameObject pauseOrExitPanel;

    [Header("Buttons"),Space(4)]
    [SerializeField] Button questionOption1;
    [SerializeField] Button questionOption2;
    [SerializeField] Button questionOption3;
    [SerializeField] Button questionOption4;

    [Header("UiText/Counters"), Space(4)]
    [SerializeField] TMP_Text questionText;
    [SerializeField] TMP_Text goodQuestionCounter_text;
    [SerializeField] TMP_Text badQuestionCounter_text;
    [SerializeField] TMP_Text timerQuestion_text;

    // Start is called before the first frame update
    void Start()
    {
        InitialPanelSet();
    }

    void InitialPanelSet()
    {
        loadPanel.SetActive(true);
        inGamePanel.SetActive(false);
        pauseOrExitPanel.SetActive(false);
    }

    public void InGamePanelSet()
    {
        loadPanel.SetActive(false);
        inGamePanel.SetActive(true);
        pauseOrExitPanel.SetActive(false);
    }

    public void PausePanelSet()
    {
        Time.timeScale = 0;
        pauseOrExitPanel.SetActive(true);
        inGamePanel.SetActive(true);
        loadPanel.SetActive(false);
    }

    public void ResumePanelSet()
    {
        Time.timeScale = 1;
        pauseOrExitPanel.SetActive(false);
        inGamePanel.SetActive(true);
        loadPanel.SetActive(false);
    }



}
