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
    /// Validator kiểm tra logic game
    /// </summary>
    public class GameValidator : IGameValidator
    {
        public bool IsValidMove(Board board, int row, int col)
        {
            return board.IsValidMove(row, col);
        }

        public WinResult CheckWin(Board board, int lastRow, int lastCol)
        {
            var symbol = board.Grid[lastRow, lastCol];
            if (symbol == PlayerSymbol.None)
            {
                return new WinResult { IsWin = false };
            }

            // Kiểm tra 4 hướng: ngang, dọc, chéo xuống, chéo lên
            var directions = new[]
            {
                (0, 1),   // Ngang
                (1, 0),   // Dọc
                (1, 1),   // Chéo xuống
                (1, -1)   // Chéo lên
            };

            foreach (var (dr, dc) in directions)
            {
                var result = CheckDirection(board, lastRow, lastCol, dr, dc, symbol);
                if (result.IsWin)
                {
                    return result;
                }
            }

            return new WinResult { IsWin = false };
        }

        private WinResult CheckDirection(Board board, int row, int col, int dr, int dc, PlayerSymbol symbol)
        {
            var cells = new List<(int, int)> { (row, col) };

            // Đếm về phía trước
            int count = 1;
            int r = row + dr;
            int c = col + dc;
            while (r >= 0 && r < board.Size && c >= 0 && c < board.Size && board.Grid[r, c] == symbol)
            {
                cells.Add((r, c));
                count++;
                r += dr;
                c += dc;
            }

            // Đếm về phía sau
            r = row - dr;
            c = col - dc;
            while (r >= 0 && r < board.Size && c >= 0 && c < board.Size && board.Grid[r, c] == symbol)
            {
                cells.Add((r, c));
                count++;
                r -= dr;
                c -= dc;
            }

            if (count >= board.WinCondition)
            {
                return new WinResult
                {
                    IsWin = true,
                    Winner = symbol,
                    WinningCells = cells
                };
            }

            return new WinResult { IsWin = false };
        }

        public bool IsBoardFull(Board board)
        {
            for (int i = 0; i < board.Size; i++)
            {
                for (int j = 0; j < board.Size; j++)
                {
                    if (board.Grid[i, j] == PlayerSymbol.None)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
