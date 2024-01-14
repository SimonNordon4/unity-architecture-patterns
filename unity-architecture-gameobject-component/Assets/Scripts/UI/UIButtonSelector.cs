using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GameObjectComponent.UI
{
    [DefaultExecutionOrder(10)]
    public class UIButtonSelector : MonoBehaviour
    {
        [SerializeField] private SelectorDirection direction;
        [SerializeField] private List<Button> buttons = new();
        [SerializeField] private bool getButtonsDynamic = false;
        private int _currentButtonIndex;
        
        [SerializeField]private Key upKey = Key.W;
        [SerializeField]private Key altUpKey = Key.UpArrow;
        [SerializeField]private Key downKey = Key.S;
        [SerializeField]private Key altDownKey = Key.DownArrow;
        [SerializeField]private Key leftKey = Key.A;
        [SerializeField]private Key altLeftKey = Key.LeftArrow;
        [SerializeField]private Key rightKey = Key.D;
        [SerializeField]private Key altRightKey = Key.RightArrow;

        private void OnEnable()
        {
            Debug.Log("Button Selector Enabled");
            GetButtonsDynamic();
           // select first button
           _currentButtonIndex = 0;
           buttons[_currentButtonIndex].Select();
        }

        private void OnDisable()
        {
            _currentButtonIndex = 0;
        }

        private enum SelectorDirection
        {
            Vertical,
            Horizontal
        }

        public void GetButtonsDynamic()
        {
            if (!getButtonsDynamic) return;
            buttons.Clear();
            buttons.AddRange(GetComponentsInChildren<Button>());
            if(direction == SelectorDirection.Vertical)
                buttons.Sort((a, b) => a.transform.position.y.CompareTo(b.transform.position.y));
            else
                buttons.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));
            Debug.Log("Buttons: " + buttons.Count);
        }

        private void Update()
        {
            if(direction == SelectorDirection.Vertical)
                SelectVertical();
            else
                SelectHorizontal();
        }

        private void SelectHorizontal()
        {
            if (Keyboard.current[leftKey].wasPressedThisFrame || Keyboard.current[altLeftKey].wasPressedThisFrame)
            {
                _currentButtonIndex--;
                if (_currentButtonIndex < 0) _currentButtonIndex = buttons.Count - 1;
                buttons[_currentButtonIndex].Select();
            }
            else if (Keyboard.current[rightKey].wasPressedThisFrame || Keyboard.current[altRightKey].wasPressedThisFrame)
            {
                _currentButtonIndex++;
                if (_currentButtonIndex > buttons.Count - 1) _currentButtonIndex = 0;
                buttons[_currentButtonIndex].Select();
            }
        }

        private void SelectVertical()
        {
            if (Keyboard.current[upKey].wasPressedThisFrame || Keyboard.current[altUpKey].wasPressedThisFrame)
            {
                _currentButtonIndex--;
                if (_currentButtonIndex < 0) _currentButtonIndex = buttons.Count - 1;
                buttons[_currentButtonIndex].Select();
            }
            else if (Keyboard.current[downKey].wasPressedThisFrame || Keyboard.current[altDownKey].wasPressedThisFrame)
            {
                _currentButtonIndex++;
                if (_currentButtonIndex > buttons.Count - 1) _currentButtonIndex = 0;
                buttons[_currentButtonIndex].Select();
            }
        }
    }
}