using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
        private Transform _transform;

        public Camera camera;
        private Vector3 _cameraOffset;
        
        [Header("Movement")]
        public float moveSpeed = 5f;

        [Header("Health")]
        public int currentHealth = 10;
        public int maxHealth = 10;
        public bool isDead = false;
        
        [Header("Weapon")]
        public float fireRate = 0.5f;
        public float range = 5;
        public int damage = 1;
        
        [Header("UI")]
        public TextMeshProUGUI healthText;
        public RectTransform healthBar;
        
        private void Awake()
        {
            _transform = transform;
            _cameraOffset = camera.transform.position - _transform.position;
        }

        private void Start()
        {
            SetUI();
        }

        private void Update()
        {
            if (isDead) return;
            
            var dir = Vector3.zero;
                
            if(Input.GetKey(KeyCode.A))
                dir += Vector3.left;
            if(Input.GetKey(KeyCode.D))
                dir += Vector3.right;
            if (Input.GetKey(KeyCode.W))
                dir += Vector3.forward;
            if(Input.GetKey(KeyCode.S))
                dir += Vector3.back;
            if (dir.magnitude > 0)
            {
                _transform.position += dir.normalized * (Time.deltaTime * moveSpeed);
                _transform.rotation = Quaternion.LookRotation(dir);
            }
            
            camera.transform.position = _transform.position + _cameraOffset;
        }

        public void TakeDamage(int damageAmount)
        {
            currentHealth -= damageAmount;

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                isDead = true;
            }
            SetUI();
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Health Pickup"))
            {
                // Get health pickup component.
            }

            if (other.CompareTag("FireRate Pickup"))
            {
                
            }

            if (other.CompareTag("Damage Pickup"))
            {
                
            }
            
            if(other.CompareTag("Range Pickup"))
            {
                
            }
            
            if(other.CompareTag("Spawn Indicator"))
            {
                Destroy(other.gameObject);   
            }
        }
        
        private void SetUI()
        {
            healthText.text = $"{currentHealth}/{maxHealth}";
            healthBar.localScale = new Vector3((float)currentHealth / maxHealth, 1, 1);
        }
}
