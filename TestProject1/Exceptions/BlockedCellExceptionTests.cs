using Xunit;
using Lab2ProjectMSO.Exceptions;

namespace Lab2ProjectMSO.Tests.Exceptions
{
    public class BlockedCellExceptionTests
    {
        [Fact]
        public void Message_Is_Preserved()
        {
            var ex = new BlockedCellException("Blocked");
            Assert.Equal("Blocked", ex.Message);
        }
    }
}