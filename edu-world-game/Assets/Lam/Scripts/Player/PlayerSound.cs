using UnityEngine;

namespace LamFusion
{
    public class PlayerSound : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;

        public void PlayOneShot(AudioClip clip, float volume = 1)
        {
            _audioSource.PlayOneShot(clip);
        }
    }
}
