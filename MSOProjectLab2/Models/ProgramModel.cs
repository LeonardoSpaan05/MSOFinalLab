using System.Collections.Generic;
using Lab2ProjectMSO.Interfaces;

namespace Lab2ProjectMSO.Models
{
    public class ProgramModel
    {
        public string Name { get; }
        public List<ICommand> Commands { get; }

        public ProgramModel(string name, List<ICommand> commands)
        {
            Name = name;
            Commands = commands;
        }
    }
}