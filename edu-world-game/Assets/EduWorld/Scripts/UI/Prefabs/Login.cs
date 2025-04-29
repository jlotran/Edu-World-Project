using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LamFusion
{
    public class Login : MonoBehaviour
    {
        [SerializeField] private Button login_btn;

        private void Start() 
        {
            login_btn.onClick.AddListener(OnLoginBtnClick);
        }

        private void OnLoginBtnClick()
        {
            SceneManager.LoadScene("CharacterCustomization");
        }
    }
}
