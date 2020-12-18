using System;
using System.Collections.Generic;
using System.Linq;

namespace Tetris
{
    class Tetromino
    {
        public bool IsErect = false;

        private static int[,] _tetrominoI = new int[1, 4] { { 1, 1, 1, 1 } };
        private static int[,] _tetrominoO = new int[2, 2] { { 1, 1 }, { 1, 1 } };
        private static int[,] _tetrominoT = new int[2, 3] { { 0, 1, 0 }, { 1, 1, 1 } };
        private static int[,] _tetrominoS = new int[2, 3] { { 0, 1, 1 }, { 1, 1, 0 } };
        private static int[,] _tetrominoZ = new int[2, 3] { { 1, 1, 0 }, { 0, 1, 1 } };
        private static int[,] _tetrominoJ = new int[2, 3] { { 1, 0, 0 }, { 1, 1, 1 } };
        private static int[,] _tetrominoL = new int[2, 3] { { 0, 0, 1 }, { 1, 1, 1 } };

        private static List<int[,]> _tetrominoes = new List<int[,]>() { _tetrominoI, _tetrominoO, _tetrominoT, _tetrominoS, _tetrominoZ, _tetrominoJ, _tetrominoL };

        public static int[,] TetrominoI => _tetrominoI;
        public static int[,] TetrominoO => _tetrominoO;
        public static int[,] TetrominoS => _tetrominoS;

        public List<int[]> location = new List<int[]>();

        private int[,] _shape;

        public int[,] Shape => _shape;
        public ConsoleColor Color { get; private set; }


        public Tetromino()
        {
            _shape = _tetrominoes[Utility.GetRandomShape()];
            Color = Utility.GetRandomColor();
        }

        public bool IsSomethingBelow()
        {
            for (int i = 0; i < 4; i++)
            {
                var belowY = location[i][0] + 1;
                var x = location[i][1];

                if (IsAtBottom(belowY) || IsOtherTetrominosBelow(belowY, x)) { return true; }
            }
            return false;
        }

        public bool? IsOverlayBelow(List<int[]> location)
        {
            List<int> ycoords = new List<int>();

            for (int i = 0; i < 4; i++)
            {
                var y = location[i][0];
                var x = location[i][1];

                ycoords.Add(y);

                if (IsAtBottom(y)) { return true; }

                if (y < 0) { return null; }
                if (x < 0) { return null; }
                if (x > GamePlay.BORDER_WIDTH - 1) { return null; }
            }

            for (int i = 0; i < 4; i++)
            {
                if (ycoords.Max() - ycoords.Min() == 3)
                {
                    if (ycoords.Max() == location[i][0] | ycoords.Max() - 1 == location[i][0])
                    {
                        if (GamePlay.droppedTetrominoLocationGrid[location[i][0], location[i][1]] == 1) { return true; }
                    }
                }
                else
                {
                    if (ycoords.Max() == location[i][0])
                    {
                        if (GamePlay.droppedTetrominoLocationGrid[location[i][0], location[i][1]] == 1) { return true; }
                    }
                }
            }

            return false;
        }

        public bool IsSomethingLeft()
        {
            for (int i = 0; i < 4; i++)
            {
                var y = location[i][0];
                var x = location[i][1];
                var leftX = location[i][1] - 1;

                if (IsAtLeft(x)) { return true; }
                if (IsOtherTetrominosLeft(y, leftX)) { return true; }
            }
            return false;
        }

        public bool? IsOverlayLeft(List<int[]> location)
        {
            List<int> xcoords = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                xcoords.Add(location[i][1]);
                if (location[i][1] < 0) { return true; }

                if (location[i][1] > GamePlay.BORDER_WIDTH - 1) { return false; }

                if (location[i][0] >= GamePlay.BORDER_HEIGHT) { return null; }

                if (location[i][0] < 0) { return null; }
            }
            for (int i = 0; i < 4; i++)
            {
                if (xcoords.Max() - xcoords.Min() == 3)
                {
                    if (xcoords.Min() == location[i][1] | xcoords.Min() + 1 == location[i][1])
                    {
                        if (GamePlay.droppedTetrominoLocationGrid[location[i][0], location[i][1]] == 1)
                        {
                            return true;
                        }
                    }

                }
                else
                {
                    if (xcoords.Min() == location[i][1])
                    {
                        if (GamePlay.droppedTetrominoLocationGrid[location[i][0], location[i][1]] == 1)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool IsSomethingRight()
        {
            for (int i = 0; i < 4; i++)
            {
                if (location[i][1] == GamePlay.BORDER_WIDTH - 1)
                {
                    return true;
                }
                else if (GamePlay.droppedTetrominoLocationGrid[location[i][0], location[i][1] + 1] == 1)
                {
                    return true;
                }
            }
            return false;
        }

        public bool? IsOverlayRight(List<int[]> location)
        {
            List<int> xcoords = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                xcoords.Add(location[i][1]);
                if (location[i][1] > GamePlay.BORDER_WIDTH - 1)
                {
                    return true;
                }
                if (location[i][1] < 0)
                {
                    return false;
                }
                if (location[i][0] >= GamePlay.BORDER_HEIGHT)
                    return null;
                if (location[i][0] < 0)
                    return null;
            }
            for (int i = 0; i < 4; i++)
            {
                if (xcoords.Max() - xcoords.Min() == 3)
                {
                    if (xcoords.Max() == location[i][1] | xcoords.Max() - 1 == location[i][1])
                    {
                        if (GamePlay.droppedTetrominoLocationGrid[location[i][0], location[i][1]] == 1)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    if (xcoords.Max() == location[i][1])
                    {
                        if (GamePlay.droppedTetrominoLocationGrid[location[i][0], location[i][1]] == 1)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool IsAtBottom(int y) => y >= GamePlay.BORDER_HEIGHT;
        private bool IsOtherTetrominosBelow(int y, int x) => GamePlay.droppedTetrominoLocationGrid[y, x] == 1;

        private bool IsAtLeft(int x) => x == 0;
        private bool IsOtherTetrominosLeft(int y, int x) => GamePlay.droppedTetrominoLocationGrid[y, x] == 1;
    }
}