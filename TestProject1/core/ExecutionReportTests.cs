using System.Collections.Generic;
using Xunit;
using Lab2ProjectMSO.Models.Execution;
using Lab2ProjectMSO.Enum;

namespace Lab2ProjectMSO.Tests.Core
{
    public class ExecutionReportTests
    {
        [Fact]
        public void ToString_Contains_Useful_Info()
        {
            var report = new ExecutionReport(2, 3, Direction.North, new List<string> { "Move", "Turn" });
            var text = report.ToString();

            Assert.False(string.IsNullOrWhiteSpace(text));
            Assert.Contains("Move", text);
        }

        [Fact]
        public void Failure_Ctor_Sets_ErrorMessage()
        {
            var report = new ExecutionReport("Hit wall", new List<string> { "Move" });
            Assert.False(report.Success);
            Assert.Equal("Hit wall", report.ErrorMessage);
        }
    }
}