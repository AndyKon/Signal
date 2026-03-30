using UnityEngine;

namespace Signal.Core
{
    public static class Bootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            var prefab = Resources.Load<GameObject>("Managers");
            if (prefab != null && GameManager.Instance == null)
            {
                var instance = Object.Instantiate(prefab);
                Object.DontDestroyOnLoad(instance);
            }
        }
    }
}
