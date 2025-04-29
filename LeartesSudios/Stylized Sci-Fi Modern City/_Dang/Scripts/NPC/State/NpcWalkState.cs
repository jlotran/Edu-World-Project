using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

namespace EduWorld
{
    public class NpcWalkState : IState
    {
        private float progress = 0f;
        private NpcController npcController;
        Spline spline;
        private int splineLength;

        public event UnityAction OnSplineNotAvailable;

        public NpcWalkState(NpcController npcController)
        {
            this.npcController = npcController;
            spline = npcController.spline;
            splineLength = npcController.splineCount;
        }

        public void Enter()
        {
            if (npcController.spline == null || npcController.spline.Count == 0)
            {
                OnSplineNotAvailable?.Invoke();
                return;
            }

            npcController.animator.SetBool("isWalking", true);

        }

        public void StateUpdate()
        {
            progress += npcController.walkSpeed * Time.deltaTime / spline.GetLength();
            progress = Mathf.Repeat(progress, 1f);

            Vector3 position = spline.EvaluatePosition(progress);
            position.y = npcController.transform.position.y;

            Vector3 direction = (position - npcController.transform.position).normalized;

            if (direction.magnitude > 0.01f)
            {
                npcController.characterController.Move(direction * npcController.walkSpeed * Time.deltaTime);
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                npcController.transform.rotation = Quaternion.Slerp(npcController.transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }

        public void Exit()
        {
            npcController.animator.SetBool("isWalking", false);
        }
    }
}
