using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab2ProjectMSO.Interfaces;


namespace Lab2ProjectMSO.Models.Strategy
{
    public class ForwardStrategy : ICommandStrategy
    {
        public void Move(Robot robot, int steps)
        {
            robot.MoveForward(steps);
        }

        public string GetCommandText(int steps)
        {
            return $"Move {steps}";
        }
    }
}