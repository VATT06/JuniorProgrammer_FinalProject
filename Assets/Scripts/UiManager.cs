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

    private GameManager gameManager;
    private Button[] questionButtons;
    private TMP_Text[] buttonTexts;

    void Start()
    {
        // Initialize GameManager reference first
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!");
            return;
        }

        // Check if all required UI components are assigned
        if (questionOption1 == null || questionOption2 == null || 
            questionOption3 == null || questionOption4 == null)
        {
            Debug.LogError("Question buttons not assigned in UIManager!");
            return;
        }

        if (questionText == null)
        {
            Debug.LogError("Question text component not assigned in UIManager!");
            return;
        }

        InitialPanelSet();
        SetupButtons();
    }

    void SetupButtons()
    {
        // Initialize button arrays
        questionButtons = new Button[] { questionOption1, questionOption2, questionOption3, questionOption4 };
        buttonTexts = new TMP_Text[4];

        // Get text components from buttons
        for (int i = 0; i < questionButtons.Length; i++)
        {
            buttonTexts[i] = questionButtons[i].GetComponentInChildren<TMP_Text>();
            if (buttonTexts[i] == null)
            {
                Debug.LogError($"Text component not found in button {i + 1}!");
            }
        }
    }

    public void DisplayQuestion(GameManager.Question question, List<string> shuffledOptions, int correctAnswerIndex)
    {
        // Validate inputs
        if (question == null || shuffledOptions == null || questionText == null)
        {
            Debug.LogError("Invalid input parameters in DisplayQuestion!");
            return;
        }

        // Make sure all components are initialized
        if (buttonTexts == null || questionButtons == null)
        {
            SetupButtons();
        }

        questionText.text = question.question;

        // Set button texts and listeners
        for (int i = 0; i < questionButtons.Length; i++)
        {
            if (i < shuffledOptions.Count && buttonTexts[i] != null)
            {
                buttonTexts[i].text = shuffledOptions[i];
                int buttonIndex = i;
                questionButtons[i].onClick.RemoveAllListeners();
                questionButtons[i].onClick.AddListener(() => HandleAnswer(buttonIndex, correctAnswerIndex));
            }
        }
    }

    void HandleAnswer(int selectedIndex, int correctIndex)
    {
        // Disable all buttons temporarily
        SetButtonsInteractable(false);

        // Show correct/wrong feedback
        if (selectedIndex == correctIndex)
        {
            questionButtons[selectedIndex].GetComponent<Image>().color = Color.green;
            gameManager.IncrementGoodCounter();
            goodQuestionCounter_text.text = "G: " + gameManager.GoodQuestionCounter.ToString();
        }
        else
        {
            questionButtons[selectedIndex].GetComponent<Image>().color = Color.red;
            questionButtons[correctIndex].GetComponent<Image>().color = Color.green;
            gameManager.IncrementBadCounter();
            badQuestionCounter_text.text = "B: " + gameManager.BadQuestionCounter.ToString();
        }

        StartCoroutine(ResetButtonsAfterDelay());
    }

    IEnumerator ResetButtonsAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        
        // Reset button colors and enable interaction
        foreach (Button button in questionButtons)
        {
            button.GetComponent<Image>().color = Color.white;
        }
        SetButtonsInteractable(true);
        
        // Check if GameManager exists before calling
        if (gameManager != null)
        {
            gameManager.LoadNextQuestion();
        }
        else
        {
            Debug.LogError("GameManager reference is missing!");
        }
    }

    void SetButtonsInteractable(bool interactable)
    {
        foreach (Button button in questionButtons)
        {
            button.interactable = interactable;
        }
    }

    public void UpdateGoodCounter()
    {
        if (gameManager != null && goodQuestionCounter_text != null)
        {
            goodQuestionCounter_text.text = gameManager.GoodQuestionCounter.ToString();
        }
    }

    public void UpdateBadCounter()
    {
        if (gameManager != null && badQuestionCounter_text != null)
        {
            badQuestionCounter_text.text = gameManager.BadQuestionCounter.ToString();
        }
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

    public void QuitGame()
    { 
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }



}
