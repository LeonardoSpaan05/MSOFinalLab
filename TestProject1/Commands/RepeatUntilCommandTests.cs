using System.Collections.Generic;
using Xunit;
using Lab2ProjectMSO.Models;
using Lab2ProjectMSO.Models.Commands;
using Lab2ProjectMSO.Models.Pathfinding;
using Lab2ProjectMSO.Enum;

namespace Lab2ProjectMSO.Tests.Commands
{
    public class RepeatUntilCommandTests
    {
        [Fact]
        public void Execute_WallAhead_StopsBeforeWall()
        {
            var grid = new PathfindingGrid(4);
            grid.Cells[0, 3] = '+'; // wall at (3,0)
            var robot = new Robot(0, 0, Direction.East);
            var trace = new List<string>();
            var cmd = new RepeatUntilCommand(RepeatUntilCondition.WallAhead, grid);
            cmd.Commands.Add(new MoveCommand(1));

            cmd.Execute(robot, trace);

            Assert.Equal(2, robot.X); // stop at x=2 (before wall at x=3)
            Assert.True(trace.Count >= 1);
        }

        [Fact]
        public void Execute_GridEdge_AllowsZeroOrOneIteration_WhenAlreadyAtEdge()
        {
            var grid = new PathfindingGrid(3);
            var robot = new Robot(0, 0, Direction.North); // at top edge
            var trace = new List<string>();
            var cmd = new RepeatUntilCommand(RepeatUntilCondition.GridEdge, grid);
            cmd.Commands.Add(new MoveCommand(1));

            cmd.Execute(robot, trace);

            Assert.True(trace.Count <= 2);
        }
    }
}