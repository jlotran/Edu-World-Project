using UnityEngine;

namespace RGSK
{
    public static class InitializationLoader
    {
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Execute()
        {
            foreach (var obj in RGSKCore.Instance.GeneralSettings.persistentObjects)
            {
                if (obj != null)
                {
                    CreatePersistentObject(obj);
                }
            }

            if (RGSKCore.Instance.GeneralSettings.terminal != null)
            {
                var useTerminal = Application.isEditor ||
                                (!Application.isEditor && RGSKCore.Instance.GeneralSettings.includeTerminalInBuild);

                if (useTerminal)
                {
                    CreatePersistentObject(RGSKCore.Instance.GeneralSettings.terminal);
                }
            }
        }

        public static void DestroyPersistentObjects()
        {
            GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.name.StartsWith("[RGSK Persistent]"))
                {
                    Object.Destroy(obj);
                }
            }
        }

        static void CreatePersistentObject(GameObject obj)
        {
            var o = Object.Instantiate(obj);
            o.name = o.name.Insert(0, "[RGSK Persistent] ");
            Object.DontDestroyOnLoad(o);
        }
    }
}