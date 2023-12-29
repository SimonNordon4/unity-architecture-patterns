using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Classic.Items
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSystemColor : MonoBehaviour
    {
        private ParticleSystem[] _particleSystems;

        private void Awake()
        {
            // get particle system and all children particle systems
            _particleSystems = GetComponentsInChildren<ParticleSystem>();
        }
        
        public void SetColor(Color color)
        {
            foreach(var particle in _particleSystems)
            {
                var main = particle.main;
                main.startColor = color;
            }
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(ParticleSystemColor))]
    public class ParticleSystemColorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var particleSystemColor = (ParticleSystemColor) target;
            if (GUILayout.Button("Set Color"))
            {
                particleSystemColor.SetColor(color: Color.red);
            }
        }
    }
#endif
}