using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EduWorld
{
    public class SeverLoading
    {
        private LevelLoaderUI ui;
        public SeverLoading(LevelLoaderUI ui)
        {
            this.ui = ui;
        }

        public void LoadSever(int sceneIndex)
        {
            ui.StartCoroutine(LoadAsynchronously(sceneIndex));
        }

        private IEnumerator LoadAsynchronously(int sceneIndex)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
            ui.ShowLoadingScreen();

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                ui.UpdateProgress(progress);
                yield return null;
            }
        }

    }
}
