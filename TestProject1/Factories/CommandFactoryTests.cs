using System;
using Xunit;
using Lab2ProjectMSO.Models.Factory;
using Lab2ProjectMSO.Models.Commands;

namespace Lab2ProjectMSO.Tests.Factories
{
    public class CommandFactoryTests
    {
        [Fact]
        public void CreateCommand_Parses_Move()
        {
            var cmd = CommandFactory.CreateCommand("MOVE 2");
            Assert.IsType<MoveCommand>(cmd);
        }

        [Fact]
        public void CreateCommand_Parses_Turn_Left_And_Right()
        {
            Assert.IsType<TurnCommand>(CommandFactory.CreateCommand("TURN LEFT"));
            Assert.IsType<TurnCommand>(CommandFactory.CreateCommand("TURN RIGHT"));
        }

        [Fact]
        public void CreateCommand_RepeatUntil_SupportedOrThrowsCleanly()
        {
            var ex = Record.Exception(() => CommandFactory.CreateCommand("REPEATUNTIL WALLAHEAD"));
            Assert.True(ex == null || ex is ArgumentException);
        }

        [Fact]
        public void CreateCommand_Unknown_ThrowsOrReturnsMeaningfulError()
        {
            var ex = Record.Exception(() => CommandFactory.CreateCommand("FLY 10"));
            Assert.NotNull(ex);
        }
    }
}