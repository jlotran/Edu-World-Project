using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace EduWorld
{
    public class NPCManager : MonoBehaviour
    {
        private static NPCManager instance;
        public static NPCManager Instance => instance;
        public SplineContainer splineContainer;
        private Dictionary<Spline, List<NpcController>> _dictNpcsBySpline;
        public List<NpcController> npcPrefab;

        [Header("Config NPCS")]
        [Space(10)]
        [SerializeField] int npcsBySpline;
        [SerializeField] float spawnInterval;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        void Start()
        {
            _dictNpcsBySpline = new Dictionary<Spline, List<NpcController>>();
            foreach (var item in splineContainer.Splines)
            {
                _dictNpcsBySpline.Add(item, new List<NpcController>());
            }
            StartCoroutine(SpawnNPCByInteval());
        }

        IEnumerator SpawnNPCByInteval()
        {
            for (int i = 0; i < npcsBySpline; i++)
            {
                StartSpawnNpcs();
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        void StartSpawnNpcs()
        {
            foreach (var item in _dictNpcsBySpline.Keys)
            {
                NpcController npc = SpawnNPC(item);
                _dictNpcsBySpline[item].Add(npc);
            }
        }

        private NpcController SpawnNPC(Spline spline)
        {
            if (spline.Count <= 0)
            {
                return null;
            }

            Vector3 spawnPosition = spline.EvaluatePosition(0);
            spawnPosition.y = 5;

            NpcController npc = Instantiate(npcPrefab[Random.Range(0, npcPrefab.Count)], spawnPosition, Quaternion.identity);
            npc.SetSpline(spline);
            return npc;
        }
    }
}
