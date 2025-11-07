using System;
using System.Collections.Generic;
using System.Drawing;
using Lab2ProjectMSO.Models;
using Lab2ProjectMSO.Models.Execution;
using Lab2ProjectMSO.Utils;
using Lab2ProjectMSO.Models.Factory;

namespace Lab2ProjectMSO
{
    public class ProgramPresenter
    {
        private readonly ProgramModel _model;
        private readonly ProgramExecutor _executor;
        private readonly Robot _robot;

        public event Action<string>? OnOutputUpdated;
        public event Action<List<Point>>? OnCanvasUpdated;

        public ProgramPresenter(ProgramModel model, ProgramExecutor executor, Robot robot)
        {
            _model = model;
            _executor = executor;
            _robot = robot;
        }

        public void RunProgram()
        {
            if (_model.Commands.Count == 0)
            {
                OnOutputUpdated?.Invoke("No commands to execute.");
                return;
            }

            // pad resetten voor nieuwe run
            _robot.Reset();

            ExecutionReport report = _executor.Execute(_model);
            OnOutputUpdated?.Invoke(report.ToString());

            // geef pad door aan UI
            OnCanvasUpdated?.Invoke(new List<Point>(_robot.Path));
        }

        public void AddCommand(string cmdText)
        {
            var cmd = CommandFactory.CreateCommand(cmdText);
            _model.Commands.Add(cmd);
        }

        public void ClearProgram()
        {
            _model.Commands.Clear();
            _robot.Reset();
            OnCanvasUpdated?.Invoke(new List<Point>()); // lege lijst voor UI
        }

        public void LoadProgram(string file)
        {
            var loaded = ProgramImporter.ImportFromFile(file);
            _model.Commands.Clear();
            _model.Commands.AddRange(loaded.Commands);
        }
    }
}