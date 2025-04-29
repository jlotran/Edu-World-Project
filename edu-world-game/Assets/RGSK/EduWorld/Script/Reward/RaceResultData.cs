using UnityEngine;

namespace RGSK
{
    [System.Serializable]
    public class RaceResultData
    {
        public string playerName;
        public int finalPosition;

        public RaceResultData(string name, int position)
        {
            playerName = name;
            finalPosition = position;
        }
    }
}
