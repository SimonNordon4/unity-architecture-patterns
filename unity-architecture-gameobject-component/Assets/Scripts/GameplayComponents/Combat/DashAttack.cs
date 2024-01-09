using System.Collections;
using GameObjectComponent.Game;
using GameplayComponents.Locomotion;
using UnityEngine;

namespace GameplayComponents.Combat
{
    public class DashAttack : GameplayComponent
    {
        [SerializeField] private Dash dash;
        [SerializeField] private GameplayComponent defaultMovement;
        [SerializeField] private CombatTarget target;
        [SerializeField] private KnockBack knockBack;
        
        
        [SerializeField] private float dashAttackSpeed = 4f;
        [SerializeField] private float minAttackDistance = 3f;
        [SerializeField] private float dashChargeUpTime = 1f;
        [SerializeField] private float dashDuration = 0.6f;
        [SerializeField] private float dashDistance = 8f;

        private float _timeSinceLastDashAttack = 0f;
        private float _dashChargeUpTime = 0f;
        private bool _isChargingUpDash = false;

        private void OnEnable()
        {
            dash.onDashEnd.AddListener(OnDashEnd);
            dash.dashTime = dashDuration;
            dash.dashDistance = dashDistance;
            
            // Start with dash in the chamber.
            _timeSinceLastDashAttack = dashAttackSpeed;
        }
        
        private void OnDisable()
        {
            dash.onDashEnd.RemoveListener(OnDashEnd);
        }

        private void Update()
        {
            if (_isChargingUpDash) return;

            _timeSinceLastDashAttack += GameTime.deltaTime;
            if (_timeSinceLastDashAttack < dashAttackSpeed) return;
            if(target.targetDistance > minAttackDistance) return;

            defaultMovement.enabled = false;
            knockBack.canBeKnockedBack = false;
            _isChargingUpDash = true;
            StartCoroutine(DashChargeUp());
        }

        private IEnumerator DashChargeUp()
        {
            yield return new WaitForSeconds(dashChargeUpTime);

            _timeSinceLastDashAttack = 0f;
            dash.DashForward();
            _isChargingUpDash = false;
        }
        
        private void OnDashEnd()
        {
            _timeSinceLastDashAttack = 0f;
            defaultMovement.enabled = true;
            knockBack.canBeKnockedBack = true;
        }

        public override void OnGameEnd()
        {
            defaultMovement.enabled = true;
            _dashChargeUpTime = 0f;
            _timeSinceLastDashAttack = 0f;
            StopAllCoroutines();
        }
    }
}