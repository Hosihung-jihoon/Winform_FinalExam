using CaroGame.Core.Enums;
using CaroGame.Core.Interfaces;
using CaroGame.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace CaroGame.Infrastructure.Repositories
{
    /// <summary>
    /// Repository để lưu/load game dưới dạng JSON
    /// </summary>
    public class JsonGameRepository : IGameRepository
    {
        private readonly JsonSerializerOptions _jsonOptions;

        public JsonGameRepository()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true, // Format đẹp để debug
                Converters = { new JsonStringEnumConverter() } // Lưu enum dưới dạng string
            };
        }

        /// <summary>
        /// Lưu game state vào file JSON
        /// </summary>
        public void SaveGame(GameState state, string filePath)
        {
            try
            {
                // Chuyển PlayerSymbol[,] thành List<List<PlayerSymbol>> để serialize
                var saveData = new SaveGameData
                {
                    BoardSize = state.BoardSize,
                    Board = ConvertBoardToList(state.Board, state.BoardSize),
                    MoveHistory = state.MoveHistory,
                    CurrentPlayer = state.CurrentPlayer,
                    PlayerXRemainingTime = state.PlayerXRemainingTime,
                    PlayerORemainingTime = state.PlayerORemainingTime,
                    PlayerXHintsRemaining = state.PlayerXHintsRemaining,
                    PlayerOHintsRemaining = state.PlayerOHintsRemaining,
                    ComputerPlaysFirst = state.ComputerPlaysFirst,
                    SavedAt = state.SavedAt
                };

                string json = JsonSerializer.Serialize(saveData, _jsonOptions);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to save game: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Load game state từ file JSON
        /// </summary>
        public GameState LoadGame(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"Save file not found: {filePath}");
                }

                string json = File.ReadAllText(filePath);
                var saveData = JsonSerializer.Deserialize<SaveGameData>(json, _jsonOptions);

                if (saveData == null)
                {
                    throw new InvalidDataException("Failed to deserialize save file");
                }

                // Chuyển List<List<PlayerSymbol>> về PlayerSymbol[,]
                var gameState = new GameState
                {
                    BoardSize = saveData.BoardSize,
                    Board = ConvertListToBoard(saveData.Board, saveData.BoardSize),
                    MoveHistory = saveData.MoveHistory ?? new List<Move>(),
                    CurrentPlayer = saveData.CurrentPlayer,
                    PlayerXRemainingTime = saveData.PlayerXRemainingTime,
                    PlayerORemainingTime = saveData.PlayerORemainingTime,
                    PlayerXHintsRemaining = saveData.PlayerXHintsRemaining,
                    PlayerOHintsRemaining = saveData.PlayerOHintsRemaining,
                    ComputerPlaysFirst = saveData.ComputerPlaysFirst,
                    SavedAt = saveData.SavedAt
                };

                return gameState;
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to load game: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Chuyển đổi mảng 2 chiều thành List để serialize
        /// </summary>
        private List<List<PlayerSymbol>> ConvertBoardToList(PlayerSymbol[,] board, int size)
        {
            var list = new List<List<PlayerSymbol>>();
            for (int i = 0; i < size; i++)
            {
                var row = new List<PlayerSymbol>();
                for (int j = 0; j < size; j++)
                {
                    row.Add(board[i, j]);
                }
                list.Add(row);
            }
            return list;
        }

        /// <summary>
        /// Chuyển đổi List về mảng 2 chiều
        /// </summary>
        private PlayerSymbol[,] ConvertListToBoard(List<List<PlayerSymbol>> list, int size)
        {
            var board = new PlayerSymbol[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    board[i, j] = list[i][j];
                }
            }
            return board;
        }

        /// <summary>
        /// Class trung gian để serialize JSON (vì không serialize được mảng 2 chiều trực tiếp)
        /// </summary>
        private class SaveGameData
        {
            public int BoardSize { get; set; }
            public List<List<PlayerSymbol>> Board { get; set; }
            public List<Move> MoveHistory { get; set; }
            public PlayerSymbol CurrentPlayer { get; set; }
            public int PlayerXRemainingTime { get; set; }
            public int PlayerORemainingTime { get; set; }
            public int PlayerXHintsRemaining { get; set; }
            public int PlayerOHintsRemaining { get; set; }
            public bool ComputerPlaysFirst { get; set; }
            public DateTime SavedAt { get; set; }
        }
    }
}
