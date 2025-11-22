using CaroGame.Core.Enums;
using CaroGame.Core.Models;
using CaroGame.Core.Services;
using CaroGame.Infrastructure.Networking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaroGame.Winforms.Forms
{
    public partial class OnlineGameForm: Form
    {
        #region Fields

        private readonly int _boardSize;
        private readonly GameEngine _gameEngine;
        private readonly SignalRClient _signalRClient;

        private Button[,] _boardButtons;
        private System.Windows.Forms.Timer _flashTimer;

        private Move _hintMove;
        private List<(int, int)> _winningCells;
        private int _flashCount;

        private bool _isMyTurn;
        private PlayerSymbol _mySymbol;
        private bool _isHost;
        private bool _gameStarted;

        #endregion

        #region Constructor

        public OnlineGameForm()
        {
            InitializeComponent();
        }

        public OnlineGameForm(int boardSize) : this()
        {
            _boardSize = boardSize;
            _gameEngine = GameEngine.Instance;
            _signalRClient = new SignalRClient();

            SetupForm();
            SetupHoverEffects();
            CreateBoardButtons();
            InitializeSignalR();
        }

        #endregion

        #region Setup Methods

        private void SetupForm()
        {
            int cellSize = _boardSize == 3 ? 100 : 35;
            int boardPixelSize = _boardSize * cellSize + (_boardSize - 1) * 2;

            // Cập nhật form size
            this.ClientSize = new Size(boardPixelSize + 360, boardPixelSize + 310);

            // Cập nhật TabControl
            tabConnection.Size = new Size(boardPixelSize, 100);

            // Cập nhật Player panels
            int panelWidth = (boardPixelSize - 20) / 2;

            pnlYouInfo.Size = new Size(panelWidth, 80);
            pnlYouInfo.Location = new Point(20, 170);

            pnlOpponentInfo.Size = new Size(panelWidth, 80);
            pnlOpponentInfo.Location = new Point(panelWidth + 40, 170);

            pbYouTime.Size = new Size(panelWidth - 20, 20);
            pbOpponentTime.Size = new Size(panelWidth - 20, 20);

            // Cập nhật Board panel
            pnlBoard.Size = new Size(boardPixelSize, boardPixelSize);
            pnlBoard.Location = new Point(20, 260);

            // Cập nhật Reconnecting overlay
            pnlReconnecting.Size = new Size(boardPixelSize, boardPixelSize);
            pnlReconnecting.Location = new Point(20, 260);
            lblReconnecting.Location = new Point((boardPixelSize - lblReconnecting.Width) / 2,
                                                  (boardPixelSize - lblReconnecting.Height) / 2);

            // Cập nhật Chat panel
            pnlChat.Location = new Point(boardPixelSize + 40, 60);
            pnlChat.Size = new Size(300, boardPixelSize + 200);

            rtbChatHistory.Size = new Size(280, boardPixelSize + 120);
            txtChat.Location = new Point(10, boardPixelSize + 150);
            btnSend.Location = new Point(220, boardPixelSize + 150);

            // Cập nhật Menu button
            btnMenu.Location = new Point(this.ClientSize.Width - 90, 7);
        }

        private void SetupHoverEffects()
        {
            var toolbarButtons = new[] { btnUndo, btnRedo, btnNewGame, btnHint };
            foreach (var btn in toolbarButtons)
            {
                btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(52, 152, 219);
                btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(41, 128, 185);
            }

            btnMenu.MouseEnter += (s, e) => btnMenu.BackColor = Color.FromArgb(231, 76, 60);
            btnMenu.MouseLeave += (s, e) => btnMenu.BackColor = Color.FromArgb(192, 57, 43);

            btnCreate.MouseEnter += (s, e) => btnCreate.BackColor = Color.FromArgb(39, 174, 96);
            btnCreate.MouseLeave += (s, e) => btnCreate.BackColor = Color.FromArgb(46, 204, 113);

            btnJoin.MouseEnter += (s, e) => btnJoin.BackColor = Color.FromArgb(41, 128, 185);
            btnJoin.MouseLeave += (s, e) => btnJoin.BackColor = Color.FromArgb(52, 152, 219);

            btnCopy.MouseEnter += (s, e) => btnCopy.BackColor = Color.FromArgb(41, 128, 185);
            btnCopy.MouseLeave += (s, e) => btnCopy.BackColor = Color.FromArgb(52, 152, 219);

            btnSend.MouseEnter += (s, e) => btnSend.BackColor = Color.FromArgb(41, 128, 185);
            btnSend.MouseLeave += (s, e) => btnSend.BackColor = Color.FromArgb(52, 152, 219);
        }

        private void CreateBoardButtons()
        {
            int cellSize = _boardSize == 3 ? 100 : 35;
            _boardButtons = new Button[_boardSize, _boardSize];

            pnlBoard.Controls.Clear();

            for (int i = 0; i < _boardSize; i++)
            {
                for (int j = 0; j < _boardSize; j++)
                {
                    var btn = new Button
                    {
                        Size = new Size(cellSize, cellSize),
                        Location = new Point(j * (cellSize + 2), i * (cellSize + 2)),
                        FlatStyle = FlatStyle.Flat,
                        BackColor = Color.White,
                        Cursor = Cursors.Hand,
                        Tag = new Point(i, j),
                        Enabled = false
                    };
                    btn.FlatAppearance.BorderColor = Color.Black;
                    btn.FlatAppearance.BorderSize = 1;
                    btn.Click += BoardButton_Click;

                    _boardButtons[i, j] = btn;
                    pnlBoard.Controls.Add(btn);
                }
            }
        }

        private async void InitializeSignalR()
        {
            // Subscribe to SignalR events
            _signalRClient.RoomCreated += OnRoomCreated;
            _signalRClient.RoomJoined += OnRoomJoined;
            _signalRClient.OpponentJoined += OnOpponentJoined;
            _signalRClient.OpponentMoveReceived += OnOpponentMoveReceived;
            _signalRClient.ChatMessageReceived += OnChatMessageReceived;
            _signalRClient.OpponentDisconnected += OnOpponentDisconnected;
            _signalRClient.OpponentReconnected += OnOpponentReconnected;
            _signalRClient.UndoRequestReceived += OnUndoRequestReceived;
            _signalRClient.UndoResponseReceived += OnUndoResponseReceived;
            _signalRClient.ErrorOccurred += OnErrorOccurred;

            var connected = await _signalRClient.ConnectAsync("https://localhost:5001/gamehub");
            if (!connected)
            {
                MessageBox.Show(
                    "Failed to connect to server.\nPlease make sure the server is running.",
                    "Connection Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                this.Close();
            }
            else
            {
                AddChatMessage("System", "Connected to server!");
            }
        }

        #endregion

        #region Connection Events (Tab Create/Join)

        private async void btnCreate_Click(object sender, EventArgs e)
        {
            btnCreate.Enabled = false;
            await _signalRClient.CreateRoomAsync();
            _isHost = true;
            _mySymbol = PlayerSymbol.X;
            lblYouName.Text = "You (X)";
            lblOpponentName.Text = "Opponent (O)";
        }

        private async void btnJoin_Click(object sender, EventArgs e)
        {
            var roomCode = txtRoomCode.Text.Trim().ToUpper();
            if (string.IsNullOrEmpty(roomCode) || roomCode.Length != 6)
            {
                MessageBox.Show("Please enter a valid 6-character room code.",
                    "Invalid Code", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnJoin.Enabled = false;
            var success = await _signalRClient.JoinRoomAsync(roomCode);
            if (success)
            {
                _isHost = false;
                _mySymbol = PlayerSymbol.O;
                lblYouName.Text = "You (O)";
                lblOpponentName.Text = "Opponent (X)";
            }
            else
            {
                btnJoin.Enabled = true;
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(lblRoomCode.Text))
            {
                Clipboard.SetText(lblRoomCode.Text);
                MessageBox.Show("Room code copied!", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OnRoomCreated(object sender, string roomCode)
        {
            this.Invoke(() =>
            {
                lblRoomCode.Text = roomCode;
                lblRoomCode.Visible = true;
                btnCopy.Visible = true;
                AddChatMessage("System", $"Room created: {roomCode}");
                AddChatMessage("System", "Waiting for opponent...");
            });
        }

        private void OnRoomJoined(object sender, bool success)
        {
            this.Invoke(() =>
            {
                if (success)
                {
                    tabConnection.Visible = false;
                    StartGame();
                    AddChatMessage("System", "Joined room successfully!");
                }
            });
        }

        #endregion

        #region Game Logic

        private void StartGame()
        {
            _gameStarted = true;
            _gameEngine.StartNewGame(_boardSize, GameMode.Online, false);

            _gameEngine.MoveMade += OnMoveMade;
            _gameEngine.GameOver += OnGameOver;
            _gameEngine.TurnChanged += OnTurnChanged;

            foreach (var btn in _boardButtons)
            {
                btn.Enabled = true;
            }

            gameTimer.Enabled = true;
            UpdateTurnIndicator();

            if (_isHost)
            {
                AddChatMessage("System", "Opponent joined! You play first (X).");
                _isMyTurn = true;
            }
            else
            {
                AddChatMessage("System", "Game started! Opponent plays first.");
                _isMyTurn = false;
            }
        }

        private async void BoardButton_Click(object sender, EventArgs e)
        {
            if (!_gameStarted || !_isMyTurn)
                return;

            var btn = (Button)sender;
            var pos = (Point)btn.Tag;

            if (_gameEngine.MakeMove(pos.X, pos.Y))
            {
                await _signalRClient.SendMoveAsync(pos.X, pos.Y, _mySymbol);
                _isMyTurn = false;
                FlashCell(pos.X, pos.Y);
            }
        }

        private void OnOpponentJoined(object sender, EventArgs e)
        {
            this.Invoke(() =>
            {
                // Ẩn tab connection
                tabConnection.Visible = false;

                // Bắt đầu game cho Host
                StartGame();

                AddChatMessage("System", "Opponent joined! Game started.");
            });
        }

        private void OnOpponentMoveReceived(object sender, MoveReceivedEventArgs e)
        {
            this.Invoke(() =>
            {
                if (_gameEngine.MakeMove(e.Row, e.Col))
                {
                    _isMyTurn = true;
                    FlashCell(e.Row, e.Col);
                }
            });
        }

        private void OnMoveMade(object sender, MoveEventArgs e)
        {
            var move = e.Move;
            var btn = _boardButtons[move.Row, move.Col];

            string imagePath = move.Symbol == PlayerSymbol.X ? "x.png" : "o.png";
            if (File.Exists(imagePath))
            {
                btn.BackgroundImage = Image.FromFile(imagePath);
                btn.BackgroundImageLayout = ImageLayout.Zoom;
            }
            else
            {
                btn.Text = move.Symbol.ToString();
                btn.Font = new Font("Arial", _boardSize == 3 ? 36 : 16, FontStyle.Bold);
                btn.ForeColor = move.Symbol == PlayerSymbol.X ? Color.Blue : Color.Red;
            }

            btn.Enabled = false;
        }

        private void OnGameOver(object sender, GameOverEventArgs e)
        {
            gameTimer.Enabled = false;
            _gameStarted = false;

            if (e.WinResult != null && e.WinResult.WinningCells.Count > 0)
            {
                _winningCells = e.WinResult.WinningCells;
                FlashWinningCells();
            }

            string message = e.Status == GameStatus.Draw ? "Draw!" :
                e.WinResult.Winner == _mySymbol ? "You win!" : "You lose!";

            Task.Delay(2200).ContinueWith(t => this.Invoke(() =>
            {
                MessageBox.Show(message, "Game Over",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }));
        }

        private void OnTurnChanged(object sender, PlayerTurnChangedEventArgs e)
        {
            UpdateTurnIndicator();
        }

        private void UpdateTurnIndicator()
        {
            bool isMyTurnNow = (_isHost && _gameEngine.CurrentTurn == PlayerSymbol.X) ||
                               (!_isHost && _gameEngine.CurrentTurn == PlayerSymbol.O);

            if (isMyTurnNow)
            {
                pnlYouInfo.BackColor = Color.FromArgb(255, 240, 240);
                pnlOpponentInfo.BackColor = Color.White;
            }
            else
            {
                pnlYouInfo.BackColor = Color.White;
                pnlOpponentInfo.BackColor = Color.FromArgb(255, 240, 240);
            }
        }

        #endregion

        #region Animation

        private void FlashCell(int row, int col, bool isHint = false)
        {
            _flashCount = 0;
            _flashTimer = new System.Windows.Forms.Timer { Interval = 200 };

            var btn = _boardButtons[row, col];
            var originalColor = btn.BackColor;
            var flashColor = isHint ? Color.Yellow : Color.LightBlue;

            _flashTimer.Tick += (s, e) =>
            {
                btn.BackColor = _flashCount % 2 == 0 ? flashColor : originalColor;
                _flashCount++;

                if (_flashCount >= 4)
                {
                    _flashTimer.Stop();
                    _flashTimer.Dispose();
                    if (!isHint) btn.BackColor = originalColor;
                }
            };
            _flashTimer.Start();
        }

        private void FlashWinningCells()
        {
            _flashCount = 0;
            var flashTimer = new System.Windows.Forms.Timer { Interval = 200 };
            var flashColor = Color.FromArgb(217, 119, 87);

            flashTimer.Tick += (s, e) =>
            {
                foreach (var (row, col) in _winningCells)
                {
                    var btn = _boardButtons[row, col];
                    btn.BackColor = _flashCount % 2 == 0 ? flashColor : Color.White;
                }
                _flashCount++;

                if (_flashCount >= 20)
                {
                    flashTimer.Stop();
                    flashTimer.Dispose();
                }
            };
            flashTimer.Start();
        }

        #endregion

        #region Chat

        private void btnSend_Click(object sender, EventArgs e)
        {
            SendChatMessage();
        }

        private void txtChat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SendChatMessage();
                e.Handled = true;
            }
        }

        private async void SendChatMessage()
        {
            var message = txtChat.Text.Trim();
            if (string.IsNullOrEmpty(message))
                return;

            await _signalRClient.SendChatMessageAsync(message);
            AddChatMessage("You", message);
            txtChat.Clear();
        }

        private void OnChatMessageReceived(object sender, ChatMessageEventArgs e)
        {
            this.Invoke(() =>
            {
                AddChatMessage(e.Sender, e.Message);
            });
        }

        private void AddChatMessage(string sender, string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm");
            var color = sender == "You" ? Color.Blue :
                        sender == "System" ? Color.Gray : Color.Green;

            rtbChatHistory.SelectionStart = rtbChatHistory.TextLength;
            rtbChatHistory.SelectionLength = 0;
            rtbChatHistory.SelectionColor = Color.Gray;
            rtbChatHistory.AppendText($"[{timestamp}] ");
            rtbChatHistory.SelectionColor = color;
            rtbChatHistory.AppendText($"{sender}: ");
            rtbChatHistory.SelectionColor = Color.Black;
            rtbChatHistory.AppendText($"{message}\n");
            rtbChatHistory.ScrollToCaret();
        }

        #endregion

        #region Connection Events

        private void OnOpponentDisconnected(object sender, EventArgs e)
        {
            this.Invoke(() =>
            {
                pnlReconnecting.Visible = true;
                pnlReconnecting.BringToFront();
                gameTimer.Enabled = false;
                AddChatMessage("System", "Opponent disconnected. Waiting 30s for reconnection...");
            });
        }

        private void OnOpponentReconnected(object sender, EventArgs e)
        {
            this.Invoke(() =>
            {
                pnlReconnecting.Visible = false;
                if (_gameStarted) gameTimer.Enabled = true;
                AddChatMessage("System", "Opponent reconnected!");
            });
        }

        private void OnErrorOccurred(object sender, string error)
        {
            this.Invoke(() =>
            {
                AddChatMessage("System", $"Error: {error}");
            });
        }

        #endregion

        #region Undo/Redo

        private async void btnUndo_Click(object sender, EventArgs e)
        {
            if (!_gameStarted)
            {
                MessageBox.Show("Game hasn't started yet!", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            await _signalRClient.RequestUndoAsync();
            AddChatMessage("System", "Undo request sent...");
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Redo is not available in online mode.", "Info",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnUndoRequestReceived(object sender, UndoRequestEventArgs e)
        {
            this.Invoke(async () =>
            {
                var result = MessageBox.Show(
                    "Opponent requests undo. Accept?",
                    "Undo Request",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                await _signalRClient.RespondToUndoAsync(result == DialogResult.Yes);

                if (result == DialogResult.Yes)
                {
                    if (_gameEngine.CanUndo())
                    {
                        _gameEngine.Undo();
                        RefreshBoard();
                        AddChatMessage("System", "Undo accepted.");
                    }
                }
                else
                {
                    AddChatMessage("System", "You denied undo request.");
                }
            });
        }

        private void OnUndoResponseReceived(object sender, bool accepted)
        {
            this.Invoke(() =>
            {
                if (accepted)
                {
                    if (_gameEngine.CanUndo())
                    {
                        _gameEngine.Undo();
                        RefreshBoard();
                    }
                    AddChatMessage("System", "Undo accepted by opponent!");
                }
                else
                {
                    AddChatMessage("System", "Undo denied by opponent.");
                }
            });
        }

        #endregion

        #region Other Toolbar Buttons

        private void btnNewGame_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Please return to menu and create a new game.", "Info",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnHint_Click(object sender, EventArgs e)
        {
            if (!_gameStarted)
            {
                MessageBox.Show("Game hasn't started yet!", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!_isMyTurn)
            {
                MessageBox.Show("It's not your turn!", "Hint",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var currentPlayer = _mySymbol == PlayerSymbol.X
                ? _gameEngine.Player1 : _gameEngine.Player2;

            if (currentPlayer.HintsRemaining <= 0)
            {
                MessageBox.Show("No hints remaining!", "Hint",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var hint = _gameEngine.GetHint();
            if (hint != null)
            {
                _boardButtons[hint.Row, hint.Col].BackColor = Color.Yellow;
                FlashCell(hint.Row, hint.Col, true);
                lblYouHints.Text = $"Hints: {currentPlayer.HintsRemaining}/3";
                AddChatMessage("System", $"Hint: Row {hint.Row + 1}, Col {hint.Col + 1}");
            }
        }

        private async void btnMenu_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Return to menu? This will end the game.",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                if (_gameStarted)
                {
                    await _signalRClient.NotifyDisconnectAsync();
                }
                await _signalRClient.DisconnectAsync();
                this.Close();
            }
        }

        #endregion

        #region Timer

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            if (!_gameStarted || _gameEngine.Status != GameStatus.Playing)
                return;

            var currentPlayer = _gameEngine.CurrentTurn == PlayerSymbol.X
                ? _gameEngine.Player1 : _gameEngine.Player2;

            currentPlayer.RemainingTime--;

            // Update progress bars
            pbYouTime.Value = Math.Max(0, _isHost
                ? _gameEngine.Player1.RemainingTime
                : _gameEngine.Player2.RemainingTime);
            pbOpponentTime.Value = Math.Max(0, _isHost
                ? _gameEngine.Player2.RemainingTime
                : _gameEngine.Player1.RemainingTime);

            bool isMyTurnNow = (_isHost && _gameEngine.CurrentTurn == PlayerSymbol.X) ||
                               (!_isHost && _gameEngine.CurrentTurn == PlayerSymbol.O);

            // Blinking when < 30s
            if (currentPlayer.RemainingTime < 30)
            {
                var panel = isMyTurnNow ? pnlYouInfo : pnlOpponentInfo;
                panel.BackColor = _flashCount % 2 == 0
                    ? Color.FromArgb(255, 200, 200)
                    : Color.FromArgb(255, 240, 240);
            }

            // Time out
            if (currentPlayer.RemainingTime <= 0)
            {
                gameTimer.Enabled = false;
                _gameStarted = false;

                string message = isMyTurnNow ? "You ran out of time! You lose."
                                             : "Opponent ran out of time! You win!";
                MessageBox.Show(message, "Time Out",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            _flashCount++;
        }

        #endregion

        #region Helper Methods

        private void RefreshBoard()
        {
            for (int i = 0; i < _boardSize; i++)
            {
                for (int j = 0; j < _boardSize; j++)
                {
                    var btn = _boardButtons[i, j];
                    var symbol = _gameEngine.Board.Grid[i, j];

                    if (symbol == PlayerSymbol.None)
                    {
                        btn.BackgroundImage = null;
                        btn.Text = "";
                        btn.Enabled = true;
                        btn.BackColor = Color.White;
                    }
                    else
                    {
                        string imagePath = symbol == PlayerSymbol.X ? "x.png" : "o.png";
                        if (File.Exists(imagePath))
                        {
                            btn.BackgroundImage = Image.FromFile(imagePath);
                            btn.BackgroundImageLayout = ImageLayout.Zoom;
                        }
                        else
                        {
                            btn.Text = symbol.ToString();
                            btn.Font = new Font("Arial", _boardSize == 3 ? 36 : 16, FontStyle.Bold);
                            btn.ForeColor = symbol == PlayerSymbol.X ? Color.Blue : Color.Red;
                        }
                        btn.Enabled = false;
                    }
                }
            }

            // Update turn after undo
            bool isMyTurnNow = (_isHost && _gameEngine.CurrentTurn == PlayerSymbol.X) ||
                               (!_isHost && _gameEngine.CurrentTurn == PlayerSymbol.O);
            _isMyTurn = isMyTurnNow;
            UpdateTurnIndicator();
        }

        #endregion

        #region Form Events

        protected override async void OnFormClosing(FormClosingEventArgs e)
        {
            gameTimer.Enabled = false;
            _flashTimer?.Stop();

            if (_gameStarted)
            {
                await _signalRClient.NotifyDisconnectAsync();
            }
            await _signalRClient.DisconnectAsync();

            _gameEngine.MoveMade -= OnMoveMade;
            _gameEngine.GameOver -= OnGameOver;
            _gameEngine.TurnChanged -= OnTurnChanged;

            base.OnFormClosing(e);
        }

        #endregion
    }
}
