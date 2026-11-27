using UnityEngine;

namespace DressMagic.Core
{
    /// <summary>Arranca el juego en cualquier escena: crea el núcleo y la cámara si falta.</summary>
    public static class Bootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Launch()
        {
            if (GameRoot.Instance != null) return;

            EnsureCamera();

            GameObject go = new GameObject("DressMagic");
            Object.DontDestroyOnLoad(go);
            go.AddComponent<GameRoot>();
        }

        static void EnsureCamera()
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                GameObject camGo = new GameObject("Main Camera");
                camGo.tag = "MainCamera";
                cam = camGo.AddComponent<Camera>();
                Object.DontDestroyOnLoad(camGo);
            }
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Pal.Hex("E7D5FA");
            cam.orthographic = true;
            cam.orthographicSize = 5f;
        }
    }
}
