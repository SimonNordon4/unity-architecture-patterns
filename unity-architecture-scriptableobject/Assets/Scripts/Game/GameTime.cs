using UnityEngine;

namespace GameObjectComponent.Game
{
    public static class GameTime
    {
        public static float hyperModeTimeScale { get; set; } = 1f;
        public static float timeScale { get; set; } = 1f;
        public static float deltaTime => Time.deltaTime * timeScale * hyperModeTimeScale;
        public static bool isPaused => timeScale == 0f;
    }
}