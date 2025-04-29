using System.Collections.Generic;
using RGSK.Helpers;
using UnityEngine;

namespace RGSK
{
    public class GetReward : MonoBehaviour
    {
        public static List<RaceResultData> ResultList = new();

        public static void ExtractFromEntities(List<RGSKEntity> competitorsList)
        {
            ResultList.Clear();

            foreach (var entity in competitorsList)
            {
                if (entity == null) continue;

                var competitor = entity.GetComponent<Competitor>();
                var profile = entity.GetComponent<ProfileDefiner>();

                if (competitor != null && profile != null)
                {
                    var name = UIHelper.FormatNameText(profile.definition);
                    ResultList.Add(new RaceResultData(name, competitor.FinalPosition));
                }
            }
        }
    }
}
