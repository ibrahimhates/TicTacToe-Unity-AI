using System.Collections;
using Enum;
using Helpers;
using UnityEngine;
using UnityEngine.UI;

public class TicTacToe : MonoBehaviour
{
    private Sprite EmptySprite;
    private int GameSize = 3;
    public GameObject GameBoardRef;
    public BoxType CurrentPlayer = BoxType.X;
    public bool GameOver = false;
    public bool IsRestartable = true;
    public Button StartGameButton;
    public Button QuitGameButton;
    public Dropdown GameSizeDropdown;
    public Dropdown PlayerSymbolDropdown;

    private void Start()
    {
        AddDropDownGameSizeOptions();
        StartGameButton.onClick.AddListener(StartGameListener);
        QuitGameButton.onClick.AddListener(QuitGameListener);
        EmptySprite = TictactoeSprites.GetEmptyBgSprite;
    }
    
    private void AddDropDownGameSizeOptions()
    {
        GameSizeDropdown.ClearOptions();
        for (int i = 3; i <= 25; i++)
        {
            if(i % 2 != 0)
                GameSizeDropdown.options.Add(new Dropdown.OptionData(i.ToString()));
        }
    }

    private void StartGameListener()
    {
        GameSize = int.Parse(GameSizeDropdown.options[GameSizeDropdown.value].text);
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
        if(!IsRestartable)
            return;
        GameSize = int.Parse(GameSizeDropdown.options[GameSizeDropdown.value].text);
        var allobjests = FindObjectsByType<Clickable>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
        foreach (var clickable in allobjests)
        {
            Destroy(clickable.gameObject);
        }
        StartCoroutine(DrawGameBoard());
    }


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

        for (int i = 0; i < GameSize; i++)
        {
            for (int j = 0; j < GameSize; j++)
            {
                yield return new WaitForSeconds(0.03f);
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