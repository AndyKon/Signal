using UnityEngine;

namespace Signal.Core
{
    public static class Bootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            Debug.Log("[Bootstrap] Init called");
            var prefab = Resources.Load<GameObject>("Managers");
            if (prefab == null)
            {
                Debug.LogError("[Bootstrap] Managers prefab not found in Resources!");
                return;
            }

            if (GameManager.Instance == null)
            {
                var instance = Object.Instantiate(prefab);
                Object.DontDestroyOnLoad(instance);
                Debug.Log("[Bootstrap] Managers instantiated");
            }
        }
    }
}
