using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using RGSK.Extensions;
using RGSK.Helpers;
using System;
using Fusion;
using EduWorld;
using UnityEngine.SceneManagement;

namespace RGSK
{
    public class RaceManager : Singleton<RaceManager>
    {
        public RaceSession Session => _session;
        public Track Track => _track;
        public RaceState CurrentState { get; private set; }
        public EntityRuntimeSet Entities => RGSKCore.Instance.GeneralSettings.entitySet;
        public bool Initialized => _initialized;
        public bool InfiniteLaps => _infiniteLaps;

        RaceSession _session;
        Track _track;
        public RaceSpawner spawner;
        RacePositioningSystem _positioningSystem;
        Timer _raceTimer;

        TimerFusion _raceTimerFusion => _raceTimer as TimerFusion;

        Timer _sessionTimer;
        DnfTimer _dnfTimer;
        Dictionary<int, Timer> _sessionTimers = new Dictionary<int, Timer>();
        bool _initialized;
        bool _infiniteLaps;
        bool _forcingRaceFinish;

        //Dang
        public List<RGSKEntity> GetCompetitorFinalList;
        private GameObject speedline;
        private GameObject driftUI;

        //end Dang

        void OnEnable()
        {
            RGSKEvents.OnLapCompleted.AddListener(OnLapCompleted);
        }

        void OnDisable()
        {
            RGSKEvents.OnLapCompleted.RemoveListener(OnLapCompleted);
        }

        public void InitializeRace(RaceSession session, Track track)
        {
            if (Initialized)
                return;

            if (session == null || track == null)
            {
                Logger.LogWarning(this, "A race session and a track are required to initialize the race!");
                return;
            }

            _session = ScriptableObject.Instantiate(session);
            _track = track;

            Session.Setup();
            track.Setup();

            if (track.IsPointToPoint())
            {
                Session.lapCount = 1;
            }

            _positioningSystem = gameObject.GetOrAddComponent<RacePositioningSystem>();
            _positioningSystem.Setup(Session.GetPositioningMode(), Session.raceType.positionSortMode);

            if (!Session.isFusionRacing)
            {
                _raceTimer = GeneralHelper.CreateTimer(0, false, false, null, null, null, null, "Timer_Race");
            }

            _dnfTimer = gameObject.GetOrAddComponent<DnfTimer>();

            if (Session.UseGlobalTimer())
            {
                _sessionTimer = GeneralHelper.CreateTimer(Session.sessionTimeLimit, true,
                    Session.raceType.globalTimerElapsedAction == GlobalTimerElapsedAction.DisqualifyLastPlace,
                    () => OnSessionTimerElapsed(),
                    null,
                    null,
                    null,
                    "Timer_Session");
            }

            _infiniteLaps = Session.IsInfiniteLaps();
            spawner = gameObject.GetOrAddComponent<RaceSpawner>();
            _initialized = true;

            RGSKEvents.OnRaceInitialized.Invoke();
            SetState(RaceState.PreRace);

            speedline = GeneralHelper.GetDynamicParent().transform.Find("SpeedLines")?.gameObject;
            driftUI = GeneralHelper.GetDynamicParent().transform.Find("DriftUI")?.gameObject;
        }

        public void SpawnEntrant(GameObject instance, RaceEntrant entrant, bool isPlayer, int indexProfile)
        {
            spawner.SpawnEntrant(instance, entrant, isPlayer, indexProfile);
            _raceTimer = FindFirstObjectByType<TimerFusion>();
            _raceTimer.Initialize(0, false, false);
        }

        public void DeInitializeRace()
        {
            if (!Initialized)
                return;

            SaveSessionRecord();

            _session = null;
            _track = null;
            _sessionTimers.Clear();

            Destroy(spawner);
            Destroy(_positioningSystem);

            if (_raceTimer != null)
            {
                Destroy(_raceTimer.gameObject);
            }

            if (_dnfTimer != null)
            {
                Destroy(_dnfTimer);
            }

            if (_sessionTimer != null)
            {
                Destroy(_sessionTimer.gameObject);
            }

            foreach (var e in Entities.Items)
            {
                Destroy(e.gameObject);
            }

            _initialized = false;
            SaveManager.Instance?.Save();
            RGSKEvents.OnRaceDeInitialized.Invoke();
        }

