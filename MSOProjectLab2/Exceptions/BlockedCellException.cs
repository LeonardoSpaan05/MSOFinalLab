using System;

namespace Lab2ProjectMSO.Exceptions
{
    public class BlockedCellException : Exception
    {
        public BlockedCellException(string message) : base(message) { }
    }
}