using UnityEngine;
using System.Collections.Generic;
using SimpleJSON;
using System.IO;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        public int id;
        public string category;
        public string question;
        public List<string> options;
        public string answer;
    }

    int goodQuestion_Counter = 0;
    int badQuestion_Counter = 0;
    float questionTimer = 0;

    public Dictionary<int, Question> questionsByNumber;
    public Dictionary<string, List<Question>> questionsByCategory;
    [SerializeField] private List<Question> questionsListByNumber;
    [SerializeField] private List<Question> questionsListByCategory;
    private UiManager uiManager;
    public int GoodQuestionCounter => goodQuestion_Counter;
    public int BadQuestionCounter => badQuestion_Counter;

    void Start()
    {
        uiManager = FindObjectOfType<UiManager>();
        LoadQuestions();
        OrganizeQuestionsByCategory();
        UpdateInspectorLists();
        LoadNextQuestion();
    }

    public void LoadNextQuestion()
    {
        // Get random question
        int randomIndex = Random.Range(0, questionsListByNumber.Count);
        Question currentQuestion = questionsListByNumber[randomIndex];

        // Shuffle options
        List<string> shuffledOptions = new List<string>(currentQuestion.options);
        for (int i = shuffledOptions.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            string temp = shuffledOptions[i];
            shuffledOptions[i] = shuffledOptions[j];
            shuffledOptions[j] = temp;
        }

        // Find correct answer index
        int correctAnswerIndex = shuffledOptions.IndexOf(currentQuestion.answer);

        // Update UI
        uiManager.DisplayQuestion(currentQuestion, shuffledOptions, correctAnswerIndex);
    }

    void LoadQuestions()
    {
        // Modificado para buscar en la carpeta StreamingAssets
        string filePath = Path.Combine(Application.dataPath, "StreamingAssets/questions_Enurm.json");
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            var jsonData = JSON.Parse(dataAsJson)["questions"].AsArray;

            questionsByNumber = new Dictionary<int, Question>();

            foreach (KeyValuePair<string, JSONNode> kvp in jsonData)
            {
                var item = kvp.Value;
                var question = new Question
                {
                    id = item["id"].AsInt,
                    category = item["category"],
                    question = item["question"],
                    options = new List<string>(),
                    answer = item["answer"]
                };

                foreach (var option in item["options"].AsArray)
                {
                    question.options.Add(option.Value);
                }

                questionsByNumber.Add(question.id, question);
            }
        }
        else
        {
            Debug.LogError("Cannot find file!");
        }
    }

    void OrganizeQuestionsByCategory()
    {
        questionsByCategory = new Dictionary<string, List<Question>>();

        foreach (var question in questionsByNumber.Values)
        {
            if (!questionsByCategory.ContainsKey(question.category))
            {
                questionsByCategory[question.category] = new List<Question>();
            }
            questionsByCategory[question.category].Add(question);
        }
    }

    void DebugQuestions()
    {
        Debug.Log("Preguntas por N�mero:");
        foreach (var kvp in questionsByNumber)
        {
            Debug.Log($"ID: {kvp.Key}, Pregunta: {kvp.Value.question}");
        }

        Debug.Log("Preguntas por Categor�a:");
        foreach (var category in questionsByCategory)
        {
            Debug.Log($"Categor�a: {category.Key}");
            foreach (var question in category.Value)
            {
                Debug.Log($"  ID: {question.id}, Pregunta: {question.question}");
            }
        }
    }

    void UpdateInspectorLists()
    {
        questionsListByNumber = new List<Question>(questionsByNumber.Values);
        questionsListByCategory = new List<Question>();

        foreach (var list in questionsByCategory.Values)
        {
            questionsListByCategory.AddRange(list);
        }
    }

    public void IncrementGoodCounter()
    {
        goodQuestion_Counter++;
    }

    public void IncrementBadCounter()
    {
        badQuestion_Counter++;
    }
}
