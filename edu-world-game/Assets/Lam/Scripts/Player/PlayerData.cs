using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace Lam.FUSION
{
    public class PlayerData
    {
        public string gender;
        public List<string> cats;
        public List<string> paths;
        public Dictionary<String, Color> colors = new Dictionary<String, Color>();

        public PlayerData(string gender, List<string> cats, List<string> paths)
        {
            this.gender = gender;
            this.cats = cats;
            this.paths = paths;
        }
    }
}
