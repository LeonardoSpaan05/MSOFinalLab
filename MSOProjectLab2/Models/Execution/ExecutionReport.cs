using System;
using System.Collections.Generic;
using Lab2ProjectMSO.Enum;

namespace Lab2ProjectMSO.Models.Execution
{
    public class ExecutionReport
    {
        public bool Success { get; }
        public string ErrorMessage { get; }
        public List<string> Trace { get; }
        public int FinalX { get; }
        public int FinalY { get; }
        public Direction FinalDirection { get; }

        // Constructor voor succesvolle uitvoering
        public ExecutionReport(int finalX, int finalY, Direction finalDirection, List<string>? trace = null)
        {
            Success = true;
            FinalX = finalX;
            FinalY = finalY;
            FinalDirection = finalDirection;
            Trace = trace ?? new List<string>();
            ErrorMessage = string.Empty;
        }

        // Constructor voor fout tijdens uitvoering
        public ExecutionReport(string errorMessage, List<string>? trace = null)
        {
            Success = false;
            ErrorMessage = errorMessage;
            Trace = trace ?? new List<string>();
            FinalX = 0;
            FinalY = 0;
            FinalDirection = Direction.East;
        }

        public override string ToString()
        {
            string traceOutput = Trace.Count > 0 ? string.Join(", ", Trace) : "(no commands executed)";
            if (Success)
            {
                return $"{traceOutput}.\nEnd state ({FinalX},{FinalY}) facing {FinalDirection.ToString().ToLower()}.";
            }
            else
            {
                return $"Execution failed: {ErrorMessage}\nTrace: {traceOutput}";
            }
        }
    }
}