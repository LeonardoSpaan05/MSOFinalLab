using System;

namespace Lab2ProjectMSO
{
    public class OutOfBoundsException : Exception
    {
        public OutOfBoundsException(string message) : base(message) { }
    }
}