using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    [RequireComponent(typeof(Movement))]
    [RequireComponent(typeof(Stats))]
    public class InputMovement : MoveBase
    {
        private void Update()
        {
            float moveHorizontal = 0f;
            float moveVertical = 0f;

            if (Input.GetKey(KeyCode.D)) moveHorizontal += 1f;
            if (Input.GetKey(KeyCode.A)) moveHorizontal -= 1f;
            if (Input.GetKey(KeyCode.W)) moveVertical += 1f;
            if (Input.GetKey(KeyCode.S)) moveVertical -= 1f;

            var direction = new Vector3(moveHorizontal, 0, moveVertical).normalized;
            
            movement.SetVelocity(direction * Speed);
            movement.SetLookDirection(direction);
        }
    }
}
