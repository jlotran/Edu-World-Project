using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
    public class RewardUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI rewardText;
        [SerializeField] private Image rewardIcon;
        // Add other UI elements as needed

        public void SetReward(TrackReward reward)
        {
            if (rewardText != null)
            {
                rewardText.text = "x" + reward.GetRewardAmount().ToString();
            }

            if (rewardIcon != null)
            {
                rewardIcon.sprite = reward.GetRewardIcon();
            }

            // Update other UI elements based on requirement data
        }
    }
}