        IEnumerator DelayedStartCountdown(float delay)
        {
            driftUI?.SetActive(true);
            yield return new WaitForSeconds(delay);
            CameraManager.Instance?.ToggleRouteCameras(false, CameraRouteType.Start);
            RGSKEvents.OnTimelineToggle?.Invoke(true);
            StartCountdown();
            yield return new WaitForSeconds(2.8f);
            speedline?.SetActive(true);
        }

        public void StartRace(float delaytime = 0.2f)
        {
            if (CurrentState == RaceState.PreRace)
            {
                UIScreenFader.Instance?.FadeOut(0.5f);


                switch (Session.startMode)
                {
                    case RaceStartMode.StandingStart:
                        {
                            foreach (var c in Entities.Items)
                            {
                                if (c)
                                {
                                    GeneralHelper.ToggleInputControl(c.gameObject, false);
                                }
                            }
                            UIManager.Instance?.CloseAllScreens();
                            CameraManager.Instance?.ToggleRouteCameras(true, CameraRouteType.Start);
                            StartCoroutine(DelayedStartCountdown(delaytime));
                            break;
                        }

                    case RaceStartMode.RollingStart:
                        {
                            foreach (var c in Entities.Items)
                            {
                                GeneralHelper.ToggleAIInput(c.gameObject, true);
                                GeneralHelper.TogglePlayerInput(c.gameObject, false);
                                GeneralHelper.ToggleInputControl(c.gameObject, true);
                            }

                            var behaviour = RGSKCore.Instance.AISettings.raceStateBehaviours.FirstOrDefault(x => x.state == RaceState.RollingStart);

                            if (behaviour != null)
                            {
                                behaviour.behaviour.speedOverride = ActiveCompetitorCount() > 1 ?
                                        RGSKCore.Instance.RaceSettings.rollingStartSpeedLimitMultiple :
                                        RGSKCore.Instance.RaceSettings.rollingStartSpeedLimitSolo;
                            }

                            SetState(RaceState.RollingStart);
                            break;
                        }
                }
            }
            else
            {
                Entities.Items.ForEach(x => StartRace(x));
            }
        }

        public void StartRace(RGSKEntity entity)
        {
            GeneralHelper.ToggleAIInput(entity.gameObject, !entity.IsPlayer);
            GeneralHelper.TogglePlayerInput(entity.gameObject, entity.IsPlayer);
            GeneralHelper.ToggleInputControl(entity.gameObject, true);
            GeneralHelper.SetAIBehaviour(entity.gameObject, RaceManager.Instance.Session.GetAiDifficulty());
            GeneralHelper.ToggleVehicleCollision(entity.gameObject, !Session.disableCollision);
            GeneralHelper.ToggleGhostedMesh(entity.gameObject, !entity.IsPlayer && Session.disableCollision);

            if (Session.UseSeparateTimers())
            {
                if (!_sessionTimers.ContainsKey(entity.ID))
                {
                    _sessionTimers.Add(entity.ID, GeneralHelper.CreateTimer(
                                Session.sessionTimeLimit,
                                true, false,
                                () => Disqualify(entity.Competitor),
                                null,
                                null,
                                null,
                                $"Timer_{entity.name}"));
                }
            }

            _sessionTimers.FirstOrDefault(x => x.Key == entity.ID).Value?.StartTimer();
            _raceTimer?.StartTimer();
            _sessionTimer?.StartTimer();

            entity.Competitor?.UpdateLapStartTime();
            entity.Competitor?.SetState(RaceState.Racing);

            if (AllCompetitorsStarted())
            {
                SetState(RaceState.Racing);
            }

            RGSKEvents.OnCompetitorStarted.Invoke(entity.Competitor);
        }

        public void StartCountdown()
        {
            if (!Initialized)
                return;

            SetState(RaceState.Countdown);
        }

