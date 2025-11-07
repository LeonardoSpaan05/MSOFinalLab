using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab2ProjectMSO.Interfaces;
using Lab2ProjectMSO.Models.Strategy;

namespace Lab2ProjectMSO.Models.Commands
{
    public class MoveCommand : ICommand
    {
        private readonly int _steps;
        public string DisplayName { get; set; } = "Move";

        public MoveCommand(int steps = 1)
        {
            _steps = steps;
        }

        public void Execute(Robot robot, List<string> trace)
        {
            robot.MoveForward(_steps);
            trace.Add($"{DisplayName} {_steps}");
        }

        public override string ToString() => DisplayName;
    }


}