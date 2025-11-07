using System.Collections.Generic;
using Lab2ProjectMSO.Models.Commands;
using Lab2ProjectMSO.Interfaces;
using Lab2ProjectMSO.Models;


namespace Lab2ProjectMSO
{
    public class ProgramMetrics
    {
        public int NumberOfCommands { get; private set; }
        public int MaxNesting { get; private set; }
        public int NumberOfRepeats { get; private set; }

        public ProgramMetrics(ProgramModel program)
        {
            NumberOfCommands = 0;
            MaxNesting = 0;
            NumberOfRepeats = 0;

            if (program?.Commands != null)
            {
                CalculateMetrics(program.Commands, 0);
            }
        }

        private void CalculateMetrics(List<ICommand> commands, int currentNesting)
        {
            foreach (var command in commands)
            {
                NumberOfCommands++;

                if (command is RepeatCommand repeat)
                {
                    NumberOfRepeats++;
                    int newNesting = currentNesting + 1;
                    if (newNesting > MaxNesting)
                        MaxNesting = newNesting;
                    CalculateMetrics(repeat.Commands, newNesting);
                }
                else if (command is RepeatUntilCommand repeatUntil)
                {
                    NumberOfRepeats++;
                    int newNesting = currentNesting + 1;
                    if (newNesting > MaxNesting)
                        MaxNesting = newNesting;
                    CalculateMetrics(repeatUntil.Commands, newNesting);
                }


            }
        }
    }
}