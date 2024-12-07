using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class UIWasdButtonSelector : MonoBehaviour
    {
        public enum Direction
        {
            Vertical,
            Horizontal
        }

        public List<Button> buttons = new();
        private Button _selectedButton;
        private int _selectedIndex = 0;

        public bool startSelected = true;

        public Direction direction = Direction.Vertical;

        private KeyCode _upKeyCode = KeyCode.W;
        private KeyCode _downKeyCode = KeyCode.S;

        private void OnEnable()
        {
            if (direction == Direction.Horizontal)
            {
                _upKeyCode = KeyCode.A;
                _downKeyCode = KeyCode.D;
            }

            else if (direction == Direction.Vertical)
            {
                _upKeyCode = KeyCode.W;
                _downKeyCode = KeyCode.S;
            }

            if (buttons.Count > 1 && startSelected) _selectedButton = buttons[0];
        }

        private void Update()
        {
            if (Input.GetKeyDown(_upKeyCode))
            {
                if (_selectedButton == null)
                {
                    _selectedButton = buttons[0];
                    _selectedButton.Select();
                    return;
                }

                _selectedIndex--;
                if (_selectedIndex < 0)
                    _selectedIndex = buttons.Count - 1;
                _selectedButton = buttons[_selectedIndex];
                _selectedButton.Select();
            }

            if (Input.GetKeyDown(_downKeyCode)) // This should check for downKeyCode, not upKeyCode
            {
                if (_selectedButton == null)
                {
                    _selectedButton = buttons[^1];
                    _selectedButton.Select();
                    return;
                }

                _selectedIndex++;
                if (_selectedIndex >= buttons.Count)
                    _selectedIndex = 0;
                _selectedButton = buttons[_selectedIndex];
                _selectedButton.Select();
                Debug.Log("Selecting Button: " + _selectedButton.name + " at index: " + _selectedIndex +
                          " in list of size: " + buttons.Count);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_selectedButton == null) return;

                Debug.Log("Invoking button: " + _selectedButton.name);
                _selectedButton.onClick.Invoke();
            }
        }
    }
}