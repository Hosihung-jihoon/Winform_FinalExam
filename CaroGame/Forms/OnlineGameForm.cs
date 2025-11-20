using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaroGame.Core.Enums;
using CaroGame.Core.Services;
using CaroGame.Core.Models;
using CaroGame.Infrastructure.Networking;

namespace CaroGame.WinForms.Forms
{
    /// <summary>
    /// Form game online với chat
    /// </summary>
    public partial class OnlineGameForm : Form
    {
        private readonly int _boardSize;
        private readonly GameEngine _gameEngine;
        private readonly SignalRClient _signalRClient;

        private Button[,] _boardButtons;
        private Panel _boardPanel;
        private System.Windows.Forms.Timer _gameTimer;
        private System.Windows.Forms.Timer _flashTimer;

        // UI Components
        private Label _lblYouName;
        private Label _lblOpponentName;
        private ProgressBar _pbYouTime;
        private ProgressBar _pbOpponentTime;
        private Label _lblYouHints;
        private Label _lblOpponentHints;
        private Panel _pnlYouInfo;
        private Panel _pnlOpponentInfo;

        // Chat
        private TextBox _txtChat;
        private Button _btnSend;
        private RichTextBox _rtbChatHistory;

        // Connection
        private TabControl _tabControl;
        private TextBox _txtRoomCode;
        private Label _lblRoomCode;
        private Button _btnCopyCode;
        private Panel _pnlReconnecting;

        private Move _hintMove;
        private List<(int, int)> _winningCells;
        private int _flashCount;
        private bool _isMyTurn;
        private PlayerSymbol _mySymbol;
        private bool _isHost;
        private bool _gameStarted;

        public OnlineGameForm(int boardSize)
        {
            _boardSize = boardSize;
            _gameEngine = GameEngine.Instance;
            _signalRClient = new SignalRClient();

            InitializeComponent();
            InitializeSignalR();
        }

        private void InitializeComponent()
        {
            this.Text = "Caro Game - Online";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Calculate form size with chat panel
            int cellSize = _boardSize == 3 ? 100 : 35;
            int boardPixelSize = _boardSize * cellSize + (_boardSize - 1) * 2;
            int chatWidth = 300;
            int formWidth = boardPixelSize + chatWidth + 60;
            int formHeight = boardPixelSize + 200;
            this.ClientSize = new Size(formWidth, formHeight);
            this.BackColor = Color.White;

            CreateToolbar();
            CreateConnectionPanel(boardPixelSize);
            CreatePlayerInfoPanels(boardPixelSize);
            CreateBoard(cellSize, boardPixelSize);
            CreateChatPanel(boardPixelSize, chatWidth, formHeight);
            CreateReconnectingOverlay(boardPixelSize);
        }

        private void CreateToolbar()
        {
            var toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(52, 73, 94)
            };

            var btnStyle = new Action<Button>((btn) =>
            {
                btn.Size = new Size(80, 35);
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.BackColor = Color.FromArgb(41, 128, 185);
                btn.ForeColor = Color.White;
                btn.Font = new Font("Arial", 9, FontStyle.Regular);
                btn.Cursor = Cursors.Hand;
                btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(52, 152, 219);
                btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(41, 128, 185);
            });

            int btnX = 10;
            int btnY = 7;
            int spacing = 90;

            var btnUndo = new Button { Text = "Undo", Location = new Point(btnX, btnY), Enabled = false };
            btnStyle(btnUndo);
            btnUndo.Click += BtnUndo_Click;
            toolbar.Controls.Add(btnUndo);

            var btnRedo = new Button { Text = "Redo", Location = new Point(btnX + spacing, btnY), Enabled = false };
            btnStyle(btnRedo);
            btnRedo.Click += BtnRedo_Click;
            toolbar.Controls.Add(btnRedo);

            var btnNewGame = new Button { Text = "New Game", Location = new Point(btnX + spacing * 2, btnY), Enabled = false };
            btnStyle(btnNewGame);
            btnNewGame.Click += BtnNewGame_Click;
            toolbar.Controls.Add(btnNewGame);

            var btnHint = new Button { Text = "Hint", Location = new Point(btnX + spacing * 3, btnY) };
            btnStyle(btnHint);
            btnHint.Click += BtnHint_Click;
            toolbar.Controls.Add(btnHint);

