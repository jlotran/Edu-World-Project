using UnityEngine;

namespace RGSK
{
    public class QuestionPresenter
    {
        private QuestionManager _questionManager;
        private QuestionPanelView _questionPanelView;

        public QuestionPresenter(QuestionPanelView questionPanelView)
        {
            _questionManager = QuestionManager.Instance;
            this._questionPanelView = questionPanelView;
            _questionManager.OnQuestionSelected += _questionPanelView.Show;
            _questionPanelView.OnAnswerSingleStringComplete += _questionManager.OnAnswerSingleStringComplete;
            _questionPanelView.OnAnswerTrueFalseComplete += _questionManager.OnAnswerTrueFalseComplete;
        }
    }
}
