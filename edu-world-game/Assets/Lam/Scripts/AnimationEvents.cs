using UnityEngine;

namespace LamFusion
{
    public class AnimationEvents : MonoBehaviour
    {
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;
        public AudioClip LandingAudioClip;

        [SerializeField] private PlayerSound _playerSound;

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    // AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.position, FootstepAudioVolume);
                    _playerSound.PlayOneShot(FootstepAudioClips[index]);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                // AudioSource.PlayClipAtPoint(LandingAudioClip, transform.position, FootstepAudioVolume);
                _playerSound.PlayOneShot(LandingAudioClip);
            }
        }
    }
}
