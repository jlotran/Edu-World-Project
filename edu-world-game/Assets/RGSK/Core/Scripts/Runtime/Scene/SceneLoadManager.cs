using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RGSK.Helpers;

namespace RGSK
{
    public class SceneLoadManager : MonoBehaviour
    {
        public static string SceneToLoad;
        public static float LoadingProgress { get; private set; }

        void Start()
        {
            if (!string.IsNullOrWhiteSpace(SceneToLoad))
            {
                StartCoroutine(LoadSceneAsync(SceneToLoad));
            }
        }

        public static void LoadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(RGSKCore.Instance.SceneSettings.loadingScene))
            {
                Logger.Log("A loading scene has not been assigned! Please assign one in the RGSK Settings Window");
                return;
            }

            SceneToLoad = sceneName;
            OpenLoadingScreen();
            RGSKEvents.OnBeforeSceneLoad.Invoke();
            Application.backgroundLoadingPriority = RGSKCore.Instance.SceneSettings.backgroundLoadingPriority;
            SceneManager.LoadScene(RGSKCore.Instance.SceneSettings.loadingScene);
        }

        public static void LoadScene(SceneReference scene)
        {
            if (string.IsNullOrEmpty(scene))
            {
                Logger.Log("The scene reference you are trying to load has not been assigned!");
                return;
            }

            LoadScene(scene.ScenePath);
        }

        public static void ReloadScene() => LoadScene(SceneManager.GetActiveScene().name);

        public static void LoadMainScene()
        {
            if (ChampionshipManager.Instance.Initialized)
            {
                ChampionshipManager.Instance.EndChampionship();
            }

            LoadScene(RGSKCore.Instance.SceneSettings.mainMenuScene);
        }

        public static void LoadIntermissionRaceScene()
        {
            LoadScene(RGSKCore.Instance.SceneSettings.intermissionRaceScene);
            // UIManager.Instance?.CloseAllScreens();
            // UIManager.Instance?.ClearScreenHistory();

            // // Start coroutine to delay screen destruction
            // if (UIManager.Instance != null)
            // {
            //     var delay = RGSKCore.Instance.SceneSettings.loadCompleteDelay + 1f;
            //     IEnumerator DelayedDestroy()
            //     {
            //         yield return GeneralHelper.GetCachedWaitForSeconds(delay);
            //         UIManager.Instance?.DestroyAllScreens();
            //         GeneralHelper.DestroyDynamicParent();
            //         InitializationLoader.DestroyPersistentObjects();
            //     }
            //     UIManager.Instance.StartCoroutine(DelayedDestroy());
            // }
        }

        public static void QuitApplication()
        {
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        IEnumerator LoadSceneAsync(string sceneName)
        {
            LoadingProgress = 0;
            var operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;

            while (operation.progress < 0.9f)
            {
                LoadingProgress = operation.progress;
                yield return null;
            }

            LoadingProgress = 1;

            yield return GeneralHelper.GetCachedWaitForSeconds(RGSKCore.Instance.SceneSettings.loadCompleteDelay);

            operation.allowSceneActivation = true;
            RGSKEvents.OnAfterSceneLoad.Invoke();
        }

        static void OpenLoadingScreen()
        {
            UIManager.Instance?.CloseAllScreens();

            var screen = RGSKCore.Instance.UISettings.screens.loadingScreen;

            if (screen != null)
            {
                UIManager.Instance.OpenScreen(screen);
                LoadingScreenManager.instance?.ShowLoading(LoadingScreenType.Loading);
            }
            else
            {
                Logger.LogWarning("A loading screen has not been assigned! Please assign one in the RGSK Menu under the 'UI' tab.");
            }

            UIManager.Instance?.ClearScreenHistory();
        }
    }
}