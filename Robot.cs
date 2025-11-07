using System;
using System.Collections.Generic;
using System.Drawing;
using Lab2ProjectMSO.Enum;
using Lab2ProjectMSO.Models.Pathfinding;

namespace Lab2ProjectMSO
{
    public class Robot
    {
        private readonly int _startX;
        private readonly int _startY;

        public int X { get; private set; }
        public int Y { get; private set; }
        public Direction Facing { get; private set; }
        public List<Point> Path { get; private set; }

        // Voeg een Grid property toe
        public PathfindingGrid Grid { get; set; }

        public Robot(int startX = 0, int startY = 0, Direction startFacing = Direction.East)
        {
            _startX = startX;
            _startY = startY;
            X = _startX;
            Y = _startY;
            Facing = startFacing;
            Path = new List<Point> { new Point(X, Y) };
        }

        public void MoveForward(int steps = 1)
        {
            for (int i = 0; i < steps; i++)
            {
                var (nextX, nextY) = GetNextPosition();
                X = nextX;
                Y = nextY;
                Path.Add(new Point(X, Y));
            }
        }

        public void TurnLeft()
        {
            Facing = Facing switch
            {
                Direction.North => Direction.West,
                Direction.West => Direction.South,
                Direction.South => Direction.East,
                Direction.East => Direction.North,
                _ => Facing
            };
        }

        public void TurnRight()
        {
            Facing = Facing switch
            {
                Direction.North => Direction.East,
                Direction.East => Direction.South,
                Direction.South => Direction.West,
                Direction.West => Direction.North,
                _ => Facing
            };
        }

        public void Reset()
        {
            X = _startX;
            Y = _startY;
            Facing = Direction.East;
            Path.Clear();
            Path.Add(new Point(X, Y));
        }

        public (int nextX, int nextY) GetNextPosition()
        {
            int nextX = X;
            int nextY = Y;

            switch (Facing)
            {
                case Direction.North: nextY -= 1; break;
                case Direction.East:  nextX += 1; break;
                case Direction.South: nextY += 1; break;
                case Direction.West:  nextX -= 1; break;
            }

            return (nextX, nextY);
        }

        public bool IsWallAhead(PathfindingGrid grid)
        {
            if (grid == null) return false;
            var (nextX, nextY) = GetNextPosition();
            if (nextX < 0 || nextY < 0 || nextX >= grid.Cols || nextY >= grid.Rows)
                return true; // buiten het grid beschouwen als muur
            return grid.IsBlocked(nextX, nextY);
        }

        public bool IsAtEdge(PathfindingGrid grid)
        {
            if (grid == null) return false;
            var (nextX, nextY) = GetNextPosition();
            return nextX < 0 || nextY < 0 || nextX >= grid.Cols || nextY >= grid.Rows;
        }

    }
}
