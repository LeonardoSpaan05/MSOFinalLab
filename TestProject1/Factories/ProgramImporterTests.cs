using System.IO;
using Xunit;
using Lab2ProjectMSO.Models;
using Lab2ProjectMSO.Models.Factory;

namespace Lab2ProjectMSO.Tests.Factories
{
    public class ProgramImporterTests
    {
        [Fact]
        public void ImportFromFile_InvalidPath_ThrowsOrHandled()
        {
            var ex = Record.Exception(() => ProgramImporter.ImportFromFile("does_not_exist.txt"));
            Assert.True(ex == null || ex is FileNotFoundException);
        }

        [Fact]
        public void ImportFromFile_ValidEmptyFile_YieldsEmptyProgram()
        {
            var path = Path.GetTempFileName();
            try
            {
                File.WriteAllText(path, string.Empty);
                var model = ProgramImporter.ImportFromFile(path);
                Assert.NotNull(model);
                Assert.Empty(model.Commands);
            }
            finally { File.Delete(path); }
        }
    }
}