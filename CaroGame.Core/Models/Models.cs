using CaroGame.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaroGame.Core.Models
{
    /// <summary>
    /// Đại diện cho một nước đi
    /// </summary>
    public class Move
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public PlayerSymbol Symbol { get; set; }
        public DateTime Timestamp { get; set; }

        public Move(int row, int col, PlayerSymbol symbol)
        {
            Row = row;
            Col = col;
            Symbol = symbol;
            Timestamp = DateTime.Now;
        }
    }

    /// <summary>
    /// Đại diện cho người chơi
    /// </summary>
    public class Player
    {
        public string Name { get; set; }
        public PlayerSymbol Symbol { get; set; }
        public int RemainingTime { get; set; } // Giây còn lại
        public int HintsRemaining { get; set; } // Số lượt gợi ý còn lại
        public bool IsComputer { get; set; }

        public Player(string name, PlayerSymbol symbol, bool isComputer = false)
        {
            Name = name;
            Symbol = symbol;
            RemainingTime = 180; // 3 phút = 180 giây
            HintsRemaining = 3;
            IsComputer = isComputer;
        }
    }

    /// <summary>
    /// Đại diện cho bàn cờ
    /// </summary>
    public class Board
    {
        public int Size { get; private set; }
        public int WinCondition { get; private set; } // Số quân cần để thắng
        public PlayerSymbol[,] Grid { get; set; }

        public Board(int size)
        {
            Size = size;
            WinCondition = size == 3 ? 3 : 5; // 3x3 cần 3 quân, 19x19 cần 5 quân
            Grid = new PlayerSymbol[size, size];
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    Grid[i, j] = PlayerSymbol.None;
                }
            }
        }

        public bool IsValidMove(int row, int col)
        {
            return row >= 0 && row < Size && col >= 0 && col < Size && Grid[row, col] == PlayerSymbol.None;
        }

        public void PlaceMove(int row, int col, PlayerSymbol symbol)
        {
            if (IsValidMove(row, col))
            {
                Grid[row, col] = symbol;
            }
        }

        public void RemoveMove(int row, int col)
        {
            Grid[row, col] = PlayerSymbol.None;
        }

        public void Clear()
        {
            InitializeBoard();
        }

        public Board Clone()
        {
            var clone = new Board(Size);
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    clone.Grid[i, j] = Grid[i, j];
                }
            }
            return clone;
        }
    }

    /// <summary>
    /// Lưu trạng thái game để save/load
    /// </summary>
    public class GameState
    {
        public int BoardSize { get; set; }
        public PlayerSymbol[,] Board { get; set; }
        public List<Move> MoveHistory { get; set; }
        public PlayerSymbol CurrentPlayer { get; set; }
        public int PlayerXRemainingTime { get; set; }
        public int PlayerORemainingTime { get; set; }
        public int PlayerXHintsRemaining { get; set; }
        public int PlayerOHintsRemaining { get; set; }
        public bool ComputerPlaysFirst { get; set; }
        public DateTime SavedAt { get; set; }

        public GameState()
        {
            MoveHistory = new List<Move>();
            SavedAt = DateTime.Now;
        }
    }

    /// <summary>
    /// Kết quả kiểm tra thắng
    /// </summary>
    public class WinResult
    {
        public bool IsWin { get; set; }
        public PlayerSymbol Winner { get; set; }
        public List<(int Row, int Col)> WinningCells { get; set; }

        public WinResult()
        {
            WinningCells = new List<(int, int)>();
        }
    }
}
