using Xunit;
using Lab2ProjectMSO.Models.Pathfinding;

namespace Lab2ProjectMSO.Tests.Pathfinding
{
    public class PathfindingGridTests
    {
        [Fact]
        public void NewGrid_DefaultEnd_Is_BottomRight()
        {
            var g = new PathfindingGrid(4);
            Assert.True(g.IsEnd(3, 3));
        }

        [Fact]
        public void IsBlocked_True_For_Wall_Cell()
        {
            var g = new PathfindingGrid(3);
            g.Cells[1, 1] = '+';
            Assert.True(g.IsBlocked(1, 1));
            Assert.False(g.IsBlocked(0, 0));
        }
    }
}