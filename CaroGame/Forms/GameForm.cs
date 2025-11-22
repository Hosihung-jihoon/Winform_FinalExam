using CaroGame.Core.Enums;
using CaroGame.Core.Models;
using CaroGame.Core.Services;
using CaroGame.Infrastructure.Repositories;
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
    public partial class GameForm : Form
    {
        #region Fields

        private readonly int _boardSize;
        private readonly GameMode _gameMode;
        private readonly GameEngine _gameEngine;
        private readonly JsonGameRepository _repository;

        private Button[,] _boardButtons;
        private System.Windows.Forms.Timer _flashTimer;

        private Move _hintMove;
        private List<(int, int)> _winningCells;
        private int _flashCount;

        #endregion

        #region Contructor
        public GameForm()
        {
            InitializeComponent();
        }

        public GameForm(int boardSize, GameMode gameMode) : this()
        {
            _boardSize = boardSize;
            _gameMode = gameMode;
            _gameEngine = GameEngine.Instance;
            _repository = new JsonGameRepository();

            SetupForm();
            SetupHoverEffects();
            CreateBoardButtons();
            InitializeGame();
        }
        #endregion

        #region Setup Methods

        private void SetupForm()
        {
            // Tính toán kích thước form dựa trên boardSize
            int cellSize = _boardSize == 3 ? 100 : 35;
            int boardPixelSize = _boardSize * cellSize + (_boardSize - 1) * 2;

            // Cập nhật kích thước form
            this.ClientSize = new Size(boardPixelSize + 60, boardPixelSize + 200);

            // Cập nhật kích thước và vị trí các panels
            int panelWidth = (boardPixelSize - 20) / 2;

            pnlPlayer1Info.Size = new Size(panelWidth, 80);
            pnlPlayer1Info.Location = new Point(20, 60);

            pnlPlayer2Info.Size = new Size(panelWidth, 80);
            pnlPlayer2Info.Location = new Point(panelWidth + 40, 60);

            // Cập nhật progressbar width
            pbPlayer1Time.Size = new Size(panelWidth - 20, 20);
            pbPlayer2Time.Size = new Size(panelWidth - 20, 20);

            // Cập nhật board panel
            pnlBoard.Size = new Size(boardPixelSize, boardPixelSize);
            pnlBoard.Location = new Point(20, 150);

            // Cập nhật vị trí button Menu
            btnMenu.Location = new Point(this.ClientSize.Width - 90, 7);

            // Ẩn Save/Load nếu không phải PvC
            if (_gameMode != GameMode.PlayerVsComputer)
            {
                btnSave.Visible = false;
                btnLoad.Visible = false;
            }
        }

        private void SetupHoverEffects()
        {
            // Toolbar buttons hover
            var toolbarButtons = new[] { btnUndo, btnRedo, btnNewGame, btnHint, btnSave, btnLoad };
            foreach (var btn in toolbarButtons)
            {
                btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(52, 152, 219);
                btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(41, 128, 185);
            }

            // Menu button hover
            btnMenu.MouseEnter += (s, e) => btnMenu.BackColor = Color.FromArgb(231, 76, 60);
            btnMenu.MouseLeave += (s, e) => btnMenu.BackColor = Color.FromArgb(192, 57, 43);
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
                        Tag = new Point(i, j)
                    };
                    btn.FlatAppearance.BorderColor = Color.Black;
                    btn.FlatAppearance.BorderSize = 1;
                    btn.Click += BoardButton_Click;

                    _boardButtons[i, j] = btn;
                    pnlBoard.Controls.Add(btn);
                }
            }
        }

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
            UpdateHintsDisplay();

            // Start timer
            gameTimer.Enabled = true;

            // Nếu máy đi trước
            if (computerPlaysFirst)
            {
                Task.Delay(500).ContinueWith(t => this.Invoke(() => MakeAIMove()));
            }
        }

        #endregion

        #region UI Update Methods

        private void UpdatePlayerNames()
        {
            lblPlayer1Name.Text = $"{_gameEngine.Player1.Name} ({_gameEngine.Player1.Symbol})";
            lblPlayer2Name.Text = $"{_gameEngine.Player2.Name} ({_gameEngine.Player2.Symbol})";
        }

        private void UpdateTurnIndicator()
        {
            var currentPlayer = _gameEngine.CurrentTurn == PlayerSymbol.X
                ? _gameEngine.Player1 : _gameEngine.Player2;

            if (currentPlayer == _gameEngine.Player1)
            {
                pnlPlayer1Info.BackColor = Color.FromArgb(255, 240, 240);
                pnlPlayer2Info.BackColor = Color.White;
            }
            else
            {
                pnlPlayer1Info.BackColor = Color.White;
                pnlPlayer2Info.BackColor = Color.FromArgb(255, 240, 240);
            }
        }

        private void UpdateHintsDisplay()
        {
            lblPlayer1Hints.Text = $"Hints: {_gameEngine.Player1.HintsRemaining}/3";
            lblPlayer2Hints.Text = $"Hints: {_gameEngine.Player2.HintsRemaining}/3";
        }

        #endregion

        #region Board Events

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

        #endregion

        #region Game Engine Events

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
                btn.Font = new Font("Arial", _boardSize == 3 ? 36 : 16, FontStyle.Bold);
                btn.ForeColor = move.Symbol == PlayerSymbol.X ? Color.Blue : Color.Red;
            }

            btn.Enabled = false;
            _hintMove = null;
        }

        private void OnGameOver(object sender, GameOverEventArgs e)
        {
            gameTimer.Enabled = false;

            if (e.WinResult != null && e.WinResult.WinningCells.Count > 0)
            {
                _winningCells = e.WinResult.WinningCells;
                FlashWinningCells();
            }

            string message = e.Status == GameStatus.Draw ? "Draw!" :
                e.WinResult.Winner == PlayerSymbol.X ?
                $"{_gameEngine.Player1.Name} wins!" :
                $"{_gameEngine.Player2.Name} wins!";

            Task.Delay(2200).ContinueWith(t => this.Invoke(() =>
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

        private void OnTurnChanged(object sender, PlayerTurnChangedEventArgs e)
        {
            UpdateTurnIndicator();
            UpdateHintsDisplay();
        }

        #endregion

        #region Animation Methods

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

        #region Timer Event

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            if (_gameEngine.Status != GameStatus.Playing)
                return;

            var currentPlayer = _gameEngine.CurrentTurn == PlayerSymbol.X
                ? _gameEngine.Player1 : _gameEngine.Player2;

            currentPlayer.RemainingTime--;

            // Update progress bars
            pbPlayer1Time.Value = Math.Max(0, _gameEngine.Player1.RemainingTime);
            pbPlayer2Time.Value = Math.Max(0, _gameEngine.Player2.RemainingTime);

            // Blinking effect when < 30s
            if (currentPlayer.RemainingTime < 30)
            {
                var panel = currentPlayer == _gameEngine.Player1 ? pnlPlayer1Info : pnlPlayer2Info;
                panel.BackColor = _flashCount % 2 == 0
                    ? Color.FromArgb(255, 200, 200)
                    : Color.FromArgb(255, 240, 240);
            }

            // Time out
            if (currentPlayer.RemainingTime <= 0)
            {
                gameTimer.Enabled = false;
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

        #endregion

        #region Toolbar Button Events

        private void btnUndo_Click(object sender, EventArgs e)
        {
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

        private void btnRedo_Click(object sender, EventArgs e)
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

        private void btnNewGame_Click(object sender, EventArgs e)
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

        private void btnHint_Click(object sender, EventArgs e)
        {
            var currentPlayer = _gameEngine.CurrentTurn == PlayerSymbol.X
                ? _gameEngine.Player1 : _gameEngine.Player2;

            if (_gameMode == GameMode.PlayerVsComputer && currentPlayer.IsComputer)
            {
                MessageBox.Show("Computer doesn't need hints!", "Hint",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (currentPlayer.HintsRemaining <= 0)
            {
                MessageBox.Show("No hints remaining!", "Hint",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var hint = _gameEngine.GetHint();
            if (hint != null)
            {
                _hintMove = hint;
                _boardButtons[hint.Row, hint.Col].BackColor = Color.Yellow;
                FlashCell(hint.Row, hint.Col, true);
                UpdateHintsDisplay();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
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
                        MessageBox.Show("Game saved successfully!", "Save Game",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to save game: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show(
                "Load a saved game? Current progress will be lost.",
                "Load Game",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmResult != DialogResult.Yes)
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

                        pbPlayer1Time.Value = _gameEngine.Player1.RemainingTime;
                        pbPlayer2Time.Value = _gameEngine.Player2.RemainingTime;

                        MessageBox.Show("Game loaded successfully!", "Load Game",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to load game: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Return to menu? Current progress will be lost.",
                "Back to Menu",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                if (_gameMode == GameMode.PlayerVsComputer && _gameEngine.Status == GameStatus.Playing)
                {
                    var saveResult = MessageBox.Show(
                        "Do you want to save before exiting?",
                        "Save Game",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question);

                    if (saveResult == DialogResult.Yes)
                    {
                        btnSave_Click(sender, e);
                    }
                    else if (saveResult == DialogResult.Cancel)
                    {
                        return;
                    }
                }

                this.Close();
            }
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
        }

        private void ResetGame()
        {
            gameTimer.Enabled = false;

            // Unsubscribe events
            _gameEngine.MoveMade -= OnMoveMade;
            _gameEngine.GameOver -= OnGameOver;
            _gameEngine.TurnChanged -= OnTurnChanged;

            // Clear board
            foreach (var btn in _boardButtons)
            {
                btn.BackgroundImage = null;
                btn.Text = "";
                btn.Enabled = true;
                btn.BackColor = Color.White;
            }

            // Reset progress bars
            pbPlayer1Time.Value = 180;
            pbPlayer2Time.Value = 180;
            pnlPlayer1Info.BackColor = Color.White;
            pnlPlayer2Info.BackColor = Color.White;

            // Restart game
            InitializeGame();
        }

        #endregion

        #region Form Events

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            gameTimer.Enabled = false;
            _flashTimer?.Stop();

            _gameEngine.MoveMade -= OnMoveMade;
            _gameEngine.GameOver -= OnGameOver;
            _gameEngine.TurnChanged -= OnTurnChanged;

            base.OnFormClosing(e);
        }

        #endregion
    }
}
