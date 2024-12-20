using System;
using System.Collections;
using System.Linq;
using AI;
using DefaultNamespace;
using Enums;
using Helpers;
using UnityEngine;
using UnityEngine.UI;

public class TicTacToe : MonoBehaviour
{
    private Sprite EmptySprite;
    private int GameSize = 3;
    public GameObject GameBoardRef;
    public BoxType CurrentPlayer = BoxType.X;
    public bool GameOver = true;
    public bool IsRestartable = true;
    public Button StartGameButton;
    public Button QuitGameButton;
    public Dropdown GameSizeDropdown;
    public Dropdown PlayerSymbolDropdown;
    public BoxType[,] Board;
    public BoxType PlayerSymbol { get; private set; }
    public MiniMaxAI MiniMaxAI { get; private set; }

    private void Start()
    {
        AddDropDownGameSizeOptions();
        StartGameButton.onClick.AddListener(StartGameListener);
        QuitGameButton.onClick.AddListener(QuitGameListener);
        EmptySprite = TictactoeSprites.GetEmptyBgSprite;
        MiniMaxAI = new MiniMaxAI(new WinChecker());
    }

    private void Update()
    {
        if (!GameOver)
        {
            if (CurrentPlayer != PlayerSymbol)
            {
                var aiBestMove = MiniMaxAI.BestMoveAI(Board);
                var aiRow = aiBestMove[0];
                var aiCol = aiBestMove[1];
                var allBoxes =
                    FindObjectsByType<Clickable>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
                var aiBox = allBoxes.First(box => box.BoxProperties.Row == aiRow && box.BoxProperties.Column == aiCol);
                aiBox.BoxProperties.BoxType = CurrentPlayer;
                aiBox.GetComponent<SpriteRenderer>().sprite = PlayerSymbol == BoxType.X 
                    ? TictactoeSprites.GetOMoveSprite
                    : TictactoeSprites.GetXMoveSprite;
                
                Board[aiRow, aiCol] = CurrentPlayer;
                CurrentPlayer = CurrentPlayer == BoxType.X ? BoxType.O : BoxType.X;

                var hasWinner = CheckWinner();
                if (hasWinner)
                {
                    Debug.Log($"Winner is {CurrentPlayer.ToString()}");
                    GameOver = true;
                }
            }
        }
    }

    private void AddDropDownGameSizeOptions()
    {
        GameSizeDropdown.ClearOptions();
        for (int i = 3; i <= 25; i++)
        {
            if (i % 2 != 0)
                GameSizeDropdown.options.Add(new Dropdown.OptionData(i.ToString()));
        }
    }

    private void StartGameListener()
    {
        if(PlayerSymbolDropdown.value == 0)
            return;
        
        var dropDownValue = PlayerSymbolDropdown.options[PlayerSymbolDropdown.value].text;
        PlayerSymbol  = (BoxType)Enum.Parse(typeof(BoxType),dropDownValue);
        
        GameSize = int.Parse(GameSizeDropdown.options[GameSizeDropdown.value].text);
        
        MiniMaxAI.SetGameSettings(GameSize, PlayerSymbol);
        StartCoroutine(DrawGameBoard());
        StartGameButton.onClick.RemoveAllListeners();
        StartGameButton.onClick.AddListener(RestartGameListener);
        StartGameButton.GetComponentInChildren<Text>().text = "Restart Game";
    }

    private void QuitGameListener()
    {
        Application.Quit();
    }

