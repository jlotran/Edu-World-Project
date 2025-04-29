using UnityEngine;
using System.Collections.Generic;
using Edu_World;

public static class MajorLoader
{
    public static List<MajorData> LoadMajors()
    {
        TextAsset json = Resources.Load<TextAsset>("majors");
        if (json == null)
        {
            Debug.LogError("majors.json not found in Resources!");
            return new List<MajorData>();
        }

        string wrappedJson = "{\"majors\":" + json.text + "}";
        return JsonUtility.FromJson<MajorList>(wrappedJson).majors;
    }
}
