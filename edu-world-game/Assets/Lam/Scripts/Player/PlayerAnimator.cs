using UnityEngine;

namespace Lam.FUSION
{
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        public Animator animator => _animator;


        private float _previousSpeed;


            
        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        private bool isSetJumping;

        void Update()
        {
            if (_animator == null)
            {
                _animator = GetComponentInChildren<Animator>();
                if (_animator)
                {
                    _animIDSpeed = Animator.StringToHash("Speed");
                    _animIDGrounded = Animator.StringToHash("Grounded");
                    _animIDJump = Animator.StringToHash("Jump");
                    _animIDFreeFall = Animator.StringToHash("FreeFall");
                    _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
                }
            }
        }

        public void SetSpeed(float newSpeed)
        {
            if (_animator == null) return;
            _animator.SetFloat(_animIDSpeed, newSpeed);
        }

        public void SetMotionSpeed(float motionSpeed)
        {
            if (_animator != null)
                _animator.SetFloat(_animIDMotionSpeed, motionSpeed);

        }

        public void SetJump(bool isJumping)
        {
            if (_animator == null) return;

            _animator.SetBool(_animIDJump, isJumping);
        }

        public void SetFall(bool isFalling)
        {
            if (_animator == null) return;

            _animator.SetBool(_animIDFreeFall, false);
        }

        public void SetIsGround(bool isGround)
        {
            if (_animator == null) return;

            _animator.SetBool(_animIDGrounded, isGround);
        }
    }
}
