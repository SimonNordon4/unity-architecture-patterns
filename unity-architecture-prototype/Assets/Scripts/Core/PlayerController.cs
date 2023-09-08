using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[DefaultExecutionOrder(10)]
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
        public Transform swordPivot;
        private float _timeSinceLastSwing = 0.0f;
        private bool _isSwingingLeftToRight = true;
        private bool _isSwordAttacking = false;

        [Header("UI")]
        public TextMeshProUGUI healthText;
        public RectTransform healthBar;
        public Transform localCanvas;
        public GameObject dodgeTextGo;
        public GameObject damageTextGo;
        private Quaternion _startRotation;

        private Coroutine _swordCoroutine;
        private Coroutine _dodgeTextCoroutine;
        private Coroutine _damageTextCoroutine;
        private Coroutine _dashCoroutine;
        private bool _isDashing;
        private bool _canTakeDamage = true;
        
        [Header("Dash")]
        public float dashDistance = 5f;
        public float dashTime = 0.2f;

        [Header("Debug")] public bool isDebugMode = false;
        
        
        private void Awake()
        {
            _transform = transform;
            _cameraOffset = camera.transform.position - _transform.position;
        }

        private void Start()
        {
            swordPivot.transform.parent = null;
            swordPivot.gameObject.SetActive(false);
            _startRotation = localCanvas.rotation;
            SetUI();
        }

        private IEnumerator Dash()
        {
            _isDashing = true;
            gameManager.dashes.value--;
            
            var elapsedTime = 0f;
            var startPosition = transform.position;
            var dashDestination = transform.forward * dashDistance + transform.position;
            
            while (elapsedTime < dashTime)
            {
                elapsedTime += Time.deltaTime;

                var normalizedTime = elapsedTime / dashTime;
                var inverseQuadraticTime = 1 - Mathf.Pow(1 - normalizedTime, 2);
                
                var desiredPos = Vector3.Lerp(startPosition, dashDestination, inverseQuadraticTime);
                
                // clamp the position to the level bounds
                transform.position = new Vector3(Mathf.Clamp(desiredPos.x, -gameManager.levelBounds.x, gameManager.levelBounds.x),
                    transform.position.y,
                    Mathf.Clamp(desiredPos.z, -gameManager.levelBounds.y, gameManager.levelBounds.y));
                
                if (!gameManager.isGameActive)
                {
                    _isDashing = false;
                    yield break;
                }
                
                yield return new WaitForEndOfFrame();
            }
            
            _isDashing = false;

        }

        private void Update()
        {
            if(!GameManager.instance.isGameActive) return;

            if (Input.GetKeyDown(KeyCode.Space) && (int)gameManager.dashes.value > 0 && !_isDashing)
            {
                _dashCoroutine = StartCoroutine(Dash());
            }
            
            swordPivot.transform.position = _transform.position;

            if (_isDashing) return;
            
            // Get Closest enemy target.
            var closestDistance = Mathf.Infinity;
            var targetIsNull = true;
            _closestTarget = null;
            foreach (var enemy in enemyManager.enemies)
            {
                var distance = Vector3.Distance(_transform.position, enemy.transform.position);
                // now we minus the radius of the enemy from the distance, so that we get the distance to its edge.
                distance -= enemy.transform.localScale.x * 0.5f;
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    _closestTarget = enemy.transform;
                    targetIsNull = false;
                }
            }

            // Fire Pistol if possible.
            _timeSinceLastFire += Time.deltaTime;
            if (_timeSinceLastFire > 1 / gameManager.pistolFireRate.value)
            {
                if (!targetIsNull)
                {
                    if (closestDistance <= gameManager.pistolRange.value)
                    {
                        var isKnockBack = false;
                        if(_closestTarget.TryGetComponent<EnemyController>(out var enemyController))
                        {
                            isKnockBack = enemyController.isKnockedBack;
                        }

                        if (isKnockBack)
                        {
                            Shoot();
                        }
                        else
                        {
                            ShootPredictive();    
                        }
                        
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
                        if(_swordCoroutine != null) StopCoroutine(_swordCoroutine);
                        _swordCoroutine = StartCoroutine(SwordAttack());
                        _timeSinceLastSwing = 0.0f;
                    }
                }
            }

            var dir = Vector3.zero;

            if (Input.GetKey(KeyCode.A))
                dir += Vector3.left;
            if (Input.GetKey(KeyCode.D))
                dir += Vector3.right;
            if (Input.GetKey(KeyCode.W))
                dir += Vector3.forward;
            if (Input.GetKey(KeyCode.S))
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
        }

        private void Shoot()
        {
            var directionToTarget = Vector3
                .ProjectOnPlane(_closestTarget.position - _transform.position, Vector3.up).normalized;
            var projectileGo = Instantiate(projectilePrefab, _transform.position,
                Quaternion.LookRotation(directionToTarget));
            var projectile = projectileGo.GetComponent<Projectile>();
            projectile.damage = Mathf.RoundToInt(gameManager.pistolDamage.value);
            projectile.knockBackIntensity = gameManager.pistolKnockBack.value;
            projectile.pierceCount = (int)gameManager.pistolPierce.value;
            _timeSinceLastFire = 0.0f;
        }

        private void ShootPredictive()
        {

            
            
                var projectileGo = Instantiate(projectilePrefab, _transform.position, Quaternion.identity);
                var projectile = projectileGo.GetComponent<Projectile>();
    
                // Calculate the time it would take for the projectile to reach the target's current position
                var distanceToTarget = Vector3.Distance(_transform.position, _closestTarget.position);
                var timeToTarget = distanceToTarget / projectile.projectileSpeed;

                // Predict the target's position after the time it would take for the projectile to reach it
                var enemy = _closestTarget.GetComponent<EnemyController>();
                var enemyVelocity = _closestTarget.forward * enemy.moveSpeed;
                var predictedTargetPosition = _closestTarget.position + enemyVelocity * timeToTarget;
                
                // now get the distance to that position
                distanceToTarget = Vector3.Distance(_transform.position, predictedTargetPosition);
                timeToTarget = distanceToTarget / projectile.projectileSpeed;
                predictedTargetPosition = _closestTarget.position + enemyVelocity * timeToTarget;
                
                // iterate again
                distanceToTarget = Vector3.Distance(_transform.position, predictedTargetPosition);
                timeToTarget = distanceToTarget / projectile.projectileSpeed;
                predictedTargetPosition = _closestTarget.position + enemyVelocity * timeToTarget;

                // Aim the projectile towards the predicted position
                var shootDirection = (predictedTargetPosition - _transform.position).normalized;

                if (isDebugMode)
                {
                    // draw a line from the player to the predicted position
                    Debug.DrawLine(_transform.position, predictedTargetPosition, Color.green,
                        1 / gameManager.pistolFireRate.value);

                }
                
                projectileGo.transform.forward = shootDirection;
                projectile.damage = Mathf.RoundToInt(gameManager.pistolDamage.value);
                projectile.knockBackIntensity = gameManager.pistolKnockBack.value;
                projectile.pierceCount = (int)gameManager.pistolPierce.value;
                _timeSinceLastFire = 0.0f;
        }
        
      

        private void LateUpdate()
        {
            localCanvas.rotation = _startRotation;
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
        swordPivot.gameObject.SetActive(true);
        swordPivot.localScale = new Vector3(1f, 1f, gameManager.swordRange.value);
    
        // Base rotation values.
        var leftRotation = Quaternion.Euler(0, swordArc * -0.5f, 0);
        var rightRotation = Quaternion.Euler(0, swordArc * 0.5f, 0);
    
        // The start rotation needs to be directed to the closest target.
        var directionToTarget = Vector3.ProjectOnPlane( _closestTarget.transform.position - transform.position, Vector3.up).normalized;
        swordPivot.forward = directionToTarget;
    
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
        var swingTime = gameManager.swordRange.value * 0.08f;

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
                    swordPivot.rotation = Quaternion.Lerp(lastStart, lastEnd, t / swingTime);
                    yield return null;
                    if (!(t >= swingTime)) continue;
                    lastStart = swordPivot.rotation;
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
                swordPivot.rotation = Quaternion.Lerp(startRotation, endRotation, t / swingTime);
                yield return null;
            }
        }

        _isSwordAttacking = false;
    
        // Toggle the swing direction for the next attack.
        _isSwingingLeftToRight = !_isSwingingLeftToRight;
    
        // Disable the sword gameobject.
        swordPivot.gameObject.SetActive(false);
    }
        
        public void TakeDamage(int damageAmount)
        {
            if (!_canTakeDamage) return;
            if (_isDashing) return;
            // Check if damage is dodged.
            var hitChance = Random.Range(0, 100);

            if (hitChance < gameManager.dodge.value)
            {
                if(_dodgeTextCoroutine != null) StopCoroutine(_dodgeTextCoroutine);
                _dodgeTextCoroutine = StartCoroutine(ShowDodgeText());
                return;
            }
            
            damageAmount -= (int)gameManager.block.value;
            // We should never be invincible imo.
            if (damageAmount <= 0)
            {
                damageAmount = 0;
            }

            if (_damageTextCoroutine != null)
            {
                StopCoroutine(_damageTextCoroutine);
            }

            _damageTextCoroutine = StartCoroutine(ShowDamageText(damageAmount));
            
            AccountManager.instance.statistics.totalDamageTaken += damageAmount;
            
            gameManager.playerCurrentHealth -= damageAmount;

            if (gameManager.playerCurrentHealth <= 0)
            {
                gameManager.playerCurrentHealth = 0;

                if ((int)gameManager.revives.value > 0)
                {
                    gameManager.revives.value--;
                    
                    var enemyCount = enemyManager.enemies.Count;
                    var enemies = enemyManager.enemies.ToArray();
                    for (var i = 0; i < enemyCount - 1; i++)
                    {
                        var controller = enemies[i].GetComponent<EnemyController>();
                        if (controller != null)
                        {
                            controller.TakeDamage(9999);
                        }
                            
                    }
                    
                    gameManager.playerCurrentHealth = (int)gameManager.playerMaxHealth.value;
                    StartCoroutine(InvincibilityFrames());

                    return;
                }
                
                AccountManager.instance.statistics.totalDeaths++;
                gameManager.LoseGame();
                
                List<Achievement> dieAchievements = AccountManager.instance.achievementSave.achievements
                    .Where(a => a.name == AchievementName.Die ||
                                a.name == AchievementName.Die50Times ||
                                a.name == AchievementName.Die100Times).ToList();
                foreach (var a in dieAchievements)
                {
                    if (a.isCompleted) return;
                    a.progress++;
                    if (a.progress >= a.goal)
                    {
                        a.isCompleted = true;
                        AccountManager.instance.AchievementUnlocked(a);
                    }
                }
            }
            SetUI();
        }
        
        private IEnumerator InvincibilityFrames()
        {
            _canTakeDamage = false;
            yield return new WaitForSeconds(0.5f);
            _canTakeDamage = true;
        }

        private IEnumerator ShowDodgeText()
        {
            dodgeTextGo.SetActive(true);
            var elapsedTime = 0f;
            var t = dodgeTextGo.transform;
        
            var startPosition = Vector3.right;
            var targetPosition = Vector3.up + Vector3.right * 1f;

            var startScale = Vector3.one * 0.25f;
            var targetScale = Vector3.one;
        
            while (elapsedTime < 0.6f)
            {
                elapsedTime += Time.deltaTime;
                var normalizedTime = elapsedTime / 0.4f;
                var inversedQuadraticTime = 1 - Mathf.Pow(1 - normalizedTime, 2);
                t.position = Vector3.Lerp(startPosition + transform.position, targetPosition + transform.position, inversedQuadraticTime);
                t.localScale = Vector3.Lerp(startScale, targetScale, inversedQuadraticTime);
                yield return new WaitForEndOfFrame();
            }
            dodgeTextGo.SetActive(false);
            yield return null;
        }

        private IEnumerator ShowDamageText(int damageAmount)
        {
            var dmgText = damageTextGo.GetComponent<TextMeshProUGUI>();
            damageTextGo.SetActive(true);
            dmgText.text = damageAmount.ToString();
            var elapsedTime = 0f;
            var t = damageTextGo.transform;
        
            var startPosition = Vector3.right;
            var targetPosition = Vector3.up + Vector3.right * 1f;

            var startScale = Vector3.one * 0.25f;
            var targetScale = Vector3.one;
        
            while (elapsedTime < 0.6f)
            {
                elapsedTime += Time.deltaTime;
                var normalizedTime = elapsedTime / 0.4f;
                var quadraticTime = normalizedTime * normalizedTime;
                var inversedQuadraticTime = 1 - Mathf.Pow(1 - normalizedTime, 2);
                t.position = Vector3.Lerp(startPosition + transform.position, targetPosition + transform.position, inversedQuadraticTime);
                t.localScale = Vector3.Lerp(startScale, targetScale, inversedQuadraticTime);
                yield return new WaitForEndOfFrame();
            }
            damageTextGo.SetActive(false);
            yield return null;
        }

        public void OnTriggerEnter(Collider other)
        {
            
            if(other.CompareTag("Spawn Indicator"))
            {
                Destroy(other.gameObject);   
            }

            if (other.CompareTag("Health Pack"))
            {
                var healthGained = (int)Mathf.Clamp( (GameManager.instance.playerCurrentHealth + GameManager.instance.playerMaxHealth.value * 0.1f + 1), 
                    0f, 
                    GameManager.instance.playerMaxHealth.value);
                
                AccountManager.instance.statistics.totalDamageHealed += healthGained;
                GameManager.instance.playerCurrentHealth = healthGained;
                
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
