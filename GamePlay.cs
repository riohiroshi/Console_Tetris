using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Media;

namespace Tetris
{
    static class GamePlay
    {
        public const int BORDER_HEIGHT = 20;
        public const int BORDER_WIDTH = 10;

        public static int[,] grid;
        public static int[,] droppedTetrominoLocationGrid;
        public static ConsoleColor[,] droppedTetrominoColorGrid;

        private static int _offset = 50;

        private static SoundPlayer _bgm = new SoundPlayer();
        private static SoundPlayer _se = new SoundPlayer();

        private static string _square = "■";

        private static Stopwatch _dropTimer = new Stopwatch();

        private static int _dropTime;
        private static int _dropRate;


        private static Tetromino tet;
        private static Tetromino nextTet;

        private static ConsoleKeyInfo key;

        private static int highestScore;
        private static Player currentPlayer;
        private static List<Player> scoreRanking;


        public static Tetromino Tet { get { return tet; } set { tet = value; } }
        public static Tetromino NextTet { get { return nextTet; } set { nextTet = value; } }

        public static bool IsDropped { get; private set; }

        public static void SetIsDropped(bool value) => IsDropped = value;

        public static int HighestScore => highestScore;
        public static List<Player> ScoreRanking => scoreRanking;


        public static void Init()
        {
            grid = new int[BORDER_HEIGHT, BORDER_WIDTH];
            droppedTetrominoLocationGrid = new int[BORDER_HEIGHT, BORDER_WIDTH];
            droppedTetrominoColorGrid = new ConsoleColor[BORDER_HEIGHT, BORDER_WIDTH];

            _dropTimer = new Stopwatch();

            _dropTime = 300;
            _dropRate = 300;

            IsDropped = false;

            scoreRanking = new List<Player>();

            if (!GameSerializationManager.LoadFromFile())
            {
                highestScore = 0;
            }

            currentPlayer = new Player();

            if (GameMenu.tempName != null && GameMenu.tempName != "")
            {
                currentPlayer.playerName = GameMenu.tempName;
            }

            //currentPlayer.AddSkill(Skill.CreateDamageSkill("Boom", 2, 5, ConsoleColor.Red));

            _dropTimer.Start();
        }

        public static void Update()
        {
            PlayBGM();
            while (true)
            {
                _dropTime = (int)_dropTimer.ElapsedMilliseconds;
                if (_dropTime > _dropRate)
                {
                    _dropTime = 0;
                    _dropTimer.Restart();
                    TetrominoManager.Instance.Drop(tet);
                }

                if (IsDropped == true)
                {
                    tet = nextTet;
                    nextTet = new Tetromino();
                    TetrominoManager.Instance.Spawn(tet);
                    DrawNextTetromino();

                    IsDropped = false;
                }

                for (int j = 0; j < BORDER_WIDTH; j++)
                {
                    if (droppedTetrominoLocationGrid[0, j] == 1)
                    {
                        if (currentPlayer.score > highestScore)
                        {
                            highestScore = currentPlayer.score;
                        }
                        scoreRanking.Add(currentPlayer);
                        scoreRanking.Sort((Player player1, Player player2) => { return player1.score.CompareTo(player2.score); });
                        scoreRanking.Reverse();
                        if (scoreRanking.Count > 10)
                        {
                            scoreRanking.RemoveAt(10);
                        }

                        SoundPlayOnce("\\sounds\\Game_Over.wav");

                        GameSerializationManager.SaveToFile();
                        return;
                    }
                }

                Input();
                ClearBlock();
            }
        }

        private static void Input()
        {
            while (Console.KeyAvailable)
            {
                key = Console.ReadKey(true);

                if (CanMoveLeft())
                {
                    for (int i = 0; i < 4; i++)
                    {
                        tet.location[i][1] -= 1;
                    }
                    TetrominoManager.Instance.Update(tet);
                }
                else if (CanMoveRight())
                {
                    for (int i = 0; i < 4; i++)
                    {
                        tet.location[i][1] += 1;
                    }
                    TetrominoManager.Instance.Update(tet);
                }

                if (key.Key == ConsoleKey.DownArrow)
                {
                    while (!tet.IsSomethingBelow()) { TetrominoManager.Instance.Drop(tet); }
                }

                if (key.Key == ConsoleKey.UpArrow)
                {
                    TetrominoManager.Instance.Rotate(tet);
                    TetrominoManager.Instance.Update(tet);
                }

                return;
            }
        }

