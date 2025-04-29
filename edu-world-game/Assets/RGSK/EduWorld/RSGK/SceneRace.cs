using RGSK.Extensions;
using RGSK.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
    public class SceneRace : MonoBehaviour
    {
        [SerializeField] UIScreenID screen;
        void Start()
        {
            if (TryGetComponent<Button>(out var btn))
            {
                btn.onClick.AddListener(() =>
                {
                    InitializationLoader.Execute();
                    UIManager.Instance?.OpenScreen(screen);
                });
            }
        }

    }
}