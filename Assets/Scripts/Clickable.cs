using System;
using System.Linq;
using Enums;
using Helpers;
using UnityEngine;

public class Clickable : MonoBehaviour
{
    public readonly BoxField BoxProperties = new();
    private TicTacToe ticTacToe;

    void Start()
    {
        BoxProperties.BoxType = BoxType.None;
        ticTacToe = FindFirstObjectByType<TicTacToe>();
    }

    private void OnMouseDown()
    {
        if (BoxProperties.BoxType == BoxType.None && !ticTacToe.GameOver)
        {
            var renderer = gameObject.GetComponent<SpriteRenderer>();
            renderer.color = new Color(0.7f, 0.7f, 0.7f);
        }
    }

    private void OnMouseUp()
    {
        if (BoxProperties.BoxType == BoxType.None
            && !ticTacToe.GameOver
            && ticTacToe.CurrentPlayer == ticTacToe.PlayerSymbol)
        {
            BoxProperties.BoxType = ticTacToe.CurrentPlayer;
            var renderer = gameObject.GetComponent<SpriteRenderer>();
            renderer.sprite = ticTacToe.PlayerSymbol == BoxType.X
                ? TictactoeSprites.GetXMoveSprite
                : TictactoeSprites.GetOMoveSprite;
            renderer.color = new Color(1, 1, 1);
            ticTacToe.Board[BoxProperties.Row, BoxProperties.Column] = ticTacToe.CurrentPlayer;

            var hasWinner = ticTacToe.CheckWinner();
            if (hasWinner)
            {
                Debug.Log($"Winner is {ticTacToe.CurrentPlayer.ToString()}");
                ticTacToe.GameOver = true;
            }

            if (!ticTacToe.GameOver)
            {
                ticTacToe.CurrentPlayer = ticTacToe.CurrentPlayer == BoxType.X ? BoxType.O : BoxType.X;
            }
        }
    }
}

public class BoxField
{
    public BoxType BoxType { get; set; } = BoxType.None;
    public int Row { get; set; }
    public int Column { get; set; }
}