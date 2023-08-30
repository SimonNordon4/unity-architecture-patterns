using System.Collections;
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
        
        [Header("Pistol")]
        public GameObject projectilePrefab;
        private float _timeSinceLastFire = 0.0f;
        private Transform _closestTarget = null;

        [Header("Sword")]
        public Transform SwordPivot;
        private float _timeSinceLastSwing = 0.0f;
        private bool _isSwingingLeftToRight = true;
        private bool _isSwordAttacking = false;

        [Header("UI")]
        public TextMeshProUGUI healthText;
        public RectTransform healthBar;

        [Header("Debug")] public bool isDebugMode = false;
        
        
        private void Awake()
        {
            _transform = transform;
            _cameraOffset = camera.transform.position - _transform.position;
        }

        private void Start()
        {
            SwordPivot.transform.parent = null;
            SwordPivot.gameObject.SetActive(false);
            SetUI();
        }

        private void Update()
        {
            if(!GameManager.instance.isGameActive) return;
            
            SwordPivot.transform.position = _transform.position;
            
            // Get Closest enemy target.
            var closestDistance = Mathf.Infinity;
            var targetIsNull = true;
            _closestTarget = null;
            foreach (var enemy in enemyManager.enemies)
            {
                var distance = Vector3.Distance(_transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    _closestTarget = enemy.transform;
                    targetIsNull = false;
                }
            }

            if (isDebugMode)
            {
                var targetPos = targetIsNull ? _transform.position : _closestTarget.position;
                Debug.DrawLine(_transform.position, targetPos, Color.red);
            }
       
            // Fire Pistol if possible.
            _timeSinceLastFire += Time.deltaTime;
            if(_timeSinceLastFire > 1 / gameManager.pistolFireRate.value)
            {
                if (!targetIsNull)
                {
                    if (closestDistance <= gameManager.pistolRange.value)
                    {
                        var directionToTarget = Vector3.ProjectOnPlane(_closestTarget.position - _transform.position, Vector3.up).normalized;
                        var projectileGo = Instantiate(projectilePrefab, _transform.position, Quaternion.LookRotation(directionToTarget));
                        var projectile = projectileGo.GetComponent<Projectile>();
                        projectile.damage = Mathf.RoundToInt(gameManager.pistolDamage.value);
                        projectile.knockBackIntensity = gameManager.pistolKnockBack.value;
                        projectile.pierceCount = (int)gameManager.pistolPierce.value;
                        _timeSinceLastFire = 0.0f;
                    }
                }
            }
            
            // Swing sword if possible.
            _timeSinceLastSwing += Time.deltaTime;
            if (_timeSinceLastSwing > 1 / gameManager.swordAttackSpeed.value && !_isSwordAttacking)
            {
                if (!targetIsNull)
                {
                    if (closestDistance <= gameManager.swordRange.value)
                    {
                        StopAllCoroutines();
                        StartCoroutine(SwordAttack());
                        _timeSinceLastSwing = 0.0f;
                    }
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
                _transform.position += dir.normalized * (Time.deltaTime * gameManager.playerSpeed.value);
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
            SetUI();
        }

        private IEnumerator SwordAttack()
    {
        _isSwordAttacking = true;
        var swordArc = gameManager.swordArc.value;
        // Enable the sword gameobject.
        SwordPivot.gameObject.SetActive(true);
        SwordPivot.localScale = new Vector3(1f, 1f, gameManager.swordRange.value);
    
        // Base rotation values.
        var leftRotation = Quaternion.Euler(0, swordArc * -0.5f, 0);
        var rightRotation = Quaternion.Euler(0, swordArc * 0.5f, 0);
    
        // The start rotation needs to be directed to the closest target.
        var directionToTarget = Vector3.ProjectOnPlane( _closestTarget.transform.position - transform.position, Vector3.up).normalized;
        SwordPivot.forward = directionToTarget;
    
        // Determine the start and end rotation based on the current swing direction.
        Quaternion startRotation, endRotation;
        if (_isSwingingLeftToRight)
        {
            startRotation = Quaternion.LookRotation(directionToTarget) * leftRotation;
            endRotation = Quaternion.LookRotation(directionToTarget) * rightRotation;
        }
        else
        {
            startRotation = Quaternion.LookRotation(directionToTarget) * rightRotation;
            endRotation = Quaternion.LookRotation(directionToTarget) * leftRotation;
        }
        
        var total180Arcs = Mathf.FloorToInt(swordArc / 180f);
        var swingTime = 0.2f;

        if (total180Arcs > 0)
        {
            var lastStart = startRotation;
            var directionSign = _isSwingingLeftToRight ? 1 : -1;
            var lastEnd = startRotation * Quaternion.Euler(0, 179.9f * directionSign, 0);
            
            for (var i = 0; i < total180Arcs; i++)
            {
                var t = 0.0f;
                var swing = true;
                while (swing)
                {
                    t += Time.deltaTime;
                    SwordPivot.rotation = Quaternion.Lerp(lastStart, lastEnd, t / swingTime);
                    yield return null;
                    if (!(t >= swingTime)) continue;
                    lastStart = SwordPivot.rotation;
                    lastEnd = lastStart * Quaternion.Euler(0, 179.9f * directionSign, 0);
                    swing = false;

                }
            }
        }
        else
        {
            // Lerp the sword rotation from start to end over 0.5 seconds.
            var t = 0.0f;

            while (t < swingTime)
            {
                t += Time.deltaTime;
                SwordPivot.rotation = Quaternion.Lerp(startRotation, endRotation, t / swingTime);
                yield return null;
            }
        }

        _isSwordAttacking = false;
    
        // Toggle the swing direction for the next attack.
        _isSwingingLeftToRight = !_isSwingingLeftToRight;
    
        // Disable the sword gameobject.
        SwordPivot.gameObject.SetActive(false);
    }
        
        public void TakeDamage(int damageAmount)
        {
            gameManager.playerCurrentHealth -= damageAmount;

            if (gameManager.playerCurrentHealth <= 0)
            {
                gameManager.playerCurrentHealth = 0;
                gameManager.LoseGame();
            }
            SetUI();
        }

        public void OnTriggerEnter(Collider other)
        {
            
            if(other.CompareTag("Spawn Indicator"))
            {
                Destroy(other.gameObject);   
            }

            if (other.CompareTag("Health Pack"))
            {
                GameManager.instance.playerCurrentHealth =
                   (int)Mathf.Clamp( (GameManager.instance.playerCurrentHealth * 1.1f + 1), 
                        0f, 
                        GameManager.instance.playerMaxHealth.value);
                
                Destroy(other.gameObject);
            }
        }
        
        private void SetUI()
        {
            healthText.text = $"{gameManager.playerCurrentHealth}/{(int)gameManager.playerMaxHealth.value}";
            healthBar.localScale = new Vector3(gameManager.playerCurrentHealth / gameManager.playerMaxHealth.value, 1f, 1f);
        }

        public void ResetPlayer()
        {
            transform.SetPositionAndRotation(Vector3.up, Quaternion.identity);
            camera.transform.position = _transform.position + _cameraOffset;
            SetUI();
        }
}
