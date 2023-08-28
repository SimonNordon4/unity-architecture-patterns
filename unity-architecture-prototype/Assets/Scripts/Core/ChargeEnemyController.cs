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
    protected bool _isCharging = false;
    
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
    
        if (playerTarget == null) return;
        var dir = Vector3.ProjectOnPlane(playerTarget.position - transform.position, Vector3.up).normalized;
        if (dir.magnitude < 0.5f)
        {
            dir = Vector3.zero;
        }

        float distanceToPlayer = Vector3.Distance(playerTarget.position, transform.position);

        if(!_isCharging)
        {
            if(distanceToPlayer <= chargeDistance && _timeSinceLastCharge >= chargeCooldown)
            {
                StartCoroutine(ChargeAtPlayer(dir));
                _timeSinceLastCharge = 0f;
            }
            else if(distanceToPlayer > chargeDistance)
            {
                transform.Translate(dir * (moveSpeed * Time.deltaTime));
            }
            else
            {
                RandomMove();
            }
        }

        _timeSinceLastCharge += Time.deltaTime;

        _timeSinceLastCharge += Time.deltaTime;

        ClampTransformToLevelBounds();
    
        healthBarUI.transform.rotation = uiStartRotation;
    }

    IEnumerator ChargeAtPlayer(Vector3 dir)
    {
        _isCharging = true;

        yield return new WaitForSeconds(chargeUpTime);  // Wait for 1 second before charging

        float chargedDistance = 0; // Track how much distance the enemy has covered during the charge

        // While the enemy hasn't charged the desired distance
        while (chargedDistance < (2 * chargeDistance))
        {
            // Calculate the distance to move in this frame
            float distanceThisFrame = chargeSpeed * Time.deltaTime;
            transform.Translate(dir * distanceThisFrame);

            chargedDistance += distanceThisFrame;

            yield return null; // Wait for next frame
        }

        _isCharging = false;
    }

    void RandomMove()
    {
        if(Vector3.Distance(transform.position, _randomDestination) <= 0.5f || _randomDestination == Vector3.zero)
        {
            _randomDestination = new Vector3(
                Random.Range(GameManager.instance.levelBounds.x, GameManager.instance.levelBounds.x),
                transform.position.y,
                Random.Range(GameManager.instance.levelBounds.y, GameManager.instance.levelBounds.y)
            );
        }

        var moveDirection = (_randomDestination - transform.position).normalized;
        transform.Translate(moveDirection * (moveSpeed * Time.deltaTime));
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
