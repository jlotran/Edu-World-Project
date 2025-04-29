using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fusion;
using RGSK.Helpers;

namespace RGSK
{
    public class TimerFusion : Timer
    {
        [Networked] public float value { get; set; }

        public override void Spawned()
        {
            base.Spawned();
            transform.SetParent(GeneralHelper.GetDynamicParent());
        }

        protected override void Update()
        {

        }

        protected override void FixedUpdate()
        {

        }

        public override void FixedUpdateNetwork()
        {
            Run(Runner.DeltaTime);
        }

        protected override void Run(float deltaTime)
        {
            if (Object.HasStateAuthority == false)
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