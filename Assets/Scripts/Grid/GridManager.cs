using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Grid
{
    public class GridManager : MonoBehaviour
    {
        #region Enum
        public enum MoveDirection { Left, Right, Up, Down }

        #endregion

        #region Constants
        private const ushort GRID_NUMBER_HEIGHT = 14;
        private const ushort GRID_NUMBER_WIDTH = 20;
        #endregion

        #region Variables

        [Header("Words Data")]
        public Word[] wordsLevelOne;
        public Word[] wordsLevelTwo;
        public Word[] wordsLevelThree;
        public Word[] wordsLevelFour;


        // For testing purposes
        public Word[] test;

        private Word[] currentLevelWords;
        private char[,] _solutionGrid;

        [NonSerialized]
        private bool _isLevelCompleted;

        private GridCell[,] _cellObjects;

        [Header("Child")]
        public RectTransform gridContainer;

        [Header("Prefabs")]
        [Tooltip("Prefab for grid cell")]
        public GameObject cellPrefab;

        [Tooltip("Prefab for empty grid cell")]
        public GameObject emptyCellPrefab;

        private Word _highlightedWord;
        private Word _typingWord;

        public ClueTooltip tooltip;

        [Header("Audio Settings")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip soundWordCorrect;
        [SerializeField] private AudioClip soundLevelComplete;
        [SerializeField] private AudioClip soundError;

        [Header("Hint")]
        public Button buttonHint;
        public Image loadingImage;
        [SerializeField] private float buttonCooldown = 20f;

        #endregion

        #region Grid initialisation
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _cellObjects = new GridCell[GRID_NUMBER_WIDTH, GRID_NUMBER_HEIGHT];

            currentLevelWords = wordsLevelOne;
            //currentLevelWords = wordsLevelTwo;
            //currentLevelWords = test;

            InitializeGrids(currentLevelWords);
            DisplayGrid();
            AssociateCellsToWords(currentLevelWords);
        }

        private void InitializeGrids(Word[] wordsLevel)
        {
            _solutionGrid = new char[GRID_NUMBER_WIDTH, GRID_NUMBER_HEIGHT];

            foreach (var data in wordsLevel)
                PlaceWordOnGrid(data, _solutionGrid);
        }

        private void DisplayGrid()
        {
            // Using Y first because of the Grid Layout Group arrangement
            for (ushort y = 0; y < GRID_NUMBER_HEIGHT; y++)
            {
                for (ushort x = 0; x < GRID_NUMBER_WIDTH; x++)
                {
                    if (_solutionGrid[x, y] != '\0')
                    {
                        GameObject cellObj = Instantiate(cellPrefab, gridContainer);
                        GridCell cellScript = cellObj.GetComponent<GridCell>();

                        char correctLetter = _solutionGrid[x, y];
                        if (correctLetter == '-')
                        {
                            cellScript.Initialize(x, y, correctLetter, this);
                            cellScript.Fill(false);
                        }
                        else
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

        #region Focus monitoring
        // If no TMP_InputField is selected
        // we set _typingWord to null
        private void Update()
        {
            if (EventSystem.current == null)
                return;

            var current = EventSystem.current.currentSelectedGameObject;

            if (current == null)
            {
                if (_typingWord != null)
                    _typingWord = null;
                return;
            }

            // If no TMP_InputField, clear _typingWord
            if (current.GetComponentInParent<TMP_InputField>() == null)
            {
                if (_typingWord != null)
                    _typingWord = null;
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
            ShowClue(word);

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
            HideClue();
        }
        private void SetHighlightedWord(Word word)
        {
            if (_highlightedWord != null && _highlightedWord != word)
                _highlightedWord.SetSize(false);

            _highlightedWord = word;
            _highlightedWord.SetSize(true);
        }

        private void ClearFocusWord(ref Word focusWord)
        {
            if (focusWord != null)
                focusWord = null;
        }

        #endregion

        #region Value Changed
        public void OnCellValueChanged(GridCell cell, bool filled)
        {
            _typingWord = FindWordContainingCell(cell);
            if (filled)
            {
                NextCell(cell);
                if (CheckIfWordIsCompleted(_typingWord))
                {
                    //Debug.Log("All cells filled");
                    if (CheckIfWordIsCorrect(_typingWord))
                    {
                        //Debug.Log("Word completed correctly: " + _typingWord.word);
                        currentLevelWords[Array.IndexOf(currentLevelWords, _typingWord)].ValidateWord();

                        // Stylise the word
                        if (Array.TrueForAll(currentLevelWords, w => w.isCompleted))
                        {
                            _isLevelCompleted = true;
                            Debug.Log("Level Completed!");
                            PlaySfx(soundLevelComplete);
                        }
                        else
                            PlaySfx(soundWordCorrect);
                    }
                    else
                    {
                        RemoveWord(_typingWord);
                        MoveToFistCharacter(_typingWord);
                    }
                }
            }
            // If char deleted move to previous cell
            else
            {
                PreviousCell(cell);
            }
        }

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
                    if (!nextCell.isValidated)
                    {
                        TMP_InputField nextInput = nextCell.inputField;
                        if (nextInput != null)
                            nextInput.Select();
                    }
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

        private void PreviousCell(GridCell currentCell)
        {
            if (_typingWord == null || currentCell == null)
                return;
            int index = _typingWord.cells.IndexOf(currentCell);
            if (index > 0)
            {
                GridCell previousCell = _typingWord.cells[index - 1];
                if (previousCell != null)
                {
                    if(!previousCell.isValidated)
                        previousCell.inputField.Select();
                    // Skip two cells if the previous one is already filled
                    else if (index - 2 >= 0)
                    {
                        TMP_InputField prevInput = _typingWord.cells[index - 2].inputField;
                        if (prevInput != null)
                            prevInput.Select();
                    }
                }
            }
        }

        private bool CheckIfWordIsCompleted(Word word)
        {
            foreach (GridCell cell in word.cells)
                if (cell == null || !cell.filled)
                    return false;

            return true;
        }

        private bool CheckIfWordIsCorrect(Word word)
        {
            foreach (GridCell cell in word.cells)
                if (!cell.filled || cell.inputField.text[0] != cell.CorrectLetter)
                    return false;

            return true;
        }

        #endregion

        #region Wrong Word

        private void RemoveWord(Word word)
        {
            foreach (GridCell cell in word.cells)
            {
                cell.Clear();
            }
            audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
            PlaySfx(soundError);
            audioSource.pitch = 1.0f;
        }

        #endregion

        #region Move Requests

        public void OnCellMoveRequested(GridCell cell, MoveDirection movement)
        {
            if (cell == null)
                return;

            int firstX = cell.X;
            int firstY = cell.Y;
            int secondX = cell.X;
            int secondY = cell.Y;

            switch (movement)
            {
                case MoveDirection.Left:
                    firstX = Math.Max(0, cell.X - 1);
                    secondX = Math.Max(0, cell.X - 2);
                    break;
                case MoveDirection.Right:
                    firstX = Math.Min(GRID_NUMBER_WIDTH - 1, cell.X + 1);
                    secondX = Math.Min(GRID_NUMBER_WIDTH - 1, cell.X + 2);
                    break;
                case MoveDirection.Up:
                    firstY = Math.Max(0, cell.Y - 1);
                    secondY = Math.Max(0, cell.Y - 2);
                    break;
                case MoveDirection.Down:
                    firstY = Math.Min(GRID_NUMBER_HEIGHT - 1, cell.Y + 1);
                    secondY = Math.Min(GRID_NUMBER_HEIGHT - 1, cell.Y + 2);
                    break;
            }

            GridCell firstTarget = _cellObjects[firstX, firstY];

            if (firstTarget != null && firstTarget.isValidated)
            {
                GridCell secondTarget = _cellObjects[secondX, secondY];
                if (secondTarget != null && !secondTarget.isValidated)
                {
                    TMP_InputField input = secondTarget.inputField;
                    if (input != null)
                        input.Select();
                }
            }
            else
            {
                if (firstTarget != null && !firstTarget.isValidated)
                {
                    TMP_InputField input = firstTarget.inputField;
                    if (input != null)
                        input.Select();
                }
            }
        }

        private void MoveToFistCharacter(Word word)
        {
            if (word == null || word.cells.Count == 0)
                return;

            GridCell firstCell = word.cells[0];
            if (firstCell != null && !firstCell.isValidated)
            {
                TMP_InputField input = firstCell.inputField;
                if (input != null)
                    input.Select();
            }
            // Move to the second cell if the first is validated
            else if (firstCell != null)
            {
                GridCell secondCell = word.cells[1];
                if (secondCell != null)
                {
                    TMP_InputField input = secondCell.inputField;
                    if (input != null)
                        input.Select();
                }
            }
        }

        #endregion

        #region Clue Management

        private void ShowClue(Word word)
        {
            if (word != null)
                tooltip.ShowClue(word.clue);
        }

        private void HideClue()
        {
            tooltip.Hide();
        }

        #endregion

        #region Sound Management

        private void PlaySfx(AudioClip clip)
        {
            if (clip != null && audioSource != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }

        #endregion

        #region Hint
        public void ButtonClick_GiveLetter()
        {
            GiveRandomLetter();
            StartCoroutine(CooldownRoutine());
        }

        private void GiveRandomLetter()
        {
            List<Word> incompleteWords = new();
            foreach (var word in currentLevelWords)
                if (!word.isCompleted)
                    incompleteWords.Add(word);

            if (incompleteWords.Count == 0)
                return;
            Word randomWord = incompleteWords[UnityEngine.Random.Range(0, incompleteWords.Count)];
            List<GridCell> unfilledCells = new();
            foreach (var cell in randomWord.cells)
                if (!cell.filled)
                    unfilledCells.Add(cell);

            if (unfilledCells.Count == 0)
                return;
            GridCell randomCell = unfilledCells[UnityEngine.Random.Range(0, unfilledCells.Count)];
            randomCell.Fill();
        }

        private IEnumerator CooldownRoutine()
        {
            buttonHint.interactable = false;
            float timer = buttonCooldown;

            while (timer > 0)
            {
                timer -= Time.deltaTime;
                loadingImage.fillAmount = timer / buttonCooldown;
                yield return null;
            }
            loadingImage.fillAmount = 1f;
            buttonHint.interactable = true;
        }

        #endregion
    }
}
