using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UnityArchitecture.SpaghettiPattern
{
    [DefaultExecutionOrder(10)]
    public class PlayerController : MonoBehaviour
    {
        private Transform _transform;

        public Camera gameCamera;
        private Vector3 _cameraOffset;

        public Vector3 targetDirection;

        [Header("References")] public GameManager gameManager;
        public EnemyManager enemyManager;

        [Header("Stats")] public int playerCurrentHealth = 5;
        public Stat playerMaxHealth = new(5);
        public Stat healthRegen = new(0);
        public Stat playerSpeed = new(5);
        public Stat armor = new(0);
        public Stat dodge = new(0);
        public Stat damage = new(1);
        public Stat critChance = new(0);
        public Stat range = new(5);
        public Stat firerate = new(5);
        public Stat knockback = new(1);
        public Stat pierce = new(1);

        public readonly Dictionary<StatType, Stat> Stats = new();

        [Header("Items")] public readonly List<ChestItem> currentlyHeldItems = new();

        [Header("Pistol")] public GameObject projectilePrefab;
        private float _timeSinceLastFire = 0.0f;
        private Transform _closestTarget = null;

        [Header("UI")] public Transform localCanvas;
        public GameObject dodgeTextGo;
        public GameObject damageTextGo;
        private Quaternion _startRotation;

        private Coroutine _dodgeTextCoroutine;
        private Coroutine _damageTextCoroutine;

        [Header("Effects")] public ParticleSystem shootEffect;

        [Header("Debug")] public bool isDebugMode = false;

        [Header("Sound")] public AudioClip shootSound;
        public AudioClip deathSound;
        public AudioClip healthPackSound;
        public AudioClip takeDamageSound;
        public AudioClip blockSound;
        
        private float _timeSinceLastHealthRegen = 0.0f;

        private void Awake()
        {
            _transform = transform;
            _cameraOffset = gameCamera.transform.position - _transform.position;
        }

        private void Start()
        {
            _startRotation = localCanvas.rotation;
            currentlyHeldItems.Clear();
            InitializeStats();
        }

        public void ResetStats()
        {
            Stats.Clear();
            currentlyHeldItems.Clear();
            InitializeStats();
        }

        public void InitializeStats()
        {
            Stats.Add(StatType.MaxHealth, playerMaxHealth);
            Stats.Add(StatType.HealthRegen, healthRegen);
            Stats.Add(StatType.Speed, playerSpeed);
            Stats.Add(StatType.Armor, armor);
            Stats.Add(StatType.Dodge, dodge);
            Stats.Add(StatType.Damage, damage);
            Stats.Add(StatType.CritChance, critChance);
            Stats.Add(StatType.Range, range);
            Stats.Add(StatType.FireRate, firerate);
            Stats.Add(StatType.KnockBack, knockback);
            Stats.Add(StatType.Pierce, pierce);
            playerMaxHealth.Reset();
            healthRegen.Reset();
            playerSpeed.Reset();
            armor.Reset();
            dodge.Reset();
            damage.Reset();
            critChance.Reset();
            range.Reset();
            firerate.Reset();
            knockback.Reset();
            pierce.Reset();
            
            // make player a little stronger.
            firerate.AddModifier(new Modifier
            {
                statType = StatType.FireRate,
                modifierValue = 25,
                isFlatPercentage = false
            });

            playerCurrentHealth = playerMaxHealth.value;
        }

        public void AddItem(ChestItem item)
        {
            currentlyHeldItems.Add(item);

            // Add modifiers to the stats.
            foreach (var mod in item.modifiers)
            {
                var stat = Stats[mod.statType];
                stat.AddModifier(mod);

                // If it's a max health mod, we need to also increase the current health.
                if (mod.statType == StatType.MaxHealth)
                {
                    // We want to change this to equal the additional amount.
                    var addedHealth = playerMaxHealth.baseValue * mod.modifierValue * 0.01f;
                    Debug.Log("Added Health: " + addedHealth);
                    
                    var newHealth = Mathf.RoundToInt(Mathf.Clamp(addedHealth + playerCurrentHealth, 1,
                        playerMaxHealth.value));

                    playerCurrentHealth = newHealth;
                } 
            }
        }

        private void Update()
        {
            if (!GameManager.Instance.isGameActive) return;

            // Get Closest enemy target.
            var closestDistance = Mathf.Infinity;
            var targetIsNull = true;
            _closestTarget = null;
            foreach (var enemy in enemyManager.activeEnemies)
            {
                // ignore enemies that are spawning.
                if (enemy.isSpawning) continue;
                
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

            foreach (var enemy in enemyManager.activeBosses)
            {
                if(enemy == null) continue;
                // ignore enemies that are spawning.
                if (enemy.isSpawning) continue;
                
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

            if (!targetIsNull)
            {
                targetDirection = Vector3.ProjectOnPlane(_closestTarget.position - _transform.position, Vector3.up)
                    .normalized;
                if (targetDirection.magnitude < 0.1f) targetDirection = transform.forward;
            }
            else
            {
                targetDirection = transform.forward;
            }

            // Fire Pistol if possible.
            _timeSinceLastFire += Time.deltaTime;
            
            // This is dividing firerate by 10 and inverting it. 
            // FireRate = 5 -> 0.5 shots/s
            // FireRate = 10 -> 1 shot/s
            // FireRate = 20 -> 2 shots/s
            var effectiveFireRate = 10f / firerate.value;
            if (_timeSinceLastFire > effectiveFireRate)
            {
                if (!targetIsNull)
                {
                    if (closestDistance <= range.value * 0.5f)
                    {
                        var isKnockBack = false;
                        if (_closestTarget.TryGetComponent<EnemyController>(out var enemyController))
                        {
                            isKnockBack = enemyController.isKnockedBack;
                        }

                        AudioManager.Instance.PlaySound(shootSound);
                        shootEffect.Play();
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
                _transform.position += dir.normalized * (Time.deltaTime * playerSpeed.value * 0.5f);
                _transform.rotation = Quaternion.LookRotation(dir);
            }
            
            
            
            // Calculate health regeneration
            if (playerCurrentHealth < playerMaxHealth.value)
            {
                _timeSinceLastHealthRegen += Time.deltaTime;
                // This is dividing regen by 100 and inverting it. 
                // HealthRegen = 10 -> 0.2 hp/s
                // HealthRegen = 100 -> 2 hp/s
                // HealthRegen = 200 -> 4 hp/s
                if (_timeSinceLastHealthRegen > 50f / healthRegen.value)
                {
                    _timeSinceLastHealthRegen = 0;
                    playerCurrentHealth += 1;
                }
            }
        }

        private void Shoot()
        {
            var directionToTarget = Vector3
                .ProjectOnPlane(_closestTarget.position - _transform.position, Vector3.up).normalized;
            var projectileGo = Instantiate(projectilePrefab, _transform.position,
                Quaternion.LookRotation(directionToTarget));
            var projectile = projectileGo.GetComponent<Projectile>();
            projectile.damage = Mathf.RoundToInt(damage.value);
            projectile.knockBackIntensity = knockback.value;
            projectile.pierceCount = pierce.value / 100;
            projectile.critChance = critChance.value;
            Debug.Log("Crit Chance = " + critChance.value);
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


            projectileGo.transform.forward = shootDirection;
            projectile.damage = Mathf.RoundToInt(damage.value);
            projectile.knockBackIntensity = knockback.value;
            projectile.pierceCount = pierce.value / 100;
            projectile.critChance = critChance.value;
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
                    new Vector3(gameCamera.transform.position.x, cameraWishPosition.y, cameraWishPosition.z);
            }

            if (_transform.position.z <= -gameManager.levelBounds.y + 5 ||
                _transform.position.z >= gameManager.levelBounds.y - 5)
            {
                cameraWishPosition =
                    new Vector3(cameraWishPosition.x, cameraWishPosition.y, gameCamera.transform.position.z);
            }

            gameCamera.transform.position = cameraWishPosition;
            // SetUI();
        }

        public void TakeDamage(int damageAmount)
        {
            // Check if damage is dodged.
            var hitChance = Random.Range(0, 100);

            if (hitChance < dodge.value)
            {
                if (_dodgeTextCoroutine != null) StopCoroutine(_dodgeTextCoroutine);
                _dodgeTextCoroutine = StartCoroutine(ShowDodgeText());
                return;
            }

            var armorMitigation = armor.value / (armor.value + 100f);

            damageAmount = Mathf.RoundToInt(damageAmount - armorMitigation);
            // We should never be invincible imo. hard cap to 1.
            if (damageAmount <= 0)
            {
                damageAmount = 1;
            }

            if (_damageTextCoroutine != null)
            {
                StopCoroutine(_damageTextCoroutine);
            }

            _damageTextCoroutine = StartCoroutine(ShowDamageText(damageAmount));

            AudioManager.Instance.PlaySound(damageAmount > 0 ? takeDamageSound : blockSound);

            playerCurrentHealth -= damageAmount;

            if (playerCurrentHealth <= 0)
            {
                playerCurrentHealth = 0;

                AudioManager.Instance.PlaySound(deathSound);
                StartCoroutine(WaitAndLoseGame());
            }
        }

        private IEnumerator WaitAndLoseGame()
        {
            yield return new WaitForSeconds(1f);
            GameManager.Instance.LoseGame();
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
                t.position = Vector3.Lerp(startPosition + transform.position, targetPosition + transform.position,
                    inversedQuadraticTime);
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
                t.position = Vector3.Lerp(startPosition + transform.position, targetPosition + transform.position,
                    inversedQuadraticTime);
                t.localScale = Vector3.Lerp(startScale, targetScale, inversedQuadraticTime);
                yield return new WaitForEndOfFrame();
            }

            damageTextGo.SetActive(false);
            yield return null;
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Spawn Indicator"))
            {
                Destroy(other.gameObject);
            }

            if (other.CompareTag("Health Pack"))
            {
                AudioManager.Instance.PlaySound(healthPackSound);
                var healthGained =
                    (int)Mathf.Clamp(playerCurrentHealth + playerMaxHealth.value * 0.1f + 1,
                        0f,
                        playerMaxHealth.value);

                playerCurrentHealth = healthGained;

                Destroy(other.gameObject);
            }
        }

        public void ResetPlayer()
        {
            transform.SetPositionAndRotation(Vector3.up, Quaternion.identity);
            gameCamera.transform.position = _transform.position + _cameraOffset;
        }
    }
}