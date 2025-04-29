using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Track/Reward", fileName = "Reward")]
    public class TrackReward : ScriptableObject
    {
        public int rewardAmount;
        public Sprite rewardIcon;

        public int GetRewardAmount()
        {
            // This method can be used to get the reward amount
            return rewardAmount;
        }

        public Sprite GetRewardIcon()
        {
            // This method can be used to get the reward icon
            return rewardIcon;
        }
    }
}
