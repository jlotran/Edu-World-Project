using UnityEngine;

namespace EduWorld
{
    public class DoorBehavior : MonoBehaviour
    {
        private Animator doorAnimator;
        private bool isEnable;

        [SerializeField] private BoxCollider doorA;
        [SerializeField] private BoxCollider doorB;

        void Start()
        {
            doorAnimator = GetComponent<Animator>();
            if (doorAnimator == null)
            {
                Debug.LogError("🚨 Door Animator is NULL! Please assign an Animator to the door.");
            }
            if (doorA == null || doorB == null)
            {
                Debug.LogError("🚨 Door colliders (doorA or doorB) are NULL! Please assign them in the Inspector.");
            }
        }

        void OnEnable()
        {
            isEnable = true;
        }

        void OnDisable()
        {
            isEnable = false;
        }

        void OnTriggerEnter(Collider other)
        {
            if (!isEnable) return;

            if (other.transform.parent != null && other.transform.parent.CompareTag("Player"))
            {
                // Open door
                doorAnimator.SetBool("isInteract", true);
                doorA.enabled = false;
                doorB.enabled = false;

                // Hủy coroutine nếu đang chạy để tránh reset sớm
                StopAllCoroutines();
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (!isEnable) return;

            if (other.transform.parent != null && other.transform.parent.CompareTag("Player"))
            {
                // Delay 4s before close door
                StartCoroutine(CloseDoorAfterDelay(4f));
            }
        }

        private System.Collections.IEnumerator CloseDoorAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            // Close door
            doorAnimator.SetBool("isInteract", false);
            doorA.enabled = true;
            doorB.enabled = true;
        }
    }
}
