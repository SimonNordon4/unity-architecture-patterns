using System.Collections;
using System.Collections.Generic;
using GameplayComponents.Combat;
using GameObjectComponent.Character;
using GameObjectComponent.Game;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private GameState gameState;    
    public CombatTarget characterTarget;
    [SerializeField] private Transform transformToFollow;

    public Transform gunPivot;
    
    private Vector3 offset;
    private Transform _transform;

    public float rotationSpeed = 1f;
    public float gunRotationSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        _transform = transform;
        offset = transform.position - transformToFollow.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (gameState.currentState != GameStateEnum.Active) return;
        var gunRotation = Quaternion.LookRotation(characterTarget.targetDirection.magnitude > 0 ? characterTarget.targetDirection : Vector3.forward);
        gunPivot.rotation = Quaternion.Lerp(gunPivot.rotation, gunRotation, Time.deltaTime * gunRotationSpeed);
        _transform.position = transformToFollow.position + offset;
        _transform.rotation = Quaternion.Lerp(_transform.rotation, transformToFollow.rotation, Time.deltaTime * rotationSpeed);
    }
}
