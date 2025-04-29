using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class RouteCameraGroup : MonoBehaviour
    {
        public List<RouteCamera> cameras = new List<RouteCamera>();

        public void Refresh()
        {
            cameras.Clear();
            GetComponentsInChildren(cameras);
        }
    }
}
