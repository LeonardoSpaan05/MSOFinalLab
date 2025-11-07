using Xunit;
using Lab2ProjectMSO.Models;
using Lab2ProjectMSO.Enum;
using Lab2ProjectMSO.Models.Strategy; // ← Corrected import

namespace Lab2ProjectMSO.Tests.Strategy
{
    public class ForwardStrategyTests
    {
        [Fact]
        public void Move_Applies_Forward_Steps()
        {
            var strategy = new ForwardStrategy();
            var robot = new Robot(0, 0, Direction.East);

            strategy.Move(robot, 2);

            Assert.Equal(2, robot.X);
            Assert.Equal(Direction.East, robot.Facing);
        }

        [Fact]
        public void GetCommandText_ReturnsMeaningfulDescription()
        {
            var strategy = new ForwardStrategy();
            var text = strategy.GetCommandText(3);

            Assert.Equal("Move 3", text);
        }
    }
}