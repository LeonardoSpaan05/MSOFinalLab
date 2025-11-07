using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Lab2ProjectMSO.Models;
using Lab2ProjectMSO.Models.Commands;
using Lab2ProjectMSO.Models.Factory;
using Lab2ProjectMSO.Enum;
using Lab2ProjectMSO.Interfaces;
using Lab2ProjectMSO.Models.Pathfinding;
using Lab2ProjectMSO.Exceptions;
using Lab2ProjectMSO.Models.Execution;


namespace Lab2ProjectMSO
{
    public partial class MainForm : Form
    {
        private ProgramModel currentProgram;
        private Robot robot;
        private PathfindingGrid currentGrid;
        private int currentGridSize = 3;

        // lijst met harde oefeningen (geladen uit Data folder)
        private List<PathfindingExercise> exercises = new List<PathfindingExercise>();

        public MainForm()
        {
            InitializeComponent();

            currentProgram = new ProgramModel("UI Program", new List<ICommand>());
            robot = new Robot();

            SetupBlockButtons();
            SetupDragDrop();
            SetupGridButtons();
            SetupExerciseButton();
            SetupProgramSelector();

            dropAreaPanel.AutoScroll = true;

            DrawGrid(currentGridSize);

            runButton.Click += RunButton_Click;
            clearButton.Click += ClearButton_Click;
            metricsButton.Click += MetricsButton_Click;
            freeModeButton.Click += FreeModeButton_Click;

        }

        // Nieuwe knop voor laden van oefenbestanden (via vaste Data-folder) 
        private void SetupExerciseButton()
        {
            loadExerciseButton.Click += LoadExerciseButton_Click;
        }

