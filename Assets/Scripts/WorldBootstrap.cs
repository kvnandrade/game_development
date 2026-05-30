using UnityEngine;

namespace ValeDosCristais
{
    public static class WorldBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateGame()
        {
            if (Object.FindObjectOfType<GameManager>() != null)
            {
                return;
            }

            GameObject root = new GameObject("Vale dos Cristais - Runtime");
            root.AddComponent<GameManager>();
            Object.DontDestroyOnLoad(root);
        }
    }
}