        private static void ClearBlock()
        {
            int combo = 0;
            for (int i = 0; i < BORDER_HEIGHT; i++)
            {
                int j;
                for (j = 0; j < BORDER_WIDTH; j++)
                {
                    if (droppedTetrominoLocationGrid[i, j] == 0)
                        break;
                }
                if (j == BORDER_WIDTH)
                {
                    currentPlayer.linesCleared++;
                    combo++;
                    for (j = 0; j < BORDER_WIDTH; j++)
                    {
                        for (int k = 0; k < Math.Max(1, 5 - currentPlayer.level); k++)
                        {
                            Flash(i, "▓");
                        }
                        Draw();
                        droppedTetrominoLocationGrid[i, j] = 0;
                        droppedTetrominoColorGrid[i, j] = ConsoleColor.Black;
                    }

                    int[,] newDroppedTetrominoeLocationGrid = new int[BORDER_HEIGHT, BORDER_WIDTH];
                    ConsoleColor[,] newDroppedTetrominoeColorGrid = new ConsoleColor[BORDER_HEIGHT, BORDER_WIDTH];

                    for (int k = 1; k < i; k++)
                    {
                        for (int l = 0; l < BORDER_WIDTH; l++)
                        {
                            newDroppedTetrominoeLocationGrid[k + 1, l] = droppedTetrominoLocationGrid[k, l];
                            newDroppedTetrominoeColorGrid[k + 1, l] = droppedTetrominoColorGrid[k, l];
                        }
                    }
                    for (int k = 1; k < i; k++)
                    {
                        for (int l = 0; l < BORDER_WIDTH; l++)
                        {
                            droppedTetrominoLocationGrid[k, l] = 0;
                            droppedTetrominoColorGrid[k, l] = ConsoleColor.Black;
                        }
                    }
                    for (int k = 0; k < BORDER_HEIGHT; k++)
                    {
                        for (int l = 0; l < BORDER_WIDTH; l++)
                        {
                            if (newDroppedTetrominoeLocationGrid[k, l] == 1)
                            {
                                droppedTetrominoLocationGrid[k, l] = 1;
                                droppedTetrominoColorGrid[k, l] = newDroppedTetrominoeColorGrid[k, l];
                            }
                        }
                    }
                }
            }

            currentPlayer.score += combo * ((50 + 10 * combo) * currentPlayer.level);

            if (highestScore < currentPlayer.score)
            {
                highestScore = currentPlayer.score;
                GameSerializationManager.SaveToFile();
            }

            if (combo > 0)
            {
                DrawStat();
            }

            if (currentPlayer.linesCleared >= currentPlayer.nextLevelLinesNum)
            {
                currentPlayer.level++;

                SoundPlayOnce("\\sounds\\LevelUp.wav");

                LevelUpFlash();

                PlayBGM();

                //currentPlayer.nextLevelLinesNum += 2;
                currentPlayer.nextLevelLinesNum = currentPlayer.level * (currentPlayer.level + 1) + 5;
            }

            _dropRate = 300 - 22 * currentPlayer.level;
        }

        private static void PlayBGM()
        {
            if (currentPlayer.level < 2)
            {
                SoundPlayLooping("\\sounds\\Slowest.wav");
                return;
            }
            if (currentPlayer.level < 4)
            {
                SoundPlayLooping("\\sounds\\Slower.wav");
                return;
            }
            if (currentPlayer.level < 6)
            {
                SoundPlayLooping("\\sounds\\Faster.wav");
                return;
            }
            else { SoundPlayLooping("\\sounds\\Fastest.wav"); }
        }
        public static void SoundPlayOnce(string location)
        {
            _bgm.SoundLocation = Environment.CurrentDirectory + location;
            _bgm.Play();
        }
        public static void SoundPlayLooping(string location)
        {
            _bgm.SoundLocation = Environment.CurrentDirectory + location;
            _bgm.PlayLooping();
        }

        private static bool CanMoveLeft() => key.Key == ConsoleKey.LeftArrow & !tet.IsSomethingLeft();
        private static bool CanMoveRight() => key.Key == ConsoleKey.RightArrow & !tet.IsSomethingRight();


        #region Draw
        public static void DrawStat()
        {
            Console.ResetColor();

            Console.SetCursorPosition(2 * BORDER_WIDTH + 5 + _offset, 0);
            Console.WriteLine("Level: " + currentPlayer.level);

            Console.SetCursorPosition(2 * BORDER_WIDTH + 5 + _offset, 1);
            Console.WriteLine("Score: " + currentPlayer.score);

            Console.SetCursorPosition(2 * BORDER_WIDTH + 5 + _offset, 2);
            Console.WriteLine("Lines Cleared: " + currentPlayer.linesCleared);

            Console.SetCursorPosition(2 * BORDER_WIDTH + 5 + _offset, 3);
            Console.WriteLine("Highest Score: " + highestScore);
        }

