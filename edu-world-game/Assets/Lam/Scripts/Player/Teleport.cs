using System;
using System.Collections;
using Fusion.Addons.SimpleKCC;
using UnityEngine;
using Fusion;

namespace Lam.FUSION
{
    public class Teleport : NetworkBehaviour
    {
        private SimpleKCC _Kcc;
        public Action OnStartTeleport;
        public Action OnFinishTeleport;

        [SerializeField] private float teleportDelay = 1.5f;
        [SerializeField] private float preTeleportDelay = 2.0f;
        
        [Networked] private Vector3 NetworkedDestination { get; set; }
        [Networked] private NetworkBool IsInTeleportProcess { get; set; }

        private void Start()
        {
            _Kcc = GetComponent<SimpleKCC>();
        }

        public IEnumerator TriggerTeleport(Vector3 targetPosition)
        {
            // Notify teleport start
            OnStartTeleport?.Invoke();
            
            // Wait for pre-teleport delay
            yield return new WaitForSeconds(preTeleportDelay);

            // Execute teleport
            if (Object.HasInputAuthority)
            {
                RPC_Teleport(targetPosition);
                ApplyTeleport(targetPosition);
            }

            // Wait for the teleport process to complete
            yield return new WaitForSeconds(teleportDelay);
            
            // Notify teleport completion
            OnFinishTeleport?.Invoke();
        }

        // Apply teleport position locally
        private void ApplyTeleport(Vector3 targetPos)
        {
            _Kcc.enabled = false;
            transform.position = targetPos;
            _Kcc.enabled = true;
            _Kcc.SetPosition(targetPos);
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_Teleport(Vector3 targetPos)
        {
            if (Object.HasStateAuthority)
            {
                NetworkedDestination = targetPos;
                IsInTeleportProcess = true;
                
                ApplyTeleport(targetPos);
                RPC_TeleportConfirmed(targetPos);
                
                // Schedule a single delayed reinforcement
                StartCoroutine(FinalizeTeleport(targetPos));
            }
        }

        private IEnumerator FinalizeTeleport(Vector3 targetPos)
        {
            // Brief wait to ensure position sticks
            yield return new WaitForSeconds(0.3f);
            
            ApplyTeleport(targetPos);
            RPC_TeleportConfirmed(targetPos);
            
            // Final enforcement
            yield return new WaitForSeconds(0.3f);
            
            ApplyTeleport(targetPos);
            RPC_TeleportConfirmed(targetPos);
            IsInTeleportProcess = false;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_TeleportConfirmed(Vector3 targetPos)
        {
            if (!Object.HasStateAuthority)
            {
                ApplyTeleport(targetPos);
            }
        }

        public void Subcribe(Action callbackOnStart, Action callbackOnFinish)
        {
            OnStartTeleport += callbackOnStart;
            OnFinishTeleport += callbackOnFinish;
        }
        
        public override void FixedUpdateNetwork()
        {
            // Enforce position during teleport if needed
            if (Object.HasStateAuthority && IsInTeleportProcess && 
                Vector3.Distance(transform.position, NetworkedDestination) > 0.5f)
            {
                ApplyTeleport(NetworkedDestination);
            }
        }
    }
}
