using System.Collections.Generic;
using UnityEngine;

namespace Edu_World
{
    [System.Serializable]
    public class MajorData
    {
        public string major;
        public List<string> categories;
    }

    [System.Serializable]
    public class MajorList
    {
        public List<MajorData> majors;
    }
}
