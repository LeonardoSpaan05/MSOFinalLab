using System.Collections.Generic;
using Xunit;
using Lab2ProjectMSO.Models;
using Lab2ProjectMSO.Models.Commands;
using Lab2ProjectMSO.Enum;

namespace Lab2ProjectMSO.Tests.Commands
{
    public class TurnCommandTests
    {
        [Fact]
        public void Execute_TurnLeft_ChangesFacing()
        {
            var robot = new Robot(0, 0, Direction.North);
            var trace = new List<string>();

            new TurnCommand(TurnDirection.Left).Execute(robot, trace);

            Assert.NotEqual(Direction.North, robot.Facing);
        }

        [Fact]
        public void Execute_TurnRight_ChangesFacing()
        {
            var robot = new Robot(0, 0, Direction.North);
            var trace = new List<string>();

            new TurnCommand(TurnDirection.Right).Execute(robot, trace);

            Assert.NotEqual(Direction.North, robot.Facing);
        }
    }
}