using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSwingDebug : MonoBehaviour
{
    public Transform SwordPivot;

    public float swordArc = 45;
    
    private float _timeSinceLastSwing = 0.0f;
    public float timeBetweenSwings = 1f;
    
    public bool _isSwingingLeftToRight = true;

    private void Start()
    {
        SwordPivot.transform.parent = null;
        SwordPivot.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        _timeSinceLastSwing += Time.deltaTime;
        if (_timeSinceLastSwing >= timeBetweenSwings)
        {
            StopAllCoroutines();
            _timeSinceLastSwing = 0.0f;
            StartCoroutine(SwordAttack());
        }
    }
    
    private IEnumerator SwordAttack()
    {
        // Enable the sword gameobject.
        SwordPivot.gameObject.SetActive(true);
        SwordPivot.localScale = new Vector3(1f, 1f, 5f);
    
        // Base rotation values.
        var leftRotation = Quaternion.Euler(0, swordArc * -0.5f, 0);
        var rightRotation = Quaternion.Euler(0, swordArc * 0.5f, 0);
    
        // The start rotation needs to be directed to the closest target.
        var directionToTarget = Vector3.ProjectOnPlane(Vector3.right, Vector3.up).normalized;
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

        // Lerp the sword rotation from start to end over 0.5 seconds.
        var t = 0.0f;
        var swingTime = 0.2f;
        while (t < swingTime)
        {
            t += Time.deltaTime;
            SwordPivot.rotation = Quaternion.Lerp(startRotation, endRotation, t / swingTime);
            yield return null;
        }
    
        // Toggle the swing direction for the next attack.
        _isSwingingLeftToRight = !_isSwingingLeftToRight;
    
        // Disable the sword gameobject.
        SwordPivot.gameObject.SetActive(false);
    }
}
