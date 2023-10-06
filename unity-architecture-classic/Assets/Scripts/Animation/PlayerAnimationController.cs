using System.Collections;
using System.Collections.Generic;
using Classic.Game;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private GameState gameState;    
    public PlayerController playerController;
    private Transform _transformToFollow;

    public Transform gunPivot;
    
    private Vector3 offset;
    private Transform _transform;

    public float rotationSpeed = 1f;
    public float gunRotationSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        _transformToFollow = playerController.transform;
        _transform = transform;
        offset = transform.position - _transformToFollow.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (gameState.currentState != GameStateEnum.Active) return;
        var gunRotation = Quaternion.LookRotation(playerController.targetDirection.magnitude > 0 ? playerController.targetDirection : Vector3.forward);
        gunPivot.rotation = Quaternion.Lerp(gunPivot.rotation, gunRotation, Time.deltaTime * gunRotationSpeed);
        _transform.position = _transformToFollow.position + offset;
        _transform.rotation = Quaternion.Lerp(_transform.rotation, _transformToFollow.rotation, Time.deltaTime * rotationSpeed);
    }
}
