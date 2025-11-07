using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridSize = 10;
    public Word[] words;
    private char[,] solutionGrid;
    private char[,] playerGrid;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeGrids();
        //DisplayGrid();
    }

    void InitializeGrids()
    {
        solutionGrid = new char[gridSize, gridSize];
        playerGrid = new char[gridSize, gridSize];

        // Remplir la grille de solution
        foreach (var data in words)
        {
            PlaceWordOnGrid(data, solutionGrid);
        }

        // Initialiser la grille du joueur avec des espaces ou des caractères de masquage
        // (La logique de masquage est souvent gérée par l'UI, pas par le tableau de char)
    }

    private void PlaceWordOnGrid(Word data, char[,] grid)
    {
        int x = data.startX;
        int y = data.startY;
        for (int i = 0; i < data.word.Length; i++)
        {
            grid[x, y] = data.word[i];
            if (data.direction == Word.Direction.Horizontal)
            {
                x++;
            }
            else
            {
                y++;
            }
        }
    }
}
