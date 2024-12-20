using System;
using DefaultNamespace;
using Enums;

namespace AI
{
    public class MiniMaxAI
    {
        private WinChecker winChecker;
        private int gameSize = 3;
        private BoxType playerSymbol;
        private BoxType aiSymbol;
        public MiniMaxAI(WinChecker winChecker)
        {
            this.winChecker = winChecker;
        }
        
        public void SetGameSettings(int size,BoxType playerSymbol)
        {
            gameSize = size; 
            this.playerSymbol = playerSymbol;
            aiSymbol = playerSymbol == BoxType.X ? BoxType.O : BoxType.X;
        }

        public int[] BestMoveAI(BoxType[,] board)
        {
            var bestMove = Array.Empty<int>();
            var bestScore = int.MinValue;
            var tempBoard = (BoxType[,])board.Clone();

            for (int i = 0; i < gameSize; i++)
            {
                for (int j = 0; j < gameSize; j++)
                {
                    if (board[i, j] == BoxType.None)
                    {
                        tempBoard[i, j] = aiSymbol;
                        int score = MiniMax(tempBoard, 0, false,int.MinValue, int.MaxValue);
                        tempBoard[i, j] = BoxType.None;
                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestMove = new[] { i, j };
                        }
                    }
                }
            }

            return bestMove;
        }

        private int MiniMax(BoxType[,] board, int depth, bool isMaximizing,int alpha,int beta)
        {
            var result = winChecker.CheckWinner(board, gameSize);
            
            if(depth == 3)
            {
                return 0;
            }
            
            if (result != BoxType.None)
            {
                return GetScore(result,depth);
            }

            if (isMaximizing)
            {
                int bestScore = int.MinValue;
                for (int i = 0; i < gameSize; i++)
                {
                    for (int j = 0; j < gameSize; j++)
                    {
                        if (board[i, j] == BoxType.None)
                        {
                            board[i, j] = aiSymbol;
                            int score = MiniMax(board, depth + 1, false,alpha, beta);
                            board[i, j] = BoxType.None;
                            bestScore = Math.Max(score, bestScore);
                            // alpha = Math.Max(alpha, score);
                            //
                            // if (beta <= alpha)
                            //     return bestScore;
                        }
                    }
                }

                return bestScore;
            }
            else
            {
                int bestScore = int.MaxValue;
                for (int i = 0; i < gameSize; i++)
                {
                    for (int j = 0; j < gameSize; j++)
                    {
                        if (board[i, j] == BoxType.None)
                        {
                            board[i, j] = playerSymbol;
                            int score = MiniMax(board, depth + 1, true,alpha, beta);
                            board[i, j] = BoxType.None;
                            bestScore = Math.Min(score, bestScore);
                            // beta = Math.Min(beta, score);
                            //
                            // if (beta <= alpha)
                            //     return bestScore;
                        }
                    }
                }

                return bestScore;
            }
        }

        private int GetScore(BoxType winner,int depth)
        {
            return winner switch
            {
                BoxType.Tie => 0,
                BoxType.X => aiSymbol == BoxType.X ? 10 - depth : depth - 10,
                BoxType.O => aiSymbol == BoxType.O ? 10 - depth : depth - 10,
            };
        }
    }
}