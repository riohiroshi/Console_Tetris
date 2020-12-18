using System;
using System.Collections.Generic;

namespace Tetris
{
    class TetrominoManager
    {
        private static TetrominoManager _instance;
        public static TetrominoManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TetrominoManager();
                }
                return _instance;
            }
        }

        public void Spawn(Tetromino tetrominoe)
        {
            for (int i = 0; i < tetrominoe.Shape.GetLength(0); i++)
            {
                for (int j = 0; j < tetrominoe.Shape.GetLength(1); j++)
                {
                    if (tetrominoe.Shape[i, j] == 1)
                    {
                        tetrominoe.location.Add(new int[] { i, (GamePlay.BORDER_WIDTH - tetrominoe.Shape.GetLength(1)) / 2 + j });
                    }
                }
            }
            Update(tetrominoe);
        }

        public void Drop(Tetromino tetrominoe)
        {
            if (tetrominoe.IsSomethingBelow())
            {
                for (int i = 0; i < 4; i++)
                {
                    GamePlay.droppedTetrominoLocationGrid[tetrominoe.location[i][0], tetrominoe.location[i][1]] = 1;
                    GamePlay.droppedTetrominoColorGrid[tetrominoe.location[i][0], tetrominoe.location[i][1]] = tetrominoe.Color - 8;
                }

                GamePlay.SetIsDropped(true);
                Console.Beep();
            }
            else
            {
                for (int numCount = 0; numCount < 4; numCount++)
                {
                    tetrominoe.location[numCount][0] += 1;
                }
                Update(tetrominoe);
            }
        }

        static bool CompareShape(int[,] a, int[,] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }
            else if (a.GetLength(0) != b.GetLength(0))
            {
                return false;
            }
            else if (a.GetLength(1) != b.GetLength(1))
            {
                return false;
            }
            else
            {
                for (int i = 0; i < a.GetLength(0); i++)
                {
                    for (int j = 0; j < a.GetLength(1); j++)
                    {
                        if (a[i, j] != b[i, j])
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        public void Rotate(Tetromino tetrominoe)
        {
            List<int[]> templocation = new List<int[]>();
            for (int i = 0; i < tetrominoe.Shape.GetLength(0); i++)
            {
                for (int j = 0; j < tetrominoe.Shape.GetLength(1); j++)
                {
                    if (tetrominoe.Shape[i, j] == 1)
                    {
                        templocation.Add(new int[] { i, (GamePlay.BORDER_WIDTH - tetrominoe.Shape.GetLength(1)) / 2 + j });
                    }
                }
            }

            if (CompareShape(tetrominoe.Shape, Tetromino.TetrominoI))
            {
                if (tetrominoe.IsErect == false)
                {
                    for (int i = 0; i < tetrominoe.location.Count; i++)
                    {
                        templocation[i] = TransformMatrix(tetrominoe.location[i], tetrominoe.location[2], "Clockwise");
                    }
                }
                else
                {
                    for (int i = 0; i < tetrominoe.location.Count; i++)
                    {
                        templocation[i] = TransformMatrix(tetrominoe.location[i], tetrominoe.location[2], "Counterclockwise");
                    }
                }
            }

            else if (CompareShape(tetrominoe.Shape, Tetromino.TetrominoS))
            {
                for (int i = 0; i < tetrominoe.location.Count; i++)
                {
                    templocation[i] = TransformMatrix(tetrominoe.location[i], tetrominoe.location[3], "Clockwise");
                }
            }

            else if (CompareShape(tetrominoe.Shape, Tetromino.TetrominoO))
            {
                return;
            }
            else
            {
                for (int i = 0; i < tetrominoe.location.Count; i++)
                {
                    templocation[i] = TransformMatrix(tetrominoe.location[i], tetrominoe.location[2], "Clockwise");
                }
            }


            for (int count = 0; tetrominoe.IsOverlayLeft(templocation) != false | tetrominoe.IsOverlayRight(templocation) != false | tetrominoe.IsOverlayBelow(templocation) != false; count++)
            {
                if (tetrominoe.IsOverlayLeft(templocation) == true)
                {
                    for (int i = 0; i < tetrominoe.location.Count; i++)
                    {
                        templocation[i][1] += 1;
                    }
                }

                if (tetrominoe.IsOverlayRight(templocation) == true)
                {
                    for (int i = 0; i < tetrominoe.location.Count; i++)
                    {
                        templocation[i][1] -= 1;
                    }
                }
                if (tetrominoe.IsOverlayBelow(templocation) == true)
                {
                    for (int i = 0; i < tetrominoe.location.Count; i++)
                    {
                        templocation[i][0] -= 1;
                    }
                }
                if (count == 3)
                {
                    return;
                }
            }

            tetrominoe.location = templocation;

        }

        public int[] TransformMatrix(int[] coord, int[] axis, string dir)
        {
            int[] pcoord = { coord[0] - axis[0], coord[1] - axis[1] };
            if (dir == "Counterclockwise")
            {
                pcoord = new int[] { -pcoord[1], pcoord[0] };
            }
            else if (dir == "Clockwise")
            {
                pcoord = new int[] { pcoord[1], -pcoord[0] };
            }

            return new int[] { pcoord[0] + axis[0], pcoord[1] + axis[1] };
        }

        public void Update(Tetromino tetrominoe)
        {
            for (int i = 0; i < GamePlay.BORDER_HEIGHT; i++)
            {
                for (int j = 0; j < GamePlay.BORDER_WIDTH; j++)
                {
                    GamePlay.grid[i, j] = 0;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                GamePlay.grid[tetrominoe.location[i][0], tetrominoe.location[i][1]] = 1;
            }

            GamePlay.Draw();
        }
    }
}
