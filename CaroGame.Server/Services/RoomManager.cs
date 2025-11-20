namespace CaroGame.Server.Services
{
    /// <summary>
    /// Quản lý các phòng chơi - Singleton
    /// </summary>
    public class RoomManager
    {
        private static RoomManager _instance;
        private static readonly object _lock = new object();

        private readonly Dictionary<string, GameRoom> _rooms;
        private readonly Random _random;

        public static RoomManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new RoomManager();
                        }
                    }
                }
                return _instance;
            }
        }

        private RoomManager()
        {
            _rooms = new Dictionary<string, GameRoom>();
            _random = new Random();
        }

        /// <summary>
        /// Tạo mã phòng ngẫu nhiên 6 ký tự
        /// </summary>
        private string GenerateRoomCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string code;
            do
            {
                code = new string(Enumerable.Repeat(chars, 6)
                    .Select(s => s[_random.Next(s.Length)]).ToArray());
            } while (_rooms.ContainsKey(code));

            return code;
        }

        /// <summary>
        /// Tạo phòng mới
        /// </summary>
        public string CreateRoom(string hostConnectionId)
        {
            var roomCode = GenerateRoomCode();
            var room = new GameRoom
            {
                RoomCode = roomCode,
                HostConnectionId = hostConnectionId,
                CreatedAt = DateTime.Now
            };

            _rooms[roomCode] = room;
            Console.WriteLine($"[RoomManager] Room created: {roomCode} by {hostConnectionId}");
            return roomCode;
        }

        /// <summary>
        /// Tham gia phòng
        /// </summary>
        public bool JoinRoom(string roomCode, string guestConnectionId)
        {
            if (!_rooms.TryGetValue(roomCode, out var room))
            {
                Console.WriteLine($"[RoomManager] Room not found: {roomCode}");
                return false;
            }

            if (room.GuestConnectionId != null)
            {
                Console.WriteLine($"[RoomManager] Room full: {roomCode}");
                return false;
            }

            room.GuestConnectionId = guestConnectionId;
            room.IsReady = true;
            Console.WriteLine($"[RoomManager] Guest joined room: {roomCode} - {guestConnectionId}");
            return true;
        }

        /// <summary>
        /// Lấy thông tin phòng
        /// </summary>
        public GameRoom GetRoom(string roomCode)
        {
            _rooms.TryGetValue(roomCode, out var room);
            return room;
        }

        /// <summary>
        /// Tìm phòng theo connection ID
        /// </summary>
        public GameRoom FindRoomByConnectionId(string connectionId)
        {
            return _rooms.Values.FirstOrDefault(r =>
                r.HostConnectionId == connectionId || r.GuestConnectionId == connectionId);
        }

        /// <summary>
        /// Lấy đối thủ trong phòng
        /// </summary>
        public string GetOpponentConnectionId(string roomCode, string currentConnectionId)
        {
            if (!_rooms.TryGetValue(roomCode, out var room))
                return null;

            if (room.HostConnectionId == currentConnectionId)
                return room.GuestConnectionId;
            else if (room.GuestConnectionId == currentConnectionId)
                return room.HostConnectionId;

            return null;
        }

        /// <summary>
        /// Đánh dấu người chơi disconnect
        /// </summary>
        public void MarkPlayerDisconnected(string roomCode, string connectionId)
        {
            if (_rooms.TryGetValue(roomCode, out var room))
            {
                if (room.HostConnectionId == connectionId)
                    room.IsHostConnected = false;
                else if (room.GuestConnectionId == connectionId)
                    room.IsGuestConnected = false;

                room.LastDisconnectTime = DateTime.Now;
                Console.WriteLine($"[RoomManager] Player disconnected from room: {roomCode} - {connectionId}");
            }
        }

        /// <summary>
        /// Đánh dấu người chơi reconnect
        /// </summary>
        public bool MarkPlayerReconnected(string roomCode, string connectionId)
        {
            if (_rooms.TryGetValue(roomCode, out var room))
            {
                if (room.HostConnectionId == connectionId)
                {
                    room.IsHostConnected = true;
                    Console.WriteLine($"[RoomManager] Host reconnected to room: {roomCode}");
                    return true;
                }
                else if (room.GuestConnectionId == connectionId)
                {
                    room.IsGuestConnected = true;
                    Console.WriteLine($"[RoomManager] Guest reconnected to room: {roomCode}");
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Xóa phòng
        /// </summary>
        public void RemoveRoom(string roomCode)
        {
            if (_rooms.Remove(roomCode))
            {
                Console.WriteLine($"[RoomManager] Room removed: {roomCode}");
            }
        }

        /// <summary>
        /// Kiểm tra và xóa phòng timeout (quá 30s không reconnect)
        /// </summary>
        public void CleanupTimeoutRooms()
        {
            var timeout = TimeSpan.FromSeconds(30);
            var now = DateTime.Now;
            var roomsToRemove = new List<string>();

            foreach (var room in _rooms.Values)
            {
                if (!room.IsHostConnected || !room.IsGuestConnected)
                {
                    if (room.LastDisconnectTime.HasValue &&
                        (now - room.LastDisconnectTime.Value) > timeout)
                    {
                        roomsToRemove.Add(room.RoomCode);
                    }
                }
            }

            foreach (var roomCode in roomsToRemove)
            {
                RemoveRoom(roomCode);
            }
        }

        /// <summary>
        /// Lấy số lượng phòng hiện tại
        /// </summary>
        public int GetActiveRoomCount()
        {
            return _rooms.Count;
        }
    }

    /// <summary>
    /// Model đại diện cho một phòng chơi
    /// </summary>
    public class GameRoom
    {
        public string RoomCode { get; set; }
        public string HostConnectionId { get; set; }
        public string GuestConnectionId { get; set; }
        public bool IsReady { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsHostConnected { get; set; } = true;
        public bool IsGuestConnected { get; set; } = true;
        public DateTime? LastDisconnectTime { get; set; }
    }
}