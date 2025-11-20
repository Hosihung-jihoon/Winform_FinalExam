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
using CaroGame.Infrastructure.Repositories;

namespace CaroGame.WinForms.Forms
{
    /// <summary>
    /// Form game chính cho PvP và PvC
    /// </summary>
    public partial class GameForm : Form
    {
        private readonly int _boardSize;
        private readonly GameMode _gameMode;
        private readonly GameEngine _gameEngine;
        private readonly JsonGameRepository _repository;

        private Button[,] _boardButtons;
        private Panel _boardPanel;
        private System.Windows.Forms.Timer _gameTimer;
        private System.Windows.Forms.Timer _flashTimer;

        // UI Components
        private Label _lblPlayer1Name;
        private Label _lblPlayer2Name;
        private ProgressBar _pbPlayer1Time;
        private ProgressBar _pbPlayer2Time;
        private Label _lblPlayer1Hints;
        private Label _lblPlayer2Hints;
        private Panel _pnlPlayer1Info;
        private Panel _pnlPlayer2Info;

        private Move _hintMove;
        private List<(int, int)> _winningCells;
        private int _flashCount;

        public GameForm(int boardSize, GameMode gameMode)
        {
            _boardSize = boardSize;
            _gameMode = gameMode;
            _gameEngine = GameEngine.Instance;
            _repository = new JsonGameRepository();

            InitializeComponent();
            InitializeGame();
        }

        private void InitializeComponent()
        {
            this.Text = "Caro Game";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Calculate form size based on board
            int cellSize = _boardSize == 3 ? 100 : 35;
            int boardPixelSize = _boardSize * cellSize + (_boardSize - 1) * 2;
            int formWidth = boardPixelSize + 40;
            int formHeight = boardPixelSize + 200;
            this.ClientSize = new Size(formWidth, formHeight);
            this.BackColor = Color.White;

            CreateToolbar();
            CreatePlayerInfoPanels(boardPixelSize);
            CreateBoard(cellSize, boardPixelSize);
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

            var btnUndo = new Button { Text = "Undo", Location = new Point(btnX, btnY) };
            btnStyle(btnUndo);
            btnUndo.Click += BtnUndo_Click;
            toolbar.Controls.Add(btnUndo);

            var btnRedo = new Button { Text = "Redo", Location = new Point(btnX + spacing, btnY) };
            btnStyle(btnRedo);
            btnRedo.Click += BtnRedo_Click;
            toolbar.Controls.Add(btnRedo);

            var btnNewGame = new Button { Text = "New Game", Location = new Point(btnX + spacing * 2, btnY) };
            btnStyle(btnNewGame);
            btnNewGame.Click += BtnNewGame_Click;
            toolbar.Controls.Add(btnNewGame);

            var btnHint = new Button { Text = "Hint", Location = new Point(btnX + spacing * 3, btnY) };
            btnStyle(btnHint);
            btnHint.Click += BtnHint_Click;
            toolbar.Controls.Add(btnHint);

            // Save/Load chỉ hiện trong PvC
            if (_gameMode == GameMode.PlayerVsComputer)
            {
                var btnSave = new Button { Text = "Save", Location = new Point(btnX + spacing * 4, btnY) };
                btnStyle(btnSave);
                btnSave.Click += BtnSave_Click;
                toolbar.Controls.Add(btnSave);

                var btnLoad = new Button { Text = "Load", Location = new Point(btnX + spacing * 5, btnY) };
                btnStyle(btnLoad);
                btnLoad.Click += BtnLoad_Click;
                toolbar.Controls.Add(btnLoad);
            }

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

        private void CreatePlayerInfoPanels(int boardPixelSize)
        {
            int panelWidth = boardPixelSize / 2 - 10;
            int panelHeight = 80;
            int panelY = 60;

            // Player 1 Info
            _pnlPlayer1Info = CreatePlayerPanel(10, panelY, panelWidth, panelHeight, true);
            this.Controls.Add(_pnlPlayer1Info);

            // Player 2 Info
            _pnlPlayer2Info = CreatePlayerPanel(panelWidth + 30, panelY, panelWidth, panelHeight, false);
            this.Controls.Add(_pnlPlayer2Info);
        }

        private Panel CreatePlayerPanel(int x, int y, int width, int height, bool isPlayer1)
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

            if (isPlayer1)
            {
                _lblPlayer1Name = lblName;
                _pbPlayer1Time = pbTime;
                _lblPlayer1Hints = lblHints;
            }
            else
            {
                _lblPlayer2Name = lblName;
                _pbPlayer2Time = pbTime;
                _lblPlayer2Hints = lblHints;
            }

            return panel;
        }

