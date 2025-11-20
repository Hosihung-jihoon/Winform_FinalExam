using CaroGame.Server.Services;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace CaroGame.Server.Hubs
{
    /// <summary>
    /// SignalR Hub xử lý real-time communication cho game
    /// </summary>
    public class GameHub : Hub
    {
        private readonly RoomManager _roomManager;

        public GameHub()
        {
            _roomManager = RoomManager.Instance;
        }

        /// <summary>
        /// Tạo phòng mới
        /// </summary>
        public async Task<string> CreateRoom()
        {
            var roomCode = _roomManager.CreateRoom(Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);

            Console.WriteLine($"[GameHub] CreateRoom: {roomCode} by {Context.ConnectionId}");
            return roomCode;
        }

        /// <summary>
        /// Tham gia phòng
        /// </summary>
        public async Task<bool> JoinRoom(string roomCode)
        {
            var success = _roomManager.JoinRoom(roomCode, Context.ConnectionId);

            if (success)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);

                // Thông báo cho host có người vào
                var room = _roomManager.GetRoom(roomCode);
                await Clients.Client(room.HostConnectionId).SendAsync("OpponentJoined");

                Console.WriteLine($"[GameHub] JoinRoom: {Context.ConnectionId} joined {roomCode}");
            }

            return success;
        }

        /// <summary>
        /// Gửi nước đi tới đối thủ
        /// </summary>
        public async Task SendMove(string roomCode, int row, int col, string symbol)
        {
            var opponentId = _roomManager.GetOpponentConnectionId(roomCode, Context.ConnectionId);

            if (opponentId != null)
            {
                await Clients.Client(opponentId).SendAsync("ReceiveMove", row, col, symbol);
                Console.WriteLine($"[GameHub] SendMove: {roomCode} - ({row},{col}) - {symbol}");
            }
        }

        /// <summary>
        /// Gửi tin nhắn chat
        /// </summary>
        public async Task SendChatMessage(string roomCode, string message)
        {
            var opponentId = _roomManager.GetOpponentConnectionId(roomCode, Context.ConnectionId);

            if (opponentId != null)
            {
                // Giới hạn 50 ký tự
                if (message.Length > 50)
                    message = message.Substring(0, 50);

                await Clients.Client(opponentId).SendAsync("ReceiveChatMessage", "Opponent", message);
                Console.WriteLine($"[GameHub] Chat: {roomCode} - {message}");
            }
        }

        /// <summary>
        /// Yêu cầu undo
        /// </summary>
        public async Task RequestUndo(string roomCode)
        {
            var opponentId = _roomManager.GetOpponentConnectionId(roomCode, Context.ConnectionId);

            if (opponentId != null)
            {
                await Clients.Client(opponentId).SendAsync("ReceiveUndoRequest");
                Console.WriteLine($"[GameHub] UndoRequest: {roomCode}");
            }
        }

        /// <summary>
        /// Trả lời yêu cầu undo
        /// </summary>
        public async Task RespondToUndo(string roomCode, bool accept)
        {
            var opponentId = _roomManager.GetOpponentConnectionId(roomCode, Context.ConnectionId);

            if (opponentId != null)
            {
                await Clients.Client(opponentId).SendAsync("ReceiveUndoResponse", accept);
                Console.WriteLine($"[GameHub] UndoResponse: {roomCode} - {accept}");
            }
        }

        /// <summary>
        /// Thông báo disconnect có chủ đích
        /// </summary>
        public async Task NotifyDisconnect(string roomCode)
        {
            var opponentId = _roomManager.GetOpponentConnectionId(roomCode, Context.ConnectionId);

            if (opponentId != null)
            {
                await Clients.Client(opponentId).SendAsync("OpponentDisconnected");
                Console.WriteLine($"[GameHub] NotifyDisconnect: {roomCode}");
            }

            // Xóa phòng
            _roomManager.RemoveRoom(roomCode);
        }

        /// <summary>
        /// Xử lý khi client disconnect
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var room = _roomManager.FindRoomByConnectionId(Context.ConnectionId);

            if (room != null)
            {
                _roomManager.MarkPlayerDisconnected(room.RoomCode, Context.ConnectionId);

                var opponentId = _roomManager.GetOpponentConnectionId(room.RoomCode, Context.ConnectionId);
                if (opponentId != null)
                {
                    await Clients.Client(opponentId).SendAsync("OpponentDisconnected");
                }

                Console.WriteLine($"[GameHub] Disconnect: {Context.ConnectionId} from {room.RoomCode}");

                // Cleanup task sẽ xóa phòng sau 30s nếu không reconnect
            }

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Xử lý khi client reconnect
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"[GameHub] Connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }
    }
}
