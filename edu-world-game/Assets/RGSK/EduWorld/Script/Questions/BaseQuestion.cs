using System;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    [Serializable]
    public abstract class BaseQuestion
    {
        public string id;
        public EQuestionCategory category;
        public EQuesionLevel level;
        public EQuestionType type;
        public string hint;
        public float time_limit;

        public BaseQuestion(string id, EQuestionCategory category, EQuesionLevel level, EQuestionType type, string hint, float time_limit)
        {
            this.id = id;
            this.category = category;
            this.level = level;
            this.type = type;
            this.hint = hint;
            this.time_limit = time_limit;
        }

        protected BaseQuestion()
        {
        }

        public abstract bool IsCorrectAnswer(object answer);

        public virtual string GetQuestionText()
        {
            return $"Question ID: {id}, Category: {category}, Level: {level}, Type: {type}, Hint: {hint}, Time Limit: {time_limit}";
        }
    }

    [Serializable]
    public class FillBlankQuestion : BaseQuestion
    {
        public string question;
        public List<string> options;
        public string answer;

        public FillBlankQuestion()
        {
        }

        public FillBlankQuestion(string id, EQuestionCategory category, EQuesionLevel level, string hint, float time_limit, string question)
            : base(id, category, level, EQuestionType.fill_blank, hint, time_limit) { 
            this.question = question;
            this.answer = question;
            this.options = new List<string> { question };
        }

        public override bool IsCorrectAnswer(object userAnswer)
        {
            return userAnswer is string str && str.Equals(answer, StringComparison.OrdinalIgnoreCase);
        }

        public override string GetQuestionText()
        {
            return question;
        }
    }

    [Serializable]
    public class MatchingPair
    {
        public string left;
        public string right;
    }

    [Serializable]
    public class MatchingQuestion : BaseQuestion
    {
        public List<MatchingPair> options;
        public List<string> answer;

        public MatchingQuestion()
        {
        }

        public MatchingQuestion(string id, EQuestionCategory category, EQuesionLevel level, string hint, float time_limit)
            : base(id, category, level, EQuestionType.matching, hint, time_limit) { }

        public override bool IsCorrectAnswer(object userAnswer)
        {
            if (userAnswer is not List<string>)
            {
                Debug.LogError("userAnswer is not List<string>");
                return false;
            }

            List<string> userAnswerList = (List<string>)userAnswer;

            if (userAnswerList.Count != answer.Count)
            {
                Debug.LogError("userAnswerList.Count != answer.Count");
                return false;
            }

            for (int i = 0; i < userAnswerList.Count; i++)
            {
                if (!userAnswerList[i].Equals(answer[i], StringComparison.OrdinalIgnoreCase))
                {
                    Debug.LogError($"userAnswerList[{i}] != answer[{i}]");
                    return false;
                }
            }
            return true;   
        }

        public override string GetQuestionText()
        {
            return $"Match the following pairs";
        }
    }

    [Serializable]
    public class MultipleChoiceQuestion : BaseQuestion
    {
        public string question;
        public List<string> options;
        public string answer;
        public List<string> help_available;
        public MultipleChoiceQuestion()
        {
        }

        public MultipleChoiceQuestion(string id, EQuestionCategory category, EQuesionLevel level, string hint, float time_limit, string question)
            : base(id, category, level, EQuestionType.multiple_choice, hint, time_limit) { 
            this.question = question;
        }

        public override bool IsCorrectAnswer(object userAnswer)
        {
            return userAnswer is string str && str.Equals(answer);
        }

        public override string GetQuestionText()
        {
            return question;
        }
    }

    [Serializable]
    public class OrderingQuestion : BaseQuestion
    {
        public string question;
        public List<string> options;
        public List<int> answer;

        public OrderingQuestion()
        {
        }

        public OrderingQuestion(string id, EQuestionCategory category, EQuesionLevel level, string hint, float time_limit,string question)
            : base(id, category, level, EQuestionType.ordering, hint, time_limit) { 
            this.question = question;
        }

        public override bool IsCorrectAnswer(object userAnswer)
        {
            if (userAnswer is not List<int>)
            {
                Debug.LogError("userAnswer is not List<int>");
                return false;
            }
            List<int> userAnswerList = (List<int>)userAnswer;
            if (userAnswerList.Count != answer.Count)
            {
                Debug.LogError("userAnswerList.Count != answer.Count");
                return false;
            }
            for (int i = 0; i < userAnswerList.Count; i++)
            {
                if (userAnswerList[i] != answer[i])
                {
                    Debug.LogError($"userAnswerList[{i}] != answer[{i}]");
                    return false;
                }
            }
            return true;
        }

        public override string GetQuestionText()
        {
            return question;
        }
    }

    [Serializable]
    public class TrueFalseQuestion : BaseQuestion
    {
        public string question;
        public bool answer;

        public TrueFalseQuestion()
        {
        }

        public TrueFalseQuestion(string id, EQuestionCategory category, EQuesionLevel level, string hint, float time_limit,string question)
            : base(id, category, level, EQuestionType.true_false, hint, time_limit) { 
            this.question = question;
        }

        public override bool IsCorrectAnswer(object userAnswer)
        {
            return userAnswer is bool b && b == answer;
        }
        public override string GetQuestionText()
        {
            return question;
        }
    }

    public enum EQuestionType
    {
        fill_blank,
        matching,
        multiple_choice,
        ordering,
        true_false
    }

    public enum EQuesionLevel
    {
        intern,
        fresher,
        junior,
        mid,
        senior
    }

    public enum EQuestionCategory
    {
        history,
        vocabulary,
        programming,
        process,
        logic,
        technology,
        general,
        data, 
        syntax,
        science,
        network,
    }
}