    private void RestartGameListener()
    {
        if (!IsRestartable)
            return;
        
        if(PlayerSymbolDropdown.value == 0)
            return;
        
        var dropDownValue = PlayerSymbolDropdown.options[PlayerSymbolDropdown.value].text;
        PlayerSymbol  = (BoxType)Enum.Parse(typeof(BoxType),dropDownValue);
        
        GameSize = int.Parse(GameSizeDropdown.options[GameSizeDropdown.value].text);
        MiniMaxAI.SetGameSettings(GameSize, PlayerSymbol);
        
        var allobjests = FindObjectsByType<Clickable>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
        foreach (var clickable in allobjests)
        {
            Destroy(clickable.gameObject);
        }

        Board = null;
        CurrentPlayer = BoxType.X;
        
        StartCoroutine(DrawGameBoard());
    }


    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator DrawGameBoard()
    {
        GameOver = true;
        IsRestartable = false;
        var startPosition = new Vector2(0, 3f);

        var defaultGameSize = 3f;
        var defaultSquareSize = 1f;

        var ratio1 = defaultGameSize * defaultGameSize;
        var ratio2 = GameSize * defaultGameSize;

        var squareSize = (ratio1 / ratio2);

        var offsetPosition = (defaultSquareSize - squareSize) + ((defaultSquareSize - squareSize) / 2);

        var spacing = squareSize + squareSize;
        var topLeft = new Vector2(startPosition.x - offsetPosition, startPosition.y + offsetPosition);

        var waitSeconds = 0.3f / GameSize;
        Board = new BoxType[GameSize, GameSize];
        for (int i = 0; i < GameSize; i++)
        {
            for (int j = 0; j < GameSize; j++)
            {
                yield return new WaitForSeconds(waitSeconds);
                var x = topLeft.x + j * (squareSize + spacing);
                var y = topLeft.y - i * (squareSize + spacing);

                var square = Instantiate(GameBoardRef, new Vector3(x, y, 0), Quaternion.identity);
                square.name = $"Square_{i}_{j}";

                var renderer = square.GetComponent<SpriteRenderer>();
                if (renderer == null)
                {
                    renderer = square.AddComponent<SpriteRenderer>();
                }

                renderer.sprite = EmptySprite;
                square.transform.localScale = new Vector3(squareSize, squareSize, 0);

                var clickable = square.GetComponent<Clickable>();
                clickable.BoxProperties.Row = i;
                clickable.BoxProperties.Column = j;
                Board[i, j] = BoxType.None;
            }
        }

        GameOver = false;
        IsRestartable = true;
    }

    public bool CheckWinner()
    {
        var allBoxes = FindObjectsByType<Clickable>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);

        for (int i = 0; i < GameSize; i++)
        {
            if (CheckLine(allBoxes, i, 0, 0, 1))
            {
                HighlightWinningLine(allBoxes, i, 0, 0, 1);
                return true;
            }
        }

        for (int j = 0; j < GameSize; j++)
        {
            if (CheckLine(allBoxes, 0, j, 1, 0))
            {
                HighlightWinningLine(allBoxes, 0, j, 1, 0);
                return true;
            }
        }

        if (CheckLine(allBoxes, 0, 0, 1, 1))
        {
            HighlightWinningLine(allBoxes, 0, 0, 1, 1);
            return true;
        }

        if (CheckLine(allBoxes, 0, GameSize - 1, 1, -1))
        {
            HighlightWinningLine(allBoxes, 0, GameSize - 1, 1, -1);
            return true;
        }
        
        int openSpots = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (Board[i, j] == BoxType.None)
                {
                    openSpots++;
                }
            }
        }

        if (openSpots == 0)
        {
            return true;
        }

        return false;
    }

    private bool CheckLine(Clickable[] allBoxes, int startRow, int startCol, int rowIncrement, int colIncrement)
    {
        var firstMark = GetMark(allBoxes, startRow, startCol);
        if (firstMark == BoxType.None) return false;

        for (int i = 1; i < GameSize; i++)
        {
            int row = startRow + i * rowIncrement;
            int col = startCol + i * colIncrement;

            if (GetMark(allBoxes, row, col) != firstMark)
            {
                return false;
            }
        }

        return true;
    }

    private void HighlightWinningLine(Clickable[] allBoxes, int startRow, int startCol, int rowIncrement,
        int colIncrement)
    {
        for (int i = 0; i < GameSize; i++)
        {
            int row = startRow + i * rowIncrement;
            int col = startCol + i * colIncrement;

            foreach (var box in allBoxes)
            {
                if (box.BoxProperties.Row == row && box.BoxProperties.Column == col)
                {
                    SpriteRenderer renderer = box.GetComponent<SpriteRenderer>();
                    if (renderer != null)
                    {
                        renderer.sprite = box.BoxProperties.BoxType == BoxType.X
                            ? TictactoeSprites.GetXWinSprite
                            : TictactoeSprites.GetOWinSprite;
                    }
                }
            }
        }
    }

    private BoxType GetMark(Clickable[] allBoxes, int row, int col)
    {
        foreach (var box in allBoxes)
        {
            if (box.BoxProperties.Row == row && box.BoxProperties.Column == col)
            {
                return box.BoxProperties.BoxType;
            }
        }

        return BoxType.None;
    }
}