        public void RestartRace()
        {
            if (!Initialized)
                return;

            _raceTimer?.ResetTimer();
            _sessionTimers.Values.ToList().ForEach(x => x?.ResetTimer());
            _sessionTimer?.ResetTimer();

            foreach (var entity in Entities.Items)
            {
                spawner?.PlaceOnGrid(entity.Competitor);

                if (!entity.IsVirtual)
                {
                    entity.Competitor?.ResetValues();
                    entity.DriftController?.ResetValues();
                }

                entity.Competitor?.SetState(RaceState.PreRace);
                entity.Competitor?.UpdateLapStartTime();

                GeneralHelper.ToggleAIInput(entity.gameObject, !entity.IsPlayer);
                GeneralHelper.TogglePlayerInput(entity.gameObject, entity.IsPlayer);
                GeneralHelper.ToggleInputControl(entity.gameObject, false);
                GeneralHelper.ToggleGhostedMesh(entity.gameObject, !entity.IsPlayer && Session.disableCollision);
            }

            SetState(RaceState.PreRace);
            RGSKEvents.OnRaceRestart.Invoke();
        }

        public void FinishRace(Competitor c)
        {
            if (c.IsFinished())
                return;

            c.SetState(RaceState.PostRace);
            _sessionTimers.FirstOrDefault(x => x.Key == c.Entity.ID).Value?.StopTimer();
            _positioningSystem.UpdatePositions();

            if (c.Entity.IsPlayer)
            {
                AudioManager.Instance?.Play(!c.IsDisqualified ?
                                        RGSKCore.Instance.RaceSettings.raceFinishSound :
                                        RGSKCore.Instance.RaceSettings.raceDisqualifySound,
                                        AudioGroup.Music);
                speedline?.SetActive(false);
                driftUI?.SetActive(false);
                SetState(RaceState.PostRace);
            }

            GeneralHelper.ToggleVehicleCollision(c.gameObject, false);
            GeneralHelper.ToggleGhostedMesh(c.gameObject, !c.Entity.IsPlayer && RGSKCore.Instance.RaceSettings.ghostDisqualifiedCompetitors);

            GeneralHelper.ToggleAIInput(c.gameObject, true);
            GeneralHelper.TogglePlayerInput(c.gameObject, false);
            RGSKEvents.OnCompetitorFinished.Invoke(c);

            if (AllCompetitorsFinished())
            {
                _sessionTimer?.StopTimer();
                _raceTimer?.StopTimer();
                _positioningSystem.enabled = false;

                if (CurrentState != RaceState.PostRace)
                {
                    SetState(RaceState.PostRace);
                }

                speedline?.SetActive(false);
                driftUI?.SetActive(false);

                //Get RaceResultData
                RuntimeSet<RGSKEntity> competitors = RGSKCore.Instance.GeneralSettings.entitySet;
                var sortedList = GetSortedCompetitors(competitors.Items);
                GetReward.ExtractFromEntities(sortedList);
                StartCoroutine(GoToRewardScreen(2f));
            }
        }

        IEnumerator GoToRewardScreen(float delay)
        {
            // Debug.Log("loading");
            yield return new WaitForSeconds(delay);
            RGSKEvents.OnTimelineRewardEnable?.Invoke();
        }

        public List<RGSKEntity> GetSortedCompetitors(List<RGSKEntity> competitorsList)
        {
            return competitorsList
                .Where(x => x != null && x.GetComponent<Competitor>() != null)
                .OrderBy(x => x.GetComponent<Competitor>().FinalPosition)
                .ToList();
        }

        public void Disqualify(Competitor c)
        {
            if (c.IsFinished())
                return;

            c.IsDisqualified = true;
            GeneralHelper.ToggleVehicleCollision(c.gameObject, false);
            GeneralHelper.ToggleGhostedMesh(c.gameObject, !c.Entity.IsPlayer && RGSKCore.Instance.RaceSettings.ghostDisqualifiedCompetitors);

            FinishRace(c);

            if (ActiveCompetitorCount() == 1 || c.Entity.IsPlayer)
            {
                ForceFinishRace(false);
            }
        }

