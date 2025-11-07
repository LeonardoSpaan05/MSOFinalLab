using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Lab2ProjectMSO
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel leftMenuPanel;
        private ComboBox programSelector;
        private Button loadExerciseButton;
        private Button freeModeButton;
        private FlowLayoutPanel blockPanel;
        private FlowLayoutPanel dropAreaPanel;
        private Button runButton;
        private Button metricsButton;
        private Button clearButton;
        private Button grid3x3Button;
        private Button grid4x4Button;
        private Button grid5x5Button;
        private Button grid6x6Button;
        private TextBox outputTextBox;
        private PictureBox visualizationArea;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.leftMenuPanel = new Panel();
            this.programSelector = new ComboBox();
            this.loadExerciseButton = new Button();
            this.freeModeButton = new Button();
            this.blockPanel = new FlowLayoutPanel();
            this.dropAreaPanel = new FlowLayoutPanel();
            this.runButton = new Button();
            this.metricsButton = new Button();
            this.clearButton = new Button();
            this.grid3x3Button = new Button();
            this.grid4x4Button = new Button();
            this.grid5x5Button = new Button();
            this.grid6x6Button = new Button();
            this.outputTextBox = new TextBox();
            this.visualizationArea = new PictureBox();

            this.SuspendLayout();

            // Form settings
            this.Text = "Program Builder";
            this.ClientSize = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Left menu panel
            this.leftMenuPanel.Location = new Point(10, 10);
            this.leftMenuPanel.Size = new Size(200, 680);
            this.leftMenuPanel.BackColor = Color.LightGray;
            this.Controls.Add(this.leftMenuPanel);

            // Program selector
            this.programSelector.Items.AddRange(new object[] { "Basic", "Advanced", "Expert", "From File" });
            this.programSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            this.programSelector.Location = new Point(20, 20);
            this.programSelector.Width = 160;
            this.programSelector.SelectedIndexChanged += ProgramSelector_SelectedIndexChanged; 
            this.leftMenuPanel.Controls.Add(this.programSelector);

            // Load exercise button
            this.loadExerciseButton.Text = "Load Exercise";
            this.loadExerciseButton.Location = new Point(20, 60);
            this.loadExerciseButton.Width = 160;
            this.leftMenuPanel.Controls.Add(this.loadExerciseButton);

            // Free Mode button
            this.freeModeButton.Text = "Free Mode";
            this.freeModeButton.Location = new Point(20, 100);
            this.freeModeButton.Width = 160;
            this.leftMenuPanel.Controls.Add(this.freeModeButton);

            // Block panel
            this.blockPanel.Location = new Point(230, 10);
            this.blockPanel.Size = new Size(500, 100);
            this.blockPanel.FlowDirection = FlowDirection.LeftToRight;
            this.Controls.Add(this.blockPanel);

            // Drop area panel
            this.dropAreaPanel.Location = new Point(230, 130);
            this.dropAreaPanel.Size = new Size(500, 300);
            this.dropAreaPanel.BorderStyle = BorderStyle.FixedSingle;
            this.dropAreaPanel.AllowDrop = true;
            this.dropAreaPanel.FlowDirection = FlowDirection.TopDown;
            this.dropAreaPanel.WrapContents = false;
            this.dropAreaPanel.AutoScroll = true;
            this.Controls.Add(this.dropAreaPanel);

            // Run button
            this.runButton.Text = "Run";
            this.runButton.Location = new Point(230, 450);
            this.runButton.Size = new Size(100, 40);
            this.Controls.Add(this.runButton);

            // Metrics button
            this.metricsButton.Text = "Metrics";
            this.metricsButton.Location = new Point(340, 450);
            this.metricsButton.Size = new Size(100, 40);
            this.Controls.Add(this.metricsButton);

            // Clear button
            this.clearButton.Text = "Clear All";
            this.clearButton.Location = new Point(450, 450);
            this.clearButton.Size = new Size(100, 40);
            this.Controls.Add(this.clearButton);

            // Grid buttons
            this.grid3x3Button.Text = "Grid 3x3";
            this.grid3x3Button.Location = new Point(230, 500);
            this.grid3x3Button.Size = new Size(100, 40);
            this.Controls.Add(this.grid3x3Button);

            this.grid4x4Button.Text = "Grid 4x4";
            this.grid4x4Button.Location = new Point(340, 500);
            this.grid4x4Button.Size = new Size(100, 40);
            this.Controls.Add(this.grid4x4Button);

            this.grid5x5Button.Text = "Grid 5x5";
            this.grid5x5Button.Location = new Point(450, 500);
            this.grid5x5Button.Size = new Size(100, 40);
            this.Controls.Add(this.grid5x5Button);

            this.grid6x6Button.Text = "Grid 6x6";
            this.grid6x6Button.Location = new Point(560, 500);
            this.grid6x6Button.Size = new Size(100, 40);
            this.Controls.Add(this.grid6x6Button);

            // Output box
            this.outputTextBox.Location = new Point(230, 560);
            this.outputTextBox.Size = new Size(500, 130);
            this.outputTextBox.Multiline = true;
            this.outputTextBox.ScrollBars = ScrollBars.Vertical;
            this.Controls.Add(this.outputTextBox);

            // Visualization area
            this.visualizationArea.Location = new Point(750, 10);
            this.visualizationArea.Size = new Size(430, 680);
            this.visualizationArea.BackColor = Color.WhiteSmoke;
            this.visualizationArea.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(this.visualizationArea);

            this.ResumeLayout(false);
        }

        

    }
}
