using CaroGame.Core.Enums;
using CaroGame.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaroGame.Core.Interfaces
{
    /// <summary>
    /// Interface cho AI player
    /// </summary>
    public interface IAIPlayer
    {
        Move GetBestMove(Board board, PlayerSymbol aiSymbol);
    }

    /// <summary>
    /// Interface cho Repository lưu/load game
    /// </summary>
    public interface IGameRepository
    {
        void SaveGame(GameState state, string filePath);
        GameState LoadGame(string filePath);
    }

    /// <summary>
    /// Interface cho validator kiểm tra logic game
    /// </summary>
    public interface IGameValidator
    {
        bool IsValidMove(Board board, int row, int col);
        WinResult CheckWin(Board board, int lastRow, int lastCol);
        bool IsBoardFull(Board board);
    }
}
