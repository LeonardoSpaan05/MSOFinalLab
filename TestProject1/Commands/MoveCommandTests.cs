using System.Collections.Generic;
using Xunit;
using Lab2ProjectMSO.Models;
using Lab2ProjectMSO.Models.Commands;
using Lab2ProjectMSO.Enum;

namespace Lab2ProjectMSO.Tests.Commands
{
    public class MoveCommandTests
    {
        [Fact]
        public void Execute_MovesRobot_ByConfiguredSteps()
        {
            var robot = new Robot(0, 0, Direction.East);
            var trace = new List<string>();
            var move = new MoveCommand(3);

            move.Execute(robot, trace);

            Assert.Equal(3, robot.X);
            Assert.Equal(0, robot.Y);
            Assert.Contains(trace, t => t.ToLower().Contains("move"));
        }

        [Fact]
        public void Execute_WithZeroSteps_LeavesPositionUnchanged()
        {
            var robot = new Robot(2, 5, Direction.North);
            var trace = new List<string>();

            new MoveCommand(0).Execute(robot, trace);

            Assert.Equal(2, robot.X);
            Assert.Equal(5, robot.Y);
        }
    }
}