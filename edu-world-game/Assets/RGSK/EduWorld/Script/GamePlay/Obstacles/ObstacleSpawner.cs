using RGSK.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class ObstacleSpawner : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] ObstacleRoute _obstacleRoute;
        [SerializeField] QuestionObstacle _obstaclePrefab;

        [Space(10)]
        [Header("Configs")]
        [SerializeField] private float _obstacleHigh = 1f;
        [SerializeField] private int _obstacleCountInRow;

        [HideInInspector] public List<QuestionObstacle> ObstacleList = new List<QuestionObstacle>();

        [ContextMenu("Spawn Obstacle")]
        public void SpawnObstacles()
        {
            if (_obstacleRoute == null)
            {
                Debug.LogError("Obstacle route is not assigned.");
                return;
            }
            if (gameObject.transform.childCount > 0)
                return;
            ObstacleList = new List<QuestionObstacle>();
            foreach (var node in _obstacleRoute.nodes)
            {
                Vector3 leftNodePosition = node.transform.position - (node.transform.right * node.leftWidth);
                Vector3 rightNodePosition = node.transform.position + (node.transform.right * node.rightWidth);
                float lerpValue = 1f / (_obstacleCountInRow - 1);

                for (int i = 0; i < _obstacleCountInRow; i++)
                {
                    Vector3 nodePosition = Vector3.Lerp(leftNodePosition, rightNodePosition, lerpValue * i);
                    nodePosition = nodePosition + (node.transform.up * _obstacleHigh);
                    QuestionObstacle obstacle = Instantiate(_obstaclePrefab, nodePosition, _obstaclePrefab.transform.rotation);
                    obstacle.transform.SetParent(gameObject.transform);
                    ObstacleList.Add(obstacle);
                }
            }
        }
    }
}
