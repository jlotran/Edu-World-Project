using UnityEditor;
using UnityEngine;

namespace Edu_World{
    public class UIInteract : MonoBehaviour{

        public static UIInteract instance;

        void Awake(){
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
        }
        [SerializeField] private GameObject ButtonE;

        private bool isButtonActive = false;
        private bool isShopOpen = false;

        private void Start(){
            ButtonE.SetActive(false);
        }

        public void ToggleButtonE()
        {
            isButtonActive = !isButtonActive;
            ButtonE.SetActive(isButtonActive);
        }

        public void SetShopState(bool isOpen)
        {
            isShopOpen = isOpen;
            if (isOpen)
            {
                DeactiveButtonE();
            }
        }

        public void ActiveButtonE(){
            if (!isShopOpen) // Only show button E if shop is not open
            {
                isButtonActive = true;
                ButtonE.SetActive(true);
            }
        }

        public void DeactiveButtonE(){
            isButtonActive = false;
            ButtonE.SetActive(false);
        }
    }
}
