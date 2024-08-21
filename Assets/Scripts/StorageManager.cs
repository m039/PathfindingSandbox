using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game
{
    public static class StorageManager
    {
        const string LevelFile = "Assets/Resources/Data/Level.txt";

        public static void Save(bool[,] grid, Node start, Node goal)
        {
#if UNITY_EDITOR
            var width = grid.GetLength(0);
            var height = grid.GetLength(1);
            var grids = new int[width * height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grids[y * width + x] = grid[x, y] ? 1 : 0;
                }
            }
            var data = new Data
            {
                width = width,
                height = height,
                grids = grids,
                start = new Vector2Int(start.x, start.y),
                goal = new Vector2Int(goal.x, goal.y)
            };

            File.WriteAllText(LevelFile, JsonUtility.ToJson(data));
            AssetDatabase.ImportAsset(LevelFile);
#endif
        }

        public static Data Load()
        {
            var json = Resources.Load<TextAsset>("Data/Level");
            if (json == null || string.IsNullOrEmpty(json.text))
                return null;

            return JsonUtility.FromJson<Data>(json.text);
        }

        [System.Serializable]
        public class Data
        {
            public int width;
            public int height;
            public Vector2Int start;
            public Vector2Int goal;
            public int[] grids;
        }
    }
}
