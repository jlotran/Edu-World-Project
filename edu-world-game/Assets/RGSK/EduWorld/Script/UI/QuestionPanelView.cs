using UnityEngine;
using TMPro;
using System;
using EduWorld;
using DG.Tweening;
namespace RGSK
{
    public class QuestionPanelView : MonoBehaviour
    {

        [Header("View Component")]
        [SerializeField] private GameObject _graphicHolder;
        [SerializeField] private RectTransform _questionFramePanel;
        [SerializeField] private RectTransform _resultPanel;
        [SerializeField] private TMP_Text _questionText;
        [SerializeField] private TMP_Text _questionIdText;
        [SerializeField] private TMP_Text _questionTimeText;
        [SerializeField] private TMP_Text _questionResultText;
        [SerializeField] private RectTransform _answersContentHolder;
        [SerializeField] private AnswerItemView _answerPrefab;
        [Header("View Data")]
        [SerializeField] private EQuestionType _questionType;
        public event Action<string> OnAnswerSingleStringComplete;
        public event Action<bool> OnAnswerTrueFalseComplete;
        private QuestionPresenter _questionPresenter;


        private const string _textIncorrect = "INCONRECT";
        private const string _textCorrect = "CORRECT";
        private const string _textTakeControl = "TAKE CONTROL IN";
        private float _coolDownTimer;

        private void Start()
        {
            _questionPresenter = new QuestionPresenter(this);
            _answersContentHolder.DestroyAllChildren();
            RGSKEvents.OnAnswerResponse.AddListener(OnAnswerResponse);
        }

        private void Update()
        {
            if(_coolDownTimer > 0)
            {
                _coolDownTimer -= Time.deltaTime;
                _questionTimeText.text = _coolDownTimer.ToString("0");
            }
        }

        private void OnAnswerResponse(bool obj)
        {
            ShowResult(obj);
        }

        public void Show(BaseQuestion question)
        {
            _answersContentHolder.DestroyAllChildren();
            _graphicHolder.SetActive(true);
            _questionText.text = question.GetQuestionText();
            _questionIdText.text = question.id;
            _questionTimeText.text = question.time_limit.ToString();
            _coolDownTimer = question.time_limit;
            _questionType = question.type;
            // Populate answers based on question type
            if (question is FillBlankQuestion fillBlankQuestion)
            {
                Debug.Log($"{fillBlankQuestion.question} + {fillBlankQuestion.answer}");
                foreach (var option in fillBlankQuestion.options)
                {
                    AnswerItemView answerGO = Instantiate(_answerPrefab, _answersContentHolder);
                    answerGO.Show(option);
                    answerGO.OnAnswerSelected += OnSingleChoiceAnswerSelected;
                }
                return;
            }

            if (question is MultipleChoiceQuestion multipleChoiceQuestion)
            {
                Debug.Log($"{multipleChoiceQuestion.question} + {multipleChoiceQuestion.answer}");
                foreach (var option in multipleChoiceQuestion.options)
                {
                    AnswerItemView answerGO = Instantiate(_answerPrefab, _answersContentHolder);
                    answerGO.Show(option);
                    answerGO.OnAnswerSelected += OnSingleChoiceAnswerSelected;
                }
                return;
            }

            if (question is TrueFalseQuestion trueFalseQuestion)
            {
                Debug.Log($"{trueFalseQuestion.question} + {trueFalseQuestion.answer}");
                AnswerItemView answerGOTrue = Instantiate(_answerPrefab, _answersContentHolder);
                answerGOTrue.Show("True");
                answerGOTrue.OnAnswerSelected += OnSingleChoiceAnswerSelected;
                AnswerItemView answerGOFalse = Instantiate(_answerPrefab, _answersContentHolder);
                answerGOFalse.Show("False");
                answerGOFalse.OnAnswerSelected += OnSingleChoiceAnswerSelected;
                return;
            }

            Debug.Log("UI question NULL");
        }

        private void OnSingleChoiceAnswerSelected(string obj)
        {
            switch (_questionType)
            {
                case EQuestionType.fill_blank:
                    OnAnswerSingleStringComplete?.Invoke(obj);
                    break;
                case EQuestionType.true_false:
                    OnAnswerTrueFalseComplete?.Invoke(obj.Equals("True"));
                    break;
                case EQuestionType.multiple_choice:
                    OnAnswerSingleStringComplete?.Invoke(obj);
                    break;
            }
        }

        public void Hide()
        {
            _questionFramePanel.gameObject.SetActive(true);
            _resultPanel.gameObject.SetActive(false);
            _graphicHolder.SetActive(false);
            _questionText.text = string.Empty;
            _questionIdText.text = string.Empty;
            _questionTimeText.text = string.Empty;

            _answersContentHolder.DestroyAllChildren();
        }

        private void ShowResult(bool isCorrect)
        {
            _questionFramePanel.gameObject.SetActive(false);
            _resultPanel.gameObject.SetActive(true);
            TweenCoolDownControl(isCorrect);
        }

        private void TweenCoolDownControl(bool isCorrect)
        {
            ShowTextSequence(isCorrect);
        }

        void ShowTextSequence(bool isCorrect)
        {
            string[] texts = { isCorrect ? _textCorrect : _textIncorrect, _textTakeControl, "2", "1", "Go" };
            Sequence seq = DOTween.Sequence();

            foreach (string txt in texts)
            {
                seq.AppendCallback(() =>
                {
                    _questionResultText.rectTransform.localScale = Vector3.one;
                    _questionResultText.color = Color.white;
                    _questionResultText.text = txt;

                });
                Tween textTween = _questionResultText.DOFade(0, 0.25f);
                Tween textScaleTween = _questionResultText.transform.DOScale(1.5f, 0.25f);
                seq.Join(textTween);
                seq.Join(textScaleTween);
                if (!txt.Equals("Go"))
                {
                    seq.AppendInterval(0.25f);
                }
                seq.OnComplete(() =>
                {
                    Hide();
                    RGSKEvents.OnAnswerReady?.Invoke(isCorrect);
                });
            }
        }

        private void OnDestroy()
        {
            RGSKEvents.OnAnswerResponse.RemoveListener(OnAnswerResponse);
        }
    }
}
