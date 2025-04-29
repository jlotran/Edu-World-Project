using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RGSK
{
    public class QuestionManager : Singleton<QuestionManager>
    {
        public List<EQuestionType> AlowQuestionType;
        private BaseQuestion currentQuestion;
        private EQuestionType currentQuestionType;

        [Space(10)]
        [Header("Question Lists")]
        public List<BaseQuestion> Questions;
        public List<FillBlankQuestion> FillBlankQuestions;
        public List<MatchingQuestion> MatchingQuestions;
        public List<MultipleChoiceQuestion> MultipleChoiceQuestions;
        public List<OrderingQuestion> OrderingQuestions;
        public List<TrueFalseQuestion> TrueFalseQuestions;

        public event Action<BaseQuestion> OnQuestionSelected;
        private QuestionLoader questionLoader;
        public Dictionary<BaseQuestion, bool> answerResult { get; private set; }

        private void Start()
        {
            questionLoader = new QuestionLoader();
            Questions = questionLoader.LoadQuestions();
            answerResult = new Dictionary<BaseQuestion, bool>();
            RGSKEvents.OnRaceStateChanged.AddListener(OnRaceStateChanged);
            if (Questions != null)
            {
                FillBlankQuestions = Questions.OfType<FillBlankQuestion>().ToList();
                MatchingQuestions = Questions.OfType<MatchingQuestion>().ToList();
                MultipleChoiceQuestions = Questions.OfType<MultipleChoiceQuestion>().ToList();
                OrderingQuestions = Questions.OfType<OrderingQuestion>().ToList();
                TrueFalseQuestions = Questions.OfType<TrueFalseQuestion>().ToList();
                return;
            }

            Debug.LogError("Failed to load questions.");

        }

        private void OnRaceStateChanged(RaceState state)
        {
            if (state != RaceState.PostRace)
                return;
            foreach (var question in answerResult)
            {
                Debug.Log($"Question: {question.Key.GetQuestionText()} - Answered Correctly: {question.Value}");
            }
        }

        [ContextMenu("Get Question")]
        public BaseQuestion GetQuestion()
        {
            currentQuestion = GetRandomQuestion();
            OnQuestionSelected?.Invoke(currentQuestion);
            return currentQuestion;
        }

        public BaseQuestion GetQuestionAI()
        {
            return GetRandomQuestionAI();
        }

        private BaseQuestion GetRandomQuestion()
        {
            EQuestionType rdQuestionType = AlowQuestionType[UnityEngine.Random.Range(0, AlowQuestionType.Count)];
            currentQuestionType = rdQuestionType;
            switch (rdQuestionType)
            {
                case EQuestionType.fill_blank:
                    currentQuestion = FillBlankQuestions[UnityEngine.Random.Range(0, FillBlankQuestions.Count)];
                    return currentQuestion;
                case EQuestionType.matching:
                    currentQuestion = MatchingQuestions[UnityEngine.Random.Range(0, MatchingQuestions.Count)];
                    return currentQuestion;

                case EQuestionType.multiple_choice:
                    currentQuestion = MultipleChoiceQuestions[UnityEngine.Random.Range(0, MultipleChoiceQuestions.Count)];
                    return currentQuestion;
                case EQuestionType.ordering:
                    currentQuestion = OrderingQuestions[UnityEngine.Random.Range(0, OrderingQuestions.Count)];
                    return currentQuestion;
                case EQuestionType.true_false:
                    currentQuestion = TrueFalseQuestions[UnityEngine.Random.Range(0, TrueFalseQuestions.Count)];
                    return currentQuestion;

            }
            return null;
        }

        private BaseQuestion GetRandomQuestionAI()
        {
            EQuestionType rdQuestionType = AlowQuestionType[UnityEngine.Random.Range(0, AlowQuestionType.Count)];
            BaseQuestion aiQuestion;
            switch (rdQuestionType)
            {
                case EQuestionType.fill_blank:
                    aiQuestion = FillBlankQuestions[UnityEngine.Random.Range(0, FillBlankQuestions.Count)];
                    return aiQuestion;
                case EQuestionType.matching:
                    aiQuestion = MatchingQuestions[UnityEngine.Random.Range(0, MatchingQuestions.Count)];
                    return aiQuestion;

                case EQuestionType.multiple_choice:
                    aiQuestion = MultipleChoiceQuestions[UnityEngine.Random.Range(0, MultipleChoiceQuestions.Count)];
                    return aiQuestion;
                case EQuestionType.ordering:
                    aiQuestion = OrderingQuestions[UnityEngine.Random.Range(0, OrderingQuestions.Count)];
                    return aiQuestion;
                case EQuestionType.true_false:
                    aiQuestion = TrueFalseQuestions[UnityEngine.Random.Range(0, TrueFalseQuestions.Count)];
                    return aiQuestion;

            }
            return null;
        }
        #region Test Editor
        [ContextMenu("Load Questions")]
        public void LoadQuestion()
        {
            questionLoader = new QuestionLoader();
            Questions = questionLoader.LoadQuestions();
            if (Questions != null)
            {
                FillBlankQuestions = Questions.OfType<FillBlankQuestion>().ToList();
                MatchingQuestions = Questions.OfType<MatchingQuestion>().ToList();
                MultipleChoiceQuestions = Questions.OfType<MultipleChoiceQuestion>().ToList();
                OrderingQuestions = Questions.OfType<OrderingQuestion>().ToList();
                TrueFalseQuestions = Questions.OfType<TrueFalseQuestion>().ToList();
            }
        }

        #endregion
        internal void OnAnswerSingleStringComplete(string obj)
        {
            bool IsCorrect = currentQuestion.IsCorrectAnswer(obj);
            RGSKEvents.OnAnswerResponse?.Invoke(IsCorrect);
            answerResult[currentQuestion] = IsCorrect;
        }

        internal void OnAnswerTrueFalseComplete(bool obj)
        {
            bool IsCorrect = currentQuestion.IsCorrectAnswer(obj);
            answerResult[currentQuestion] = IsCorrect;
            RGSKEvents.OnAnswerResponse?.Invoke(IsCorrect);
        }

    }
}
