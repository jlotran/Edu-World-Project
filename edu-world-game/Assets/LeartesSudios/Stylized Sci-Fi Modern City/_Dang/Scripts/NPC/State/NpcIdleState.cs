using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace EduWorld
{
    public class NpcIdleState : IState
    {
        private NpcController npcController;
        public UnityAction onPlayerLost;
        private Coroutine checkPlayerCoroutine;

        public NpcIdleState(NpcController npcController)
        {
            this.npcController = npcController;
        }

        public void Enter()
        {
            npcController.animator.SetBool("isWalking", false);
            npcController.animator.SetBool("isWaving", false);

        }

        public void StateUpdate()
        {
            checkPlayerCoroutine = npcController.StartCoroutine(CheckPlayerAfterDelay());
        }

        private IEnumerator CheckPlayerAfterDelay()
        {
            yield return new WaitForSeconds(2f);

            if (!npcController.npcSensor.isPlayerDetected)
            {
                onPlayerLost?.Invoke();
            }
        }

        public void Exit()
        {
            if (checkPlayerCoroutine != null)
            {
                npcController.StopCoroutine(checkPlayerCoroutine);
                checkPlayerCoroutine = null;
            }
        }
    }
}
