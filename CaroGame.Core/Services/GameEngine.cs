using CaroGame.Core.Enums;
using CaroGame.Core.Interfaces;
using CaroGame.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaroGame.Core.Services
{
    /// <summary>
    /// Game Engine - Singleton quản lý toàn bộ logic game
    /// </summary>
    public class GameEngine
    {
        private static GameEngine _instance;
        private static readonly object _lock = new object();

        // Dependencies
        private readonly GameValidator _validator;
        private readonly MoveHistory _moveHistory;
        private IAIPlayer _aiPlayer;

        // Game state
        public Board Board { get; private set; }
        public Player Player1 { get; private set; }
        public Player Player2 { get; private set; }
        public PlayerSymbol CurrentTurn { get; private set; }
        public GameStatus Status { get; private set; }
        public GameMode Mode { get; private set; }
        public WinResult WinResult { get; private set; }

        // Events
        public event EventHandler<MoveEventArgs> MoveMade;
        public event EventHandler<GameOverEventArgs> GameOver;
        public event EventHandler<PlayerTurnChangedEventArgs> TurnChanged;
        public event EventHandler<UndoRedoEventArgs> UndoRequested;
        public event EventHandler<UndoRedoEventArgs> RedoRequested;

        public static GameEngine Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new GameEngine();
                        }
                    }
                }
                return _instance;
            }
        }

        private GameEngine()
        {
            _validator = new GameValidator();
            _moveHistory = new MoveHistory();
            Status = GameStatus.NotStarted;
        }

        /// <summary>
        /// Khởi tạo game mới
        /// </summary>
        public void StartNewGame(int boardSize, GameMode mode, bool computerPlaysFirst = false)
        {
            Board = new Board(boardSize);
            Mode = mode;
            _moveHistory.Clear();
            WinResult = null;

            // Khởi tạo players dựa trên mode
            switch (mode)
            {
                case GameMode.PlayerVsComputer:
                    if (computerPlaysFirst)
                    {
                        Player1 = new Player("Computer", PlayerSymbol.X, true);
                        Player2 = new Player("Player", PlayerSymbol.O, false);
                    }
                    else
                    {
                        Player1 = new Player("Player", PlayerSymbol.X, false);
                        Player2 = new Player("Computer", PlayerSymbol.O, true);
                    }
                    _aiPlayer = new AIPlayer(boardSize == 3 ? 5 : 3); // 3x3 sâu hơn, 19x19 nông hơn
                    break;

                case GameMode.PlayerVsPlayer:
                    Player1 = new Player("Player 1", PlayerSymbol.X, false);
                    Player2 = new Player("Player 2", PlayerSymbol.O, false);
                    break;

                case GameMode.Online:
                    // Sẽ được set từ bên ngoài
                    Player1 = new Player("You", PlayerSymbol.X, false);
                    Player2 = new Player("Opponent", PlayerSymbol.O, false);
                    break;
            }

            CurrentTurn = PlayerSymbol.X;
            Status = GameStatus.Playing;

            TurnChanged?.Invoke(this, new PlayerTurnChangedEventArgs(CurrentTurn));
        }

        /// <summary>
        /// Thực hiện nước đi
        /// </summary>
        public bool MakeMove(int row, int col)
        {
            if (Status != GameStatus.Playing)
                return false;

            if (!_validator.IsValidMove(Board, row, col))
                return false;

            // Đặt quân
            Board.PlaceMove(row, col, CurrentTurn);
            var move = new Move(row, col, CurrentTurn);
            _moveHistory.AddMove(move);

            // Raise event
            MoveMade?.Invoke(this, new MoveEventArgs(move));

            // Kiểm tra thắng
            var winResult = _validator.CheckWin(Board, row, col);
            if (winResult.IsWin)
            {
                WinResult = winResult;
                Status = winResult.Winner == PlayerSymbol.X ? GameStatus.XWin : GameStatus.OWin;
                GameOver?.Invoke(this, new GameOverEventArgs(Status, winResult));
                return true;
            }

            // Kiểm tra hòa
            if (_validator.IsBoardFull(Board))
            {
                Status = GameStatus.Draw;
                GameOver?.Invoke(this, new GameOverEventArgs(GameStatus.Draw, null));
                return true;
            }

            // Chuyển lượt
            SwitchTurn();
            return true;
        }

        /// <summary>
        /// AI thực hiện nước đi
        /// </summary>
        public Move MakeAIMove()
        {
            if (Mode != GameMode.PlayerVsComputer || _aiPlayer == null)
                return null;

            var currentPlayer = CurrentTurn == PlayerSymbol.X ? Player1 : Player2;
            if (!currentPlayer.IsComputer)
                return null;

            var move = _aiPlayer.GetBestMove(Board, CurrentTurn);
            if (move != null)
            {
                MakeMove(move.Row, move.Col);
            }

            return move;
        }

        /// <summary>
        /// Gợi ý nước đi tốt nhất
        /// </summary>
        public Move GetHint()
        {
            var currentPlayer = CurrentTurn == PlayerSymbol.X ? Player1 : Player2;

            if (currentPlayer.HintsRemaining <= 0)
                return null;

            // Tạo AI tạm để tính nước đi tốt nhất
            var hintAI = new AIPlayer(Board.Size == 3 ? 5 : 3);
            var hint = hintAI.GetBestMove(Board, CurrentTurn);

            if (hint != null)
            {
                currentPlayer.HintsRemaining--;
            }

            return hint;
        }

        /// <summary>
        /// Undo nước đi cuối cùng
        /// </summary>
        public bool Undo()
        {
            if (!_moveHistory.CanUndo())
                return false;

            var move = _moveHistory.Undo();
            Board.RemoveMove(move.Row, move.Col);

            // Nếu là PvC, undo cả nước của máy
            if (Mode == GameMode.PlayerVsComputer && _moveHistory.CanUndo())
            {
                var computerMove = _moveHistory.Undo();
                Board.RemoveMove(computerMove.Row, computerMove.Col);
            }

            // Reset game status nếu đã kết thúc
            if (Status != GameStatus.Playing)
            {
                Status = GameStatus.Playing;
                WinResult = null;
            }

            // Cập nhật lượt
            CurrentTurn = move.Symbol == PlayerSymbol.X ? PlayerSymbol.O : PlayerSymbol.X;
            TurnChanged?.Invoke(this, new PlayerTurnChangedEventArgs(CurrentTurn));

            UndoRequested?.Invoke(this, new UndoRedoEventArgs(move));
            return true;
        }

        /// <summary>
        /// Redo nước đi đã undo
        /// </summary>
        public bool Redo()
        {
            if (!_moveHistory.CanRedo())
                return false;

            var move = _moveHistory.Redo();
            Board.PlaceMove(move.Row, move.Col, move.Symbol);

            // Nếu là PvC, redo cả nước của máy
            if (Mode == GameMode.PlayerVsComputer && _moveHistory.CanRedo())
            {
                var computerMove = _moveHistory.Redo();
                Board.PlaceMove(computerMove.Row, computerMove.Col, computerMove.Symbol);
            }

            CurrentTurn = move.Symbol;
            TurnChanged?.Invoke(this, new PlayerTurnChangedEventArgs(CurrentTurn));

            RedoRequested?.Invoke(this, new UndoRedoEventArgs(move));
            return true;
        }

        /// <summary>
        /// Lấy GameState để save
        /// </summary>
        public GameState GetGameState()
        {
            return new GameState
            {
                BoardSize = Board.Size,
                Board = Board.Grid,
                MoveHistory = _moveHistory.GetAllMoves(),
                CurrentPlayer = CurrentTurn,
                PlayerXRemainingTime = Player1.RemainingTime,
                PlayerORemainingTime = Player2.RemainingTime,
                PlayerXHintsRemaining = Player1.HintsRemaining,
                PlayerOHintsRemaining = Player2.HintsRemaining,
                ComputerPlaysFirst = Mode == GameMode.PlayerVsComputer && Player1.IsComputer
            };
        }

        /// <summary>
        /// Load GameState
        /// </summary>
        public void LoadGameState(GameState state)
        {
            Board = new Board(state.BoardSize);
            Board.Grid = state.Board;
            CurrentTurn = state.CurrentPlayer;

            // Restore players
            if (state.ComputerPlaysFirst)
            {
                Player1 = new Player("Computer", PlayerSymbol.X, true);
                Player2 = new Player("Player", PlayerSymbol.O, false);
            }
            else
            {
                Player1 = new Player("Player", PlayerSymbol.X, false);
                Player2 = new Player("Computer", PlayerSymbol.O, true);
            }

            Player1.RemainingTime = state.PlayerXRemainingTime;
            Player2.RemainingTime = state.PlayerORemainingTime;
            Player1.HintsRemaining = state.PlayerXHintsRemaining;
            Player2.HintsRemaining = state.PlayerOHintsRemaining;

            _moveHistory.LoadMoves(state.MoveHistory);
            _aiPlayer = new AIPlayer(Board.Size == 3 ? 5 : 3);
            Mode = GameMode.PlayerVsComputer;
            Status = GameStatus.Playing;

            TurnChanged?.Invoke(this, new PlayerTurnChangedEventArgs(CurrentTurn));
        }

        public bool CanUndo() => _moveHistory.CanUndo();
        public bool CanRedo() => _moveHistory.CanRedo();

        private void SwitchTurn()
        {
            CurrentTurn = CurrentTurn == PlayerSymbol.X ? PlayerSymbol.O : PlayerSymbol.X;
            TurnChanged?.Invoke(this, new PlayerTurnChangedEventArgs(CurrentTurn));
        }
    }

    #region Event Args

    public class MoveEventArgs : EventArgs
    {
        public Move Move { get; }
        public MoveEventArgs(Move move) { Move = move; }
    }

    public class GameOverEventArgs : EventArgs
    {
        public GameStatus Status { get; }
        public WinResult WinResult { get; }
        public GameOverEventArgs(GameStatus status, WinResult winResult)
        {
            Status = status;
            WinResult = winResult;
        }
    }

    public class PlayerTurnChangedEventArgs : EventArgs
    {
        public PlayerSymbol CurrentPlayer { get; }
        public PlayerTurnChangedEventArgs(PlayerSymbol player) { CurrentPlayer = player; }
    }

    public class UndoRedoEventArgs : EventArgs
    {
        public Move Move { get; }
        public UndoRedoEventArgs(Move move) { Move = move; }
    }

    #endregion
}
