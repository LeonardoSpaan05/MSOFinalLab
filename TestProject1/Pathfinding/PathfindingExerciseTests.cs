using Xunit;
using Lab2ProjectMSO.Models;

namespace Lab2ProjectMSO.Tests.Pathfinding
{
    public class PathfindingExerciseTests
    {
        [Fact]
        public void Exercise_LoadsGrid_FromFile()
        {
            // Create temporary map file
            var path = Path.GetTempFileName();
            System.IO.File.WriteAllText(path, "...\n");

            var ex = new PathfindingExercise("TestGrid", path);

            Assert.NotNull(ex.Grid);
            Assert.Equal("TestGrid", ex.Name);
            System.IO.File.Delete(path);
        }
    }
}