using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace LamFusion
{
    public class Test : MonoBehaviour
    {
        private bool _isLoaded;
        void Start()
        {
            Addressables.InitializeAsync().Completed += OnAddressablesInitialized;
        }

        private void Update()
        {
            // if (!_isReady) return;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!_isLoaded)
                {
                    Debug.Log("Space key pressed.");
                    LoadScene();
                    _isLoaded = true;
                }
            }
        }

        private string sceneKey = "25ccbf199a926f949a73432ced168ad1";
        private void LoadScene()
        {
            Debug.Log($"📥 Bắt đầu tải scene: {sceneKey}");

            // Bước 1: Kiểm tra và tải dữ liệu trước khi load scene
            Addressables.DownloadDependenciesAsync(sceneKey).Completed += (downloadHandle) =>
            {
                if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log($"✅ Đã tải xong scene {sceneKey}, tiến hành load...");

                    // Bước 2: Sau khi tải xong, tiến hành load scene
                    Addressables.LoadSceneAsync(sceneKey, LoadSceneMode.Additive).Completed += (loadHandle) =>
                    {
                        if (loadHandle.Status == AsyncOperationStatus.Succeeded)
                        {
                            Debug.Log($"🚀 Scene {sceneKey} đã được load thành công!");
                        }
                        else
                        {
                            Debug.LogError($"❌ Lỗi khi load scene {sceneKey}!");
                        }
                    };
                }
                else
                {
                    Debug.LogError($"❌ Lỗi khi tải scene {sceneKey}!");
                }
            };
        }

        private void OnAddressablesInitialized(AsyncOperationHandle<IResourceLocator> locator)
        {
            Debug.Log("Done init addressable");
            // ReadAllKey();
            if (locator.Status == AsyncOperationStatus.Succeeded)
            {
                // StartCoroutine(CheckForUpdateAndDownload());
            }
            else
            {
                Debug.LogWarning(locator.OperationException);
            }
        }

        private void ReadAllKey()
        {
            foreach (var locator in Addressables.ResourceLocators)
            {
                foreach (var key in locator.Keys)
                {
                    Debug.Log(key);
                }
            }
        }

        // private IEnumerator CheckForUpdateAndDownload()
        // {
        //     while (Caching.ready == false) yield return null;
        //     var check = Addressables.CheckForCatalogUpdates();
        //     check.Completed += CheckUpdateCompleted;
        // }

        private void CheckUpdateCompleted(AsyncOperationHandle<List<string>> arg)
        {
            if (arg.Result.Count > 0)
            {
                Addressables.UpdateCatalogs(true, arg.Result);
            }
            // if (obj.Status == AsyncOperationStatus.Succeeded)
            // {
            //     if (obj.Result.Count > 0)
            //     {
            //         Debug.Log("Update available.");
            //         Addressables.ClearDependencyCacheAsync(obj.Result);
            //         Addressables.DownloadDependenciesAsync(obj.Result, Addressables.MergeMode.Union).Completed += OnDownloadCompleted;
            //     }
            //     else
            //     {
            //         Debug.Log("No update available.");
            //     }
            // }
            // else
            // {
            //     Debug.LogError(obj.OperationException);
            // }
        }
    }
}
