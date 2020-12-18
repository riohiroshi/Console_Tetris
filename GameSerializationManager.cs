using System;
using System.IO;

namespace Tetris
{
    static class GameSerializationManager
    {
        public static bool LoadFromFile()
        {
            FileStream file = null;

            try
            {
                file = new FileStream("saveData.txt", FileMode.Open);
            }
            catch (FileNotFoundException e)
            {
                //Console.WriteLine("FileNotFoundException: " + e.FileName);
            }
            catch (Exception e)
            {
                //Console.WriteLine("Exception: " + e);
            }

            if (file == null)
            {
                return false;
            }

            BinaryReader reader = new BinaryReader(file);

            Deserialization(reader);

            file.Close();

            return true;
        }
        public static bool SaveToFile()
        {
            FileStream file = new FileStream("saveData.txt", FileMode.Create);
            BinaryWriter writer = new BinaryWriter(file);

            Serialization(writer);

            file.Close();

            return true;
        }

        private static bool Deserialization(BinaryReader reader)
        {
            var highestScore = GamePlay.HighestScore;
            var scoreRanking = GamePlay.ScoreRanking;

            int tempScore = reader.ReadInt32();
            highestScore = tempScore;

            int tempCount = reader.ReadInt32();
            scoreRanking.Clear();
            for (int i = 0; i < tempCount; i++)
            {
                Player tempPlayer = new Player();

                tempPlayer.playerName = reader.ReadString();
                tempPlayer.score = reader.ReadInt32();

                scoreRanking.Add(tempPlayer);
            }
            scoreRanking.Sort((Player player1, Player player2) => { return player1.score.CompareTo(player2.score); });
            scoreRanking.Reverse();
            if (scoreRanking.Count > 10)
            {
                scoreRanking.RemoveAt(10);
            }

            return true;
        }
        private static void Serialization(BinaryWriter writer)
        {
            var highestScore = GamePlay.HighestScore;
            var scoreRanking = GamePlay.ScoreRanking;

            writer.Write(highestScore);

            writer.Write(scoreRanking.Count);
            for (int i = 0; i < scoreRanking.Count; i++)
            {
                writer.Write(scoreRanking[i].playerName);
                writer.Write(scoreRanking[i].score);
            }
        }
    }
}