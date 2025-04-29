using System;
using Rukha93.ModularAnimeCharacter.Customization.UI;
using UnityEngine;

namespace EduWorld
{
    public class CustomizationCharacterAnimation : MonoBehaviour
    {
        private Animator animator;

        void Start()
        {
            animator = GetComponent<Animator>();

            // Lắng nghe sự kiện từ UIManager
            UICustomizationDemo.OnIdle += PlayIdle;
            UICustomizationDemo.OnLookAround += PlayLookAround;
        }

        private void OnDestroy()
        {
            // Hủy đăng ký sự kiện để tránh lỗi
            UICustomizationDemo.OnIdle -= PlayIdle;
            UICustomizationDemo.OnLookAround -= PlayLookAround;
        }

        private void PlayIdle()
        {
            animator.SetBool("IsLook", false);
        }

        private void PlayLookAround()
        {
            animator.SetBool("IsLook", true);
        }
    }
}