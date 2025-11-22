namespace CaroGame.Winforms.Forms
{
    partial class OnlineGameForm
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
            btnNewGame = new Button();
            btnRedo = new Button();
            btnUndo = new Button();
            tabConnection = new TabControl();
            tabPage1 = new TabPage();
            btnCopy = new Button();
            lblRoomCode = new Label();
            btnCreate = new Button();
            tabPage2 = new TabPage();
            btnJoin = new Button();
            txtRoomCode = new TextBox();
            pnlYouInfo = new Panel();
            lblYouHints = new Label();
            pbYouTime = new ProgressBar();
            lblYouName = new Label();
            pnlOpponentInfo = new Panel();
            lblOpponentHints = new Label();
            pbOpponentTime = new ProgressBar();
            lblOpponentName = new Label();
            pnlBoard = new Panel();
            pnlChat = new Panel();
            btnSend = new Button();
            txtChat = new TextBox();
            rtbChatHistory = new RichTextBox();
            lblChatTitle = new Label();
            pnlReconnecting = new Panel();
            lblReconnecting = new Label();
            gameTimer = new System.Windows.Forms.Timer(components);
            btnHint = new Button();
            pnlToolbar.SuspendLayout();
            tabConnection.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            pnlYouInfo.SuspendLayout();
            pnlOpponentInfo.SuspendLayout();
            pnlChat.SuspendLayout();
            pnlReconnecting.SuspendLayout();
            SuspendLayout();
            // 
            // pnlToolbar
            // 
            pnlToolbar.BackColor = Color.FromArgb(52, 73, 94);
            pnlToolbar.Controls.Add(btnMenu);
            pnlToolbar.Controls.Add(btnHint);
            pnlToolbar.Controls.Add(btnNewGame);
            pnlToolbar.Controls.Add(btnRedo);
            pnlToolbar.Controls.Add(btnUndo);
            pnlToolbar.Dock = DockStyle.Top;
            pnlToolbar.Location = new Point(0, 0);
            pnlToolbar.Name = "pnlToolbar";
            pnlToolbar.Size = new Size(1106, 50);
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
            btnMenu.Location = new Point(1014, 7);
            btnMenu.Name = "btnMenu";
            btnMenu.Size = new Size(80, 35);
            btnMenu.TabIndex = 0;
            btnMenu.Text = "Menu";
            btnMenu.UseVisualStyleBackColor = false;
            btnMenu.Click += btnMenu_Click;
            // 
            // btnNewGame
            // 
            btnNewGame.AutoSize = true;
            btnNewGame.BackColor = Color.FromArgb(41, 128, 185);
            btnNewGame.Cursor = Cursors.Hand;
            btnNewGame.FlatAppearance.BorderSize = 0;
            btnNewGame.FlatStyle = FlatStyle.Flat;
            btnNewGame.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            btnNewGame.ForeColor = Color.White;
            btnNewGame.Location = new Point(182, 7);
            btnNewGame.Name = "btnNewGame";
            btnNewGame.Size = new Size(109, 35);
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
            btnRedo.Location = new Point(96, 7);
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
            btnUndo.Location = new Point(10, 7);
            btnUndo.Name = "btnUndo";
            btnUndo.Size = new Size(80, 35);
            btnUndo.TabIndex = 0;
            btnUndo.Text = "Undo";
            btnUndo.UseVisualStyleBackColor = false;
            btnUndo.Click += btnUndo_Click;
            // 
            // tabConnection
            // 
            tabConnection.Controls.Add(tabPage1);
            tabConnection.Controls.Add(tabPage2);
            tabConnection.Location = new Point(20, 60);
            tabConnection.Name = "tabConnection";
            tabConnection.SelectedIndex = 0;
            tabConnection.Size = new Size(594, 100);
            tabConnection.TabIndex = 1;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(btnCopy);
            tabPage1.Controls.Add(lblRoomCode);
            tabPage1.Controls.Add(btnCreate);
            tabPage1.Location = new Point(4, 34);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(586, 62);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Create Room";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnCopy
            // 
            btnCopy.BackColor = Color.FromArgb(52, 152, 219);
            btnCopy.FlatAppearance.BorderSize = 0;
            btnCopy.FlatStyle = FlatStyle.Flat;
            btnCopy.Font = new Font("Arial", 10F, FontStyle.Regular, GraphicsUnit.Point);
            btnCopy.ForeColor = Color.White;
            btnCopy.Location = new Point(486, 10);
            btnCopy.Name = "btnCopy";
            btnCopy.Size = new Size(80, 40);
            btnCopy.TabIndex = 2;
            btnCopy.Text = "Copy";
            btnCopy.UseVisualStyleBackColor = false;
            btnCopy.Visible = false;
            btnCopy.Click += btnCopy_Click;
            // 
            // lblRoomCode
            // 
            lblRoomCode.Font = new Font("Arial", 16F, FontStyle.Bold, GraphicsUnit.Point);
            lblRoomCode.ForeColor = Color.FromArgb(52, 73, 94);
            lblRoomCode.Location = new Point(193, 6);
            lblRoomCode.Name = "lblRoomCode";
            lblRoomCode.Size = new Size(150, 40);
            lblRoomCode.TabIndex = 1;
            lblRoomCode.Text = "label1";
            lblRoomCode.TextAlign = ContentAlignment.MiddleCenter;
            lblRoomCode.Visible = false;
            // 
            // btnCreate
            // 
            btnCreate.AutoSize = true;
            btnCreate.BackColor = Color.FromArgb(46, 204, 113);
            btnCreate.Cursor = Cursors.Hand;
            btnCreate.FlatAppearance.BorderSize = 0;
            btnCreate.FlatStyle = FlatStyle.Flat;
            btnCreate.Font = new Font("Arial", 11F, FontStyle.Bold, GraphicsUnit.Point);
            btnCreate.ForeColor = Color.White;
            btnCreate.Location = new Point(6, 6);
            btnCreate.Name = "btnCreate";
            btnCreate.Size = new Size(156, 40);
            btnCreate.TabIndex = 0;
            btnCreate.Text = "Create Room";
            btnCreate.UseVisualStyleBackColor = false;
            btnCreate.Click += btnCreate_Click;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(btnJoin);
            tabPage2.Controls.Add(txtRoomCode);
            tabPage2.Location = new Point(4, 34);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(586, 62);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Join Room";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnJoin
            // 
            btnJoin.BackColor = Color.FromArgb(52, 152, 219);
            btnJoin.Cursor = Cursors.Hand;
            btnJoin.FlatAppearance.BorderSize = 0;
            btnJoin.FlatStyle = FlatStyle.Flat;
            btnJoin.Font = new Font("Arial", 11F, FontStyle.Bold, GraphicsUnit.Point);
            btnJoin.ForeColor = Color.White;
            btnJoin.Location = new Point(177, 6);
            btnJoin.Name = "btnJoin";
            btnJoin.Size = new Size(100, 30);
            btnJoin.TabIndex = 1;
            btnJoin.Text = "Join";
            btnJoin.UseVisualStyleBackColor = false;
            btnJoin.Click += btnJoin_Click;
            // 
            // txtRoomCode
            // 
            txtRoomCode.CharacterCasing = CharacterCasing.Upper;
            txtRoomCode.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point);
            txtRoomCode.Location = new Point(21, 6);
            txtRoomCode.MaxLength = 6;
            txtRoomCode.Name = "txtRoomCode";
            txtRoomCode.Size = new Size(150, 35);
            txtRoomCode.TabIndex = 0;
            // 
            // pnlYouInfo
            // 
            pnlYouInfo.BorderStyle = BorderStyle.FixedSingle;
            pnlYouInfo.Controls.Add(lblYouHints);
            pnlYouInfo.Controls.Add(pbYouTime);
            pnlYouInfo.Controls.Add(lblYouName);
            pnlYouInfo.Location = new Point(24, 166);
            pnlYouInfo.Name = "pnlYouInfo";
            pnlYouInfo.Size = new Size(350, 80);
            pnlYouInfo.TabIndex = 2;
            // 
            // lblYouHints
            // 
            lblYouHints.AutoSize = true;
            lblYouHints.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lblYouHints.ForeColor = Color.FromArgb(127, 140, 141);
            lblYouHints.Location = new Point(250, 10);
            lblYouHints.Name = "lblYouHints";
            lblYouHints.Size = new Size(86, 21);
            lblYouHints.TabIndex = 2;
            lblYouHints.Text = "HInts: 3/3";
            // 
            // pbYouTime
            // 
            pbYouTime.Location = new Point(10, 42);
            pbYouTime.Maximum = 180;
            pbYouTime.Name = "pbYouTime";
            pbYouTime.Size = new Size(330, 20);
            pbYouTime.TabIndex = 1;
            pbYouTime.Value = 180;
            // 
            // lblYouName
            // 
            lblYouName.AutoSize = true;
            lblYouName.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblYouName.ForeColor = Color.FromArgb(52, 73, 94);
            lblYouName.Location = new Point(10, 10);
            lblYouName.Name = "lblYouName";
            lblYouName.Size = new Size(95, 29);
            lblYouName.TabIndex = 0;
            lblYouName.Text = "You (X)";
            // 
            // pnlOpponentInfo
            // 
            pnlOpponentInfo.BorderStyle = BorderStyle.FixedSingle;
            pnlOpponentInfo.Controls.Add(lblOpponentHints);
            pnlOpponentInfo.Controls.Add(pbOpponentTime);
            pnlOpponentInfo.Controls.Add(lblOpponentName);
            pnlOpponentInfo.Location = new Point(414, 166);
            pnlOpponentInfo.Name = "pnlOpponentInfo";
            pnlOpponentInfo.Size = new Size(350, 80);
            pnlOpponentInfo.TabIndex = 3;
            // 
            // lblOpponentHints
            // 
            lblOpponentHints.AutoSize = true;
            lblOpponentHints.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lblOpponentHints.ForeColor = Color.FromArgb(127, 140, 141);
            lblOpponentHints.Location = new Point(250, 10);
            lblOpponentHints.Name = "lblOpponentHints";
            lblOpponentHints.Size = new Size(86, 21);
            lblOpponentHints.TabIndex = 2;
            lblOpponentHints.Text = "HInts: 3/3";
            // 
            // pbOpponentTime
            // 
            pbOpponentTime.Location = new Point(10, 42);
            pbOpponentTime.Maximum = 180;
            pbOpponentTime.Name = "pbOpponentTime";
            pbOpponentTime.Size = new Size(330, 20);
            pbOpponentTime.TabIndex = 1;
            pbOpponentTime.Value = 180;
            // 
            // lblOpponentName
            // 
            lblOpponentName.AutoSize = true;
            lblOpponentName.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblOpponentName.ForeColor = Color.FromArgb(52, 73, 94);
            lblOpponentName.Location = new Point(10, 10);
            lblOpponentName.Name = "lblOpponentName";
            lblOpponentName.Size = new Size(168, 29);
            lblOpponentName.TabIndex = 0;
            lblOpponentName.Text = "Opponent (O)";
            // 
            // pnlBoard
            // 
            pnlBoard.Location = new Point(24, 268);
            pnlBoard.Name = "pnlBoard";
            pnlBoard.Size = new Size(740, 550);
            pnlBoard.TabIndex = 4;
            // 
            // pnlChat
            // 
            pnlChat.BorderStyle = BorderStyle.FixedSingle;
            pnlChat.Controls.Add(btnSend);
            pnlChat.Controls.Add(txtChat);
            pnlChat.Controls.Add(rtbChatHistory);
            pnlChat.Controls.Add(lblChatTitle);
            pnlChat.Location = new Point(794, 166);
            pnlChat.Name = "pnlChat";
            pnlChat.Size = new Size(300, 651);
            pnlChat.TabIndex = 5;
            // 
            // btnSend
            // 
            btnSend.BackColor = Color.FromArgb(52, 152, 219);
            btnSend.Cursor = Cursors.Hand;
            btnSend.FlatAppearance.BorderSize = 0;
            btnSend.FlatStyle = FlatStyle.Flat;
            btnSend.Font = new Font("Arial", 9F, FontStyle.Bold, GraphicsUnit.Point);
            btnSend.ForeColor = Color.White;
            btnSend.Location = new Point(216, 580);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(70, 30);
            btnSend.TabIndex = 3;
            btnSend.Text = "Send";
            btnSend.UseVisualStyleBackColor = false;
            btnSend.Click += btnSend_Click;
            // 
            // txtChat
            // 
            txtChat.Font = new Font("Arial", 10F, FontStyle.Regular, GraphicsUnit.Point);
            txtChat.Location = new Point(10, 580);
            txtChat.MaxLength = 50;
            txtChat.Name = "txtChat";
            txtChat.Size = new Size(200, 30);
            txtChat.TabIndex = 2;
            txtChat.KeyPress += txtChat_KeyPress;
            // 
            // rtbChatHistory
            // 
            rtbChatHistory.BackColor = Color.FromArgb(250, 250, 250);
            rtbChatHistory.BorderStyle = BorderStyle.None;
            rtbChatHistory.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            rtbChatHistory.Location = new Point(10, 40);
            rtbChatHistory.Name = "rtbChatHistory";
            rtbChatHistory.ReadOnly = true;
            rtbChatHistory.Size = new Size(280, 520);
            rtbChatHistory.TabIndex = 1;
            rtbChatHistory.Text = "";
            // 
            // lblChatTitle
            // 
            lblChatTitle.AutoSize = true;
            lblChatTitle.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblChatTitle.Location = new Point(10, 10);
            lblChatTitle.Name = "lblChatTitle";
            lblChatTitle.Size = new Size(66, 29);
            lblChatTitle.TabIndex = 0;
            lblChatTitle.Text = "Chat";
            // 
            // pnlReconnecting
            // 
            pnlReconnecting.BackColor = Color.FromArgb(100, 0, 0, 0);
            pnlReconnecting.Controls.Add(lblReconnecting);
            pnlReconnecting.Location = new Point(24, 268);
            pnlReconnecting.Name = "pnlReconnecting";
            pnlReconnecting.Size = new Size(740, 550);
            pnlReconnecting.TabIndex = 0;
            pnlReconnecting.Visible = false;
            // 
            // lblReconnecting
            // 
            lblReconnecting.AutoSize = true;
            lblReconnecting.BackColor = Color.Transparent;
            lblReconnecting.Font = new Font("Arial", 20F, FontStyle.Bold, GraphicsUnit.Point);
            lblReconnecting.ForeColor = Color.White;
            lblReconnecting.Location = new Point(213, 165);
            lblReconnecting.Name = "lblReconnecting";
            lblReconnecting.Size = new Size(322, 46);
            lblReconnecting.TabIndex = 0;
            lblReconnecting.Text = "Reconnecting ...";
            // 
            // gameTimer
            // 
            gameTimer.Interval = 1000;
            gameTimer.Tick += gameTimer_Tick;
            // 
            // btnHint
            // 
            btnHint.BackColor = Color.FromArgb(41, 128, 185);
            btnHint.Cursor = Cursors.Hand;
            btnHint.FlatAppearance.BorderSize = 0;
            btnHint.FlatStyle = FlatStyle.Flat;
            btnHint.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            btnHint.ForeColor = Color.White;
            btnHint.Location = new Point(297, 7);
            btnHint.Name = "btnHint";
            btnHint.Size = new Size(80, 35);
            btnHint.TabIndex = 0;
            btnHint.Text = "Hint";
            btnHint.UseVisualStyleBackColor = false;
            btnHint.Click += btnHint_Click;
            // 
            // OnlineGameForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1106, 841);
            Controls.Add(pnlReconnecting);
            Controls.Add(pnlChat);
            Controls.Add(pnlBoard);
            Controls.Add(pnlOpponentInfo);
            Controls.Add(pnlYouInfo);
            Controls.Add(tabConnection);
            Controls.Add(pnlToolbar);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "OnlineGameForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Caro Game - Online";
            pnlToolbar.ResumeLayout(false);
            pnlToolbar.PerformLayout();
            tabConnection.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            pnlYouInfo.ResumeLayout(false);
            pnlYouInfo.PerformLayout();
            pnlOpponentInfo.ResumeLayout(false);
            pnlOpponentInfo.PerformLayout();
            pnlChat.ResumeLayout(false);
            pnlChat.PerformLayout();
            pnlReconnecting.ResumeLayout(false);
            pnlReconnecting.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlToolbar;
        private Button btnMenu;
        private Button btnNewGame;
        private Button btnRedo;
        private Button btnUndo;
        private TabControl tabConnection;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Button btnCreate;
        private Button btnCopy;
        private Label lblRoomCode;
        private Button btnJoin;
        private TextBox txtRoomCode;
        private Panel pnlYouInfo;
        private ProgressBar pbYouTime;
        private Label lblYouName;
        private Label lblYouHints;
        private Panel pnlOpponentInfo;
        private Label lblOpponentHints;
        private ProgressBar pbOpponentTime;
        private Label lblOpponentName;
        private Panel pnlBoard;
        private Panel pnlChat;
        private Label lblChatTitle;
        private RichTextBox rtbChatHistory;
        private TextBox txtChat;
        private Button btnSend;
        private Panel pnlReconnecting;
        private Label lblReconnecting;
        private System.Windows.Forms.Timer gameTimer;
        private Button btnHint;
    }
}