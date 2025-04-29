using UnityEngine;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Track/Requirement", fileName = "NewTrackRequirement")]
    public class TrackRequirementSO : ScriptableObject
    {
        [TextArea]
        public string description;
        public Sprite icon;

        public string GetDescription()
        {
            // This method can be used to get the description of the requirement
            return description;
        }

        public Sprite GetIcon()
        {
            // This method can be used to get the icon of the requirement
            return icon;
        }
    }

}