        public void ForceFinishRace(bool forceDisqualification)
        {
            if (!Initialized || AllCompetitorsFinished() || _forcingRaceFinish)
                return;

            _forcingRaceFinish = true;
            speedline?.SetActive(false);
            driftUI?.SetActive(false);

            var sortedList = _positioningSystem.CombinedList.ToList();

            if (forceDisqualification)
            {
                sortedList.Reverse();
            }

            sortedList.ForEach(x =>
            {
                if (forceDisqualification)
                {
                    Disqualify(x.Competitor);
                }
                else
                {
                    FinishRace(x.Competitor);
                }
            });

            _forcingRaceFinish = false;
        }

        public void ExtendCompetitorTimer(Competitor c, float value)
        {
            if (_sessionTimers.TryGetValue(c.Entity.ID, out var timer))
            {
                timer.AddTimerValue(value);
                RGSKEvents.OnTimerExtended.Invoke(c, value);
            }
        }

        public void AddCompetitorScore(Competitor c, float value)
        {
            c.Score += value;
            RGSKEvents.OnScoreAdded.Invoke(c, value);
        }

        void SetState(RaceState state)
        {
            CurrentState = state;

            switch (state)
            {
                case RaceState.PreRace:
                    {
                        InputManager.Instance?.SetInputMode(InputMode.Disabled);
                        _positioningSystem.FinishedList.Clear();
                        _positioningSystem.DisqualifiedList.Clear();
                        _positioningSystem.enabled = true;
                        _forcingRaceFinish = false;

                        if (RGSKCore.Instance.RaceSettings.skipPreRaceState)
                        {
                            StartRace();
                        }
                        break;
                    }

                case RaceState.Countdown:
                    {
                        InputManager.Instance?.SetInputMode(InputMode.Gameplay);
                        Entities.Items.ForEach(x => x?.Competitor?.SetState(Session.startMode == RaceStartMode.StandingStart ? RaceState.Countdown : RaceState.RollingStart));
                        break;
                    }

                case RaceState.RollingStart:
                    {
                        InputManager.Instance?.SetInputMode(InputMode.Gameplay);
                        Entities.Items.ForEach(x => x?.Competitor?.SetState(RaceState.RollingStart));
                        break;
                    }

                case RaceState.PostRace:
                    {
                        InputManager.Instance?.SetInputMode(InputMode.Disabled);
                        break;
                    }
            }

            //enable the cinematic camera based on the race state
            CameraManager.Instance?.ToggleRouteCameras(EnumFlags.GetSelectedIndexes
                                (RGSKCore.Instance.RaceSettings.cinematicCameraStates).
                                Contains((int)state), CameraRouteType.End);

            RGSKEvents.OnRaceStateChanged.Invoke(CurrentState);
        }

        void DisqualifyLastPlace()
        {
            var last = GetCompetitorInLastPlace();
            if (last != null)
            {
                Disqualify(last.Entity.Competitor);
            }
        }

        void OnSessionTimerElapsed()
        {
            switch (Session.raceType.globalTimerElapsedAction)
            {
                case GlobalTimerElapsedAction.Finish:
                    {
                        ForceFinishRace(false);
                        break;
                    }

                case GlobalTimerElapsedAction.DisqualifyLastPlace:
                    {
                        DisqualifyLastPlace();
                        break;
                    }

                case GlobalTimerElapsedAction.FinalLap:
                    {
                        var finalLap = GetCompetitorInPosition(1).CurrentLap;
                        Session.lapCount = finalLap;
                        _infiniteLaps = false;
                        foreach (var entity in Entities.Items)
                        {
                            if (entity.Competitor != null)
                            {
                                entity.Competitor.TotalLaps = finalLap;
                                RGSKEvents.OnLapCompleted.Invoke(entity.Competitor);
                            }
                        }
                        break;
                    }
            }
        }

