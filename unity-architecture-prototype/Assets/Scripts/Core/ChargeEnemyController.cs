using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeEnemyController : EnemyController
{
    [Header("Charge Values")]
    public float chargeSpeed = 10f;
    public float chargeDistance = 3f;
    public float chargeCooldown = 2f;
    public float chargeUpTime = 0.5f;
    protected float _timeSinceLastCharge;
    public bool _isCharging = false;
    
    private Vector3 _randomDestination;
    
    protected override void Start()
    {
        base.Start();
        _timeSinceLastCharge = chargeCooldown;
    }
    
    protected override void Update()
    {
        if(GameManager.instance.isGameActive == false) return;
        if(_isKnockedBack) return;
        if(_isCharging) return;
    
        if (playerTarget == null) return;
        var dir = Vector3.ProjectOnPlane(playerTarget.position - transform.position, Vector3.up);
        if (dir.magnitude < 0.5f)
        {
            dir = Vector3.zero;
        }

        dir = dir.normalized;

        float distanceToPlayer = Vector3.Distance(playerTarget.position, transform.position);


            if(distanceToPlayer <= chargeDistance && _timeSinceLastCharge >= chargeCooldown)
            {
                StartCoroutine(ChargeAtPlayer(dir));
                _timeSinceLastCharge = 0f;
            }
            
            
            var avoidanceDirection = GetAvoidanceFromOtherEnemies();
            var updatedDir = (dir + avoidanceDirection * repulsionForce).normalized;
            // Apply direction to transform.
            if (dir.magnitude > 0)
            {
                transform.position += updatedDir * (Time.deltaTime * moveSpeed);
            }


        _timeSinceLastCharge += Time.deltaTime;
        ClampTransformToLevelBounds();
        if (dir.magnitude > 0 && !_isCharging)
        {
            transform.forward = dir;
        }
        healthBarUI.transform.rotation = uiStartRotation;
    }

    private IEnumerator ChargeAtPlayer(Vector3 dir)
    {
        _isCharging = true;

        var elapsedChargeUpTime = 0f;
        while (elapsedChargeUpTime < chargeUpTime)
        {
            elapsedChargeUpTime += Time.deltaTime;
            transform.forward = dir;
            yield return new WaitForEndOfFrame();
        }

        float chargedDistance = 0; // Track how much distance the enemy has covered during the charge

        // While the enemy hasn't charged the desired distance
        while (chargedDistance < (2 * chargeDistance))
        {
            // Calculate the distance to move in this frame
            float distanceThisFrame = chargeSpeed * Time.deltaTime;
            transform.position += dir * distanceThisFrame;
            transform.forward = dir;
            chargedDistance += distanceThisFrame;
            ClampTransformToLevelBounds();
            
            if(GameManager.instance.isGameActive == false)
            {
                _isCharging = false;
                yield break;
            }
            yield return new WaitForEndOfFrame(); // Wait for next frame
        }

        _isCharging = false;
    }

    public override void ApplyKnockBack(Vector3 direction, float intensity)
    {
        if (_isCharging) return;
        base.ApplyKnockBack(direction, intensity);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (_isCharging)
        {
            if (other.CompareTag("Player"))
            {
                var playerController = other.GetComponent<PlayerController>();
                if (playerController != null)
                {
                        playerController.TakeDamage(damageAmount);

                }
            }
        }
        else
        {
            base.OnTriggerEnter(other);
        }
    }
    protected override void OnTriggerExit(Collider other)
    {
        if (_isCharging) return;
        base.OnTriggerExit(other);
    }
}
