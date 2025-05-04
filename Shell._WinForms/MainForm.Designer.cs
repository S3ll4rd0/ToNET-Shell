namespace Shell._WinForms
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Shell = new RichTextBox();
            Monitor = new RichTextBox();
            SuspendLayout();
            // 
            // Shell
            // 
            Shell.BackColor = Color.Black;
            Shell.BorderStyle = BorderStyle.None;
            Shell.Font = new Font("Consolas", 12F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            Shell.ForeColor = Color.Lime;
            Shell.Location = new Point(0, 0);
            Shell.Margin = new Padding(4);
            Shell.Name = "Shell";
            Shell.Size = new Size(780, 470);
            Shell.TabIndex = 0;
            Shell.Text = "";
            Shell.TextChanged += Shell_TextChanged;
            Shell.KeyDown += Shell_KeyDown;
            // 
            // Monitor
            // 
            Monitor.BackColor = Color.Black;
            Monitor.BorderStyle = BorderStyle.None;
            Monitor.Font = new Font("Consolas", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Monitor.ForeColor = Color.LightPink;
            Monitor.Location = new Point(0, 477);
            Monitor.Name = "Monitor";
            Monitor.ReadOnly = true;
            Monitor.Size = new Size(780, 80);
            Monitor.TabIndex = 1;
            Monitor.Text = "";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(780, 557);
            Controls.Add(Monitor);
            Controls.Add(Shell);
            Font = new Font("Consolas", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ForeColor = SystemColors.ControlText;
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Margin = new Padding(4);
            Name = "MainForm";
            Text = "DotNet Shell by Antonio Nicolás Salmerón Rubio (S3||4rd0)";
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox Shell;
        private RichTextBox Monitor;
    }
}
