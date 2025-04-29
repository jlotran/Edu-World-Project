using UnityEngine;

namespace EduWorld
{
    public static class UIHealper
    {
        public static void DestroyAllChildren(this RectTransform rectTransform)
        {
            foreach (Transform child in rectTransform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }
}
