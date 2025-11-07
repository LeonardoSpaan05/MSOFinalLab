using System;
using System.Collections.Generic;
using System.IO;
using Lab2ProjectMSO.Models;
using Lab2ProjectMSO.Models.Factory;

namespace Lab2ProjectMSO.Utils
{
    public static class ExamplePrograms
    {
        private static readonly Dictionary<string, string[]> ProgramFiles = new()
        {
            { "Basic", new[] { "Rectangle1.txt", "Rectangle3.txt", "Spiral.txt", "ZigZag.txt" } },
            { "Advanced", new[] { "Rectangle.txt", "Rectangle4.txt", "LetterNPattern.txt", "ZigZag3" } },
            { "Expert", new[] { "Random.txt", "ComplexPattern.txt", "DoubleSquare.txt", "Spiral2.txt", "ZigZag2.txt" } }
        };

        private static string GetFullPath(string fileName)
        {
            string fullPath = Path.Combine(AppContext.BaseDirectory, fileName);

            if (!File.Exists(fullPath))
            {
                Console.WriteLine($"Bestand niet gevonden: {fullPath}");
                throw new FileNotFoundException($"File not found: {fullPath}");
            }

            return fullPath;
        }

        public static ProgramModel GetProgramByCategory(string category)
        {
            if (!ProgramFiles.ContainsKey(category))
                throw new ArgumentException($"Unknown category: {category}");

            var files = ProgramFiles[category];

            Console.WriteLine($"Select a {category} program:");
            for (int i = 0; i < files.Length; i++)
                Console.WriteLine($"{i + 1}. {files[i]}");

            string? input = Console.ReadLine();
            int choice;
            if (!int.TryParse(input, out choice) || choice < 1 || choice > files.Length)
            {
                Console.WriteLine("Invalid choice, using first program by default.");
                choice = 1;
            }

            string path = GetFullPath(files[choice - 1]);
            return ProgramImporter.ImportFromFile(path);
        }

        public static ProgramModel GetRandomProgram()
        {
            var allFiles = new List<string>();
            foreach (var list in ProgramFiles.Values)
                allFiles.AddRange(list);

            Random rnd = new();
            string randomFile = allFiles[rnd.Next(allFiles.Count)];

            string path = GetFullPath(randomFile);
            return ProgramImporter.ImportFromFile(path);
        }
    }
}
