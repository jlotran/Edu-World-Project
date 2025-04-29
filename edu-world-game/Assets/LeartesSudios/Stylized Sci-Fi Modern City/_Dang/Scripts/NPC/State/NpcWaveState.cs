using UnityEngine;

namespace EduWorld
{
    public class NpcWaveState : IState
    {
        private NpcController npcController;

        public NpcWaveState(NpcController npcController)
        {
            this.npcController = npcController;
        }
        public void Enter()
        {
            npcController.animator.SetBool("isWaving", true);
        }

        public void StateUpdate()
        {
        }

        public void Exit()
        {
            npcController.animator.SetBool("isWaving", false);
        }
    }
}
