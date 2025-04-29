using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
    public class RequirementUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI requirementText;
        // [SerializeField] private Image requirementIcon;
        // Add other UI elements as needed

        public void SetRequirement(TrackRequirementSO requirement)
        {
            if (requirementText != null)
            {
                requirementText.text = requirement.GetDescription();
            }

            // if (requirementIcon != null)
            // {
            //     requirementIcon.sprite = requirement.GetIcon();
            // }

            // Update other UI elements based on requirement data
        }
    }
}