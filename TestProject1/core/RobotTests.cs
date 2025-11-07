using Xunit;
using Lab2ProjectMSO.Models;
using Lab2ProjectMSO.Enum;

namespace Lab2ProjectMSO.Tests.Core
{
    public class RobotTests
    {
        [Fact]
        public void GetNextPosition_FollowsFacing()
        {
            var r = new Robot(1, 1, Direction.East);
            var (nx, ny) = r.GetNextPosition();
            Assert.Equal(2, nx);
            Assert.Equal(1, ny);
        }

        [Fact]
        public void TurnLeft_ThenRight_ReturnsToStartDirection()
        {
            var r = new Robot(0, 0, Direction.North);
            r.TurnLeft();
            r.TurnRight();
            Assert.Equal(Direction.North, r.Facing);
        }
    }
}