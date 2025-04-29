using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

namespace EduWorld
{
    public class NpcController : MonoBehaviour
    {
        [Header("References")]
        private NpcStateMachine stateMachine;
        public Animator animator;
        public CharacterController characterController;
        // private NpcAnimationEventHandler animationEventHandler;
        public NpcSensor npcSensor;
        public Spline spline;
        public int splineCount;

        [Header("Atribute")]
        public float walkSpeed = 0.1f;
        private float gravity = 9.81f;
        private float velocityY = 0f;

        void Start()
        {
            stateMachine = new NpcStateMachine(this);
            animator = GetComponent<Animator>();
            // animationEventHandler = GetComponent<NpcAnimationEventHandler>();
            npcSensor = GetComponent<NpcSensor>();
            RegisterEvent();
            npcSensor.StartCheckPlayer();
        }

        public void SetSpline(Spline spline)
        {
            this.spline = spline;
            splineCount = this.spline.Count;
        }

        private void NpcPhysics()
        {
            if (!characterController.isGrounded)
            {
                velocityY -= gravity * Time.deltaTime;
            }
            else
            {
                velocityY = -0.1f;
            }

            Vector3 gravityMovement = new Vector3(0, velocityY, 0);
            characterController.Move(gravityMovement * Time.deltaTime);
        }

        private void RegisterEvent()
        {
            // animationEventHandler.onEventWaveEnd += OnAnimationWaveEnd;
            npcSensor.onPlayerDetected += OnPlayerDetected;
            npcSensor.onPlayerLost += OnPlayerLost;
            npcSensor.onRotateToPlayerDirection += OnRotateToPlayerDirection;
        }

        private void OnPlayerDetected()
        {
            stateMachine.ChangeState(ENpcState.Wave);
        }

        private void OnPlayerLost()
        {
            StopAllCoroutines();
            stateMachine.ChangeState(ENpcState.Idle);
            StartCoroutine(ChangeToWalk());
        }

        IEnumerator ChangeToWalk()
        {
            yield return new WaitForSeconds(3f);
            stateMachine.ChangeState(ENpcState.Walk);
        }

        private void OnRotateToPlayerDirection(Transform player)
        {
            StartCoroutine(RotateTowards(player));
        }

        IEnumerator RotateTowards(Transform playerDirection)
        {
            if (splineCount <= 1)
            {
                yield break;
            }

            while (playerDirection != null)
            {
                Vector3 direction = playerDirection.position - transform.position;
                direction.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * npcSensor.rotationSpeed);
                yield return null;
            }
        }

        // private void OnAnimationWaveEnd()
        // {
        //     OnPlayerLost();
        // }

        void Update()
        {
            NpcPhysics();
            if (stateMachine == null)
            {
                return;
            }
            stateMachine.Update();
        }
    }
}
