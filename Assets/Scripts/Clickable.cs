using System;
using Enum;
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

    void Update()
    {
    }

    private void OnMouseDown()
    {
        if (BoxProperties.BoxType == BoxType.None && !ticTacToe.GameOver)
        {
            var renderer = gameObject.GetComponent<SpriteRenderer>();
            renderer.color =new Color(0.6f,0.6f,0.6f);
        }
    }

    private void OnMouseUp()
    {
        if (BoxProperties.BoxType == BoxType.None && !ticTacToe.GameOver)
        {
            if (ticTacToe.CurrentPlayer == BoxType.X)
            {
                BoxProperties.BoxType = BoxType.X;
                var renderer = gameObject.GetComponent<SpriteRenderer>();
                renderer.sprite = TictactoeSprites.GetXMoveSprite;
                renderer.color = new Color(1,1,1);
            }
            else
            {
                BoxProperties.BoxType = BoxType.O;
                var renderer = gameObject.GetComponent<SpriteRenderer>();
                renderer.sprite = TictactoeSprites.GetOMoveSprite;
                renderer.color = new Color(1,1,1);
            }

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