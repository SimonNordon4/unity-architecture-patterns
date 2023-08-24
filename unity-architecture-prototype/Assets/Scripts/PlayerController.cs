using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
        private Transform _transform;

        public Camera camera;
        private Vector3 _cameraOffset;

        [Header("References")]
        public GameManager gameManager;
        public EnemyManager enemyManager;
        
        [Header("Movement")]
        public float moveSpeed = 5f;

        [Header("Health")]
        public int currentHealth = 10;
        public int maxHealth = 10;
        public bool isDead = false;
        
        [Header("Pistol")]
        public GameObject projectilePrefab;
        public float fireRate = 0.5f;
        public float pistolRange = 5;
        public int damage = 1;
        private float _timeSinceLastFire = 0.0f;
        private Transform _currentTarget = null;
        private Transform _closestTarget = null;
        public float knockBackIntensity = 1;

        
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
            if(!GameManager.instance.isGameActive) return;
            
            // Get Closest enemy target.
            var closestDistance = Mathf.Infinity;
            _closestTarget = null;
            foreach (var enemy in enemyManager.enemies)
            {
                var distance = Vector3.Distance(_transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    _closestTarget = enemy.transform;
                }
            }
       
            if(closestDistance <= pistolRange)
                _currentTarget = _closestTarget;
            else
                _currentTarget = null;
            
            // Fire Weapon if possible.
            _timeSinceLastFire += Time.deltaTime;
            
            if(_timeSinceLastFire > fireRate)
            {
                if (_currentTarget != null)
                {
                    var directionToTarget = Vector3.ProjectOnPlane(_currentTarget.position - _transform.position, Vector3.up).normalized;
                    var projectileGo = Instantiate(projectilePrefab, _transform.position, Quaternion.LookRotation(directionToTarget));
                    var projectile = projectileGo.GetComponent<Projectile>();
                    projectile.damage = damage;
                    projectile.knockBackIntensity = knockBackIntensity;
                    _timeSinceLastFire = 0.0f;
                }
            }
            
            var dir = Vector3.zero;
                
            if(Input.GetKey(KeyCode.A))
                dir += Vector3.left;
            if(Input.GetKey(KeyCode.D))
                dir += Vector3.right;
            if (Input.GetKey(KeyCode.W))
                dir += Vector3.forward;
            if(Input.GetKey(KeyCode.S))
                dir += Vector3.back;
            
            // Check if the player is at the level bounds, if they are, make sure they cant move in the direction of the bound
            if (_transform.position.x <= -gameManager.levelBounds.x && dir.x < 0)
                dir.x = 0;
            if (_transform.position.x >= gameManager.levelBounds.x && dir.x > 0)
                dir.x = 0;
            if (_transform.position.z <= -gameManager.levelBounds.y && dir.z < 0)
                dir.z = 0;
            if (_transform.position.z >= gameManager.levelBounds.y && dir.z > 0)
                dir.z = 0;
            
            // Apply movement
            if (dir.magnitude > 0)
            {
                _transform.position += dir.normalized * (Time.deltaTime * moveSpeed);
                _transform.rotation = Quaternion.LookRotation(dir);
            }
            
            
            // Camera position.
            var cameraWishPosition = _transform.position + _cameraOffset;
            
            // We want the same level bound logic for the camera, but it stops its position if the player is within 5m of the level bounds
            if (_transform.position.x <= -gameManager.levelBounds.x + 5 ||
                _transform.position.x >= gameManager.levelBounds.x - 5)
            {
                cameraWishPosition =
                    new Vector3(camera.transform.position.x, cameraWishPosition.y, cameraWishPosition.z);
            }

            if (_transform.position.z <= -gameManager.levelBounds.y + 5 ||
                _transform.position.z >= gameManager.levelBounds.y - 5)
            {
                cameraWishPosition =
                    new Vector3(cameraWishPosition.x, cameraWishPosition.y, camera.transform.position.z);
            }
            
            camera.transform.position = cameraWishPosition;
        }

        public void TakeDamage(int damageAmount)
        {
            currentHealth -= damageAmount;

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                gameManager.LoseGame();
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

        public void ResetPlayer()
        {
            transform.SetPositionAndRotation(Vector3.up, Quaternion.identity);
            isDead = false;
            currentHealth = maxHealth;
            SetUI();
        }
}
