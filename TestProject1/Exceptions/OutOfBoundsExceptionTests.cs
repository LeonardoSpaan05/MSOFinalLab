using Xunit;
using Lab2ProjectMSO.Exceptions;

namespace Lab2ProjectMSO.Tests.Exceptions
{
    public class OutOfBoundsExceptionTests
    {
        [Fact]
        public void Message_Is_Preserved()
        {
            var ex = new OutOfBoundsException("OOB");
            Assert.Equal("OOB", ex.Message);
        }
    }
}