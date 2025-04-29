using RGSK.Helpers;
using System;
using UnityEngine;
using DG.Tweening;
namespace RGSK
{
    [Serializable]
    public class VehicleQuestionCatcher : VehicleComponent
    {
        float _timeLimit;
        float _timeLimitTimer;
        public bool isAnswering { get; private set; }
        public bool isAI;
        float rdTimeAnswer;
        bool rdTrueFalse;
        Tween tweenDelay;
        public override void Initialize(VehicleController vc)
        {
            base.Initialize(vc);
            isAI = !Vehicle.GetComponent<RGSKEntity>().IsPlayer;
            if (isAI)
                return;
/*            RGSKEvents.OnAnswerResponse.AddListener(OnAnswerResponse);*/
            RGSKEvents.OnAnswerReady.AddListener(OnAnswerResponse);
        }

        private void OnAnswerResponse(bool obj)
        {
            if (!isAnswering)
            {
                return;
            }

            if (obj) {
                OnAnswerCorrect();
                return;
            }
            OnAnswerWrong();
        }

        public override void Update()
        {
            if (isAnswering)
            {
                _timeLimitTimer -= Time.deltaTime;
                if (_timeLimitTimer <= 0f)
                {
                    isAnswering = false;
                    OnAnswerWrong();
                }
                if (isAI)
                {
                    if (_timeLimitTimer <= rdTimeAnswer)
                    {
                        if (rdTrueFalse)
                            OnAnswerCorrect();
                        else
                            OnAnswerWrong();
                    }
                }
            }
        }

        private void AIAnswer(float timeLimit)
        {
            rdTimeAnswer = timeLimit - UnityEngine.Random.Range(0, 7);
            rdTrueFalse = UnityEngine.Random.Range(0, 2) == 0;
        }

        public void OnQuestionStart(float timeLimit)
        {
            if (isAnswering)
            {
                return;
            }
            EnableAIOnAnswering();
            _timeLimit = timeLimit;
            _timeLimitTimer = _timeLimit;
            AIAnswer(timeLimit);
            isAnswering = true;
            Vehicle.effect.ApplySlowEffect(timeLimit);
        }

        public void OnAnswerCorrect()
        {
            DisableAIOnAnswering();
            isAnswering = false;
            Vehicle.effect.ApplySpeedUpEffect();
        }

        public void OnAnswerWrong()
        {
            DisableAIOnAnswering();
            isAnswering = false;
            Vehicle.effect.ClearEffect();
        }

        private void EnableAIOnAnswering()
        {
            if (isAI)
            {
                return;
            }
            SetInputActive(false);
        }

        private void DisableAIOnAnswering() {
            if (isAI)
            {
                return;
            }
            SetInputActive(true);
        }

        private void SetInputActive(bool active)
        {
            if (!active)
            {
                GeneralHelper.ToggleAIInput(Vehicle.gameObject, true);
                GeneralHelper.TogglePlayerInput(Vehicle.gameObject, false);
                return;
            }
            GeneralHelper.ToggleAIInput(Vehicle.gameObject, false);
            GeneralHelper.TogglePlayerInput(Vehicle.gameObject, true);
            GeneralHelper.ToggleInputControl(Vehicle.gameObject, true);
        }

    }
}