        public static void LevelUpFlash()
        {
            for (int i = 0; i < 10; i++)
            {
                //Console.Clear();

                Console.ForegroundColor = Utility.GetRandomColor();
                Console.SetCursorPosition(0, 25);
                string s1 = "|     ----- \\       / ----- |          |   | ----\\";
                string s2 = "|     |      \\     /  |     |          |   | |   |";
                string s3 = "|     ---     \\   /   ---   |          |   | ----/";
                string s4 = "|     |        \\ /    |     |          |   | |    ";
                string s5 = "----- -----     .     ----- -----      \\---/ |    ";

                Console.WriteLine(s1.PadLeft((Console.WindowWidth + s1.Length) / 2));
                Console.WriteLine(s2.PadLeft((Console.WindowWidth + s2.Length) / 2));
                Console.WriteLine(s3.PadLeft((Console.WindowWidth + s3.Length) / 2));
                Console.WriteLine(s4.PadLeft((Console.WindowWidth + s4.Length) / 2));
                Console.WriteLine(s5.PadLeft((Console.WindowWidth + s5.Length) / 2));

                Console.SetCursorPosition(2 * BORDER_WIDTH + 5 + _offset, 0);
                Console.WriteLine("Level: " + currentPlayer.level);
                Thread.Sleep(100);
            }
            Console.ResetColor();
            Console.Clear();

            DrawBorder();
            DrawStat();
            DrawNextTetromino();
            Draw();
        }

        public static void DrawNextTetromino()
        {
            for (int i = BORDER_WIDTH * 2 + 2; i < BORDER_WIDTH * 2 + 5 + 9; i++)
            {
                for (int j = 6; j < BORDER_WIDTH * 2; j++)
                {
                    Console.SetCursorPosition(i + _offset, j);
                    Console.Write("  ");
                }
            }

            for (int i = 0; i < nextTet.Shape.GetLength(0); i++)
            {
                for (int j = 0; j < nextTet.Shape.GetLength(1); j++)
                {
                    if (nextTet.Shape[i, j] == 1)
                    {
                        Console.ForegroundColor = nextTet.Color;
                        Console.SetCursorPosition(((BORDER_WIDTH - nextTet.Shape.GetLength(1)) / 2 + j) * 2 + BORDER_WIDTH * 2 + _offset, i + 7);
                        Console.Write(_square);
                    }
                }
            }
        }

        public static void Draw()
        {
            for (int i = 0; i < BORDER_HEIGHT; i++)
            {
                for (int j = 0; j < BORDER_WIDTH; j++)
                {
                    Console.SetCursorPosition(1 + 2 * j + _offset, i);
                    if (grid[i, j] == 1)
                    {
                        Console.ForegroundColor = tet.Color;
                        Console.Write(_square);
                    }
                    else if (droppedTetrominoLocationGrid[i, j] == 1)
                    {
                        Console.ForegroundColor = droppedTetrominoColorGrid[i, j];
                        Console.Write(_square);
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                }
            }
        }

        public static void DrawBorder()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            for (int i = 0; i < BORDER_HEIGHT; i++)
            {
                Console.SetCursorPosition(0 + _offset, i);
                Console.Write("*");
                Console.SetCursorPosition(2 * BORDER_WIDTH + 1 + _offset, i);
                Console.Write("*");
            }
            Console.SetCursorPosition(0 + _offset, BORDER_HEIGHT);
            for (int j = 0; j <= BORDER_WIDTH; j++)
            {
                Console.Write("*-");
            }
        }

        public static void Flash(int i, string s)
        {
            for (int j = 0; j < BORDER_WIDTH; j++)
            {
                if (droppedTetrominoLocationGrid[i, j] == 1)
                {
                    Console.SetCursorPosition(1 + 2 * j + _offset, i);
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(s);
                }
            }

            for (int j = 0; j < BORDER_WIDTH; j++)
            {
                if (droppedTetrominoLocationGrid[i, j] == 1)
                {
                    Console.SetCursorPosition(1 + 2 * j + _offset, i);
                    Console.ForegroundColor = droppedTetrominoColorGrid[i, j];
                    Console.Write(s);
                }
            }
        }

        public static void DrawRanking()
        {
            Init();

            SoundPlayLooping("\\sounds\\Ranking.wav");

            Console.WriteLine();

            string boardTitle0 = string.Format("╔════════════════════════════╗");
            string boardTitle1 = string.Format("║        Score Board         ║");
            string boardTitle2 = string.Format("╠══════╦═════════════╦═══════╣");
            string boardBottom0 = string.Format("╚══════╩═════════════╩═══════╝");

            Console.WriteLine(boardTitle0);
            Console.WriteLine(boardTitle1);
            Console.WriteLine(boardTitle2);

            for (int i = 0; i < scoreRanking.Count; i++)
            {
                string s = string.Format("║\t{0, -6}║\t{1,-18}║\t{2,-10}║", i + 1, scoreRanking[i].playerName, scoreRanking[i].score);
                Console.WriteLine(s);

            }

            Console.WriteLine(boardBottom0);

            Console.WriteLine();

            string temp = "Press Enter to return.";
            Console.WriteLine(temp.PadLeft((60 + temp.Length) / 2));
            Console.ReadLine();
            Console.Clear();

            SoundPlayLooping("\\sounds\\Title.wav");
        }

        #endregion
    }
}