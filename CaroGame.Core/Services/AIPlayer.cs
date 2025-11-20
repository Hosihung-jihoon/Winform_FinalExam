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
    /// AI Player sử dụng thuật toán Minimax với Alpha-Beta Pruning
    /// </summary>
    public class AIPlayer : IAIPlayer
    {
        private readonly GameValidator _validator;
        private readonly int _maxDepth;
        private PlayerSymbol _aiSymbol;
        private PlayerSymbol _humanSymbol;

        public AIPlayer(int maxDepth = 3)
        {
            _validator = new GameValidator();
            _maxDepth = maxDepth; // Độ sâu cho bàn 19x19, có thể tăng lên 4-5 cho khó hơn
        }

        public Move GetBestMove(Board board, PlayerSymbol aiSymbol)
        {
            _aiSymbol = aiSymbol;
            _humanSymbol = aiSymbol == PlayerSymbol.X ? PlayerSymbol.O : PlayerSymbol.X;

            int bestScore = int.MinValue;
            Move bestMove = null;

            // Lấy danh sách các ô có thể đi (ưu tiên các ô gần quân đã đánh)
            var possibleMoves = GetPossibleMoves(board);

            foreach (var (row, col) in possibleMoves)
            {
                board.PlaceMove(row, col, _aiSymbol);
                int score = Minimax(board, _maxDepth - 1, int.MinValue, int.MaxValue, false);
                board.RemoveMove(row, col);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = new Move(row, col, _aiSymbol);
                }
            }

            // Fallback nếu không tìm được nước đi
            if (bestMove == null)
            {
                for (int i = 0; i < board.Size; i++)
                {
                    for (int j = 0; j < board.Size; j++)
                    {
                        if (board.IsValidMove(i, j))
                        {
                            return new Move(i, j, _aiSymbol);
                        }
                    }
                }
            }

            return bestMove;
        }

        private int Minimax(Board board, int depth, int alpha, int beta, bool isMaximizing)
        {
            // Kiểm tra điều kiện dừng
            var gameOverScore = EvaluateGameOver(board);
            if (gameOverScore.HasValue)
                return gameOverScore.Value;

            if (depth == 0)
                return EvaluateBoard(board);

            var possibleMoves = GetPossibleMoves(board);
            if (possibleMoves.Count == 0)
                return 0; // Hòa

            if (isMaximizing)
            {
                int maxScore = int.MinValue;
                foreach (var (row, col) in possibleMoves)
                {
                    board.PlaceMove(row, col, _aiSymbol);
                    int score = Minimax(board, depth - 1, alpha, beta, false);
                    board.RemoveMove(row, col);

                    maxScore = Math.Max(maxScore, score);
                    alpha = Math.Max(alpha, score);
                    if (beta <= alpha)
                        break; // Alpha-Beta pruning
                }
                return maxScore;
            }
            else
            {
                int minScore = int.MaxValue;
                foreach (var (row, col) in possibleMoves)
                {
                    board.PlaceMove(row, col, _humanSymbol);
                    int score = Minimax(board, depth - 1, alpha, beta, true);
                    board.RemoveMove(row, col);

                    minScore = Math.Min(minScore, score);
                    beta = Math.Min(beta, score);
                    if (beta <= alpha)
                        break; // Alpha-Beta pruning
                }
                return minScore;
            }
        }

        private int? EvaluateGameOver(Board board)
        {
            // Kiểm tra tất cả các ô xem có ai thắng không
            for (int i = 0; i < board.Size; i++)
            {
                for (int j = 0; j < board.Size; j++)
                {
                    if (board.Grid[i, j] != PlayerSymbol.None)
                    {
                        var result = _validator.CheckWin(board, i, j);
                        if (result.IsWin)
                        {
                            return result.Winner == _aiSymbol ? 10000 : -10000;
                        }
                    }
                }
            }

            if (_validator.IsBoardFull(board))
                return 0; // Hòa

            return null; // Game chưa kết thúc
        }

        private int EvaluateBoard(Board board)
        {
            int score = 0;

            // Đánh giá theo hàng, cột, chéo
            for (int i = 0; i < board.Size; i++)
            {
                for (int j = 0; j < board.Size; j++)
                {
                    if (board.Grid[i, j] != PlayerSymbol.None)
                    {
                        score += EvaluatePosition(board, i, j);
                    }
                }
            }

            return score;
        }

        private int EvaluatePosition(Board board, int row, int col)
        {
            var symbol = board.Grid[row, col];
            int score = 0;

            // Kiểm tra 4 hướng
            var directions = new[] { (0, 1), (1, 0), (1, 1), (1, -1) };

            foreach (var (dr, dc) in directions)
            {
                int count = CountConsecutive(board, row, col, dr, dc, symbol);
                int openEnds = CountOpenEnds(board, row, col, dr, dc, symbol);

                // Tính điểm dựa trên số quân liên tiếp và số đầu mở
                if (count >= board.WinCondition - 1) // 4 quân liên tiếp
                {
                    score += symbol == _aiSymbol ? 1000 : -1000;
                }
                else if (count == board.WinCondition - 2 && openEnds == 2) // 3 quân, 2 đầu mở
                {
                    score += symbol == _aiSymbol ? 100 : -100;
                }
                else if (count == board.WinCondition - 3 && openEnds == 2) // 2 quân, 2 đầu mở
                {
                    score += symbol == _aiSymbol ? 10 : -10;
                }
                else if (count > 0)
                {
                    score += symbol == _aiSymbol ? 1 : -1;
                }
            }

            return score;
        }

        private int CountConsecutive(Board board, int row, int col, int dr, int dc, PlayerSymbol symbol)
        {
            int count = 0;
            int r = row;
            int c = col;

            // Đếm về một phía
            while (r >= 0 && r < board.Size && c >= 0 && c < board.Size && board.Grid[r, c] == symbol)
            {
                count++;
                r += dr;
                c += dc;
            }

            // Đếm về phía ngược lại
            r = row - dr;
            c = col - dc;
            while (r >= 0 && r < board.Size && c >= 0 && c < board.Size && board.Grid[r, c] == symbol)
            {
                count++;
                r -= dr;
                c -= dc;
            }

            return count - 1; // Trừ đi ô hiện tại đã đếm 2 lần
        }

        private int CountOpenEnds(Board board, int row, int col, int dr, int dc, PlayerSymbol symbol)
        {
            int openEnds = 0;

            // Kiểm tra đầu thứ nhất
            int r = row + dr;
            int c = col + dc;
            while (r >= 0 && r < board.Size && c >= 0 && c < board.Size && board.Grid[r, c] == symbol)
            {
                r += dr;
                c += dc;
            }
            if (r >= 0 && r < board.Size && c >= 0 && c < board.Size && board.Grid[r, c] == PlayerSymbol.None)
                openEnds++;

            // Kiểm tra đầu thứ hai
            r = row - dr;
            c = col - dc;
            while (r >= 0 && r < board.Size && c >= 0 && c < board.Size && board.Grid[r, c] == symbol)
            {
                r -= dr;
                c -= dc;
            }
            if (r >= 0 && r < board.Size && c >= 0 && c < board.Size && board.Grid[r, c] == PlayerSymbol.None)
                openEnds++;

            return openEnds;
        }

        private List<(int, int)> GetPossibleMoves(Board board)
        {
            var moves = new List<(int, int)>();
            bool hasMove = false;

            // Tìm các ô trống gần quân đã đánh (trong bán kính 2 ô)
            for (int i = 0; i < board.Size; i++)
            {
                for (int j = 0; j < board.Size; j++)
                {
                    if (board.Grid[i, j] != PlayerSymbol.None)
                    {
                        hasMove = true;
                        // Thêm các ô xung quanh
                        for (int di = -2; di <= 2; di++)
                        {
                            for (int dj = -2; dj <= 2; dj++)
                            {
                                int ni = i + di;
                                int nj = j + dj;
                                if (board.IsValidMove(ni, nj) && !moves.Contains((ni, nj)))
                                {
                                    moves.Add((ni, nj));
                                }
                            }
                        }
                    }
                }
            }

            // Nếu bàn cờ trống, đi vào giữa
            if (!hasMove)
            {
                int center = board.Size / 2;
                moves.Add((center, center));
            }

            return moves;
        }
    }
}
