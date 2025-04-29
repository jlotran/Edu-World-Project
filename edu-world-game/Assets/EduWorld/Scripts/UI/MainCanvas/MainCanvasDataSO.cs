using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MainCanvasDataSO", menuName = "EduWorld/UI/MainCanvasDataSO")]
public class MainCanvasDataSO : ScriptableObject
{
    public List<GameObject> defaultUI;
    public List<UIDataByScene> uiDataByScenes;
}

[Serializable]
public struct UIDataByScene
{
    public string sceneName;
    public GameObject[] uiPrefabs;
}

