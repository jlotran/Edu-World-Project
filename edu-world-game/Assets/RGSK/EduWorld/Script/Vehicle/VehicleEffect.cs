using RGSK.Extensions;
using System;
using UnityEngine;

namespace RGSK
{
    [Serializable]
    public class VehicleEffect : VehicleComponent
    {
        [SerializeField, Tooltip("The type of effect to apply to the vehicle.")]
        public EVehicleEffectType CurrentEffect { get; set; } = EVehicleEffectType.None;
        [SerializeField, Tooltip("Slow Speed (Km/H)")]
        public float SlowSpeedAmount = 40; //Km/h

        [SerializeField, Tooltip("Slow Speed duration (Second)")]
        public float SlowSpeedDuration = 15f; //seconds
        private float _slowTime = 0f;

        [SerializeField, Tooltip("Speed Up Speed (Km/H)")]
        public float SpeedUpSpeedAmount = 120; //Km/h
        [SerializeField,Tooltip("Speed Up duration (Second)")]
        public float SpeedUpDuration = 2f; //seconds
        private float _speedUpTime = 0f;

        public override void Update()
        {
            if (CurrentEffect == EVehicleEffectType.None)
            {
                return;
            }
            if (CurrentEffect == EVehicleEffectType.SlowZone)
            {
                if (_slowTime < SlowSpeedDuration)
                {
                    _slowTime += Time.deltaTime;
                }
                else
                {
                    CurrentEffect = EVehicleEffectType.None;
                    _slowTime = 0f;
                }
                if(Vehicle.Entity.Rigid.SpeedInKPH() > SlowSpeedAmount)
                {
                    Vehicle.Entity.Rigid.linearVelocity  = SlowSpeedAmount / 3.6f * Vehicle.Entity.Rigid.transform.forward;
                }
                return;
            }

            if (CurrentEffect == EVehicleEffectType.SpeedUp)
            {
                if (_speedUpTime < SpeedUpDuration)
                {
                    _speedUpTime += Time.deltaTime;
                }
                else
                {
                    CurrentEffect = EVehicleEffectType.None;
                    _speedUpTime = 0f;
                }
                if (Vehicle.Entity.Rigid.SpeedInKPH() < SpeedUpSpeedAmount)
                {
                    /*                    Vehicle.EntitwDDy.Rigid.linearVelocity = SpeedUpSpeedAmount / 3.6f * Vehicle.Entity.Rigid.transform.forward;*/
                    Vehicle.ApplyBooters();
                }
                return;
            }
        }

        public void ApplySlowEffect(float time)
        {
            CurrentEffect = EVehicleEffectType.SlowZone;
            SlowSpeedDuration = time;
        }

        public void ApplySpeedUpEffect(float time = 2f)
        {
            CurrentEffect = EVehicleEffectType.SpeedUp;
            SpeedUpDuration = time;
        }

        public void ClearEffect()
        {
            CurrentEffect = EVehicleEffectType.None;
            _slowTime = 0f;
            _speedUpTime = 0f;
        }
    }

    [Serializable]
    public enum EVehicleEffectType
    {
        None,
        SlowZone,
        SpeedUp,
    }
}
