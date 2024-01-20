using GameObjectComponent.App;
using UnityEngine;

namespace GameObjectComponent.UI
{
    public class UIButtonAudioInjector : MonoBehaviour
    {
        [SerializeField]private SoundManager soundManager;
        
        private void Awake()
        {
            foreach (var buttonAudio in GetComponentsInChildren<UIButtonAudio>())
            {
                buttonAudio.Construct(soundManager);
            }
        }
    }
}