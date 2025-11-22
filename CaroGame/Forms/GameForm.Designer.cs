namespace CaroGame.Winforms.Forms
{
    partial class GameForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pnlToolbar = new Panel();
            btnMenu = new Button();
            btnLoad = new Button();
            btnSave = new Button();
            btnHint = new Button();
            btnNewGame = new Button();
            btnRedo = new Button();
            btnUndo = new Button();
            pnlPlayer1Info = new Panel();
            lblPlayer1Hints = new Label();
            pbPlayer1Time = new ProgressBar();
            lblPlayer1Name = new Label();
            pnlPlayer2Info = new Panel();
            lblPlayer2Hints = new Label();
            pbPlayer2Time = new ProgressBar();
            lblPlayer2Name = new Label();
            pnlBoard = new Panel();
            gameTimer = new System.Windows.Forms.Timer(components);
            pnlToolbar.SuspendLayout();
            pnlPlayer1Info.SuspendLayout();
            pnlPlayer2Info.SuspendLayout();
            SuspendLayout();
            // 
            // pnlToolbar
            // 
            pnlToolbar.BackColor = Color.FromArgb(52, 73, 94);
            pnlToolbar.Controls.Add(btnMenu);
            pnlToolbar.Controls.Add(btnLoad);
            pnlToolbar.Controls.Add(btnSave);
            pnlToolbar.Controls.Add(btnHint);
            pnlToolbar.Controls.Add(btnNewGame);
            pnlToolbar.Controls.Add(btnRedo);
            pnlToolbar.Controls.Add(btnUndo);
            pnlToolbar.Dock = DockStyle.Top;
            pnlToolbar.Location = new Point(0, 0);
            pnlToolbar.Name = "pnlToolbar";
            pnlToolbar.Size = new Size(800, 59);
            pnlToolbar.TabIndex = 0;
            // 
            // btnMenu
            // 
            btnMenu.BackColor = Color.FromArgb(41, 128, 185);
            btnMenu.Cursor = Cursors.Hand;
            btnMenu.FlatAppearance.BorderSize = 0;
            btnMenu.FlatStyle = FlatStyle.Flat;
            btnMenu.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            btnMenu.ForeColor = Color.White;
            btnMenu.Location = new Point(708, 12);
            btnMenu.Name = "btnMenu";
            btnMenu.Size = new Size(80, 35);
            btnMenu.TabIndex = 0;
            btnMenu.Text = "Menu";
            btnMenu.UseVisualStyleBackColor = false;
            btnMenu.Click += btnMenu_Click;
            // 
            // btnLoad
            // 
            btnLoad.BackColor = Color.FromArgb(41, 128, 185);
            btnLoad.Cursor = Cursors.Hand;
            btnLoad.FlatAppearance.BorderSize = 0;
            btnLoad.FlatStyle = FlatStyle.Flat;
            btnLoad.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            btnLoad.ForeColor = Color.White;
            btnLoad.Location = new Point(442, 12);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(80, 35);
            btnLoad.TabIndex = 0;
            btnLoad.Text = "Load";
            btnLoad.UseVisualStyleBackColor = false;
            btnLoad.Click += btnLoad_Click;
            // 
            // btnSave
            // 
            btnSave.BackColor = Color.FromArgb(41, 128, 185);
            btnSave.Cursor = Cursors.Hand;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            btnSave.ForeColor = Color.White;
            btnSave.Location = new Point(356, 12);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(80, 35);
            btnSave.TabIndex = 0;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;
            // 
            // btnHint
            // 
            btnHint.BackColor = Color.FromArgb(41, 128, 185);
            btnHint.Cursor = Cursors.Hand;
            btnHint.FlatAppearance.BorderSize = 0;
            btnHint.FlatStyle = FlatStyle.Flat;
            btnHint.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            btnHint.ForeColor = Color.White;
            btnHint.Location = new Point(270, 12);
            btnHint.Name = "btnHint";
            btnHint.Size = new Size(80, 35);
            btnHint.TabIndex = 0;
            btnHint.Text = "Hint";
            btnHint.UseVisualStyleBackColor = false;
            btnHint.Click += btnHint_Click;
            // 
            // btnNewGame
            // 
            btnNewGame.BackColor = Color.FromArgb(41, 128, 185);
            btnNewGame.Cursor = Cursors.Hand;
            btnNewGame.FlatAppearance.BorderSize = 0;
            btnNewGame.FlatStyle = FlatStyle.Flat;
            btnNewGame.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            btnNewGame.ForeColor = Color.White;
            btnNewGame.Location = new Point(184, 12);
            btnNewGame.Name = "btnNewGame";
            btnNewGame.Size = new Size(80, 35);
            btnNewGame.TabIndex = 0;
            btnNewGame.Text = "New Game";
            btnNewGame.UseVisualStyleBackColor = false;
            btnNewGame.Click += btnNewGame_Click;
            // 
            // btnRedo
            // 
            btnRedo.BackColor = Color.FromArgb(41, 128, 185);
            btnRedo.Cursor = Cursors.Hand;
            btnRedo.FlatAppearance.BorderSize = 0;
            btnRedo.FlatStyle = FlatStyle.Flat;
            btnRedo.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            btnRedo.ForeColor = Color.White;
            btnRedo.Location = new Point(98, 12);
            btnRedo.Name = "btnRedo";
            btnRedo.Size = new Size(80, 35);
            btnRedo.TabIndex = 0;
            btnRedo.Text = "Redo";
            btnRedo.UseVisualStyleBackColor = false;
            btnRedo.Click += btnRedo_Click;
            // 
            // btnUndo
            // 
            btnUndo.BackColor = Color.FromArgb(41, 128, 185);
            btnUndo.Cursor = Cursors.Hand;
            btnUndo.FlatAppearance.BorderSize = 0;
            btnUndo.FlatStyle = FlatStyle.Flat;
            btnUndo.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            btnUndo.ForeColor = Color.White;
            btnUndo.Location = new Point(12, 12);
            btnUndo.Name = "btnUndo";
            btnUndo.Size = new Size(80, 35);
            btnUndo.TabIndex = 0;
            btnUndo.Text = "Undo";
            btnUndo.UseVisualStyleBackColor = false;
            btnUndo.Click += btnUndo_Click;
            // 
            // pnlPlayer1Info
            // 
            pnlPlayer1Info.BorderStyle = BorderStyle.FixedSingle;
            pnlPlayer1Info.Controls.Add(lblPlayer1Hints);
            pnlPlayer1Info.Controls.Add(pbPlayer1Time);
            pnlPlayer1Info.Controls.Add(lblPlayer1Name);
            pnlPlayer1Info.Location = new Point(35, 71);
            pnlPlayer1Info.Name = "pnlPlayer1Info";
            pnlPlayer1Info.Size = new Size(350, 80);
            pnlPlayer1Info.TabIndex = 1;
            // 
            // lblPlayer1Hints
            // 
            lblPlayer1Hints.AutoSize = true;
            lblPlayer1Hints.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lblPlayer1Hints.ForeColor = Color.FromArgb(127, 140, 141);
            lblPlayer1Hints.Location = new Point(159, 16);
            lblPlayer1Hints.Name = "lblPlayer1Hints";
            lblPlayer1Hints.Size = new Size(86, 21);
            lblPlayer1Hints.TabIndex = 2;
            lblPlayer1Hints.Text = "Hints: 3/3";
            // 
            // pbPlayer1Time
            // 
            pbPlayer1Time.Location = new Point(10, 42);
            pbPlayer1Time.Maximum = 180;
            pbPlayer1Time.Name = "pbPlayer1Time";
            pbPlayer1Time.Size = new Size(330, 20);
            pbPlayer1Time.Style = ProgressBarStyle.Continuous;
            pbPlayer1Time.TabIndex = 1;
            pbPlayer1Time.Value = 180;
            // 
            // lblPlayer1Name
            // 
            lblPlayer1Name.AutoSize = true;
            lblPlayer1Name.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblPlayer1Name.ForeColor = Color.FromArgb(52, 73, 94);
            lblPlayer1Name.Location = new Point(10, 10);
            lblPlayer1Name.Name = "lblPlayer1Name";
            lblPlayer1Name.Size = new Size(143, 29);
            lblPlayer1Name.TabIndex = 0;
            lblPlayer1Name.Text = "Player 1 (X)";
            // 
            // pnlPlayer2Info
            // 
            pnlPlayer2Info.BorderStyle = BorderStyle.FixedSingle;
            pnlPlayer2Info.Controls.Add(lblPlayer2Hints);
            pnlPlayer2Info.Controls.Add(pbPlayer2Time);
            pnlPlayer2Info.Controls.Add(lblPlayer2Name);
            pnlPlayer2Info.Location = new Point(416, 71);
            pnlPlayer2Info.Name = "pnlPlayer2Info";
            pnlPlayer2Info.Size = new Size(350, 80);
            pnlPlayer2Info.TabIndex = 2;
            // 
            // lblPlayer2Hints
            // 
            lblPlayer2Hints.AutoSize = true;
            lblPlayer2Hints.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lblPlayer2Hints.ForeColor = Color.FromArgb(127, 140, 141);
            lblPlayer2Hints.Location = new Point(159, 16);
            lblPlayer2Hints.Name = "lblPlayer2Hints";
            lblPlayer2Hints.Size = new Size(86, 21);
            lblPlayer2Hints.TabIndex = 2;
            lblPlayer2Hints.Text = "Hints: 3/3";
            // 
            // pbPlayer2Time
            // 
            pbPlayer2Time.Location = new Point(10, 42);
            pbPlayer2Time.Maximum = 180;
            pbPlayer2Time.Name = "pbPlayer2Time";
            pbPlayer2Time.Size = new Size(330, 20);
            pbPlayer2Time.Style = ProgressBarStyle.Continuous;
            pbPlayer2Time.TabIndex = 1;
            pbPlayer2Time.Value = 180;
            // 
            // lblPlayer2Name
            // 
            lblPlayer2Name.AutoSize = true;
            lblPlayer2Name.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblPlayer2Name.ForeColor = Color.FromArgb(52, 73, 94);
            lblPlayer2Name.Location = new Point(10, 10);
            lblPlayer2Name.Name = "lblPlayer2Name";
            lblPlayer2Name.Size = new Size(145, 29);
            lblPlayer2Name.TabIndex = 0;
            lblPlayer2Name.Text = "Player 2 (O)";
            // 
            // pnlBoard
            // 
            pnlBoard.Location = new Point(30, 168);
            pnlBoard.Name = "pnlBoard";
            pnlBoard.Size = new Size(740, 550);
            pnlBoard.TabIndex = 3;
            // 
            // gameTimer
            // 
            gameTimer.Interval = 1000;
            gameTimer.Tick += gameTimer_Tick;
            // 
            // GameForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(800, 450);
            Controls.Add(pnlBoard);
            Controls.Add(pnlPlayer2Info);
            Controls.Add(pnlPlayer1Info);
            Controls.Add(pnlToolbar);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "GameForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Caro Game";
            pnlToolbar.ResumeLayout(false);
            pnlPlayer1Info.ResumeLayout(false);
            pnlPlayer1Info.PerformLayout();
            pnlPlayer2Info.ResumeLayout(false);
            pnlPlayer2Info.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlToolbar;
        private Button btnUndo;
        private Button btnRedo;
        private Button btnNewGame;
        private Button btnMenu;
        private Button btnLoad;
        private Button btnSave;
        private Button btnHint;
        private Panel pnlPlayer1Info;
        private Label lblPlayer1Name;
        private ProgressBar pbPlayer1Time;
        private Label lblPlayer1Hints;
        private Panel pnlPlayer2Info;
        private Label lblPlayer2Hints;
        private ProgressBar pbPlayer2Time;
        private Label lblPlayer2Name;
        private Panel pnlBoard;
        private System.Windows.Forms.Timer gameTimer;
    }
}