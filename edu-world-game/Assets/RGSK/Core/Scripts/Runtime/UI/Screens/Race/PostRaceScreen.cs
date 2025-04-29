using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using System.Linq;

namespace RGSK
{
    public class PostRaceScreen : RaceScreenBase
    {
        [SerializeField] PlayableDirector finishSequence;
        [SerializeField] UIScreenID rewardsScreenID;
        [SerializeField] Button continueButton;
        [SerializeField] RaceBoardLayout championshipBoardLayout;
        [SerializeField] bool autoStartReplay;

        [Header("Modal")]
        [SerializeField] bool showChampionshipModalWindow = true;
        [SerializeField]
        ModalWindowProperties championshipModal = new ModalWindowProperties
        {
            header = "Proceed",
            message = "Do you want to proceed to the next round?",
            confirmButtonText = "Yes",
            declineButtonText = "No",
            startSelection = 1
        };

        bool _skipFinishSequence;
        RaceBoard _championshipBoard;

        public override void Initialize()
        {
            base.Initialize();
            // continueButton?.onClick?.AddListener(Continue);
            continueButton?.onClick?.AddListener(Continuetest);
            _skipFinishSequence = false;

            if (ChampionshipManager.Instance.Initialized)
            {
                if (championshipBoardLayout != null)
                {
                    _championshipBoard = Instantiate(raceBoard, boardParent, false);
                    _championshipBoard.Initialize(championshipBoardLayout, BoardSortOrder.ChampionshipStandings);
                }
            }
        }

        public override void Open()
        {
            base.Open();

            if (finishSequence != null)
            {
                if (!_skipFinishSequence)
                {
                    _skipFinishSequence = true;
                    finishSequence.initialTime = 0;
                    finishSequence.Play();
                }
                else
                {
                    finishSequence.initialTime = finishSequence.duration;
                    finishSequence.Play();
                }
            }

            ResetChampionshipBoardCycle();
        }

        public override void Back()
        {
            base.Back();
            ResetChampionshipBoardCycle();
        }

        void Continue()
        {
            if (ChampionshipManager.Instance.Initialized)
            {
                RaceManager.Instance.ForceFinishRace(true);

                if (_raceBoard != null && _championshipBoard != null)
                {
                    if (!_championshipBoard.IsVisible)
                    {
                        _raceBoard.ToggleVisible(false);
                        _championshipBoard.ToggleVisible(true);
                        _championshipBoard.Refresh();
                        return;
                    }
                }

                if (showChampionshipModalWindow && !ChampionshipManager.Instance.IsFinalRound)
                {
                    ModalWindowManager.Instance.Show(new ModalWindowProperties
                    {
                        header = championshipModal.header,
                        message = championshipModal.message,
                        confirmButtonText = championshipModal.confirmButtonText,
                        declineButtonText = championshipModal.declineButtonText,
                        confirmAction = () => ChampionshipManager.Instance.LoadNextRound(),
                        declineAction = () => { },
                        startSelection = championshipModal.startSelection
                    });
                }
                else
                {
                    ChampionshipManager.Instance.LoadNextRound();
                }

                if (ChampionshipManager.Instance.IsFinished)
                {
                    OpenRewardsScreen();
                }
            }
            else
            {
                OpenRewardsScreen();
            }
        }

        public void Continuetest(){
            if (ChampionshipManager.Instance.Initialized)
            {
                RaceManager.Instance.ForceFinishRace(true);
                if (showChampionshipModalWindow && !ChampionshipManager.Instance.IsFinalRound)
                {
                    ModalWindowManager.Instance.Show(new ModalWindowProperties
                    {
                        header = championshipModal.header,
                        message = championshipModal.message,
                        confirmButtonText = championshipModal.confirmButtonText,
                        declineButtonText = championshipModal.declineButtonText,
                        confirmAction = () => ChampionshipManager.Instance.LoadNextRound(),
                        declineAction = () => { },
                        startSelection = championshipModal.startSelection
                    });
                }
                else
                {
                    ChampionshipManager.Instance.LoadNextRound();
                }

                if (ChampionshipManager.Instance.IsFinished)
                {
                    OpenRewardsScreenTest();
                }
            }
            else
            {
                OpenRewardsScreenTest();
            }
        }

        void OpenRewardsScreen()
        {
            if (RaceRewardManager.Instance.ActiveReward != null)
            {
                rewardsScreenID?.Open();
            }
            else
            {
                SceneLoadManager.LoadMainScene();
            }
        }

        void OpenRewardsScreenTest()
        {
            if (RaceRewardManager.Instance.ActiveReward != null)
            {
                rewardsScreenID?.Open();
            }
            else
            {
                SceneLoadManager.LoadIntermissionRaceScene();
            }
        }

        public void AutoStartReplay()
        {
            if (!autoStartReplay)
                return;

            if (RaceManager.Instance.Initialized && RaceManager.Instance.CurrentState == RaceState.PostRace)
            {
                RecorderManager.Instance?.ReplayRecorder?.StartPlayback();
            }
        }

        void ResetChampionshipBoardCycle()
        {
            if (_raceBoard != null && _championshipBoard != null)
            {
                _raceBoard.ToggleVisible(true);
                _championshipBoard.ToggleVisible(false);
            }
        }

        protected override void OnRaceRestart()
        {
            _skipFinishSequence = false;
        }

        protected override void OnCompetitorFinished(Competitor c) { }
        protected override void OnWrongwayStart(Competitor c) { }
        protected override void OnWrongwayStop(Competitor c) { }
    }
}