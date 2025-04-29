using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class RewardManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> riderPrefabs;
        [SerializeField] private List<Transform> spawnPoints;
        RewardTimeLine _timeline;

        void OnEnable()
        {
            RGSKEvents.OnTimelineRewardEnable.AddListener(enableRewardScreen);
            RGSKEvents.OnTimelineRewardDisable.AddListener(disableRewardScreen);
        }

        void OnDisable()
        {
            RGSKEvents.OnTimelineRewardEnable.RemoveListener(enableRewardScreen);
            RGSKEvents.OnTimelineRewardDisable.AddListener(disableRewardScreen);
        }

        void Start()
        {
            _timeline = GetComponentInChildren<RewardTimeLine>();
            disableRewardScreen();
            SpawnRiders();
        }

        public void RaceResult()
        {
            foreach (var result in GetReward.ResultList)
            {
                Debug.Log($"[RewardScene] FinalPosition: {result.finalPosition}, Player: {result.playerName}");
            }
        }

        private void SpawnRiders()
        {
            List<GameObject> riders = riderPrefabs;
            for (int i = 0; i < 3; i++)
            {
                GameObject prefab = riders[UnityEngine.Random.Range(0, riders.Count)];
                GameObject rider = Instantiate(prefab, spawnPoints[i]);
                Animator animator = rider.GetComponent<Animator>();
                animator.SetFloat("Blend", i);
                riders.Remove(prefab);
            }
        }

        public void enableRewardScreen()
        {
            RaceResult();
            _timeline?.gameObject.SetActive(true);
            _timeline?.ToggleTimeline(true);
        }

        public void disableRewardScreen()
        {
            _timeline?.ToggleTimeline(false);
            _timeline?.gameObject.SetActive(false);
        }
    }
}
