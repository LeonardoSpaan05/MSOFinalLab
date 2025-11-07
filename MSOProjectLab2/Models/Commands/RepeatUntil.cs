using Lab2ProjectMSO.Enum;
using Lab2ProjectMSO.Interfaces;
using Lab2ProjectMSO.Models.Pathfinding;
using System.Collections.Generic;

namespace Lab2ProjectMSO.Models.Commands
{
    public class RepeatUntilCommand : ICommand
    {
        public RepeatUntilCondition Condition { get; set; }
        public List<ICommand> Commands { get; set; } = new List<ICommand>();
        private readonly PathfindingGrid _grid;

        public RepeatUntilCommand(RepeatUntilCondition condition, PathfindingGrid grid)
        {
            Condition = condition;
            _grid = grid;
        }

        public void Execute(Robot robot, List<string> trace)
        {
            trace.Add($"Start RepeatUntil {Condition}");
            while (!CheckCondition(robot))
            {
                foreach (var cmd in Commands)
                    cmd.Execute(robot, trace);
            }
            trace.Add($"End RepeatUntil {Condition}");
        }

        private bool CheckCondition(Robot robot)
        {
            return Condition switch
            {
                RepeatUntilCondition.WallAhead => robot.IsWallAhead(_grid),
                RepeatUntilCondition.GridEdge => robot.IsAtEdge(_grid),
                _ => false
            };
        }
    }
}