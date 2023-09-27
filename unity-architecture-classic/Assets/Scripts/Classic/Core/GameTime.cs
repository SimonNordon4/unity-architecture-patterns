using UnityEngine;

namespace Classic.Core
{
    public static class GameTime
    {
        public static float timeScale { get; set; } = 1f;
        public static float deltaTime => Time.deltaTime * timeScale;
    }
}