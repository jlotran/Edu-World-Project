using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace EduWorld
{
    public class NpcSensor : MonoBehaviour
    {
        public float radius = 3f;
        public float rotationSpeed = 5f;
        public LayerMask layerMask;
        public bool isPlayerDetected;
        Transform playerDetected;

        Coroutine checkPlayerCoroutine;

        public UnityAction onPlayerDetected;
        public UnityAction<Transform> onRotateToPlayerDirection;
        public UnityAction onPlayerLost;

        public void StartCheckPlayer()
        {
            checkPlayerCoroutine = StartCoroutine(CheckPlayer());
        }

        public void StopCheckPlayer()
        {
            if (checkPlayerCoroutine != null)
            {
                StopCoroutine(checkPlayerCoroutine);
            }
        }

        public IEnumerator CheckPlayer()
        {
            while (true)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layerMask);
                if (colliders.Length > 0)
                {
                    isPlayerDetected = true;
                    playerDetected = colliders[0].transform;
                    onPlayerDetected?.Invoke();
                    onRotateToPlayerDirection?.Invoke(playerDetected);
                }
                else
                {
                    if (isPlayerDetected)
                    {
                        onPlayerLost?.Invoke();
                    }
                    isPlayerDetected = false;
                }
                yield return new WaitForSeconds(1.75f);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