        private void CreateBoard(int cellSize, int boardPixelSize)
        {
            _boardPanel = new Panel
            {
                Location = new Point(20, 150),
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
                        Tag = new Point(i, j)
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

        // Continue in Part 2...

        private void InitializeGame()
        {
            // Hỏi người chơi có muốn đi trước không (chỉ PvC)
            bool computerPlaysFirst = false;
            if (_gameMode == GameMode.PlayerVsComputer)
            {
                var result = MessageBox.Show(
                    "Do you want to play first (X)?",
                    "Choose Player",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                computerPlaysFirst = (result == DialogResult.No);
            }

            _gameEngine.StartNewGame(_boardSize, _gameMode, computerPlaysFirst);

            // Subscribe to events
            _gameEngine.MoveMade += OnMoveMade;
            _gameEngine.GameOver += OnGameOver;
            _gameEngine.TurnChanged += OnTurnChanged;

            // Update UI
            UpdatePlayerNames();
            UpdateTurnIndicator();

            // Start timer
            _gameTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _gameTimer.Tick += GameTimer_Tick;
            _gameTimer.Start();

            // Nếu máy đi trước
            if (computerPlaysFirst)
            {
                Task.Delay(500).ContinueWith(t => this.Invoke(() => MakeAIMove()));
            }
        }

        private void UpdatePlayerNames()
        {
            _lblPlayer1Name.Text = $"{_gameEngine.Player1.Name} ({_gameEngine.Player1.Symbol})";
            _lblPlayer2Name.Text = $"{_gameEngine.Player2.Name} ({_gameEngine.Player2.Symbol})";
        }

        private void UpdateTurnIndicator()
        {
            var currentPlayer = _gameEngine.CurrentTurn == PlayerSymbol.X
                ? _gameEngine.Player1 : _gameEngine.Player2;

            _pnlPlayer1Info.BorderStyle = currentPlayer == _gameEngine.Player1
                ? BorderStyle.FixedSingle : BorderStyle.FixedSingle;
            _pnlPlayer2Info.BorderStyle = currentPlayer == _gameEngine.Player2
                ? BorderStyle.FixedSingle : BorderStyle.FixedSingle;

            // Red border for current player
            if (currentPlayer == _gameEngine.Player1)
            {
                _pnlPlayer1Info.BackColor = Color.FromArgb(255, 240, 240);
                _pnlPlayer2Info.BackColor = Color.White;
            }
            else
            {
                _pnlPlayer1Info.BackColor = Color.White;
                _pnlPlayer2Info.BackColor = Color.FromArgb(255, 240, 240);
            }
        }

        private void BoardButton_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            var pos = (Point)btn.Tag;

            // Kiểm tra nếu đang là lượt máy
            var currentPlayer = _gameEngine.CurrentTurn == PlayerSymbol.X
                ? _gameEngine.Player1 : _gameEngine.Player2;
            if (currentPlayer.IsComputer)
                return;

            // Thực hiện nước đi
            if (_gameEngine.MakeMove(pos.X, pos.Y))
            {
                // Flash animation
                FlashCell(pos.X, pos.Y);

                // Nếu là PvC và game chưa kết thúc, máy sẽ đi
                if (_gameMode == GameMode.PlayerVsComputer &&
                    _gameEngine.Status == GameStatus.Playing)
                {
                    Task.Delay(500).ContinueWith(t => this.Invoke(() => MakeAIMove()));
                }
            }
        }

        private void MakeAIMove()
        {
            var move = _gameEngine.MakeAIMove();
            if (move != null)
            {
                FlashCell(move.Row, move.Col);
            }
        }

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

                if (_flashCount >= (isHint ? 4 : 4)) // Flash 2 lần
                {
                    _flashTimer.Stop();
                    _flashTimer.Dispose();
                    btn.BackColor = originalColor;
                }
            };
            _flashTimer.Start();
        }

        private void OnMoveMade(object sender, MoveEventArgs e)
        {
            var move = e.Move;
            var btn = _boardButtons[move.Row, move.Col];

            // Load image
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

            // Clear hint
            if (_hintMove != null)
            {
                _hintMove = null;
            }
        }