        private void LoadExerciseButton_Click(object sender, EventArgs e)
        {
            try
            {
                string exercisesFolder = GetDataFolder();

                if (!Directory.Exists(exercisesFolder))
                {
                    MessageBox.Show($"Data folder niet gevonden:\n{exercisesFolder}",
                        "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string[] files = Directory.GetFiles(exercisesFolder, "*.txt");
                if (files.Length == 0)
                {
                    MessageBox.Show("Geen oefeningen gevonden in de Data-map.",
                        "Leeg", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                exercises.Clear();
                foreach (var file in files)
                {
                    string name = Path.GetFileNameWithoutExtension(file);
                    exercises.Add(new PathfindingExercise(name, file));
                }

                string[] exerciseNames = exercises.Select(e => e.Name).ToArray();
                string chosenExercise = ShowExerciseSelectionDialog(exerciseNames);
                if (string.IsNullOrEmpty(chosenExercise))
                    return;

                var exercise = exercises.Find(e => e.Name == chosenExercise);
                if (exercise != null)
                {
                    currentGrid = exercise.Grid;
                    currentGridSize = currentGrid.Rows;
                    robot = new Robot(currentGrid.Start.X, currentGrid.Start.Y);
                    DrawGrid(currentGridSize);
                    MessageBox.Show($"Oefening '{chosenExercise}' geladen.",
                        "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden van oefening:\n{ex.Message}",
                    "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private string GetDataFolder()
        {
            string dir = AppContext.BaseDirectory;

            while (dir != null && !Directory.Exists(Path.Combine(dir, "Data")))
            {
                dir = Directory.GetParent(dir)?.FullName;
            }

            if (dir == null)
                throw new DirectoryNotFoundException("De map 'Data' is niet gevonden in de projectstructuur.");

            return Path.Combine(dir, "Data");
        }


        private string ShowExerciseSelectionDialog(string[] options)
        {
            using (Form form = new Form())
            {
                form.Text = "Selecteer oefening";
                form.Size = new Size(300, 250);
                form.StartPosition = FormStartPosition.CenterScreen;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                Label lbl = new Label()
                {
                    Text = "Kies een oefening uit de lijst:",
                    Dock = DockStyle.Top,
                    Height = 30,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                form.Controls.Add(lbl);

                ListBox lb = new ListBox()
                {
                    Dock = DockStyle.Top,
                    Height = 150
                };
                lb.Items.AddRange(options);
                form.Controls.Add(lb);

                Button okBtn = new Button()
                {
                    Text = "OK",
                    Dock = DockStyle.Bottom,
                    Height = 35,
                    DialogResult = DialogResult.OK
                };
                form.Controls.Add(okBtn);
                form.AcceptButton = okBtn;

                if (form.ShowDialog() == DialogResult.OK && lb.SelectedItem != null)
                    return lb.SelectedItem.ToString();

                return null;
            }
        }

        private void SetupBlockButtons()
        {
            string[] blockNames = { "Move", "Turn Left", "Turn Right", "Repeat", "RepeatUntil" };
            foreach (var name in blockNames)
            {
                var blockLabel = new Label
                {
                    Text = name,
                    BackColor = Color.LightSteelBlue,
                    AutoSize = false,
                    Width = 100,
                    Height = 40,
                    TextAlign = ContentAlignment.MiddleCenter,
                    BorderStyle = BorderStyle.FixedSingle,
                    Margin = new Padding(5),
                    Cursor = Cursors.Hand
                };

                blockLabel.MouseDown += (s, e) =>
                {
                    if (e.Button == MouseButtons.Left)
                        blockLabel.DoDragDrop(blockLabel.Text, DragDropEffects.Copy);
                };

                blockPanel.Controls.Add(blockLabel);
            }
        }


        private void SetupDragDrop()
        {
            dropAreaPanel.AllowDrop = true;

            // DragEnter: bepaal effect bij drag
            dropAreaPanel.DragEnter += (s, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.Text))
                    e.Effect = DragDropEffects.Copy;
            };

            // DragDrop: verwerk zowel oude als nieuwe commando's
            dropAreaPanel.DragDrop += (s, e) =>
            {
                string cmdText = e.Data.GetData(DataFormats.Text)?.ToString() ?? "";
                if (string.IsNullOrEmpty(cmdText)) return;

                ICommand cmd;

                if (cmdText == "Repeat")
                {
                    int? times = ShowRepeatInputDialog(2);
                    if (!times.HasValue) return;
                    cmd = new RepeatCommand(times.Value);
                }
                else if (cmdText == "RepeatUntil")
                {
                    var condition = ShowRepeatUntilConditionDialog();
                    if (!condition.HasValue) return;
                    cmd = new RepeatUntilCommand(condition.Value, currentGrid);
                }
                else
                {
                    cmd = CommandFactory.CreateCommand(cmdText);
                }

                currentProgram.Commands.Add(cmd);
                RedrawBlocks();
            };
        }


// Grid-knoppen koppelen aan correcte grid sizes
        private void SetupGridButtons()
        {
            grid3x3Button.Click += Grid3x3Button_Click;
            grid4x4Button.Click += Grid4x4Button_Click;
            grid5x5Button.Click += Grid5x5Button_Click;
            grid6x6Button.Click += Grid6x6Button_Click;
        }

        private void Grid3x3Button_Click(object sender, EventArgs e)
        {
            currentGridSize = 3;
            currentGrid = new PathfindingGrid(currentGridSize);
            DrawGrid(currentGridSize);
        }

        private void Grid4x4Button_Click(object sender, EventArgs e)
        {
            currentGridSize = 4;
            currentGrid = new PathfindingGrid(currentGridSize);
            DrawGrid(currentGridSize);
        }

        private void Grid5x5Button_Click(object sender, EventArgs e)
        {
            currentGridSize = 5;
            currentGrid = new PathfindingGrid(currentGridSize);
            DrawGrid(currentGridSize);
        }

        private void Grid6x6Button_Click(object sender, EventArgs e)
        {
            currentGridSize = 6;
            currentGrid = new PathfindingGrid(currentGridSize);
            DrawGrid(currentGridSize);
        }


        private void RedrawBlocks()
        {
            dropAreaPanel.Controls.Clear();
            DrawBlockList(currentProgram.Commands, dropAreaPanel, 0);
        }

        private void DrawBlockList(IList<ICommand> commands, FlowLayoutPanel parentPanel, int indent)
        {
            foreach (var cmd in commands)
            {
                var block = CreateBlockUI(cmd, indent);
                parentPanel.Controls.Add(block);

                // Recursief nested commands tekenen voor RepeatCommand
                if (cmd is RepeatCommand repeatCmd && repeatCmd.Commands.Any())
                {
                    DrawBlockList(repeatCmd.Commands, parentPanel, indent + 1);
                }
                // Recursief nested commands tekenen voor RepeatUntilCommand
                else if (cmd is RepeatUntilCommand repeatUntilCmd && repeatUntilCmd.Commands.Any())
                {
                    DrawBlockList(repeatUntilCmd.Commands, parentPanel, indent + 1);
                }
            }
        }


        private Panel CreateBlockUI(ICommand cmd, int indentLevel)
        {
            int indentPixels = indentLevel * 30;

            Panel panel = new Panel
            {
                Width = Math.Max(100, dropAreaPanel.ClientSize.Width - 40),
                Height = 40,
                BackColor = cmd is RepeatCommand || cmd is RepeatUntilCommand ? Color.LightCoral : Color.LightBlue,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(indentPixels + 5, 3, 5, 3),
                Tag = cmd
            };

            Label label = new Label
            {
                Text = cmd switch
                {
                    TurnCommand turn => turn.Direction == TurnDirection.Left ? "Turn Left" : "Turn Right",
                    RepeatCommand repeat => $"Repeat {repeat.Times}",
                    RepeatUntilCommand repeatUntil => $"RepeatUntil {repeatUntil.Condition}",
                    _ => cmd.ToString()
                },
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

            panel.Controls.Add(label);

            ContextMenuStrip cms = new ContextMenuStrip();
            cms.Items.Add("Delete", null, (s, e) =>
            {
                RemoveCommandFromModel(cmd, currentProgram.Commands);
                RedrawBlocks();
            });

            // Voeg menu-items toe voor RepeatCommand en RepeatUntilCommand
            if (cmd is RepeatCommand repeatCmd)
            {
                cms.Items.Add("Set Repeat Count", null, (s, e) =>
                {
                    int? result = ShowRepeatInputDialog(repeatCmd.Times);
                    if (result.HasValue)
                    {
                        repeatCmd.Times = result.Value;
                        RedrawBlocks();
                    }
                });
            }
            else if (cmd is RepeatUntilCommand repeatUntilCmd)
            {
                cms.Items.Add("Set Condition", null, (s, e) =>
                {
                    var result = ShowRepeatUntilConditionDialog();
                    if (result.HasValue)
                    {
                        repeatUntilCmd.Condition = result.Value;
                        RedrawBlocks();
                    }
                });
            }

            panel.ContextMenuStrip = cms;

            // Enable drag & drop voor nested commands
            panel.AllowDrop = true;
            panel.DragEnter += (s, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.Text))
                    e.Effect = DragDropEffects.Copy;
            };
            panel.DragDrop += (s, e) =>
            {
                if (cmd is not RepeatCommand && cmd is not RepeatUntilCommand)
                {
                    MessageBox.Show("You can only drop commands on a Repeat or RepeatUntil block.");
                    return;
                }

                string cmdText = e.Data.GetData(DataFormats.Text)?.ToString() ?? "";
                if (string.IsNullOrEmpty(cmdText)) return;

                ICommand nested;

                if (cmdText == "Repeat")
                {
                    nested = new RepeatCommand(2); // je kunt hier ook een inputdialog tonen
                }
                else if (cmdText == "RepeatUntil")
                {
                    var condition = ShowRepeatUntilConditionDialog();
                    if (!condition.HasValue) return;
                    nested = new RepeatUntilCommand(condition.Value, currentGrid);
                }
                else
                {
                    nested = CommandFactory.CreateCommand(cmdText);
                }

                if (cmd is RepeatCommand r) r.Commands.Add(nested);
                if (cmd is RepeatUntilCommand ru) ru.Commands.Add(nested);

                RedrawBlocks();
            };


            return panel;
        }


        private int? ShowRepeatInputDialog(int currentValue)
        {
            Form inputForm = new Form
            {
                Text = "Set Repeat Count",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,
                ClientSize = new Size(250, 130),
                MinimizeBox = false,
                MaximizeBox = false,
                TopMost = true
            };

            Label lbl = new Label()
            {
                Text = "Number of repetitions:",
                Left = 15,
                Top = 20,
                AutoSize = true
            };

            TextBox txt = new TextBox()
            {
                Left = 15,
                Top = 50,
                Width = 210,
                Text = currentValue.ToString()
            };

            Button okBtn = new Button()
                { Text = "OK", Left = 50, Width = 60, Top = 80, DialogResult = DialogResult.OK };
            Button cancelBtn = new Button()
                { Text = "Cancel", Left = 130, Width = 60, Top = 80, DialogResult = DialogResult.Cancel };

            inputForm.Controls.Add(lbl);
            inputForm.Controls.Add(txt);
            inputForm.Controls.Add(okBtn);
            inputForm.Controls.Add(cancelBtn);
            inputForm.AcceptButton = okBtn;
            inputForm.CancelButton = cancelBtn;

            DialogResult result = inputForm.ShowDialog(this);
            if (result == DialogResult.OK && int.TryParse(txt.Text, out int n) && n > 0)
                return n;
            return null;
        }

        private bool RemoveCommandFromModel(ICommand cmd, IList<ICommand> list)
        {
            if (list.Remove(cmd))
                return true;

            foreach (var repeat in list.OfType<RepeatCommand>())
                if (RemoveCommandFromModel(cmd, repeat.Commands))
                    return true;

            return false;
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete all blocks?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                currentProgram.Commands.Clear();
                dropAreaPanel.Controls.Clear();
                robot.Reset();
                DrawGrid(currentGridSize);
            }
        }

        private async void RunButton_Click(object sender, EventArgs e)
        {
            if (!currentProgram.Commands.Any())
            {
                MessageBox.Show("No commands to run.");
                return;
            }

            bool isExerciseMode = exercises.Any(e => e.Grid == currentGrid); // check of er een oefening geladen is
            bool hitWall = false;

            // Als er geen oefening geladen is, maak een leeg grid aan
            if (!isExerciseMode)
            {
                // Gebruik de huidige gridgrootte van de gebruiker
                if (currentGrid == null || currentGrid.Rows != currentGridSize)
                {
                    currentGrid = new PathfindingGrid(currentGridSize);
                }

                robot = new Robot(0, 0);
            }
            else
            {
                // Gebruik de grid uit de oefening, NIET overschrijven
                robot = new Robot(currentGrid.Start.X, currentGrid.Start.Y);
            }


            DrawGrid(currentGrid.Rows);

            var executor = new ProgramExecutor(currentGrid);
            outputTextBox.Clear();

            try
            {
                // voer elk commando uit met animatie
                foreach (var cmd in currentProgram.Commands)
                {
                    try
                    {
                        await ExecuteCommandRecursive(cmd, executor, robot, currentGrid);

                    }
                    catch (Exception ex)
                    {
                        // Als robot een muur raakt, markeer dit
                        if (ex is OutOfBoundsException || ex is BlockedCellException)
                            hitWall = true;

                        throw; // stop uitvoering
                    }
                }

                if (isExerciseMode)
                {
                    if (hitWall || !currentGrid.IsEnd(robot.X, robot.Y))
                    {
                        MessageBox.Show(
                            "Je hebt de opdracht niet gehaald. Probeer het opnieuw!",
                            "Oefening niet gehaald",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show(
                            "Gefeliciteerd! Je hebt de opdracht gehaald!",
                            "Succes",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }

                outputTextBox.AppendText("Program finished.\r\n");
            }
            catch (Exception ex)
            {
                outputTextBox.AppendText($"Runtime error: {ex.Message}\r\n");

                if (isExerciseMode)
                {
                    MessageBox.Show(
                        "Je hebt de opdracht niet gehaald. Probeer het opnieuw!",
                        "Oefening niet gehaald",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
        }


        // Hulpmethode voor stapsgewijze uitvoering inclusief RepeatCommand
        private async System.Threading.Tasks.Task ExecuteCommandRecursive(
            ICommand cmd, ProgramExecutor executor, Robot robot, PathfindingGrid grid)
        {
            switch (cmd)
            {
                case RepeatCommand repeat:
                    for (int i = 0; i < repeat.Times; i++)
                        foreach (var nested in repeat.Commands)
                            await ExecuteCommandRecursive(nested, executor, robot, grid);
                    break;

                case RepeatUntilCommand repeatUntil:
                    while (!executor.CheckCondition(repeatUntil.Condition, robot, grid))
                        foreach (var nested in repeatUntil.Commands)
                            await ExecuteCommandRecursive(nested, executor, robot, grid);
                    break;

                default:
                    executor.ExecuteCommand(cmd, robot, grid); // nu met grid
                    DrawGrid(grid.Rows); // jouw tekenmethode voor animatie
                    await System.Threading.Tasks.Task.Delay(300); // animatie delay
                    break;
            }
        }





        private void MetricsButton_Click(object sender, EventArgs e)
        {
            int moveCount = CountCommands(currentProgram.Commands, typeof(MoveCommand));
            int turnCount = CountCommands(currentProgram.Commands, typeof(TurnCommand));
            int repeatCount = CountRepeats(currentProgram.Commands);

            outputTextBox.Text = $"Move: {moveCount}\r\nTurn: {turnCount}\r\nRepeat: {repeatCount}";
        }

        // Telt commando’s inclusief herhalingen
        private int CountCommands(IList<ICommand> list, Type type, int multiplier = 1)
        {
            int count = 0;
            foreach (var cmd in list)
            {
                if (cmd.GetType() == type)
                    count += multiplier;

                if (cmd is RepeatCommand repeat)
                    count += CountCommands(repeat.Commands, type, multiplier * repeat.Times);
                else if (cmd is RepeatUntilCommand repeatUntil)
                    count += CountCommands(repeatUntil.Commands, type, multiplier); 
                // RepeatUntil loopt totdat een conditie klopt, dus we tellen hier gewoon 1x per instance
            }
            return count;
        }

        // Telt alle Repeat-achtige commando's
        private int CountRepeats(IList<ICommand> list)
        {
            int count = 0;
            foreach (var cmd in list)
            {
                if (cmd is RepeatCommand || cmd is RepeatUntilCommand)
                    count++;

                if (cmd is RepeatCommand repeat)
                    count += CountRepeats(repeat.Commands);
                else if (cmd is RepeatUntilCommand repeatUntil)
                    count += CountRepeats(repeatUntil.Commands);
            }
            return count;
        }



        // Tekent het rooster; als currentGrid is gezet, gebruikt het de cellen ('+', 'o', 'x')
        private void DrawGrid(int size)
        {
            if (visualizationArea == null || size <= 0)
                return;

            currentGridSize = size;
            int cellSize = Math.Max(4, Math.Min(visualizationArea.Width / size, visualizationArea.Height / size));
            Bitmap bmp = new Bitmap(visualizationArea.Width, visualizationArea.Height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);

                // teken grid met obstakels en eindpunt
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        Rectangle rect = new Rectangle(x * cellSize, y * cellSize, cellSize, cellSize);
                        if (currentGrid != null && y < currentGrid.Rows && x < currentGrid.Cols)
                        {
                            char c = currentGrid.Cells[y, x];
                            if (c == '+') g.FillRectangle(Brushes.DarkGray, rect);
                            else if (c == 'x') g.FillRectangle(Brushes.LightGreen, rect);
                            else g.FillRectangle(Brushes.White, rect);
                        }
                        else
                        {
                            g.FillRectangle(Brushes.White, rect);
                        }

                        g.DrawRectangle(Pens.Black, rect);
                    }
                }

                // teken robotpad
                if (robot != null && robot.Path.Count > 1)
                {
                    using (Pen pathPen = new Pen(Color.Blue, 3))
                    {
                        for (int i = 1; i < robot.Path.Count; i++)
                        {
                            var prev = robot.Path[i - 1];
                            var curr = robot.Path[i];
                            g.DrawLine(pathPen,
                                prev.X * cellSize + cellSize / 2,
                                prev.Y * cellSize + cellSize / 2,
                                curr.X * cellSize + cellSize / 2,
                                curr.Y * cellSize + cellSize / 2);
                        }
                    }
                }

                // teken robot zelf
                if (robot != null)
                {
                    var pos = new Point(robot.X, robot.Y);
                    int centerX = pos.X * cellSize + cellSize / 2;
                    int centerY = pos.Y * cellSize + cellSize / 2;
                    int radius = Math.Max(2, cellSize / 4);

                    g.FillEllipse(Brushes.Red, centerX - radius, centerY - radius, radius * 2, radius * 2);
                    g.DrawEllipse(Pens.Black, centerX - radius, centerY - radius, radius * 2, radius * 2);

                    int tipSize = 5;
                    int offset = radius + 3;
                    int tipX = centerX, tipY = centerY;
                    switch (robot.Facing)
                    {
                        case Direction.North:
                            tipY -= offset;
                            break;
                        case Direction.East:
                            tipX += offset;
                            break;
                        case Direction.South:
                            tipY += offset;
                            break;
                        case Direction.West:
                            tipX -= offset;
                            break;
                    }

                    g.FillEllipse(Brushes.Black, tipX - tipSize / 2, tipY - tipSize / 2, tipSize, tipSize);
                }
            }

            visualizationArea.Image = bmp;
        }

        private void FreeModeButton_Click(object sender, EventArgs e)
        {
            currentGrid = null; // geen oefening meer
            currentProgram.Commands.Clear(); // reset alle commando's
            dropAreaPanel.Controls.Clear();
            robot = new Robot(); // nieuwe robot
            DrawGrid(currentGridSize); // teken leeg grid
            MessageBox.Show("Je bent nu in Free Mode.", "Free Mode", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }



        private void SetupProgramSelector()
        {
            // Voeg programma-categorieën toe aan dropdown
            programSelector.Items.Clear();
            programSelector.Items.AddRange(new object[] { "Basic", "Advanced", "Expert" });

            programSelector.SelectedIndexChanged += ProgramSelector_SelectedIndexChanged;
        }

        private void ProgramSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Ontkoppel event tijdelijk om dubbele pop-ups te voorkomen
            programSelector.SelectedIndexChanged -= ProgramSelector_SelectedIndexChanged;

            try
            {
                string selectedLevel = programSelector.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(selectedLevel))
                    return;

                string programsFolder = Path.Combine(
                    Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)
                        .Parent.Parent.Parent.FullName, "Programs");

                if (!Directory.Exists(programsFolder))
                {
                    MessageBox.Show($"Programs folder niet gevonden:\n{programsFolder}",
                        "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string levelPath = Path.Combine(programsFolder, selectedLevel);
                if (!Directory.Exists(levelPath))
                {
                    MessageBox.Show($"Geen map gevonden voor '{selectedLevel}'.",
                        "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string[] files = Directory.GetFiles(levelPath, "*.txt");
                if (files.Length == 0)
                {
                    MessageBox.Show($"Geen programma’s gevonden in '{selectedLevel}'.",
                        "Leeg", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Laat gebruiker kiezen welk programma te laden
                string[] programNames = files.Select(f => Path.GetFileNameWithoutExtension(f)).ToArray();
                string chosen = ShowExerciseSelectionDialog(programNames);
                if (string.IsNullOrEmpty(chosen))
                    return;

                string chosenPath = Path.Combine(levelPath, chosen + ".txt");
                LoadProgramFromFile(chosenPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden van programma's:\n{ex.Message}",
                    "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Koppel event weer terug
                programSelector.SelectedIndexChanged += ProgramSelector_SelectedIndexChanged;
            }
        }

        private void LoadProgramFromFile(string filePath)
        {
            currentProgram.Commands.Clear();
            dropAreaPanel.Controls.Clear();

            var lines = File.ReadAllLines(filePath)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToList();

            Stack<IList<ICommand>> cmdStack = new Stack<IList<ICommand>>();
            cmdStack.Push(currentProgram.Commands);

            Stack<int> indentStack = new Stack<int>();
            indentStack.Push(0);

            foreach (string rawLine in lines)
            {
                int indent = rawLine.TakeWhile(char.IsWhiteSpace).Count();
                string line = rawLine.Trim();

                while (indent < indentStack.Peek() && cmdStack.Count > 1)
                {
                    cmdStack.Pop();
                    indentStack.Pop();
                }

                if (line.StartsWith("Repeat", StringComparison.OrdinalIgnoreCase))
                {
                    int repeatTimes = 2;
                    var parts = line.Split(' ');
                    if (parts.Length >= 2 && int.TryParse(parts[1], out int n))
                        repeatTimes = n;

                    var repeatCmd = new RepeatCommand(repeatTimes);
                    cmdStack.Peek().Add(repeatCmd);
                    cmdStack.Push(repeatCmd.Commands);
                    indentStack.Push(indent + 4);
                }
                else
                {
                    ICommand cmd = CommandFactory.CreateCommand(line);
                    cmdStack.Peek().Add(cmd);
                }
            }

            RedrawBlocks();
        }
        
        private RepeatUntilCondition? ShowRepeatUntilConditionDialog()
        {
            using (Form form = new Form())
            {
                form.Text = "Select Condition";
                form.Size = new Size(250, 150);
                form.StartPosition = FormStartPosition.CenterParent;

                ComboBox cb = new ComboBox() { Left = 20, Top = 20, Width = 200 };
                cb.Items.AddRange(new object[] { RepeatUntilCondition.WallAhead, RepeatUntilCondition.GridEdge });
                cb.SelectedIndex = 0;

                Button okBtn = new Button() { Text = "OK", Left = 40, Width = 60, Top = 60, DialogResult = DialogResult.OK };
                Button cancelBtn = new Button() { Text = "Cancel", Left = 120, Width = 60, Top = 60, DialogResult = DialogResult.Cancel };

                form.Controls.Add(cb); form.Controls.Add(okBtn); form.Controls.Add(cancelBtn);
                form.AcceptButton = okBtn; form.CancelButton = cancelBtn;

                if (form.ShowDialog(this) == DialogResult.OK)
                    return (RepeatUntilCondition)cb.SelectedItem;

                return null;
            }
        }









    }





}
