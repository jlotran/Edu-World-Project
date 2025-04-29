using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace Rukha93.ModularAnimeCharacter.Customization.UI
{
    public class UICategoryItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_CategoryTitle;
        // [SerializeField] private Text m_EquipedItemTitle;
        [SerializeField] private Image m_EquipedItemImage;
        [SerializeField] private Button m_Button;
        [SerializeField] private ScrollRect scrollRect;


        [SerializeField] private Sprite sprite1;
        [SerializeField] private Sprite sprite2;
        [SerializeField] private float animationDuration = 0.3f;
        
        [SerializeField]private Image Background;
        public System.Action OnClick;

        private CategoryTweener tweener;
        private bool isHovered = false;

        public string Title
        {
            get => m_CategoryTitle.text;
            set => m_CategoryTitle.text = value;
        }

        // public string Value
        // {
        //     get => m_EquipedItemTitle.text;
        //     set => m_EquipedItemTitle.text = value;
        // }
        public Sprite Icon
        {
            get => m_EquipedItemImage.sprite;
            set => m_EquipedItemImage.sprite = value;
        }

        private void Awake()
        {
            Background = GetComponentInChildren<Image>();
            m_Button.onClick.AddListener(Callback_OnClick);
            m_Button.onClick.AddListener(ChangeCategoryChoose);
            m_Button.onClick.AddListener(() => scrollRect.verticalNormalizedPosition = 1f);

            // Initialize CategoryTweener
            tweener = new CategoryTweener(
                transform,
                Background,
                animationDuration,
                1.1f,  // scale multiplier
                5f,    // rotation angle
                1.05f, // hover scale
                0.2f   // hover duration
            );
        }

        void Start()
        {
            // Background.sprite = sprite1;
        }
        public void SetupBackground(){
            Background.sprite = sprite1;
        }
        public void SetupBackgroundForShop(string category){
           if(category == "hairstyle"){
            ChangeCategoryChoose();
           }
           else{
            Background.sprite = sprite1;
           }
        }

        public void setNormal()
        {
            Background.sprite = sprite1;
            // Reset transform properties
            transform.localScale = Vector3.one;
            transform.rotation = Quaternion.identity;
        }

        private void Callback_OnClick()
        {
            OnClick?.Invoke();
        }

        public void ChangeCategoryChoose()
        {
            tweener.PlaySelectAnimation(sprite2);
        }


        public void ResetToLastCategory()
        {
            tweener.PlayResetAnimation(sprite1);
        }

        public void OnPointerEnter()
        {
            if (!isHovered)
            {
                isHovered = true;
                tweener.PlayHoverAnimation(true);
            }
        }

        public void OnPointerExit()
        {
            if (isHovered)
            {
                isHovered = false;
                tweener.PlayHoverAnimation(false);
            }
        }

        private void OnDestroy()
        {
            if (tweener != null)
            {
                tweener.Cleanup();
            }
        }

    }
}