        private void OnGameOver(object sender, GameOverEventArgs e)
        {
            _gameTimer.Stop();

            // Highlight winning cells
            if (e.WinResult != null && e.WinResult.WinningCells.Count > 0)
            {
                _winningCells = e.WinResult.WinningCells;
                FlashWinningCells();
            }

            // Show result
            string message = e.Status == GameStatus.Draw ? "Draw!" :
                e.WinResult.Winner == PlayerSymbol.X ?
                $"{_gameEngine.Player1.Name} wins!" :
                $"{_gameEngine.Player2.Name} wins!";

            Task.Delay(2000).ContinueWith(t => this.Invoke(() =>
            {
                var result = MessageBox.Show(
                    message + "\n\nPlay again?",
                    "Game Over",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                {
                    ResetGame();
                }
                else
                {
                    this.Close();
                }
            }));
        }

        private void FlashWinningCells()
        {
            _flashCount = 0;
            var flashTimer = new System.Windows.Forms.Timer { Interval = 200 };
            var flashColor = Color.FromArgb(217, 119, 87); // #d97757

            flashTimer.Tick += (s, e) =>
            {
                foreach (var (row, col) in _winningCells)
                {
                    var btn = _boardButtons[row, col];
                    btn.BackColor = _flashCount % 2 == 0 ? flashColor : Color.White;
                }
                _flashCount++;

                if (_flashCount >= 20) // Flash 10 lần
                {
                    flashTimer.Stop();
                    flashTimer.Dispose();
                }
            };
            flashTimer.Start();
        }

        private void OnTurnChanged(object sender, PlayerTurnChangedEventArgs e)
        {
            UpdateTurnIndicator();
            UpdateHintsDisplay();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (_gameEngine.Status != GameStatus.Playing)
                return;

            var currentPlayer = _gameEngine.CurrentTurn == PlayerSymbol.X
                ? _gameEngine.Player1 : _gameEngine.Player2;

            currentPlayer.RemainingTime--;

            if (currentPlayer == _gameEngine.Player1)
            {
                _pbPlayer1Time.Value = Math.Max(0, currentPlayer.RemainingTime);
                if (currentPlayer.RemainingTime < 30)
                {
                    _pbPlayer1Time.ForeColor = Color.Red;
                    // Blinking effect
                    _pnlPlayer1Info.BackColor = _flashCount % 2 == 0
                        ? Color.FromArgb(255, 200, 200)
                        : Color.FromArgb(255, 240, 240);
                }
            }
            else
            {
                _pbPlayer2Time.Value = Math.Max(0, currentPlayer.RemainingTime);
                if (currentPlayer.RemainingTime < 30)
                {
                    _pbPlayer2Time.ForeColor = Color.Red;
                    _pnlPlayer2Info.BackColor = _flashCount % 2 == 0
                        ? Color.FromArgb(255, 200, 200)
                        : Color.FromArgb(255, 240, 240);
                }
            }

            // Time out
            if (currentPlayer.RemainingTime <= 0)
            {
                _gameTimer.Stop();
                var winner = currentPlayer == _gameEngine.Player1
                    ? _gameEngine.Player2 : _gameEngine.Player1;
                MessageBox.Show(
                    $"{currentPlayer.Name} ran out of time!\n{winner.Name} wins!",
                    "Time Out",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                this.Close();
            }

            _flashCount++;
        }

        private void UpdateHintsDisplay()
        {
            _lblPlayer1Hints.Text = $"Hints: {_gameEngine.Player1.HintsRemaining}/3";
            _lblPlayer2Hints.Text = $"Hints: {_gameEngine.Player2.HintsRemaining}/3";
        }

        // Continue in Part 3 for button handlers...

        private void BtnUndo_Click(object sender, EventArgs e)
        {
            // Trong PvP, cần xác nhận từ đối thủ
            if (_gameMode == GameMode.PlayerVsPlayer)
            {
                var result = MessageBox.Show(
                    "Do you want to undo?",
                    "Undo Request",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                    return;
            }

            if (_gameEngine.CanUndo())
            {
                // Lưu vị trí để xóa hình ảnh
                var moveToUndo = _gameEngine.Board.Grid;

                if (_gameEngine.Undo())
                {
                    RefreshBoard();
                    UpdateTurnIndicator();
                }
            }
            else
            {
                MessageBox.Show("No moves to undo!", "Undo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnRedo_Click(object sender, EventArgs e)
        {
            if (_gameEngine.CanRedo())
            {
                if (_gameEngine.Redo())
                {
                    RefreshBoard();
                    UpdateTurnIndicator();
                }
            }
            else
            {
                MessageBox.Show("No moves to redo!", "Redo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnNewGame_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Start a new game?",
                "New Game",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                ResetGame();
            }
        }

        private void BtnHint_Click(object sender, EventArgs e)
        {
            var currentPlayer = _gameEngine.CurrentTurn == PlayerSymbol.X
                ? _gameEngine.Player1 : _gameEngine.Player2;

            // Kiểm tra trong PvP, mỗi người có 3 lượt
            if (_gameMode == GameMode.PlayerVsPlayer)
            {
                if (currentPlayer.HintsRemaining <= 0)
                {
                    MessageBox.Show(
                        "No hints remaining!",
                        "Hint",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }
            }
            // Trong PvC, chỉ Player có hint
            else if (_gameMode == GameMode.PlayerVsComputer)
            {
                if (currentPlayer.IsComputer)
                {
                    MessageBox.Show(
                        "Computer doesn't need hints!",
                        "Hint",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                if (currentPlayer.HintsRemaining <= 0)
                {
                    MessageBox.Show(
                        "No hints remaining!",
                        "Hint",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }
            }

            var hint = _gameEngine.GetHint();
            if (hint != null)
            {
                _hintMove = hint;
                var btn = _boardButtons[hint.Row, hint.Col];

                // Highlight hint cell
                var originalColor = btn.BackColor;
                btn.BackColor = Color.Yellow;

                // Flash until player makes a move
                FlashCell(hint.Row, hint.Col, true);

                UpdateHintsDisplay();
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Caro Save Files (*.json)|*.json";
                saveDialog.DefaultExt = "json";
                saveDialog.FileName = $"CaroGame_{DateTime.Now:yyyyMMdd_HHmmss}.json";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var gameState = _gameEngine.GetGameState();
                        _repository.SaveGame(gameState, saveDialog.FileName);
                        MessageBox.Show(
                            "Game saved successfully!",
                            "Save Game",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Failed to save game: {ex.Message}",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Load a saved game? Current progress will be lost.",
                "Load Game",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "Caro Save Files (*.json)|*.json";
                openDialog.DefaultExt = "json";

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var gameState = _repository.LoadGame(openDialog.FileName);
                        _gameEngine.LoadGameState(gameState);

                        RefreshBoard();
                        UpdatePlayerNames();
                        UpdateTurnIndicator();
                        UpdateHintsDisplay();

                        _pbPlayer1Time.Value = _gameEngine.Player1.RemainingTime;
                        _pbPlayer2Time.Value = _gameEngine.Player2.RemainingTime;

                        MessageBox.Show(
                            "Game loaded successfully!",
                            "Load Game",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Failed to load game: {ex.Message}",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnMenu_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Return to menu? Current progress will be lost.",
                "Back to Menu",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                // Hỏi có muốn save không (chỉ PvC)
                if (_gameMode == GameMode.PlayerVsComputer && _gameEngine.Status == GameStatus.Playing)
                {
                    var saveResult = MessageBox.Show(
                        "Do you want to save before exiting?",
                        "Save Game",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question);

                    if (saveResult == DialogResult.Yes)
                    {
                        BtnSave_Click(sender, e);
                    }
                    else if (saveResult == DialogResult.Cancel)
                    {
                        return;
                    }
                }

                this.Close();
            }
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

        private void ResetGame()
        {
            _gameTimer?.Stop();

            // Clear board
            foreach (var btn in _boardButtons)
            {
                btn.BackgroundImage = null;
                btn.Text = "";
                btn.Enabled = true;
                btn.BackColor = Color.White;
            }

            // Restart game
            InitializeGame();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _gameTimer?.Stop();
            _flashTimer?.Stop();

            // Unsubscribe events
            _gameEngine.MoveMade -= OnMoveMade;
            _gameEngine.GameOver -= OnGameOver;
            _gameEngine.TurnChanged -= OnTurnChanged;

            base.OnFormClosing(e);
        }
    }
}