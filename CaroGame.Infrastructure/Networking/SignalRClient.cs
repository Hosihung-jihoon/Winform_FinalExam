using CaroGame.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace CaroGame.Infrastructure.Networking
{
    /// <summary>
    /// Wrapper cho SignalR Client để kết nối với server
    /// </summary>
    public class SignalRClient
    {
        private HubConnection _connection;
        private string _roomCode;
        private string _playerId;

        // Events
        public event EventHandler<string> RoomCreated;
        public event EventHandler<bool> RoomJoined;
        public event EventHandler OpponentJoined;
        public event EventHandler<MoveReceivedEventArgs> OpponentMoveReceived;
        public event EventHandler<ChatMessageEventArgs> ChatMessageReceived;
        public event EventHandler OpponentDisconnected;
        public event EventHandler OpponentReconnected;
        public event EventHandler<string> ErrorOccurred;
        public event EventHandler<UndoRequestEventArgs> UndoRequestReceived;
        public event EventHandler<bool> UndoResponseReceived;

        public bool IsConnected => _connection?.State == HubConnectionState.Connected;
        public string RoomCode => _roomCode;

        /// <summary>
        /// Kết nối tới SignalR Hub
        /// </summary>
        public async Task<bool> ConnectAsync(string serverUrl = "https://localhost:5001/gamehub")
        {
            try
            {
                _connection = new HubConnectionBuilder()
                    .WithUrl(serverUrl)
                    .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5) })
                    .Build();

                // Register handlers
                RegisterHandlers();

                await _connection.StartAsync();
                _playerId = _connection.ConnectionId;
                return true;
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"Connection failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Tạo phòng mới
        /// </summary>
        public async Task CreateRoomAsync()
        {
            try
            {
                if (!IsConnected)
                {
                    ErrorOccurred?.Invoke(this, "Not connected to server");
                    return;
                }

                _roomCode = await _connection.InvokeAsync<string>("CreateRoom");
                RoomCreated?.Invoke(this, _roomCode);
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"Failed to create room: {ex.Message}");
            }
        }

        /// <summary>
        /// Tham gia phòng
        /// </summary>
        public async Task<bool> JoinRoomAsync(string roomCode)
        {
            try
            {
                if (!IsConnected)
                {
                    ErrorOccurred?.Invoke(this, "Not connected to server");
                    return false;
                }

                var success = await _connection.InvokeAsync<bool>("JoinRoom", roomCode);
                if (success)
                {
                    _roomCode = roomCode;
                    RoomJoined?.Invoke(this, true);
                }
                else
                {
                    RoomJoined?.Invoke(this, false);
                    ErrorOccurred?.Invoke(this, "Room not found or full");
                }
                return success;
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"Failed to join room: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gửi nước đi tới đối thủ
        /// </summary>
        public async Task SendMoveAsync(int row, int col, PlayerSymbol symbol)
        {
            try
            {
                if (!IsConnected || string.IsNullOrEmpty(_roomCode))
                    return;

                await _connection.InvokeAsync("SendMove", _roomCode, row, col, symbol.ToString());
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"Failed to send move: {ex.Message}");
            }
        }

        /// <summary>
        /// Gửi tin nhắn chat
        /// </summary>
        public async Task SendChatMessageAsync(string message)
        {
            try
            {
                if (!IsConnected || string.IsNullOrEmpty(_roomCode))
                    return;

                if (message.Length > 50)
                    message = message.Substring(0, 50);

                await _connection.InvokeAsync("SendChatMessage", _roomCode, message);
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"Failed to send chat: {ex.Message}");
            }
        }

        /// <summary>
        /// Yêu cầu undo từ đối thủ
        /// </summary>
        public async Task RequestUndoAsync()
        {
            try
            {
                if (!IsConnected || string.IsNullOrEmpty(_roomCode))
                    return;

                await _connection.InvokeAsync("RequestUndo", _roomCode);
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"Failed to request undo: {ex.Message}");
            }
        }

        /// <summary>
        /// Trả lời yêu cầu undo
        /// </summary>
        public async Task RespondToUndoAsync(bool accept)
        {
            try
            {
                if (!IsConnected || string.IsNullOrEmpty(_roomCode))
                    return;

                await _connection.InvokeAsync("RespondToUndo", _roomCode, accept);
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"Failed to respond to undo: {ex.Message}");
            }
        }

        /// <summary>
        /// Thông báo disconnect
        /// </summary>
        public async Task NotifyDisconnectAsync()
        {
            try
            {
                if (!IsConnected || string.IsNullOrEmpty(_roomCode))
                    return;

                await _connection.InvokeAsync("NotifyDisconnect", _roomCode);
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"Failed to notify disconnect: {ex.Message}");
            }
        }

        /// <summary>
        /// Ngắt kết nối
        /// </summary>
        public async Task DisconnectAsync()
        {
            try
            {
                if (_connection != null)
                {
                    await NotifyDisconnectAsync();
                    await _connection.StopAsync();
                    await _connection.DisposeAsync();
                }
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"Disconnect error: {ex.Message}");
            }
        }

        /// <summary>
        /// Đăng ký các handlers để nhận events từ server
        /// </summary>
        private void RegisterHandlers()
        {
            // Opponent joined (cho Host)
            _connection.On("OpponentJoined", () =>
            {
                OpponentJoined?.Invoke(this, EventArgs.Empty);
            });

            // Nhận nước đi từ đối thủ
            _connection.On<int, int, string>("ReceiveMove", (row, col, symbolStr) =>
            {
                Enum.TryParse<PlayerSymbol>(symbolStr, out var symbol);
                OpponentMoveReceived?.Invoke(this, new MoveReceivedEventArgs(row, col, symbol));
            });

            // Nhận tin nhắn chat
            _connection.On<string, string>("ReceiveChatMessage", (sender, message) =>
            {
                ChatMessageReceived?.Invoke(this, new ChatMessageEventArgs(sender, message));
            });

            // Đối thủ disconnect
            _connection.On("OpponentDisconnected", () =>
            {
                OpponentDisconnected?.Invoke(this, EventArgs.Empty);
            });

            // Đối thủ reconnect
            _connection.On("OpponentReconnected", () =>
            {
                OpponentReconnected?.Invoke(this, EventArgs.Empty);
            });

            // Nhận yêu cầu undo
            _connection.On("ReceiveUndoRequest", () =>
            {
                UndoRequestReceived?.Invoke(this, new UndoRequestEventArgs());
            });

            // Nhận phản hồi undo
            _connection.On<bool>("ReceiveUndoResponse", (accepted) =>
            {
                UndoResponseReceived?.Invoke(this, accepted);
            });

            // Xử lý reconnection
            _connection.Reconnecting += error =>
            {
                ErrorOccurred?.Invoke(this, "Connection lost, reconnecting...");
                return Task.CompletedTask;
            };

            _connection.Reconnected += connectionId =>
            {
                ErrorOccurred?.Invoke(this, "Reconnected successfully");
                return Task.CompletedTask;
            };

            _connection.Closed += error =>
            {
                ErrorOccurred?.Invoke(this, "Connection closed");
                return Task.CompletedTask;
            };

        }
    }

    #region Event Args

    public class MoveReceivedEventArgs : EventArgs
    {
        public int Row { get; }
        public int Col { get; }
        public PlayerSymbol Symbol { get; }

        public MoveReceivedEventArgs(int row, int col, PlayerSymbol symbol)
        {
            Row = row;
            Col = col;
            Symbol = symbol;
        }
    }

    public class ChatMessageEventArgs : EventArgs
    {
        public string Sender { get; }
        public string Message { get; }
        public DateTime Timestamp { get; }

        public ChatMessageEventArgs(string sender, string message)
        {
            Sender = sender;
            Message = message;
            Timestamp = DateTime.Now;
        }
    }

    public class UndoRequestEventArgs : EventArgs
    {
        public DateTime RequestedAt { get; }

        public UndoRequestEventArgs()
        {
            RequestedAt = DateTime.Now;
        }
    }

    #endregion
}
