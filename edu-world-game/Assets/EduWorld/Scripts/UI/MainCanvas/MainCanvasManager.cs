using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainCanvasManager : MonoBehaviour
{
    private static MainCanvasManager instance;
    public static MainCanvasManager Instance => instance;

    [Header("Scriptable Object")]
    [SerializeField] private MainCanvasDataSO mainCanvasDataSO;

    [Space(10)]
    [Header("Rects")]
    [SerializeField] private RectTransform panelsByScene;
    [SerializeField] private RectTransform panelsDefault;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        foreach (var uiData in mainCanvasDataSO.defaultUI)
        {
            Instantiate(uiData, panelsDefault);
        }
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        foreach (var uiData in mainCanvasDataSO.uiDataByScenes)
        {
            if (uiData.sceneName == currentSceneName)
            {
                foreach (var prefab in uiData.uiPrefabs)
                {
                    Instantiate(prefab, panelsByScene);
                }
            }
        }
    }

    [ContextMenu("Change Scene Login")]
    public void ChangeSceneLogin()
    {
        SceneManager.LoadScene("Login");
    }


    [ContextMenu("Change Scene Main")]
    public void ChangeSceneCharcterCustomization()
    {
        SceneManager.LoadScene("CharacterCustomization");
    }
}
