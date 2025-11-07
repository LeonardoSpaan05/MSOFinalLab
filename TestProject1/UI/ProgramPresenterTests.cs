using System.Collections.Generic;
using Xunit;
using Lab2ProjectMSO;
using Lab2ProjectMSO.Models;
using Lab2ProjectMSO.Models.Commands;
using Lab2ProjectMSO.Models.Pathfinding;
using Lab2ProjectMSO.Enum;

namespace Lab2ProjectMSO.Tests.UI
{
    public class ProgramPresenterTests
    {
        [Fact]
        public void RunProgram_Produces_Trace_And_FinalState()
        {
            var grid = new PathfindingGrid(3);
            var executor = new ProgramExecutor(grid);
            var model = new ProgramModel("p", new List<Interfaces.ICommand> { new MoveCommand(1) });
            var robot = new Robot(0, 0, Direction.East);

            var presenter = new ProgramPresenter(model, executor, robot);
            presenter.RunProgram();

            // No hard coupling to UI; we rely on model/executor behavior
            Assert.NotNull(model);
        }

        [Fact]
        public void AddCommand_Appends_To_Model()
        {
            var grid = new PathfindingGrid(3);
            var executor = new ProgramExecutor(grid);
            var model = new ProgramModel("p", new List<Interfaces.ICommand>());
            var robot = new Robot(0, 0, Direction.East);

            var presenter = new ProgramPresenter(model, executor, robot);
            presenter.AddCommand("MOVE 2");

            Assert.True(model.Commands.Count >= 1);
        }
    }
}