using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Grid
{
    public class GridManager : MonoBehaviour
    {
        #region Constants
        private const ushort GRID_SIZE = 20;
        #endregion

        #region Variables
        private Word[] currentLevelWords;

        public Word[] wordsLevelOne;
        //private Word[] wordsLevelTwo;
        private char[,] _solutionGrid;

        private bool _isLevelCompleted;

        private GridCell[,] _cellObjects;

        public RectTransform gridContainer;

        [Tooltip("Prefab for grid cell")]
        public GameObject cellPrefab;

        [Tooltip("Prefab for empty grid cell")]
        public GameObject emptyCellPrefab;

        private Word _highlightedWord;
        private Word _typingWord;


        #endregion

        #region Grid initialisation
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _cellObjects = new GridCell[GRID_SIZE, GRID_SIZE];

            currentLevelWords = wordsLevelOne;

            InitializeGrids(currentLevelWords);
            DisplayGrid();
            AssociateCellsToWords(currentLevelWords);
        }

        private void InitializeGrids(Word[] wordsLevel)
        {
            _solutionGrid = new char[GRID_SIZE, GRID_SIZE];

            foreach (var data in wordsLevel)
                PlaceWordOnGrid(data, _solutionGrid);
        }

        private void DisplayGrid()
        {
            // Using Y first because of the Grid Layout Group arrangement
            for (ushort y = 0; y < GRID_SIZE; y++)
            {
                for (ushort x = 0; x < GRID_SIZE; x++)
                {
                    if (_solutionGrid[x, y] != '\0')
                    {
                        GameObject cellObj = Instantiate(cellPrefab, gridContainer);
                        GridCell cellScript = cellObj.GetComponent<GridCell>();

                        char correctLetter = _solutionGrid[x, y];

                        cellScript.Initialize(x, y, correctLetter, this);

                        _cellObjects[x, y] = cellScript;
                    }
                    else
                        Instantiate(emptyCellPrefab, gridContainer);
                }
            }
        }

        private void PlaceWordOnGrid(Word data, char[,] grid)
        {
            int x = data.startX;
            int y = data.startY;

            if (data.direction == Word.Direction.Horizontal)
                for (int i = 0; i < data.word.Length; i++)
                {
                    grid[x, y] = data.word[i];
                    x++;
                }

            else
                for (int i = 0; i < data.word.Length; i++)
                {
                    grid[x, y] = data.word[i];
                    y++;
                }
        }

        // This fonction allows to associate the GridCell objects to the Word data structure for the highlighting
        private void AssociateCellsToWords(Word[] words)
        {
            foreach (Word data in words)
            {
                int x = data.startX;
                int y = data.startY;

                data.cells = new List<GridCell>(data.word.Length);

                //data.cells.Clear();

                if (data.direction == Word.Direction.Horizontal)
                {
                    for (int i = 0; i < data.word.Length; i++)
                    {
                        if (_cellObjects[x, y] != null)
                            data.cells.Add(_cellObjects[x, y]);
                        x++;
                    }
                }
                else
                {
                    for (int i = 0; i < data.word.Length; i++)
                    {
                        if (_cellObjects[x, y] != null)
                            data.cells.Add(_cellObjects[x, y]);
                        y++;
                    }
                }
            }
        }
        #endregion

        #region Hover Size
        public void OnCellHovered(GridCell cell)
        {
            // Word or null
            var word = FindWordContainingCell(cell);
            if (word is null)
                return;
            SetHighlightedWord(word);
        }

        public void OnCellUnhovered(GridCell cell)
        {
            // Word or null
            var word = FindWordContainingCell(cell);
            if (word != null && _highlightedWord == word)
            {
                _highlightedWord.SetSize(false);
                ClearFocusWord(ref _highlightedWord);
            }
        }
        private void SetHighlightedWord(Word word)
        {
            if (_highlightedWord != null && _highlightedWord != word)
                _highlightedWord.SetSize(false);

            _highlightedWord = word;
            _highlightedWord.SetSize(true);
        }


        #endregion

        #region Value Changed
        public void OnCellValueChanged(GridCell cell)
        {
            _typingWord = FindWordContainingCell(cell);
            NextCell(cell);
            if (CheckIfWordIsCompleted(_typingWord))
            {
                Debug.Log("Word completed: " + _typingWord.word);
            }
        }

        // Fix the new words if we are already typing in a word
        private Word FindWordContainingCell(GridCell cell)
        {
            if (cell == null)
                return null;

            if (_typingWord != null && _typingWord.cells.Contains(cell))
                return _typingWord;

            // Prefer a word that starts at this cell — if found, make it the new typing word
            Word startMatch = null;
            foreach (Word w in currentLevelWords)
            {
                if (w.startX == cell.X && w.startY == cell.Y)
                {
                    if (startMatch == null)
                        startMatch = w;
                    else
                        // Case multiple words start on same cell
                        if (w.direction == Word.Direction.Horizontal)
                            startMatch = w;
                }

                if (startMatch != null)
                    return startMatch;
            }

            // Else search in all words
            // Might have an issue with end of the word, start of another word
            if (currentLevelWords != null)
                foreach (Word word in currentLevelWords)
                    if (word.cells != null && word.cells.Contains(cell))
                        return word;

            return null;
        }

        private void NextCell(GridCell currentCell)
        {
            if (_typingWord == null || currentCell == null)
                return;

            int index = _typingWord.cells.IndexOf(currentCell);
            if (index >= 0 && index < _typingWord.cells.Count - 1)
            {
                GridCell nextCell = _typingWord.cells[index + 1];
                if (nextCell != null)
                {
                    TMP_InputField input = nextCell.inputField;
                    if (input != null && String.IsNullOrEmpty(input.text))
                        input.Select();
                    // Skip two cells if the next one is already filled
                    else if (index + 2 < _typingWord.cells.Count)
                    {
                        TMP_InputField nextInput = _typingWord.cells[index + 2].inputField;
                        if (nextInput != null)
                            nextInput.Select();
                    }
                }
            }
        }

        private bool CheckIfWordIsCompleted(Word word)
        {
            foreach (GridCell cell in word.cells)
                if (cell == null || String.IsNullOrEmpty(cell.inputField.text))
                    return false;

            return true;
        }

        #endregion
        private void ClearFocusWord(ref Word focusWord)
        {
            if (focusWord != null)
                focusWord = null;
        }
    }
}
