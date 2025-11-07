using System.Collections.Generic;
using Xunit;
using Lab2ProjectMSO;
using Lab2ProjectMSO.Models;
using Lab2ProjectMSO.Models.Commands;
using Lab2ProjectMSO.Models.Pathfinding;
using Lab2ProjectMSO.Enum;

namespace Lab2ProjectMSO.Tests.Core
{
    public class ProgramExecutorTests
    {
        [Fact]
        public void Execute_Successful_Run_Produces_Report_With_FinalState()
        {
            var grid = new PathfindingGrid(3); // end defaults (2,2)
            var program = new ProgramModel("p", new List<Interfaces.ICommand>
            {
                new MoveCommand(1),                         // (1,0)
                new TurnCommand(TurnDirection.Right),       // facing South
                new MoveCommand(1),                         // (1,1)
                new TurnCommand(TurnDirection.Left),        // facing East
                new MoveCommand(1),                         // (2,1)
                new TurnCommand(TurnDirection.Right),       // South
                new MoveCommand(1)                          // (2,2) -> end
            });

            var exec = new ProgramExecutor(grid);
            var report = exec.Execute(program);

            Assert.NotNull(report);
            Assert.True(report.Success);
            Assert.True(report.Trace.Count > 0);
        }

        [Fact]
        public void Execute_Failure_On_Blocked_Cell_ReturnsErrorReport()
        {
            var grid = new PathfindingGrid(3);
            grid.Cells[0, 1] = '+'; // block (1,0)
            var program = new ProgramModel("p", new List<Interfaces.ICommand> { new MoveCommand(1) });

            var exec = new ProgramExecutor(grid);
            var report = exec.Execute(program);

            Assert.False(report.Success);
            Assert.False(string.IsNullOrWhiteSpace(report.ErrorMessage));
        }

        [Fact]
        public void CheckCondition_WallAhead_And_GridEdge_Work()
        {
            var grid = new PathfindingGrid(3);
            var exec = new ProgramExecutor(grid);

            var r1 = new Robot(0, 0, Direction.East);
            Assert.False(exec.CheckCondition(RepeatUntilCondition.WallAhead, r1, grid));
            grid.Cells[0, 1] = '+';
            Assert.True(exec.CheckCondition(RepeatUntilCondition.WallAhead, r1, grid));

            var r2 = new Robot(0, 0, Direction.North);
            Assert.True(exec.CheckCondition(RepeatUntilCondition.GridEdge, r2, grid));
        }
    }
}
