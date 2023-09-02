using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyController : EnemyController
{
    [Header("Ranged Attributes")] public GameObject projectilePrefab;
    public float range = 6f;
    public float targetDistance = 5.5f;
    public int bulletCount = 1;
    public float projectileSpeed = 2f;
    public bool startLoaded = true;
    public float bulletSpread = 15f;

    protected override void Start()
    {
        base.Start();
        // start with a bullet in the chamber!
        if (startLoaded)
            _timeSinceLastDamage = damageCooldown;
    }

    protected override void Update()
    {
        if (GameManager.instance.isGameActive == false) return;

        if (_isKnockedBack) return;

        if (playerTarget == null) return;
        var difference = Vector3.ProjectOnPlane(playerTarget.position - transform.position, Vector3.up);
        var distance = difference.magnitude;
        difference = difference.normalized;
        var wishDir = Vector3.zero;

        // Move towards if far away, move away if too close, don't move if just right.
        if (distance < targetDistance - 0.5f)
            wishDir = -difference.normalized;

        else if (distance > targetDistance + 0.5f)
            wishDir = difference.normalized;

        var avoidanceDirection = GetAvoidanceFromOtherEnemies();
        var updatedDir = (wishDir + avoidanceDirection * repulsionForce).normalized;

        transform.position += updatedDir * (Time.deltaTime * moveSpeed);
        if (distance > 0) transform.rotation = Quaternion.LookRotation(difference.normalized);

        ClampTransformToLevelBounds();

        _timeSinceLastDamage += Time.deltaTime;
        if (_timeSinceLastDamage > damageCooldown && difference.magnitude < range)
        {
            Shoot(difference);
            _timeSinceLastDamage = 0;
        }

        healthBarUI.transform.rotation = uiStartRotation;
    }

    private void Shoot(Vector3 direction)
    {
        // Calculate total spread angle.
        var totalSpread = bulletSpread * (bulletCount - 1);

        // Determine the starting angle. If the bulletCount is odd, 
        // there will always be one bullet going directly forward.
        var startAngle = bulletCount % 2 == 1 ? -(totalSpread / 2) : -totalSpread / 2 + bulletSpread / 2;

        // Get the rotation that will take the Vector3.forward to the 'direction' vector
        var toDirection = Quaternion.FromToRotation(Vector3.forward, direction);

        for (var i = 0; i < bulletCount; i++)
        {
            // Rotate the direction by the current angle to get the bullet direction
            var dir = toDirection * Quaternion.Euler(0, startAngle + i * bulletSpread, 0) * Vector3.forward;

            // Create and setup the projectile
            var projectileGo = Instantiate(projectilePrefab, transform.position, Quaternion.LookRotation(dir));
            var projectile = projectileGo.GetComponent<Projectile>();
            projectile.damage = damageAmount;
            projectile.projectileSpeed = projectileSpeed;
        }

        _timeSinceLastDamage = 0;
    }
}