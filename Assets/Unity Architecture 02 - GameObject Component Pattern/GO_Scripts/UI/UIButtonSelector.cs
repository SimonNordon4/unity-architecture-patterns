using System.Collections.Generic;
using UnityEngine;
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
        
        [SerializeField]private KeyCode upKey = KeyCode.W;
        [SerializeField]private KeyCode altUpKey = KeyCode.UpArrow;
        [SerializeField]private KeyCode downKey = KeyCode.S;
        [SerializeField]private KeyCode altDownKey = KeyCode.DownArrow;
        [SerializeField]private KeyCode leftKey = KeyCode.A;
        [SerializeField]private KeyCode altLeftKey = KeyCode.LeftArrow;
        [SerializeField]private KeyCode rightKey = KeyCode.D;
        [SerializeField]private KeyCode altRightKey = KeyCode.RightArrow;

        private void OnEnable()
        {
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
            if (Input.GetKeyDown(leftKey) || Input.GetKeyDown(altLeftKey))
            {
                _currentButtonIndex--;
                if (_currentButtonIndex < 0) _currentButtonIndex = buttons.Count - 1;
                var currentButton = buttons[_currentButtonIndex];
                if (currentButton != null)
                    currentButton.Select();
                else
                    buttons[0].Select();

            }
            else if (Input.GetKeyDown(rightKey) || Input.GetKeyDown(altRightKey))
            {
                _currentButtonIndex++;
                if (_currentButtonIndex > buttons.Count - 1) _currentButtonIndex = 0;
                var currentButton = buttons[_currentButtonIndex];
                if (currentButton != null)
                    currentButton.Select();
                else
                    buttons[0].Select();
            }
        }

        private void SelectVertical()
        {
            if (Input.GetKeyDown(upKey) || Input.GetKeyDown(altUpKey))
            {
                _currentButtonIndex--;
                if (_currentButtonIndex < 0) _currentButtonIndex = buttons.Count - 1;
                buttons[_currentButtonIndex].Select();
            }
            else if (Input.GetKeyDown(downKey) || Input.GetKeyDown(altDownKey))
            {
                _currentButtonIndex++;
                if (_currentButtonIndex > buttons.Count - 1) _currentButtonIndex = 0;
                buttons[_currentButtonIndex].Select();
            }
        }
    }
}