            var btnMenu = new Button
            {
                Text = "Menu",
                Location = new Point(this.ClientSize.Width - 90, btnY),
                BackColor = Color.FromArgb(192, 57, 43)
            };
            btnStyle(btnMenu);
            btnMenu.MouseEnter += (s, e) => btnMenu.BackColor = Color.FromArgb(231, 76, 60);
            btnMenu.MouseLeave += (s, e) => btnMenu.BackColor = Color.FromArgb(192, 57, 43);
            btnMenu.Click += BtnMenu_Click;
            toolbar.Controls.Add(btnMenu);

            this.Controls.Add(toolbar);
        }

        private void CreateConnectionPanel(int boardPixelSize)
        {
            _tabControl = new TabControl
            {
                Location = new Point(20, 60),
                Size = new Size(boardPixelSize, 100)
            };

            // Tab Create Room
            var tabCreate = new TabPage("Create Room");
            var btnCreate = new Button
            {
                Text = "Create Room",
                Size = new Size(150, 40),
                Location = new Point(10, 20),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 11, FontStyle.Bold)
            };
            btnCreate.FlatAppearance.BorderSize = 0;
            btnCreate.Click += BtnCreateRoom_Click;
            tabCreate.Controls.Add(btnCreate);

            _lblRoomCode = new Label
            {
                Location = new Point(170, 20),
                AutoSize = false,
                Size = new Size(200, 40),
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                TextAlign = ContentAlignment.MiddleLeft,
                Visible = false
            };
            tabCreate.Controls.Add(_lblRoomCode);

