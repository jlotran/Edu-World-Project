using Fusion;
using UnityEngine;

namespace Lam.FUSION
{
    public struct GameInput : INetworkInput
    {
		public Vector2 move;
		public Vector2 look;
		public bool sprint;
		public bool analogMovement;
		public float camDirect;
		public NetworkButtons Actions;
		public const int JUMP_BUTTON = 0;
    }
}
