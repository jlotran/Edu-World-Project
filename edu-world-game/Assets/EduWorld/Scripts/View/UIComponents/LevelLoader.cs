using UnityEngine;
using UnityEngine.UI;

namespace EduWorld
{
    public class LevelLoaderUI : MonoBehaviour
    {
        public GameObject loadingScreen;
        public Slider slider;
        private SeverLoading severLoading;
        private void Awake()
        {
            severLoading = new SeverLoading(this);
        }

        public void LoadSever(int sceneIndex)
        {
            severLoading.LoadSever(sceneIndex);
        }

        public void ShowLoadingScreen()
        {
            loadingScreen.SetActive(true);
        }

        public void UpdateProgress(float progress)
        {
            slider.value = progress;
        }
    }
}
