using System;
using System.Collections.Generic;
using System.IO;
using Lab2ProjectMSO.Interfaces;
using Lab2ProjectMSO.Models.Commands;

namespace Lab2ProjectMSO.Models.Factory
{
    public static class ProgramImporter
    {
        public static ProgramModel ImportFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            var lines = File.ReadAllLines(filePath);

            var stack = new Stack<(List<ICommand> commands, int indent)>();
            stack.Push((new List<ICommand>(), -1));

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                int indent = line.Length - line.TrimStart().Length;
                string trimmed = line.Trim();

                while (stack.Count > 1 && indent <= stack.Peek().indent)
                    stack.Pop();

                if (trimmed.StartsWith("Repeat", StringComparison.OrdinalIgnoreCase))
                {
                    int times = 2;
                    var parts = trimmed.Split(' ');
                    if (parts.Length >= 2 && int.TryParse(parts[1], out int n))
                        times = n;

                    var nestedCommands = new List<ICommand>();
                    stack.Peek().commands.Add(new RepeatCommand(times, nestedCommands));
                    stack.Push((nestedCommands, indent));
                }
                else
                {
                    stack.Peek().commands.Add(CommandFactory.CreateCommand(trimmed));
                }
            }

            while (stack.Count > 1)
                stack.Pop();

            var allCommands = stack.Pop().commands;
            var programName = Path.GetFileNameWithoutExtension(filePath);
            var program = new ProgramModel(programName, allCommands);
            return program;
        }
    }
}