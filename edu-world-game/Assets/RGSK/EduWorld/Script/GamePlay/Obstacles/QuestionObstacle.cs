using System;
using UnityEngine;
using DG.Tweening;
namespace RGSK
{
    public class QuestionObstacle : MonoBehaviour, IQuestionObstacle
    {
        public event Action<VehicleController> OnContactEvent;
        Sequence sequenceFloating;
        Sequence sequenceBlink;
        [SerializeField] private Transform _modelTranform;

        private void Awake()
        {
            OnContactEvent += ObstacleManager.Instance.OnObstacleContact;
            TweenRotateYAndFloatUpDown();
        }

        private void TweenRotateYAndFloatUpDown()
        {
            sequenceFloating = DOTween.Sequence();
            sequenceFloating.Append(_modelTranform.DOLocalRotate(new Vector3(0, 360, 0), 3f, RotateMode.FastBeyond360).SetEase(Ease.Linear));
            sequenceFloating.Join(_modelTranform.DOLocalMoveY(_modelTranform.localPosition.y + 0.5f, 1f).SetEase(Ease.InOutSine));
            sequenceFloating.SetLoops(-1, LoopType.Yoyo);
        }

        private void TweenBlink()
        {
            sequenceBlink = DOTween.Sequence();
            for (int i = 0; i < 4; i++)
            {
                sequenceBlink.AppendCallback(() => _modelTranform.gameObject.SetActive(!_modelTranform.gameObject.active));
                sequenceBlink.AppendInterval(0.2f);
            }

            sequenceBlink.OnComplete(() =>
            {
                _modelTranform.gameObject.SetActive(true);
            });
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other is not BoxCollider)
                return;
            TweenBlink();
            OnContact(other.gameObject);
        }

        public void OnContact(GameObject other)
        {
            if (RaceManager.Instance.CurrentState != RaceState.Racing)
                return;
            VehicleController vehicle = other.GetComponentInParent<VehicleController>();
            RGSKEntity entity = other.GetComponentInParent<RGSKEntity>();

            if (vehicle != null)
            {
                if (vehicle.questionCatcher.isAnswering)
                    return;
                OnContactEvent?.Invoke(vehicle);
            }
            
        }

        private void OnDestroy()
        {
            DOTween.Kill(sequenceBlink);
            DOTween.Kill(sequenceFloating);
        }
    }

    public enum EQuestionObstacleType
    {
        Zone,
        Object
    }
}
