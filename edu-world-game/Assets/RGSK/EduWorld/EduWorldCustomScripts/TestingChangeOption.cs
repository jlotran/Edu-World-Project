using RGSK;
using System.Collections;
using UnityEngine;
using DG.Tweening;
public class TestingChangeOption : MonoBehaviour
{
    [SerializeField] RaceInitializer raceInitializer;
    [SerializeField] GameObject carPrefab;

    [ContextMenu("Start Init")]
    public void StartInit()
    {
        if (raceInitializer != null) {
             raceInitializer.session.entrants.Clear();
            for (int i = 0; i < 4; i++) {
                RaceEntrant entrant = new RaceEntrant();
                entrant.prefab = carPrefab;
                entrant.isPlayer = Random.Range(0,2) == 1;
                raceInitializer.session.entrants.Add(entrant);
            }
        }

        StartCoroutine(InitStart());
    }

    public IEnumerator InitStart()
    {
        yield return new WaitForSeconds(1);

        raceInitializer.Initialize();
    }
}
