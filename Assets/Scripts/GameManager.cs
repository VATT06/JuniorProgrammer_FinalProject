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

    public Dictionary<int, Question> questionsByNumber;
    public Dictionary<string, List<Question>> questionsByCategory;
    [SerializeField] private List<Question> questionsListByNumber;
    [SerializeField] private List<Question> questionsListByCategory;

    void Start()
    {
        LoadQuestions();
        OrganizeQuestionsByCategory();
        UpdateInspectorLists();
        DebugQuestions();
    }

    void LoadQuestions()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "questions_Enurm.json");
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
        Debug.Log("Preguntas por Número:");
        foreach (var kvp in questionsByNumber)
        {
            Debug.Log($"ID: {kvp.Key}, Pregunta: {kvp.Value.question}");
        }

        Debug.Log("Preguntas por Categoría:");
        foreach (var category in questionsByCategory)
        {
            Debug.Log($"Categoría: {category.Key}");
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
}
