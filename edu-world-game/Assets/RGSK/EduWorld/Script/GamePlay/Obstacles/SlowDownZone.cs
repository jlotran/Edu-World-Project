using RGSK.Helpers;
using UnityEngine;

namespace RGSK
{
    public class SlowDownZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other is not BoxCollider)
                return;
            VehicleController vehicle = other.gameObject.GetComponentInParent<VehicleController>();
            Debug.Log("trigger");
            if (vehicle != null)
            {
                vehicle.effect.ApplySlowEffect(15f);
                try
                {
                    GeneralHelper.ToggleAIInput(vehicle.gameObject, true);
                    GeneralHelper.TogglePlayerInput(vehicle.gameObject, false);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error setting AI behaviour: {e.Message}");
                }
            }
        }

/*        private void OnTriggerExit(Collider other)
        {
            if (other is not BoxCollider)
                return;
            Debug.Log("exit");
            VehicleController vehicle = other.gameObject.GetComponentInParent<VehicleController>();
            if (vehicle != null)
            {
                vehicle.effect.CurrentEffect = EVehicleEffectType.SpeedUp;
            }
        }*/
    }
}
