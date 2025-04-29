using UnityEngine;
using System.Collections;

namespace EduWorld
{
    public enum ENpcState : byte
    {
        Idle = 0,
        Walk = 1,
        Wave = 2
    }

    public class NpcStateMachine : IStateMachine
    {
        private IState idleState;
        private IState walkState;
        private IState waveState;
        private IState currentState;
        NpcController npcController;


        public NpcStateMachine(NpcController npcController)
        {
            this.npcController = npcController;
            walkState = new NpcWalkState(this.npcController);
            waveState = new NpcWaveState(this.npcController);
            idleState = new NpcIdleState(this.npcController);
            Register();
            currentState = idleState;

            if (currentState != null)
            {
                currentState.Enter();
            }
        }

        private void Register()
        {
            if (idleState is NpcIdleState idleStateInstance)
            {
                idleStateInstance.onPlayerLost += OnPlayerLost;
            }

            if (walkState is NpcWalkState walkStateInstance)
            {
                walkStateInstance.OnSplineNotAvailable += HandleSplineNotAvailable;
            }
        }

        private void OnPlayerLost()
        {
            ChangeState(ENpcState.Walk);
        }

        private void HandleSplineNotAvailable()
        {
            ChangeState(ENpcState.Idle);
        }

        public void ChangeState(ENpcState EnewEState)
        {
            IState newState = null;
            switch (EnewEState)
            {
                case ENpcState.Idle:
                    newState = idleState;
                    break;
                case ENpcState.Walk:
                    newState = walkState;
                    break;
                case ENpcState.Wave:
                    newState = waveState;
                    break;
            }
            if (newState == currentState) return;
            currentState?.Exit();
            currentState = newState;
            currentState.Enter();
        }

        public void Update()
        {
            currentState.StateUpdate();
        }
    }
}
