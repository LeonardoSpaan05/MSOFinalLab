using Lab2ProjectMSO.Models.Pathfinding;

namespace Lab2ProjectMSO.Models
{
    public class PathfindingExercise
    {
        public string Name { get; }
        public PathfindingGrid Grid { get; }

        public PathfindingExercise(string name, string filePath)
        {
            Name = name;
            Grid = PathfindingGrid.LoadFromFile(filePath);
        }
    }
}