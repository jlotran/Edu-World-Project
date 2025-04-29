using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace RGSK
{
    public class DigitalGauge : Gauge
    {
        [SerializeField] Image fillImage;
        [SerializeField] float tweenDuration = 0.3f;
        private Tween currentTween;

        public override void SetValue(float value)
        {
            if (fillImage == null)
                return;

            base.SetValue(value);
            // fillImage.fillAmount = _reading;


            
            // Kill previous tween if it exists
            if (currentTween != null && currentTween.IsPlaying())
            {
                currentTween.Kill();
            }

            // Create new tween
            currentTween = fillImage.DOFillAmount(_reading, tweenDuration)
                .SetEase(Ease.OutCubic);
        }

        private void OnDestroy()
        {
            if (currentTween != null && currentTween.IsPlaying())
            {
                currentTween.Kill();
            }
        }
    }
}