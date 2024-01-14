using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GameObjectComponent.UI
{
    public class UIButtonSelector : MonoBehaviour
    {
        private Button[] _buttons;
        private int _rowIndex = 0;
        private int _columnIndex = 0;
        private int _numberOfRows = 0;
        private int _numberOfColumns = 0;

        [SerializeField] private float rowTolerance = 100;
        [SerializeField] private float columnTolerance = 100;

        private List<List<Button>> _grid = new List<List<Button>>();

        private void OnEnable()
        {
            _buttons = GetComponentsInChildren<Button>();
            CalculateRows();
            CalculateColumns();
            // select the first button
            SelectButton();
        }

        private void CalculateRows()
        {
            // sort buttons by y position in descending order
            var sortedYButtons = _buttons.OrderByDescending(b => b.transform.position.y).ToList();

            List<Button> currentRow = new List<Button>();
            if (sortedYButtons.Count > 0)
            {
                currentRow.Add(sortedYButtons[0]);
                _grid.Add(currentRow);
                _numberOfRows++;
            }

            // if one button is greater than the _rowTolerance to the next button, add a row
            for (var i = 1; i < sortedYButtons.Count; i++)
            {
                var currentButton = sortedYButtons[i];
                var previousButton = sortedYButtons[i - 1];
                if (Mathf.Abs(currentButton.transform.position.y - previousButton.transform.position.y) > rowTolerance)
                {
                    currentRow = new List<Button>();
                    _grid.Add(currentRow);
                    _numberOfRows++;
                }

                currentRow.Add(currentButton);
            }

            Debug.Log($"Number of rows: {_numberOfRows}");
        }

        private void CalculateColumns()
        {
            foreach (var row in _grid)
            {
                // sort buttons in each row by x position
                row.Sort((b1, b2) => b1.transform.position.x.CompareTo(b2.transform.position.x));
            }

            _numberOfColumns = _grid.Max(row => row.Count);

            Debug.Log($"Number of columns: {_numberOfColumns}");
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                // Move up in the grid
                _rowIndex = Mathf.Max(_rowIndex - 1, 0);
                SelectButton();
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                // Move down in the grid
                _rowIndex = Mathf.Min(_rowIndex + 1, _numberOfRows - 1);
                SelectButton();
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                // Move left in the grid
                _columnIndex = Mathf.Max(_columnIndex - 1, 0);
                SelectButton();
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                // Move right in the grid
                _columnIndex = Mathf.Min(_columnIndex + 1, _numberOfColumns - 1);
                SelectButton();
            }
        }

        void SelectButton()
        {
            // Deselect all buttons
            foreach (var button in _buttons)
            {
                button.interactable = true;
            }


            // Select the button at the current index
            var buttonToSelect = _grid[_rowIndex][_columnIndex];
            Debug.Log("Selecting button at row " + _rowIndex + " column " + _columnIndex);
            Debug.Log("Button name: " + buttonToSelect.name);
            buttonToSelect.Select();
        }
    }
}