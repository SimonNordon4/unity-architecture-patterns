using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityArchitecture.SpaghettiPattern
{
    [DefaultExecutionOrder(10)]
    public class PlayerManager : MonoBehaviour
    {
        private Transform _transform;

        public Camera gameCamera;
        private Vector3 _cameraOffset;

        public Vector3 targetDirection;

        [Header("References")]
        public GameManager gameManager;
        public EnemyManagerOld enemyManager;

        [Header("Stats")] 
        public int playerCurrentHealth = 10;
        public Stat playerMaxHealth = new(10);
        public Stat healthRegen = new(1);
        public Stat playerSpeed = new(5);
        public Stat armor = new(0);
        public Stat dodge = new(0);
        public Stat dashCooldown = new(8);
        public Stat dashDistance = new(0);

        public Stat damage = new(1);
        public Stat critChance = new(0);
        public Stat critDamage = new(1.5f);
        public Stat range = new(5);
        public Stat firerate = new(0.5f);
        public Stat knockback = new(1);
        public Stat pierce = new(0);
        public Stat bulletSize = new(1);

        public readonly Dictionary<StatType, Stat> Stats = new();

        [Header("Pistol")]
        public GameObject projectilePrefab;
        private float _timeSinceLastFire = 0.0f;
        private Transform _closestTarget = null;

        [Header("UI")]
        public Transform localCanvas;
        public GameObject dodgeTextGo;
        public GameObject damageTextGo;
        private Quaternion _startRotation;

        private Coroutine _dodgeTextCoroutine;
        private Coroutine _damageTextCoroutine;
        private bool _isDashing;
        private bool _canTakeDamage = true;

        [Header("Dash")]
        public float dashTime = 0.2f;
        private bool _dashOnCooldown = false;
        private float _timeSinceLastDash = 0f;

        [Header("Effects")]
        public ParticleSystem shootEffect;
        public ParticleSystem dashParticle;

        [Header("Debug")] public bool isDebugMode = false;

        [Header("Sound")]
        public SoundDefinition shootSound;
        public SoundDefinition dashSound;
        public SoundDefinition deathSound;
        public SoundDefinition healthPackSound;
        public SoundDefinition takeDamageSound;
        public SoundDefinition blockSound;

        private void Awake()
        {
            _transform = transform;
            _cameraOffset = gameCamera.transform.position - _transform.position;
        }

        private void Start()
        {
            _startRotation = localCanvas.rotation;
        }

        public void ResetStats()
        {
            Stats.Clear();
            InitializeStats();
        }

        public void InitializeStats()
        {
            Stats.Add(StatType.MaxHealth, playerMaxHealth);
            Stats.Add(StatType.HealthRegen, healthRegen);
            Stats.Add(StatType.Speed, playerSpeed);
            Stats.Add(StatType.Armor, armor);
            Stats.Add(StatType.Dodge, dodge);
            Stats.Add(StatType.DashCooldown, dashCooldown);
            Stats.Add(StatType.DashDistance, dashDistance);
            Stats.Add(StatType.Damage, damage);
            Stats.Add(StatType.CritChance, critChance);
            Stats.Add(StatType.CritDamage, critDamage);
            Stats.Add(StatType.Range, range);
            Stats.Add(StatType.FireRate, firerate);
            Stats.Add(StatType.KnockBack, knockback);
            Stats.Add(StatType.Pierce, pierce);
            Stats.Add(StatType.BulletSize, bulletSize);
        }

        private IEnumerator Dash()
        {
            _isDashing = true;

            var elapsedTime = 0f;
            var startPosition = transform.position;
            var dashDestination = transform.forward * dashDistance.value + transform.position;

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
            _dashOnCooldown = true;
        }

        private void Update()
        {
            if(_dashOnCooldown)
            {
                _timeSinceLastDash -= Time.deltaTime;
                if(_timeSinceLastDash > dashCooldown.value)
                {
                    _dashOnCooldown = false;
                }
            }

            if (!GameManager.instance.isGameActive) return;

            if (Input.GetKeyDown(KeyCode.Space) && !_dashOnCooldown && !_isDashing)
            {
                AudioManager.instance.PlaySound(dashSound);
                dashParticle.Play();
            }

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

            if (!targetIsNull)
            {
                targetDirection = Vector3.ProjectOnPlane(_closestTarget.position - _transform.position, Vector3.up).normalized;
                if (targetDirection.magnitude < 0.1f) targetDirection = transform.forward;
            }
            else
            {
                targetDirection = transform.forward;
            }

            // Fire Pistol if possible.
            _timeSinceLastFire += Time.deltaTime;
            if (_timeSinceLastFire > 1 / firerate.value)
            {
                if (!targetIsNull)
                {
                    if (closestDistance <= range.value)
                    {
                        var isKnockBack = false;
                        if (_closestTarget.TryGetComponent<EnemyController>(out var enemyController))
                        {
                            isKnockBack = enemyController.isKnockedBack;
                        }

                        AudioManager.instance.PlaySound(shootSound);
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
                _transform.position += dir.normalized * (Time.deltaTime * playerSpeed.value);
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
            projectile.damage = Mathf.RoundToInt(damage.value);
            projectile.knockBackIntensity = knockback.value;
            projectile.pierceCount = (int)pierce.value;
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
                    1 / firerate.value);
            }

            projectileGo.transform.forward = shootDirection;
            projectile.damage = Mathf.RoundToInt(damage.value);
            projectile.knockBackIntensity = knockback.value;
            projectile.pierceCount = (int)pierce.value;
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
            if (!_canTakeDamage) return;
            if (_isDashing) return;
            // Check if damage is dodged.
            var hitChance = Random.Range(0, 100);

            if (hitChance < dodge.value)
            {
                if (_dodgeTextCoroutine != null) StopCoroutine(_dodgeTextCoroutine);
                _dodgeTextCoroutine = StartCoroutine(ShowDodgeText());
                return;
            }

            var armorMitigation = armor.value / (armor.value + 100);

            damageAmount = Mathf.RoundToInt(1-armorMitigation);
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

            AudioManager.instance.PlaySound(damageAmount > 0 ? takeDamageSound : blockSound);

            AccountManager.Instance.statistics.totalDamageTaken += damageAmount;

            playerCurrentHealth -= damageAmount;

            if (playerCurrentHealth <= 0)
            {
                playerCurrentHealth = 0;

                AccountManager.Instance.statistics.totalDeaths++;
                AudioManager.instance.PlaySound(deathSound);
                gameManager.LoseGame();

                List<Achievement> dieAchievements = AccountManager.Instance.achievementSave.achievements
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
                        AccountManager.Instance.AchievementUnlocked(a);
                    }
                }
            }
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

            if (other.CompareTag("Spawn Indicator"))
            {
                Destroy(other.gameObject);
            }

            if (other.CompareTag("Health Pack"))
            {
                AudioManager.instance.PlaySound(healthPackSound);
                var healthGained =
                 (int)Mathf.Clamp(playerCurrentHealth + playerMaxHealth.value * 0.1f + 1,
                    0f,
                    playerMaxHealth.value);

                AccountManager.Instance.statistics.totalDamageHealed += healthGained;
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