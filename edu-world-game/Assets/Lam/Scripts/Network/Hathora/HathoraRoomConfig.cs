using Lam.GAMEPLAY;
using UnityEngine;

namespace Lam.FUSION
{
    public enum GameRoomType // number = index of scene id scene list
    {
        city = 1,
        racing = 2,
        officeLobby = 3,
        office = 4,
    }

    [System.Serializable]
    public class HathoraRoomConfig
    {
        public GameRoomType roomType;
        public LevelType level; // only use for office lobby
        public string name; // name of room
    }
}
