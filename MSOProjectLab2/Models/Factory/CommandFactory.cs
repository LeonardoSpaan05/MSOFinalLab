using System;
using System.Collections.Generic;
using Lab2ProjectMSO.Enum;
using Lab2ProjectMSO.Interfaces;
using Lab2ProjectMSO.Models.Commands;

namespace Lab2ProjectMSO.Models.Factory
{
    public static class CommandFactory
    {
        public static ICommand CreateCommand(string line, List<string>? nestedLines = null)
        {
            if (string.IsNullOrWhiteSpace(line))
                throw new ArgumentException("Empty command line");

            var parts = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string keyword = parts[0].ToLower();

            switch (keyword)
            {
                case "move":
                    int distance = parts.Length > 1 && int.TryParse(parts[1], out int d) ? d : 1;
                    return new MoveCommand(distance) { DisplayName = "Move" };

                case "turn":
                    TurnDirection dir = parts.Length > 1 && parts[1].ToLower() == "left"
                        ? TurnDirection.Left
                        : TurnDirection.Right;
                    return new TurnCommand(dir) { DisplayName = "Turn" };

                case "repeat":
                    int times = parts.Length > 1 && int.TryParse(parts[1], out int t) ? t : 1;

                    var commands = new List<ICommand>();
                    if (nestedLines != null)
                    {
                        foreach (var nested in nestedLines)
                        {
                            commands.Add(CreateCommand(nested));
                        }
                    }

                    return new RepeatCommand(times, commands) { DisplayName = "Repeat" };

                default:
                    throw new ArgumentException($"Unknown command: {line}");
            }
        }
    }
}