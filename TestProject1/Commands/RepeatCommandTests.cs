using System.Collections.Generic;
using Xunit;
using Lab2ProjectMSO.Models;
using Lab2ProjectMSO.Models.Commands;
using Lab2ProjectMSO.Interfaces;
using Lab2ProjectMSO.Enum;

namespace Lab2ProjectMSO.Tests.Commands
{
    public class RepeatCommandTests
    {
        [Fact]
        public void Execute_Repeats_InnerCommands_Correctly()
        {
            var robot = new Robot(0, 0, Direction.East);
            var trace = new List<string>();
            var inner = new List<ICommand> { new MoveCommand(1) };
            var repeat = new RepeatCommand(4, inner);

            repeat.Execute(robot, trace);

            Assert.Equal(4, robot.X);
            Assert.True(trace.Count >= 4);
        }

        [Fact]
        public void Execute_NestedRepeat_AccumulatesSteps()
        {
            var robot = new Robot(0, 0, Direction.East);
            var trace = new List<string>();
            var inner = new RepeatCommand(2, new List<ICommand> { new MoveCommand(1) });
            var outer = new RepeatCommand(3, new List<ICommand> { inner });

            outer.Execute(robot, trace);

            Assert.Equal(6, robot.X); // 3 * (2 * 1)
        }
    }
}