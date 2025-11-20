using CaroGame.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaroGame.Core.Services
{
    /// <summary>
    /// Quản lý lịch sử nước đi để hỗ trợ Undo/Redo
    /// </summary>
    public class MoveHistory
    {
        private Stack<Move> _history;      // Stack lưu các nước đã đi
        private Stack<Move> _redoStack;    // Stack lưu các nước đã undo

        public MoveHistory()
        {
            _history = new Stack<Move>();
            _redoStack = new Stack<Move>();
        }

        /// <summary>
        /// Thêm nước đi mới vào history
        /// </summary>
        public void AddMove(Move move)
        {
            _history.Push(move);
            _redoStack.Clear(); // Khi đi nước mới, xóa hết redo stack
        }

        /// <summary>
        /// Undo nước đi cuối cùng
        /// </summary>
        public Move Undo()
        {
            if (_history.Count == 0)
                return null;

            var move = _history.Pop();
            _redoStack.Push(move);
            return move;
        }

        /// <summary>
        /// Redo nước đi đã bị undo
        /// </summary>
        public Move Redo()
        {
            if (_redoStack.Count == 0)
                return null;

            var move = _redoStack.Pop();
            _history.Push(move);
            return move;
        }

        /// <summary>
        /// Kiểm tra có thể undo không
        /// </summary>
        public bool CanUndo()
        {
            return _history.Count > 0;
        }

        /// <summary>
        /// Kiểm tra có thể redo không
        /// </summary>
        public bool CanRedo()
        {
            return _redoStack.Count > 0;
        }

        /// <summary>
        /// Xóa toàn bộ lịch sử
        /// </summary>
        public void Clear()
        {
            _history.Clear();
            _redoStack.Clear();
        }

        /// <summary>
        /// Lấy tất cả nước đi trong history (để save game)
        /// </summary>
        public List<Move> GetAllMoves()
        {
            return _history.Reverse().ToList();
        }

        /// <summary>
        /// Load history từ danh sách moves (khi load game)
        /// </summary>
        public void LoadMoves(List<Move> moves)
        {
            Clear();
            foreach (var move in moves)
            {
                _history.Push(move);
            }
        }
    }
}
