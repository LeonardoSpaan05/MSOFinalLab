using System;
using System.Windows.Forms;

namespace Lab2ProjectMSO
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Windows Forms configuratie (voor .NET 6+)
            ApplicationConfiguration.Initialize();
            
            // Start de MainForm
            Application.Run(new MainForm());
        }
    }
}