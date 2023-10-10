using System.Collections.Generic;
using Classic.Interfaces;
using UnityEngine;

namespace Classic.Enemies.Enemy
{
    public class EnemyState : MonoBehaviour
    {
        [SerializeField] private EnemyScope scope;
        private IResettable[] _resettable;
        [SerializeField] private List<GameObject> playObjects = new();

        private void OnEnable()
        {
            scope.gameState.onStateChanged.AddListener(OnStateChanged);
        }

        private void OnStateChanged()
        {
            if (scope.gameState.isGameActive)
            {
                EnableEnemy();
            }
            else
            {
                DisableEnemy();
            }
        }

        public void EnableEnemy()
        {
            _resettable = GetComponentsInChildren<IResettable>();
            foreach (var playObject in playObjects)
            {
                playObject.SetActive(true);
            }
            
            foreach (var resettable in _resettable)
            {
                resettable.Reset();
            }
        }
        
        public void DisableEnemy()
        {
            foreach (var playObject in playObjects)
            {
                playObject.SetActive(false);
            }
        }
    }
}