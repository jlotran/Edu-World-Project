using RGSK.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
    public class Play : MonoBehaviour
    {
        void Start()
        {
            if (TryGetComponent<Button>(out var btn))
            {
                btn.onClick.AddListener(() =>
                {
                    StartCoroutine(GeneralHelper.OpenTrackSelectScreenWithCallback(() =>
                    {
                        // StartCoroutine(GeneralHelper.OpenVehicleSelectScreenWithCallback(() =>
                        // {
                        // }));
                    }));
                });
            }
        }

    }
}
