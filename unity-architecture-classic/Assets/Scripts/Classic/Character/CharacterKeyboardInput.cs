using Classic.Game;
using UnityEngine;

namespace Classic.Character
{
    public class CharacterKeyboardInput : MonoBehaviour
    {
        [SerializeField] private GameState state;
        [SerializeField] private CharacterMovement movement;
        [SerializeField] private KeyCode upKey = KeyCode.W;
        [SerializeField] private KeyCode leftKey = KeyCode.A;
        [SerializeField] private KeyCode downKey = KeyCode.S;
        [SerializeField] private KeyCode rightKey = KeyCode.D;
        
        private void Update()
        {
            if(!state.isGameActive) return;
            
            var dir = Vector3.zero;
            if(Input.GetKey(upKey))
                dir += Vector3.forward;
            if(Input.GetKey(leftKey))
                dir += Vector3.left;
            if(Input.GetKey(downKey))
                dir += Vector3.back;
            if(Input.GetKey(rightKey))
                dir += Vector3.right;
            movement.direction = dir.normalized;
        }
    }
}