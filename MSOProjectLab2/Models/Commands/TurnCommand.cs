using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab2ProjectMSO.Enum;
using Lab2ProjectMSO.Interfaces;

namespace Lab2ProjectMSO.Models.Commands
{
    public class TurnCommand : ICommand
    {
        public TurnDirection Direction { get; }
        public string DisplayName { get; set; } = "Turn";

        public TurnCommand(TurnDirection direction)
        {
            Direction = direction;
        }

        public void Execute(Robot robot, List<string> trace)
        {
            if (Direction == TurnDirection.Left)
                robot.TurnLeft();
            else
                robot.TurnRight();

            trace.Add($"{DisplayName} {Direction.ToString().ToLower()}");
        }

        public override string ToString() => DisplayName;
    }
}