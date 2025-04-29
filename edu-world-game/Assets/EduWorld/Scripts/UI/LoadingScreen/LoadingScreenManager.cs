using UnityEngine;
using DG.Tweening;
using System;
public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager instance;

    [Header("Prefabs Component")]
    [SerializeField] private GameObject progressingBarPrefab;
    [SerializeField] private GameObject progressingCirclePrefab;
    [SerializeField] private GameObject loadingPrefab;
    [SerializeField] private ProgressingHandler progressingHandler;
    [Header("UI Components")]
    [SerializeField] private RectTransform graphicHolder;
    [SerializeField] private LoadingScreenType currentType;
    public event Action EventEndProcess;
    private bool isFinish;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        switch (currentType)
        {
            case LoadingScreenType.Progressing_Bar:
                progressingBarPrefab.SetActive(true);
                break;
            case LoadingScreenType.Progressing_Circle:
                progressingCirclePrefab.SetActive(true);
                break;
            case LoadingScreenType.Loading:
                loadingPrefab.SetActive(true);
                break;
        }
    }

    // create function testing fake loading in editor
    [ContextMenu("Fake Loading")]
    public void FakeLoading()
    {
        FakeLoading(2f);
    }

    [ContextMenu("Active event end")]
    public void ActiveEventEnd()
    {
        EventEndProcess.Invoke();
    }

    public void FakeLoading(float fakeLoadTo90Duration)
    {
        EventEndProcess = new Action(() =>
        {
            OnEndProcess();
        });
        DOTween.To(() => 0, x => UpdateProcess(x), 0.9f, fakeLoadTo90Duration);
    }

    private void OnEndProcess()
    {
        DOTween.To(() => 0.9f, x => UpdateProcess(x), 1f, 0.5f);
    }

    public void UpdateProcess(float value) {

        if(currentType == LoadingScreenType.Loading)
        {
            return;
        }
        progressingHandler.UpdateProgress(value);
    }
    public void ShowLoading(LoadingScreenType loadingScreenType)
    {
        graphicHolder.gameObject.SetActive(true);
        currentType = loadingScreenType;
        switch (loadingScreenType)
        {
            case LoadingScreenType.Progressing_Bar:
                progressingBarPrefab.SetActive(true);
                break;
            case LoadingScreenType.Progressing_Circle:
                progressingCirclePrefab.SetActive(true);
                break;
            case LoadingScreenType.Loading:
                loadingPrefab.SetActive(true);
                break;
        }
    }

    public void HideLoading()
    {
        if (progressingHandler != null)
        {
            progressingHandler.UpdateProgress(0);
        }
        switch (currentType)
        {
            case LoadingScreenType.Progressing_Bar:
                progressingBarPrefab.SetActive(false);
                break;
            case LoadingScreenType.Progressing_Circle:
                progressingCirclePrefab.SetActive(false);
                break;
            case LoadingScreenType.Loading:
                loadingPrefab.SetActive(false);
                break;
        }
        graphicHolder.gameObject.SetActive(false);
    }
}

public enum LoadingScreenType
{
    Progressing_Bar,
    Progressing_Circle,
    Loading,
}
