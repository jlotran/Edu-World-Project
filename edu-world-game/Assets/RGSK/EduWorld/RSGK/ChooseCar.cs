using RGSK.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
    public class ChooseCar : MonoBehaviour
    {
        void Start()
        {
            if (TryGetComponent<Button>(out var btn))
            {
                btn.onClick.AddListener(() =>
                {
                    StartCoroutine(GeneralHelper.OpenVehicleSelectScreenWithCallback(() =>
                    {
                    }));
                });
            }
        }
    }
}
