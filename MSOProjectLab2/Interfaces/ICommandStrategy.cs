using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2ProjectMSO.Interfaces
{
    public interface ICommandStrategy
    {
        void Move(Robot robot, int steps);
        string GetCommandText(int steps);
    }
}