            _btnCopyCode = new Button
            {
                Text = "Copy",
                Size = new Size(80, 40),
                Location = new Point(380, 20),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 10, FontStyle.Regular),
                Visible = false
            };
            _btnCopyCode.FlatAppearance.BorderSize = 0;
            _btnCopyCode.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(_lblRoomCode.Text))
                {
                    Clipboard.SetText(_lblRoomCode.Text);
                    MessageBox.Show("Room code copied!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };
            tabCreate.Controls.Add(_btnCopyCode);

            // Tab Join Room
            var tabJoin = new TabPage("Join Room");
            _txtRoomCode = new TextBox
            {
                Location = new Point(10, 25),
                Size = new Size(150, 30),
                Font = new Font("Arial", 12, FontStyle.Regular),
                MaxLength = 6
            };
            tabJoin.Controls.Add(_txtRoomCode);

            var btnJoin = new Button
            {
                Text = "Join",
                Size = new Size(100, 30),
                Location = new Point(170, 25),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 11, FontStyle.Bold)
            };
            btnJoin.FlatAppearance.BorderSize = 0;
            btnJoin.Click += BtnJoinRoom_Click;
            tabJoin.Controls.Add(btnJoin);

            _tabControl.TabPages.Add(tabCreate);
            _tabControl.TabPages.Add(tabJoin);
            this.Controls.Add(_tabControl);
        }

        private void CreatePlayerInfoPanels(int boardPixelSize)
        {
            int panelWidth = boardPixelSize / 2 - 10;
            int panelHeight = 80;
            int panelY = 170;

            // You Info
            _pnlYouInfo = CreatePlayerPanel(20, panelY, panelWidth, panelHeight, true);
            this.Controls.Add(_pnlYouInfo);

            // Opponent Info
            _pnlOpponentInfo = CreatePlayerPanel(panelWidth + 30, panelY, panelWidth, panelHeight, false);
            this.Controls.Add(_pnlOpponentInfo);
        }

        // Part 2 of OnlineGameForm.cs - Methods and Event Handlers

        private Panel CreatePlayerPanel(int x, int y, int width, int height, bool isYou)
        {
            var panel = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            var lblName = new Label
            {
                Location = new Point(10, 10),
                AutoSize = true,
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            var pbTime = new ProgressBar
            {
                Location = new Point(10, 35),
                Size = new Size(width - 20, 20),
                Maximum = 180,
                Value = 180,
                Style = ProgressBarStyle.Continuous
            };

            var lblHints = new Label
            {
                Location = new Point(10, 58),
                AutoSize = true,
                Font = new Font("Arial", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                Text = "Hints: 3/3"
            };

            panel.Controls.Add(lblName);
            panel.Controls.Add(pbTime);
            panel.Controls.Add(lblHints);

            if (isYou)
            {
                _lblYouName = lblName;
                _pbYouTime = pbTime;
                _lblYouHints = lblHints;
            }
            else
            {
                _lblOpponentName = lblName;
                _pbOpponentTime = pbTime;
                _lblOpponentHints = lblHints;
            }

            return panel;
        }

        private void CreateBoard(int cellSize, int boardPixelSize)
        {
            _boardPanel = new Panel
            {
                Location = new Point(20, 260),
                Size = new Size(boardPixelSize, boardPixelSize),
                BackColor = Color.White
            };

            _boardButtons = new Button[_boardSize, _boardSize];

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
                    _boardPanel.Controls.Add(btn);
                }
            }

            this.Controls.Add(_boardPanel);
        }

        private void CreateChatPanel(int boardPixelSize, int chatWidth, int formHeight)
        {
            var chatPanel = new Panel
            {
                Location = new Point(boardPixelSize + 40, 60),
                Size = new Size(chatWidth, formHeight - 80),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            var lblChatTitle = new Label
            {
                Text = "Chat",
                Location = new Point(10, 10),
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true
            };
            chatPanel.Controls.Add(lblChatTitle);

            _rtbChatHistory = new RichTextBox
            {
                Location = new Point(10, 40),
                Size = new Size(chatWidth - 20, formHeight - 200),
                ReadOnly = true,
                BackColor = Color.FromArgb(250, 250, 250),
                BorderStyle = BorderStyle.None,
                Font = new Font("Arial", 9, FontStyle.Regular)
            };
            chatPanel.Controls.Add(_rtbChatHistory);

            _txtChat = new TextBox
            {
                Location = new Point(10, formHeight - 145),
                Size = new Size(chatWidth - 90, 30),
                Font = new Font("Arial", 10, FontStyle.Regular),
                MaxLength = 50
            };
            _txtChat.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    BtnSend_Click(s, e);
                    e.Handled = true;
                }
            };
            chatPanel.Controls.Add(_txtChat);

            _btnSend = new Button
            {
                Text = "Send",
                Location = new Point(chatWidth - 75, formHeight - 145),
                Size = new Size(65, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            _btnSend.FlatAppearance.BorderSize = 0;
            _btnSend.Click += BtnSend_Click;
            chatPanel.Controls.Add(_btnSend);

            this.Controls.Add(chatPanel);
        }

        private void CreateReconnectingOverlay(int boardPixelSize)
        {
            _pnlReconnecting = new Panel
            {
                Location = new Point(20, 260),
                Size = new Size(boardPixelSize, boardPixelSize),
                BackColor = Color.FromArgb(200, 0, 0, 0),
                Visible = false
            };

            var lblReconnecting = new Label
            {
                Text = "Reconnecting...",
                Font = new Font("Arial", 20, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                BackColor = Color.Transparent
            };
            lblReconnecting.Location = new Point(
                (boardPixelSize - lblReconnecting.Width) / 2,
                (boardPixelSize - lblReconnecting.Height) / 2
            );
            _pnlReconnecting.Controls.Add(lblReconnecting);

            this.Controls.Add(_pnlReconnecting);
            _pnlReconnecting.BringToFront();
        }

        private async void InitializeSignalR()
        {
            // Subscribe to events
            _signalRClient.RoomCreated += OnRoomCreated;
            _signalRClient.RoomJoined += OnRoomJoined;
            _signalRClient.OpponentMoveReceived += OnOpponentMoveReceived;
            _signalRClient.ChatMessageReceived += OnChatMessageReceived;
            _signalRClient.OpponentDisconnected += OnOpponentDisconnected;
            _signalRClient.OpponentReconnected += OnOpponentReconnected;
            _signalRClient.UndoRequestReceived += OnUndoRequestReceived;
            _signalRClient.UndoResponseReceived += OnUndoResponseReceived;
            _signalRClient.ErrorOccurred += OnErrorOccurred;

            // Connect to server
            var connected = await _signalRClient.ConnectAsync("https://localhost:5001/gamehub");
            if (!connected)
            {
                MessageBox.Show(
                    "Failed to connect to server. Please make sure the server is running.",
                    "Connection Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                this.Close();
            }
        }

        private async void BtnCreateRoom_Click(object sender, EventArgs e)
        {
            await _signalRClient.CreateRoomAsync();
            _isHost = true;
            _mySymbol = PlayerSymbol.X;
            _lblYouName.Text = "You (X)";
            _lblOpponentName.Text = "Opponent (O)";
        }

        private async void BtnJoinRoom_Click(object sender, EventArgs e)
        {
            var roomCode = _txtRoomCode.Text.Trim().ToUpper();
            if (string.IsNullOrEmpty(roomCode) || roomCode.Length != 6)
            {
                MessageBox.Show("Please enter a valid 6-character room code.", "Invalid Code",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var success = await _signalRClient.JoinRoomAsync(roomCode);
            if (success)
            {
                _isHost = false;
                _mySymbol = PlayerSymbol.O;
                _lblYouName.Text = "You (O)";
                _lblOpponentName.Text = "Opponent (X)";
            }
        }

        private void OnRoomCreated(object sender, string roomCode)
        {
            this.Invoke(() =>
            {
                _lblRoomCode.Text = roomCode;
                _lblRoomCode.Visible = true;
                _btnCopyCode.Visible = true;
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
                    _tabControl.Visible = false;
                    StartGame();
                    AddChatMessage("System", "Joined room successfully!");
                }
            });
        }

        private void StartGame()
        {
            _gameStarted = true;
            _gameEngine.StartNewGame(_boardSize, GameMode.Online, false);

            // Subscribe to game events
            _gameEngine.MoveMade += OnMoveMade;
            _gameEngine.GameOver += OnGameOver;
            _gameEngine.TurnChanged += OnTurnChanged;

            // Enable board
            foreach (var btn in _boardButtons)
            {
                btn.Enabled = true;
            }

            // Start timer
            _gameTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _gameTimer.Tick += GameTimer_Tick;
            _gameTimer.Start();

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

        // Part 3 of OnlineGameForm.cs - Game Logic and Handlers

        private async void BoardButton_Click(object sender, EventArgs e)
        {
            if (!_gameStarted || !_isMyTurn)
                return;

            var btn = (Button)sender;
            var pos = (Point)btn.Tag;

            if (_gameEngine.MakeMove(pos.X, pos.Y))
            {
                // Send move to opponent
                await _signalRClient.SendMoveAsync(pos.X, pos.Y, _mySymbol);
                _isMyTurn = false;
                FlashCell(pos.X, pos.Y);
            }
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
                btn.Font = new Font("Arial", _boardSize == 3 ? 36 : 20, FontStyle.Bold);
            }

            btn.Enabled = false;
        }

        private void OnGameOver(object sender, GameOverEventArgs e)
        {
            _gameTimer?.Stop();
            _gameStarted = false;

            if (e.WinResult != null && e.WinResult.WinningCells.Count > 0)
            {
                _winningCells = e.WinResult.WinningCells;
                FlashWinningCells();
            }

            string message = e.Status == GameStatus.Draw ? "Draw!" :
                e.WinResult.Winner == _mySymbol ? "You win!" : "You lose!";

            Task.Delay(2000).ContinueWith(t => this.Invoke(() =>
            {
                MessageBox.Show(message, "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                _pnlYouInfo.BackColor = Color.FromArgb(255, 240, 240);
                _pnlOpponentInfo.BackColor = Color.White;
            }
            else
            {
                _pnlYouInfo.BackColor = Color.White;
                _pnlOpponentInfo.BackColor = Color.FromArgb(255, 240, 240);
            }
        }

        private void FlashCell(int row, int col)
        {
            _flashCount = 0;
            _flashTimer = new System.Windows.Forms.Timer { Interval = 200 };

            var btn = _boardButtons[row, col];
            var originalColor = btn.BackColor;

            _flashTimer.Tick += (s, e) =>
            {
                btn.BackColor = _flashCount % 2 == 0 ? Color.LightBlue : originalColor;
                _flashCount++;

                if (_flashCount >= 4)
                {
                    _flashTimer.Stop();
                    _flashTimer.Dispose();
                    btn.BackColor = originalColor;
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

        private async void BtnSend_Click(object sender, EventArgs e)
        {
            var message = _txtChat.Text.Trim();
            if (string.IsNullOrEmpty(message))
                return;

            await _signalRClient.SendChatMessageAsync(message);
            AddChatMessage("You", message);
            _txtChat.Clear();
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
            var color = sender == "You" ? Color.Blue : sender == "System" ? Color.Gray : Color.Green;

            _rtbChatHistory.SelectionStart = _rtbChatHistory.TextLength;
            _rtbChatHistory.SelectionLength = 0;
            _rtbChatHistory.SelectionColor = Color.Gray;
            _rtbChatHistory.AppendText($"[{timestamp}] ");
            _rtbChatHistory.SelectionColor = color;
            _rtbChatHistory.AppendText($"{sender}: ");
            _rtbChatHistory.SelectionColor = Color.Black;
            _rtbChatHistory.AppendText($"{message}\n");
            _rtbChatHistory.ScrollToCaret();
        }

        private void OnOpponentDisconnected(object sender, EventArgs e)
        {
            this.Invoke(() =>
            {
                _pnlReconnecting.Visible = true;
                _gameTimer?.Stop();
                AddChatMessage("System", "Opponent disconnected. Waiting for reconnection...");
            });
        }

        private void OnOpponentReconnected(object sender, EventArgs e)
        {
            this.Invoke(() =>
            {
                _pnlReconnecting.Visible = false;
                _gameTimer?.Start();
                AddChatMessage("System", "Opponent reconnected!");
            });
        }

        private async void OnUndoRequestReceived(object sender, UndoRequestEventArgs e)
        {
            this.Invoke(() =>
            {
                var result = MessageBox.Show(
                    "Opponent requests undo. Accept?",
                    "Undo Request",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                _signalRClient.RespondToUndoAsync(result == DialogResult.Yes);

                if (result == DialogResult.Yes)
                {
                    // Perform undo locally
                    if (_gameEngine.CanUndo())
                    {
                        _gameEngine.Undo();
                        RefreshBoard();
                    }
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
                    MessageBox.Show("Undo accepted!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Undo denied by opponent.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            });
        }

        private void OnErrorOccurred(object sender, string error)
        {
            this.Invoke(() =>
            {
                AddChatMessage("System", $"Error: {error}");
            });
        }

        private async void BtnUndo_Click(object sender, EventArgs e)
        {
            await _signalRClient.RequestUndoAsync();
            AddChatMessage("System", "Undo request sent...");
        }

        private void BtnRedo_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Redo is not available in online mode.", "Info",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnNewGame_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Please return to menu and create a new game.", "Info",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnHint_Click(object sender, EventArgs e)
        {
            if (!_isMyTurn)
            {
                MessageBox.Show("It's not your turn!", "Hint", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var currentPlayer = _mySymbol == PlayerSymbol.X ? _gameEngine.Player1 : _gameEngine.Player2;
            if (currentPlayer.HintsRemaining <= 0)
            {
                MessageBox.Show("No hints remaining!", "Hint", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var hint = _gameEngine.GetHint();
            if (hint != null)
            {
                var btn = _boardButtons[hint.Row, hint.Col];
                FlashCell(hint.Row, hint.Col);
                _lblYouHints.Text = $"Hints: {currentPlayer.HintsRemaining}/3";
            }
        }

        private async void BtnMenu_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Return to menu? This will end the game.",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                await _signalRClient.NotifyDisconnectAsync();
                await _signalRClient.DisconnectAsync();
                this.Close();
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (!_gameStarted || _gameEngine.Status != GameStatus.Playing)
                return;

            // Update time for current player
            var currentPlayer = _gameEngine.CurrentTurn == PlayerSymbol.X
                ? _gameEngine.Player1 : _gameEngine.Player2;

            currentPlayer.RemainingTime--;

            bool isMyTurnNow = (_isHost && _gameEngine.CurrentTurn == PlayerSymbol.X) ||
                               (!_isHost && _gameEngine.CurrentTurn == PlayerSymbol.O);

            if (isMyTurnNow)
            {
                _pbYouTime.Value = Math.Max(0, currentPlayer.RemainingTime);
                if (currentPlayer.RemainingTime < 30)
                {
                    _pbYouTime.ForeColor = Color.Red;
                    _pnlYouInfo.BackColor = _flashCount % 2 == 0
                        ? Color.FromArgb(255, 200, 200)
                        : Color.FromArgb(255, 240, 240);
                }

                if (currentPlayer.RemainingTime <= 0)
                {
                    _gameTimer.Stop();
                    MessageBox.Show("You ran out of time!", "Time Out",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                _pbOpponentTime.Value = Math.Max(0, currentPlayer.RemainingTime);
                if (currentPlayer.RemainingTime < 30)
                {
                    _pbOpponentTime.ForeColor = Color.Red;
                }
            }

            _flashCount++;
        }

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
                            btn.Font = new Font("Arial", _boardSize == 3 ? 36 : 20, FontStyle.Bold);
                        }
                        btn.Enabled = false;
                    }
                }
            }
        }

        protected override async void OnFormClosing(FormClosingEventArgs e)
        {
            _gameTimer?.Stop();
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
    }
}