using System.Collections.Generic;
using Lab2ProjectMSO.Interfaces;

namespace Lab2ProjectMSO.Models.Commands
{
    public class RepeatCommand : ICommand
    {
        public int Times { get; set; }
        public List<ICommand> Commands { get; set; } = new();
        public string DisplayName { get; set; } = "Repeat";

        public RepeatCommand(int times = 2, List<ICommand>? commands = null)
        {
            Times = times;
            if (commands != null)
                Commands = commands;
        }

        public void Execute(Robot robot, List<string> trace)
        {
            for (int i = 0; i < Times; i++)
            {
                foreach (var cmd in Commands)
                {
                    cmd.Execute(robot, trace);
                }
            }
        }

        public override string ToString()
        {
            return $"{DisplayName} ({Times}x)";
        }
    }
}