using System;
using System.Collections.Generic;
using Lab2ProjectMSO.Interfaces;
using Lab2ProjectMSO.Models;
using Lab2ProjectMSO.Models.Commands;
using Lab2ProjectMSO.Enum;
using Lab2ProjectMSO.Models.Pathfinding;
using Lab2ProjectMSO.Exceptions;
using Lab2ProjectMSO.Models.Execution;

namespace Lab2ProjectMSO
{
    public class ProgramExecutor
    {
        private readonly PathfindingGrid _grid;
        public List<string> Trace { get; private set; } = new List<string>();

        public ProgramExecutor(PathfindingGrid grid)
        {
            _grid = grid;
        }

        public ExecutionReport Execute(ProgramModel program)
        {
            var robot = new Robot(_grid.Start.X, _grid.Start.Y);
            Trace.Clear();

            try
            {
                // Gebruik nu de 3-parameter versie, met _grid
                ExecuteCommands(program.Commands, robot, _grid);

                bool success = _grid.IsEnd(robot.X, robot.Y);
                string result = success ? "Success! Reached the goal." : "Failure: wrong end position.";
                Trace.Add(result);

                return new ExecutionReport(robot.X, robot.Y, robot.Facing, new List<string>(Trace));
            }
            catch (Exception ex)
            {
                Trace.Add($"Runtime error: {ex.Message}");
                return new ExecutionReport($"Runtime error: {ex.Message}", new List<string>(Trace));
            }
        }


        private void ExecuteCommands(IList<ICommand> commands, Robot robot, PathfindingGrid grid)
        {
            foreach (var cmd in commands)
                ExecuteCommand(cmd, robot, grid);
        }


        public void ExecuteCommand(ICommand cmd, Robot robot, PathfindingGrid grid)
        {
            switch (cmd)
            {
                case MoveCommand:
                    var (nextX, nextY) = robot.GetNextPosition();
                    if (nextX < 0 || nextY < 0 || nextX >= grid.Cols || nextY >= grid.Rows)
                        throw new OutOfBoundsException($"Robot tried to move out of bounds to ({nextX}, {nextY})");
                    if (grid.IsBlocked(nextX, nextY))
                        throw new BlockedCellException($"Robot tried to move to a blocked cell at ({nextX}, {nextY})");
                    robot.MoveForward();
                    Trace.Add($"Move to ({robot.X}, {robot.Y})");
                    break;

                case TurnCommand turn:
                    if (turn.Direction == TurnDirection.Left)
                        robot.TurnLeft();
                    else
                        robot.TurnRight();
                    Trace.Add($"Turn {turn.Direction}");
                    break;

                case RepeatCommand repeat:
                    Trace.Add($"Start Repeat x{repeat.Times}");
                    for (int i = 0; i < repeat.Times; i++)
                        foreach (var nested in repeat.Commands)
                            ExecuteCommand(nested, robot, grid);
                    Trace.Add($"End Repeat x{repeat.Times}");
                    break;

                case RepeatUntilCommand repeatUntil:
                    Trace.Add($"Start RepeatUntil {repeatUntil.Condition}");
                    while (!CheckCondition(repeatUntil.Condition, robot, grid))
                        foreach (var nested in repeatUntil.Commands)
                            ExecuteCommand(nested, robot, grid);
                    Trace.Add($"End RepeatUntil {repeatUntil.Condition}");
                    break;

                default:
                    throw new InvalidOperationException($"Unknown command type: {cmd.GetType().Name}");
            }
        }

        public bool CheckCondition(RepeatUntilCondition condition, Robot robot, PathfindingGrid grid)
        {
            return condition switch
            {
                RepeatUntilCondition.WallAhead => robot.IsWallAhead(grid),
                RepeatUntilCondition.GridEdge => robot.IsAtEdge(grid),
                _ => false
            };
        }


        private void ExecuteMove(Robot robot)
        {
            var (nextX, nextY) = robot.GetNextPosition();

            if (nextX < 0 || nextY < 0 || nextX >= _grid.Cols || nextY >= _grid.Rows)
                throw new OutOfBoundsException($"Robot tried to move out of bounds to ({nextX}, {nextY})");

            if (_grid.IsBlocked(nextX, nextY))
                throw new BlockedCellException($"Robot tried to move to a blocked cell at ({nextX}, {nextY})");

            robot.MoveForward();
            Trace.Add($"Move to ({robot.X}, {robot.Y})");
        }

        private void ExecuteTurn(Robot robot, TurnCommand turn)
        {
            if (turn.Direction == TurnDirection.Left)
                robot.TurnLeft();
            else
                robot.TurnRight();
            Trace.Add($"Turn {turn.Direction}");
        }


    }
}
