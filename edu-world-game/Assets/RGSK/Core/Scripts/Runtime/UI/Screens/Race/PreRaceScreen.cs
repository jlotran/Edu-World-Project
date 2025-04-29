using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
    using RGSK.Fusion;
    public class PreRaceScreen : RaceScreenBase
    {
        // [SerializeField] Button startRaceButton;

        [Tooltip("The board layout that will be used during championships.")]
        [SerializeField] RaceBoardLayout championshipBoardLayout;

        public override void Initialize()
        {
            base.Initialize();
            // startRaceButton?.onClick?.AddListener(OnStartRace);
            StartCoroutine(OnStartRace());
            if (ChampionshipManager.Instance.Initialized && championshipBoardLayout != null)
            {
                raceBoardLayout = championshipBoardLayout;
            }
        }

        IEnumerator OnStartRace()
        {
            if (RaceManager.Instance?.Session?.isFusionRacing ?? false)
            {
                Rider.LocalRider.StartRace();
            }
            else
            {
                yield return new WaitForSeconds(2f);
                RaceManager.Instance?.StartRace();
            }
        }

        protected override void OnCompetitorFinished(Competitor c) { }
        protected override void OnRaceRestart() { }
        protected override void OnWrongwayStart(Competitor c) { }
        protected override void OnWrongwayStop(Competitor c) { }
    }
}