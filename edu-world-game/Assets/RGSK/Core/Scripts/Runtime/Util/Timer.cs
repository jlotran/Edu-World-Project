using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fusion;
namespace RGSK
{
    public class Timer : NetworkBehaviour
    {
        [SerializeField] float value;
        [SerializeField] protected bool isRunning;
        [SerializeField] protected bool countdown;
        [SerializeField] protected bool autoReset;

        public float Value => value;
        public bool IsRunning => isRunning;

        public Action OnTimerElapsed;
        public Action OnTimeStart;
        public Action OnTimerStop;
        public Action OnTimerRestart;

        float _startValue;
        bool _fixedUpdate;

        public void Initialize(float startingValue, bool countdown, bool autoReset, bool fixedUpdate = false)
        {
            isRunning = false;
            _fixedUpdate = fixedUpdate;
            value = _startValue = startingValue;
            this.countdown = countdown;
            this.autoReset = autoReset;
        }

        public void StartTimer()
        {
            if (isRunning)
                return;

            isRunning = true;
            OnTimeStart?.Invoke();
        }

        public void StopTimer()
        {
            if (!isRunning)
                return;

            isRunning = false;
            OnTimerStop?.Invoke();
        }

        public void RestartTimer()
        {
            value = _startValue;
            OnTimerRestart?.Invoke();
        }

        public void ResetTimer()
        {
            isRunning = false;
            value = _startValue;
        }

        public void AddTimerValue(float value)
        {
            this.value += value;
        }

        public void SetValue(float value) => this.value = value;

        protected virtual void FixedUpdate()
        {
            if (!_fixedUpdate)
                return;

            Run(Time.fixedDeltaTime);
        }

        protected virtual void Update()
        {
            if (_fixedUpdate)
                return;

            Run(Time.deltaTime);
        }

        protected virtual void Run(float deltaTime)
        {
            if (!isRunning)
                return;

            if (countdown)
            {
                if (value > 0)
                {
                    value -= deltaTime;
                }
                else
                {
                    OnTimerElapsed?.Invoke();

                    if (autoReset)
                    {
                        RestartTimer();
                    }
                    else
                    {
                        StopTimer();
                    }
                }
            }
            else
            {
                value += deltaTime;
            }
        }
    }
}