        void OnLapCompleted(Competitor c)
        {
            if (Session.raceType.disqualifyLastPlaceEachLap)
            {
                if (c.Position == ActiveCompetitorCount() - 1)
                {
                    DisqualifyLastPlace();
                }
            }

            if (!_infiniteLaps && c.CurrentLap > c.TotalLaps)
            {
                FinishRace(c.Entity.Competitor);
            }

            if (c.Entity.IsPlayer && Track.definition != null && c.CurrentLap > 1)
            {
                var record = Track.GetBestLap();
                var best = c.GetBestLapTime();

                if (record == 0 || record > best)
                {
                    Track.definition.SaveBestLap(best);
                    RGSKEvents.OnSetNewBestLapTime.Invoke(c);
                }
            }
        }

        void SaveSessionRecord()
        {
            var player = Entities.Items.FirstOrDefault(x => x.IsPlayer);

            if (player != null && player.Competitor != null)
            {
                if (!player.Competitor.IsFinished() || player.Competitor.IsDisqualified)
                    return;

                var pos = player.Competitor.FinalPosition;

                if (Session.IsTargetScoreSession() && pos > Session.targetScores.Count)
                    return;

                if (Session.saveRecords)
                {
                    var record = Session.LoadBestPosition();

                    if (record <= 0 || record > pos)
                    {
                        Session.SaveBestPosition(pos);
                    }
                }

                if (pos == 1)
                {
                    SaveData.Instance.playerData.totalWins += 1;
                }

                SaveData.Instance.playerData.totalRaces += 1;
            }
        }

        public float GetRaceTime()
        {
            if (_raceTimer == null)
                return 0;

            if (Session.isFusionRacing)
                return _raceTimerFusion.value;

            return _raceTimer.Value;
        }

        public float GetSessionTime()
        {
            if (!Initialized || Session.raceType.raceDurationMode == RaceDurationMode.LapBased)
                return 0;

            switch (Session.raceType.timerMode)
            {
                case RaceTimerMode.Global:
                    {
                        return _sessionTimer.Value;
                    }

                case RaceTimerMode.PerCompetitor:
                    {
                        var c = GeneralHelper.GetFocusedEntity();

                        if (c != null)
                        {
                            if (_sessionTimers.TryGetValue(c.ID, out var timer))
                            {
                                return timer.Value;
                            }
                        }

                        return 0;
                    }
            }

            return 0;
        }

        public float GetDnfTime() => _dnfTimer?.GetRemainingTime() ?? 0;

        public Competitor GetCompetitorInPosition(int pos)
        {
            if (pos < 1 || pos > _positioningSystem.CombinedList.Count)
                return null;

            return _positioningSystem.CombinedList[pos - 1].Competitor;
        }

        public Competitor GetCompetitorInLastPlace()
            => GetCompetitorInPosition(ActiveCompetitorCount());

        public Competitor GetCompetitorWithBestLap()
            => _positioningSystem.CombinedList.OrderBy(x => x.Competitor.GetBestLapTime()).FirstOrDefault()?.Competitor;

        public bool AllCompetitorsStarted()
            => _positioningSystem.CombinedList.Where(x => !x.IsVirtual && x.Competitor != null).All(x => x.Competitor.IsRacing());

        public bool AllCompetitorsFinished()
            => _positioningSystem.CombinedList.Where(x => !x.IsVirtual && x.Competitor != null).All(x => x.Competitor.IsFinished() || x.Competitor.IsDisqualified);

        public int ActiveCompetitorCount()
            => _positioningSystem.CombinedList.Where(x => !x.IsVirtual && x.Competitor != null).Count(x => !x.Competitor.IsDisqualified);

        public int FinishedCompetitorCount()
            => _positioningSystem.CombinedList.Where(x => !x.IsVirtual && x.Competitor != null).Count(x => x.Competitor.IsFinished() && !x.Competitor.IsDisqualified);

        public int DisqualifiedCompetitorCount()
            => _positioningSystem.CombinedList.Where(x => !x.IsVirtual && x.Competitor != null).Count(x => x.Competitor.IsDisqualified);
    }
}