using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using RGSK.Extensions;
using RGSK.Helpers;
using Fusion;

namespace RGSK
{
    public class CompetitorFusion : Competitor
    {
        [Networked] public int position { get; set; }
        public int Position
        {
            get => position;
            set
            {
                position = value;
            }
        }

        [Networked] public int finalPosition { get; set; }

        [Networked] public int currentLap { get; set; }

        [Networked] public int totalLaps { get; set; }

        [Networked] public float totalRaceTime { get; set; }
        public float TotalRaceTime
        {
            get => totalRaceTime;
            set
            {
                totalRaceTime = value;
            }
        }
    }
}