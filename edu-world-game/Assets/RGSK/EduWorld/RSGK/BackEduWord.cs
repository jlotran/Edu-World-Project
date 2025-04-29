using RGSK.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
    public class BackEduWord : MonoBehaviour
    {
        [SerializeField] UIScreenID screen;

        

        void Start()
        {
            if (TryGetComponent<Button>(out var btn))
            {
                btn.onClick.AddListener(() =>
                {
                    UIManager.Instance?.CloseAllScreens();
                    UIManager.Instance?.ClearScreenHistory();
                    UIManager.Instance?.DestroyScreen(screen);
                    GeneralHelper.DestroyDynamicParent();
                    InitializationLoader.DestroyPersistentObjects();
                    IntermissionRace.Instance?.Open();
                });
            }
        }
    }
}
