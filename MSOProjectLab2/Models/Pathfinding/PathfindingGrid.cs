using System;
using System.IO;

namespace Lab2ProjectMSO.Models.Pathfinding
{
    public class PathfindingGrid
    {
        public char[,] Cells { get; private set; }
        public int Rows => Cells.GetLength(0);
        public int Cols => Cells.GetLength(1);

        public (int X, int Y) Start { get; private set; } = (0, 0);
        public (int X, int Y) End { get; private set; }

        private PathfindingGrid() { }

        // Nieuwe constructor voor een leeg grid
        public PathfindingGrid(int size)
        {
            Cells = new char[size, size];

            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                Cells[y, x] = ' '; // lege cel

            Start = (0, 0);
            End = (size - 1, size - 1); // standaard eindpunt rechtsonder
        }

        // Statische loader
        public static PathfindingGrid LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Grid file not found: {filePath}");

            var lines = File.ReadAllLines(filePath);
            int rows = lines.Length;
            int cols = lines[0].Length;

            var grid = new PathfindingGrid
            {
                Cells = new char[rows, cols]
            };

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    grid.Cells[y, x] = lines[y][x];
                    if (lines[y][x] == 'x') grid.End = (x, y);
                }
            }

            return grid;
        }

        public bool IsBlocked(int x, int y) => Cells[y, x] == '+';
        public bool IsEnd(int x, int y) => (x, y) == End;
    }
}