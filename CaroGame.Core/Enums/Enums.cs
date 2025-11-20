using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaroGame.Core.Enums
{
    /// <summary>
    /// Các chế độ chơi của game
    /// </summary>
    public enum GameMode
    {
        PlayerVsComputer,   // Chơi với máy
        PlayerVsPlayer,     // 2 người chơi trên 1 máy
        Online              // Chơi online
    }

    /// <summary>
    /// Ký hiệu quân cờ
    /// </summary>
    public enum PlayerSymbol
    {
        None,   // Ô trống
        X,      // Quân X
        O       // Quân O
    }

    /// <summary>
    /// Trạng thái game
    /// </summary>
    public enum GameStatus
    {
        NotStarted,     // Chưa bắt đầu
        Playing,        // Đang chơi
        XWin,           // X thắng
        OWin,           // O thắng
        Draw,           // Hòa
        Paused,         // Tạm dừng (reconnection)
        Abandoned       // Bỏ cuộc
    }

    /// <summary>
    /// Hướng kiểm tra thắng
    /// </summary>
    public enum Direction
    {
        Horizontal,     // Ngang
        Vertical,       // Dọc
        DiagonalDown,   // Chéo xuống (\)
        DiagonalUp      // Chéo lên (/)
    }
}