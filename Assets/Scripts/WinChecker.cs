using Enums;

namespace DefaultNamespace
{
    public class WinChecker
    {
        private int gameSize;
        public BoxType CheckWinner(BoxType[,] board, int boardSize)
        {
            gameSize = boardSize;
            for (int i = 0; i < gameSize; i++)
            {
                var first = board[i, 0];
                if (first != BoxType.None && IsRowEqual(board, i, first))
                    return first;
            }

            for (int i = 0; i < gameSize; i++)
            {
                var first = board[0, i];
                if (first != BoxType.None && IsColumnEqual(board, i, first))
                    return first;
            }

            var diagonal1First = board[0, 0];
            if (diagonal1First != BoxType.None && IsDiagonal1Equal(board, diagonal1First))
                return diagonal1First;

            var diagonal2First = board[0, gameSize - 1];
            if (diagonal2First != BoxType.None && IsDiagonal2Equal(board, diagonal2First))
                return diagonal2First;

            int openSpots = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == BoxType.None)
                    {
                        openSpots++;
                    }
                }
            }

            if (openSpots == 0)
            {
                return BoxType.Tie;
            }

            return BoxType.None;
        }

        private bool IsRowEqual(BoxType[,] board, int row, BoxType player)
        {
            for (int col = 0; col < gameSize; col++)
            {
                if (board[row, col] != player)
                    return false;
            }

            return true;
        }

        private bool IsColumnEqual(BoxType[,] board, int col, BoxType player)
        {
            for (int row = 0; row < gameSize; row++)
            {
                if (board[row, col] != player)
                    return false;
            }

            return true;
        }

        private bool IsDiagonal1Equal(BoxType[,] board, BoxType player)
        {
            for (int i = 0; i < gameSize; i++)
            {
                if (board[i, i] != player)
                    return false;
            }

            return true;
        }

        private bool IsDiagonal2Equal(BoxType[,] board, BoxType player)
        {
            for (int i = 0; i < gameSize; i++)
            {
                if (board[i, gameSize - 1 - i] != player)
                    return false;
            }

            return true;
        }
    }
}