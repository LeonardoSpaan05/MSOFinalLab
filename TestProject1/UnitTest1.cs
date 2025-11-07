using Xunit;
using System.Collections.Generic;
using Lab2ProjectMSO.Enum;
using Lab2ProjectMSO.Models.Commands;
using Lab2ProjectMSO.Models.Execution;
using Lab2ProjectMSO.Models.Factory;
using Lab2ProjectMSO.Models;
using Lab2ProjectMSO;
using Lab2ProjectMSO.Interfaces;


public class RobotTests
{
    [Fact]
    public void Robot_MoveForward_East_IncreasesX()
    {
        var robot = new Robot();
        robot.MoveForward(3);
        Assert.Equal(3, robot.X);
        Assert.Equal(0, robot.Y);
    }


    [Fact]
    public void TurnLeft_UpdatesFacing()
    {
        var robot = new Robot();
        robot.TurnLeft();
        Assert.Equal(Direction.North, robot.Facing);
    }

    [Fact]
    public void TurnRight_UpdatesFacing()
    {
        var robot = new Robot();
        robot.TurnRight();
        Assert.Equal(Direction.South, robot.Facing);
    }
}

public class MoveCommandTests
{
    [Fact]
    public void Execute_MovesRobotAndAppendsTrace()
    {
        var robot = new Robot();
        var command = new MoveCommand(2);
        var trace = new List<string>();
        command.Execute(robot, trace);
        Assert.Equal(2, robot.X); // Standaard EAST!
        Assert.Equal(0, robot.Y);
        Assert.Single(trace);
    }
}

public class RepeatCommandTests
{
    [Fact]
    public void RepeatCommand_Execute_RepeatsCommands()
    {
        var robot = new Robot();
        var commands = new List<ICommand> { new MoveCommand(1) };
        var repeat = new RepeatCommand(3, commands);
        var trace = new List<string>();
        repeat.Execute(robot, trace);
        Assert.Equal(3, robot.X);    // MOVES EAST
        Assert.Equal(0, robot.Y);
        Assert.Equal(3, trace.Count);
    }
}

public class TurnCommandTests
{
    [Fact]
    public void Execute_TurnsRobot_Left_FromEast()
    {
        var robot = new Robot();
        var cmd = new TurnCommand(TurnDirection.Left);
        var trace = new List<string>();
        cmd.Execute(robot, trace);
        Assert.Equal(Direction.North, robot.Facing);  // East → Left = North
    }

}

public class CommandFactoryTests
{
    [Fact]
    public void CreateCommand_Move()
    {
        var result = CommandFactory.CreateCommand("move 2", null);
        Assert.IsType<MoveCommand>(result);
    }
    [Fact]
    public void CreateCommand_Repeat()
    {
        var nested = new List<string> { "move 1" };
        var result = CommandFactory.CreateCommand("repeat 2", nested);
        Assert.IsType<RepeatCommand>(result);
    }
}

public class ProgramModelTests
{
    [Fact]
    public void ProgramModel_StoresNameAndCommands()
    {
        var commands = new List<ICommand>();
        var model = new ProgramModel("Demo", commands);
        Assert.Equal("Demo", model.Name);
        Assert.Same(commands, model.Commands);
    }
}

public class ExecutionReportTests
{
    [Fact]
    public void SuccessReport_StoresProperties()
    {
        var trace = new List<string> { "step" };
        var rep = new ExecutionReport(2, 3, Direction.South, trace);
        Assert.True(rep.Success);
        Assert.Equal(2, rep.FinalX);
        Assert.Equal(Direction.South, rep.FinalDirection);
        Assert.Equal(trace, rep.Trace);
    }
    [Fact]
    public void ErrorReport_StoresError()
    {
        var rep = new ExecutionReport("Oops", null);
        Assert.False(rep.Success);
        Assert.Equal("Oops", rep.ErrorMessage);
        Assert.Equal(0, rep.FinalX);
    }
}