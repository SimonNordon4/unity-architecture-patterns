using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameObjectComponent.UI
{
    public class UITextPool : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI damageNumberPrefab;
        [SerializeField] private int poolSize = 10;
        private readonly Queue<TextMeshProUGUI> _damageNumberPool = new();
        
        private void InitializePool()
        {
            for (var i = 0; i < poolSize; i++)
            {
                var damageNumber = Instantiate(damageNumberPrefab, transform);
                damageNumber.gameObject.SetActive(false);
                _damageNumberPool.Enqueue(damageNumber);
            }
        }
        
        private void Awake()
        {
            InitializePool();
        }
        
        public TextMeshProUGUI GetText()
        {
            if (_damageNumberPool.Count == 0)
            {
                var newDamageNumber = Instantiate(damageNumberPrefab, transform);
                newDamageNumber.gameObject.SetActive(true);
                return newDamageNumber;
            }
            
            var damageNumber = _damageNumberPool.Dequeue();
            damageNumber.gameObject.SetActive(true);
            return damageNumber;
        }
        
        public void ReturnDamageNumber(TextMeshProUGUI damageNumber)
        {
            damageNumber.gameObject.SetActive(false);
            damageNumber.transform.localScale = Vector3.one;
            _damageNumberPool.Enqueue(damageNumber);
        }
    }
}