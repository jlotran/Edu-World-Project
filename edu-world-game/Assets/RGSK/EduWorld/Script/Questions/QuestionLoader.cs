using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace RGSK
{
    public class QuestionLoader
    {
        public List<BaseQuestion> LoadQuestions()
        {
            TextAsset jsonFile = Resources.Load<TextAsset>("questions_100");
            List<BaseQuestion> Questions;
            if (jsonFile != null)
            {
                Questions = JsonConvert.DeserializeObject<List<BaseQuestion>>(jsonFile.text, new QuestionConverter());
                Debug.Log($"Loaded {Questions.Count} questions from Resources.");
            }
            else
            {
                Debug.LogError("Failed to load questions_100.json from Resources.");
                return null;
            }
            return Questions;
        }
    }
}
