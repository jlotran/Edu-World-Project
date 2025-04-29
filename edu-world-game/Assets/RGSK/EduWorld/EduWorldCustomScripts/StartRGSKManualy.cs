using UnityEngine;
using UnityEngine.SceneManagement;
namespace RGSK
{
    public class StartRGSKManualy : MonoBehaviour
    {
        private void Awake()
        {
            InitializationLoader.Execute();
        }
        private void Start()
        {
            Invoke("LoadDemo", 1);
        }

        [ContextMenu("LoadDemo")]
        public void LoadDemo()
        {
            // AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("EduRacing", LoadSceneMode.Additive);
            
        }
    }
}
