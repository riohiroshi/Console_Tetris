using System;
using System.Threading;

namespace Tetris
{
    static class GameMenu
    {
        public static string tempName;

        public static void Selection()
        {
            GamePlay.SoundPlayLooping("\\sounds\\Title.wav");

            ConsoleKeyInfo key = new ConsoleKeyInfo();

            while (true)
            {
                while (!IsValidSelection(ref key))
                {
                    DrawTitle();
                    Console.ResetColor();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();

                    DrawSelection();

                    Thread.Sleep(200);
                    Console.Clear();
                }

                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        while (tempName == null)
                        {
                            DrawTitle();

                            Console.ResetColor();
                            Console.WriteLine();
                            Console.WriteLine();
                            Console.WriteLine();
                            Console.WriteLine();
                            Console.WriteLine();

                            EnterPlayerName();
                        }
                        GameLoop();
                        break;
                    case ConsoleKey.D2:
                        GamePlay.DrawRanking();
                        break;
                    case ConsoleKey.D3:
                        return;
                    default:
                        break;
                }
            }
        }

        private static void DrawTitle()
        {
            string[,] textT = {
                {"■■■■■  "},
                {"    ■      "},
                {"    ■      "},
                {"    ■      "},
                {"    ■      "} };

            string[,] textE ={
                {"■■■■  "},
                {"■        "},
                {"■■■    "},
                {"■        "},
                {"■■■■  "} };

            string[,] textR = {
                {"■■■    "},
                {"■    ■  "},
                {"■■■■  "},
                {"■  ■    "},
                {"■    ■  "} };

            string[,] textI = {
                {"■■■  "},
                {"  ■    "},
                {"  ■    "},
                {"  ■    "},
                {"■■■  "} };

            string[,] textS = {
                {"  ■■■"},
                {"■      "},
                {"  ■■  "},
                {"      ■"},
                {"■■■  "} };

            ConsoleColor color1 = Utility.GetRandomColor();
            ConsoleColor color2 = Utility.GetRandomColor();
            ConsoleColor color3 = Utility.GetRandomColor();
            ConsoleColor color4 = Utility.GetRandomColor();
            ConsoleColor color5 = Utility.GetRandomColor();
            ConsoleColor color6 = Utility.GetRandomColor();

            for (int i = 0; i < textT.GetLength(0); i++)
            {
                Console.SetCursorPosition(30, 10 + i);

                for (int j = 0; j < textT.GetLength(1); j++)
                {
                    Console.ForegroundColor = color1;
                    Console.Write(textT[i, j]);
                    Console.ForegroundColor = color2;
                    Console.Write(textE[i, j]);
                    Console.ForegroundColor = color3;
                    Console.Write(textT[i, j]);
                    Console.ForegroundColor = color4;
                    Console.Write(textR[i, j]);
                    Console.ForegroundColor = color5;
                    Console.Write(textI[i, j]);
                    Console.ForegroundColor = color6;
                    Console.Write(textS[i, j]);
                }
                Console.WriteLine();
            }
        }

        private static void DrawSelection()
        {
            string s1 = "1. Start";
            string s2 = "2. Score Board";
            string s3 = "3. Exit";

            Console.WriteLine(s1.PadLeft((Console.WindowWidth + s1.Length) / 2));
            Console.WriteLine(s2.PadLeft((Console.WindowWidth + s2.Length) / 2));
            Console.WriteLine(s3.PadLeft((Console.WindowWidth + s3.Length) / 2));
        }

        private static bool IsValidSelection(ref ConsoleKeyInfo key)
        {
            bool isValid = false;

            if (!Console.KeyAvailable)
            {
                isValid = false;
            }
            else
            {
                key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.D2:
                    case ConsoleKey.D3:
                        isValid = true;
                        break;

                    default:
                        isValid = false;
                        break;
                }
            }

            return isValid;
        }

        private static void EnterPlayerName()
        {
            string s = "Please Enter Your Name:";
            Console.WriteLine(s.PadLeft((Console.WindowWidth + s.Length) / 2));
            while (Console.KeyAvailable)
            {
                Console.SetCursorPosition(Console.WindowWidth / 2 - 2, 22);

                tempName = Console.ReadLine();
            }
            Thread.Sleep(200);
            Console.Clear();
        }

        private static void GameLoop()
        {
            while (true)
            {
                GamePlay.Init();

                GamePlay.DrawBorder();
                GamePlay.DrawStat();

                GamePlay.NextTet = new Tetromino();
                GamePlay.Tet = GamePlay.NextTet;
                TetrominoManager.Instance.Spawn(GamePlay.Tet);
                GamePlay.NextTet = new Tetromino();
                GamePlay.DrawNextTetromino();

                GamePlay.Update();

                Console.Clear();

                GamePlay.DrawStat();

                string temp = "Game Over";
                Console.WriteLine(temp.PadLeft((50 + temp.Length) / 2));
                temp = "Replay? (Y/N)";
                Console.WriteLine(temp.PadLeft((50 + temp.Length) / 2));
                Console.SetCursorPosition(24, 7);
                string input = Console.ReadLine();
                Console.Clear();

                if (input == "y" || input == "Y")
                {
                    GC.Collect();
                }
                else
                {
                    tempName = null;
                    GamePlay.SoundPlayLooping("\\sounds\\Title.wav");
                    return;
                }
            }
        }
    }
}
