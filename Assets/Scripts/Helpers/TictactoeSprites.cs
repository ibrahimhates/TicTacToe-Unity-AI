using UnityEngine;

namespace Helpers
{
    public static class TictactoeSprites
    {
        private static string empty_bg = "empty_bg";
        private static string o_move = "o_move";
        private static string x_move = "x_move";
        private static string o_win = "o_win";
        private static string x_win = "x_win";
        
        public static Sprite GetEmptyBgSprite => Resources.Load<Sprite>(empty_bg);
        public static Sprite GetOMoveSprite => Resources.Load<Sprite>(o_move);
        public static Sprite GetXMoveSprite => Resources.Load<Sprite>(x_move);
        public static Sprite GetOWinSprite => Resources.Load<Sprite>(o_win);
        public static Sprite GetXWinSprite => Resources.Load<Sprite>(x_win);
    }
}