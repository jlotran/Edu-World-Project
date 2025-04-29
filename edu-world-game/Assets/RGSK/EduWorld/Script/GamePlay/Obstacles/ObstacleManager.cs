using RGSK.Helpers;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace RGSK
{
    public class ObstacleManager : Singleton<ObstacleManager>
    {
        ObstacleSpawner _obstacleSpawner;
        List<QuestionObstacle> _obstacleList = new List<QuestionObstacle>();
        VehicleController _currentVehicleController;
        private void Start()
        {
            _obstacleSpawner = GetComponent<ObstacleSpawner>();
            RGSKEvents.OnRaceStateChanged.AddListener(OnRaceInitialized);
        }

        private void OnRaceInitialized(RaceState state)
        {
            if (state != RaceState.Racing)
                return;
            _obstacleSpawner.SpawnObstacles();

        }

        public void OnObstacleContact(VehicleController controller)
        {
            RGSKEntity rGSKEntity = controller.GetComponent<RGSKEntity>();
            BaseQuestion questionOnContact;

            if (!rGSKEntity.IsPlayer)
            {
                questionOnContact = QuestionManager.Instance.GetQuestionAI();
                controller.questionCatcher.OnQuestionStart(questionOnContact.time_limit);
                return;
            }

            questionOnContact = QuestionManager.Instance.GetQuestion();
            _currentVehicleController = controller;
            try
            {
                _currentVehicleController.questionCatcher.OnQuestionStart(questionOnContact.time_limit);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error setting AI behaviour: {e.Message}");
            }
        }

        private void OnDestroy()
        {
            RGSKEvents.OnRaceStateChanged.RemoveListener(OnRaceInitialized);
        }
    }
}
