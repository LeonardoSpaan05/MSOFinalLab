using System.Collections.Generic;
using Xunit;
using Lab2ProjectMSO.Models;
using Lab2ProjectMSO.Models.Commands;
using Lab2ProjectMSO.Interfaces;
using Lab2ProjectMSO.Enum;

namespace Lab2ProjectMSO.Tests.Core
{
    public class ProgramMetricsTests
    {
        [Fact]
        public void Computes_NumberOfCommands_NumberOfRepeats_And_MaxNesting()
        {
            var inner = new RepeatCommand(2, new List<ICommand> { new MoveCommand(1) });
            var outer = new RepeatCommand(3, new List<ICommand> { inner, new TurnCommand(TurnDirection.Left) });

            var model = new ProgramModel("Nested", new List<ICommand> { outer, new MoveCommand(2) });
            var metrics = new ProgramMetrics(model);

            Assert.True(metrics.NumberOfCommands > 0);
            Assert.True(metrics.NumberOfRepeats >= 1);
            Assert.True(metrics.MaxNesting >= 2);
        }
    }
}