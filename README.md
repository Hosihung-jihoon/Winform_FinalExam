# 🎮 Caro Game - WinForms .NET 6

Game Caro (Gomoku) với 3 chế độ chơi: Player vs Computer, Player vs Player, và Online Multiplayer.

![.NET](https://img.shields.io/badge/.NET-6.0-purple)
![Platform](https://img.shields.io/badge/Platform-Windows-blue)
![License](https://img.shields.io/badge/License-MIT-green)

---

## 📋 Mục lục

- [Tính năng](#-tính-năng)
- [Yêu cầu hệ thống](#-yêu-cầu-hệ-thống)
- [Cài đặt](#-cài-đặt)
- [Cách chạy](#-cách-chạy)
- [Hướng dẫn chơi](#-hướng-dẫn-chơi)
- [Cấu trúc Project](#-cấu-trúc-project)
- [Troubleshooting](#-troubleshooting)

---

## ✨ Tính năng

### 🎯 Chế độ chơi
- **Player vs Computer**: Chơi với AI (Minimax + Alpha-Beta Pruning)
- **Player vs Player**: 2 người chơi trên 1 máy
- **Online Multiplayer**: Chơi online qua mã phòng 6 ký tự

### 🎲 Gameplay
- Bàn cờ 3x3 (3 quân thắng) hoặc 19x19 (5 quân thắng)
- Undo/Redo nước đi
- Gợi ý nước đi (Hint)
- Save/Load game (chế độ vs Computer)
- Timer đếm ngược 3 phút mỗi người

### 💬 Online Features
- Chat real-time
- Tạo/Join phòng bằng mã code
- Auto-reconnection (30 giây)
- Xác nhận Undo từ đối thủ

### 🎨 UI/UX
- Giao diện classic, dễ sử dụng
- Animation flash khi đặt quân
- Highlight hàng thắng
- Progress bar thời gian nhấp nháy khi < 30s

---

## 💻 Yêu cầu hệ thống

| Yêu cầu | Phiên bản |
|---------|-----------|
| OS | Windows 10/11 |
| .NET | 6.0 trở lên |
| Visual Studio | 2022 (khuyến nghị) |
| RAM | 4GB+ |

---

## 📦 Cài đặt

### Bước 1: Clone repository

```bash
git clone https://github.com/Hosihung-jihoon/Winform_FinalExam.git
cd Winform_FinalExam
```

### Bước 2: Mở Solution trong Visual Studio

```
1. Mở file CaroGame.sln
2. Chờ Visual Studio restore packages
```

### Bước 3: Install NuGet Packages (nếu cần)

Mở **Package Manager Console** (Tools → NuGet Package Manager → Package Manager Console):

```powershell
# Cho CaroGame.Infrastructure
Install-Package Microsoft.AspNetCore.SignalR.Client -Version 6.0.36 -ProjectName CaroGame.Infrastructure

# Cho CaroGame.WinForms
Install-Package Microsoft.AspNetCore.SignalR.Client -Version 6.0.36 -ProjectName CaroGame.WinForms
```

### Bước 4: Build Solution

```
Ctrl + Shift + B
```

### Bước 5: Copy hình ảnh

Copy file `x.png` và `o.png` vào:
```
CaroGame.WinForms/bin/Debug/net6.0-windows/
```

---

## ▶️ Cách chạy

### 🎮 Chế độ Offline (vs Computer / vs Friend)

**Cách 1: Chạy trong Visual Studio**
1. Set **CaroGame.WinForms** làm Startup Project
2. Nhấn **F5** hoặc click **Start**

**Cách 2: Chạy file .exe**
1. Mở folder: `CaroGame.WinForms/bin/Debug/net6.0-windows/`
2. Double-click `CaroGame.WinForms.exe`

---

### 🌐 Chế độ Online

**⚠️ QUAN TRỌNG: Phải chạy Server TRƯỚC khi chơi Online!**

#### Bước 1: Chạy Server

**Cách 1: Trong Visual Studio**
1. Right-click **CaroGame.Server** → **Set as Startup Project**
2. Nhấn **F5**
3. Chờ thấy message: `🎮 Caro Game SignalR Server Started!`

**Cách 2: Command Line**
```bash
cd CaroGame.Server/bin/Debug/net6.0
dotnet CaroGame.Server.dll
```

#### Bước 2: Chạy 2 Game Clients

**Instance 1:**
```bash
cd CaroGame.WinForms/bin/Debug/net6.0-windows
CaroGame.WinForms.exe
```

**Instance 2:**
```bash
# Mở folder và double-click CaroGame.WinForms.exe lần nữa
```

#### Bước 3: Tạo và Join phòng

| Player 1 (Host) | Player 2 (Guest) |
|-----------------|------------------|
| 1. Chọn "Play Online" | 1. Chọn "Play Online" |
| 2. Chọn kích thước bàn cờ | 2. Chọn kích thước bàn cờ |
| 3. Tab "Create Room" | 3. Tab "Join Room" |
| 4. Click "Create Room" | 4. Nhập mã 6 ký tự |
| 5. Copy mã, gửi cho bạn | 5. Click "Join" |
| 6. Chờ đối thủ join... | 6. Game bắt đầu! |

---

### ⚡ Chạy cả Server và Client cùng lúc (Development)

1. Right-click **Solution** → **Properties**
2. Chọn **Multiple startup projects**
3. Set:
   - `CaroGame.Server` → **Start**
   - `CaroGame.WinForms` → **Start**
4. Click **OK**
5. Nhấn **F5**

---

## 🎯 Hướng dẫn chơi

### Điều khiển cơ bản

| Nút | Chức năng |
|-----|-----------|
| **Undo** | Quay lại nước đi trước |
| **Redo** | Làm lại nước đã undo |
| **New Game** | Bắt đầu ván mới |
| **Hint** | Gợi ý nước đi tốt nhất |
| **Save** | Lưu game (chỉ vs Computer) |
| **Load** | Tải game đã lưu |
| **Menu** | Quay về menu chính |

### Luật chơi

- **Bàn 3x3**: Ai có 3 quân liên tiếp (ngang/dọc/chéo) thắng
- **Bàn 19x19**: Ai có 5 quân liên tiếp (ngang/dọc/chéo) thắng
- **Timer**: Mỗi người có 3 phút, hết giờ = thua
- **Hints**: Mỗi người có 3 lượt gợi ý

### Chế độ Online

- **Chat**: Gõ tin nhắn và nhấn Enter hoặc click Send
- **Undo**: Cần đối thủ đồng ý
- **Disconnect**: Có 30 giây để reconnect, nếu không = thua

---

## 📁 Cấu trúc Project

```
CaroGame.sln
│
├── 📂 CaroGame.Core (Class Library)
│   ├── Enums/
│   │   └── Enums.cs
│   ├── Models/
│   │   └── Models.cs
│   ├── Interfaces/
│   │   └── Interfaces.cs
│   └── Services/
│       ├── GameValidator.cs
│       ├── MoveHistory.cs
│       ├── AIPlayer.cs
│       └── GameEngine.cs
│
├── 📂 CaroGame.Infrastructure (Class Library)
│   ├── Repositories/
│   │   └── JsonGameRepository.cs
│   └── Networking/
│       └── SignalRClient.cs
│
├── 📂 CaroGame.Server (ASP.NET Core)
│   ├── Hubs/
│   │   └── GameHub.cs
│   ├── Services/
│   │   └── RoomManager.cs
│   ├── Program.cs
│   └── appsettings.json
│
└── 📂 CaroGame.WinForms (Windows Forms)
    ├── Forms/
    │   ├── MenuForm.cs
    │   ├── BoardSizeForm.cs
    │   ├── GameForm.cs
    │   └── OnlineGameForm.cs
    ├── Program.cs
    ├── x.png
    └── o.png
```

---

## 🔧 Troubleshooting

### ❌ Lỗi: "Failed to connect to server"

**Nguyên nhân:** Server chưa chạy hoặc sai URL

**Giải pháp:**
1. Đảm bảo **CaroGame.Server** đang chạy
2. Kiểm tra URL: `https://localhost:5001/gamehub`
3. Trust SSL certificate:
   ```bash
   dotnet dev-certs https --trust
   ```

---

### ❌ Lỗi: "Room not found"

**Nguyên nhân:** Mã phòng sai hoặc phòng đã bị xóa

**Giải pháp:**
1. Kiểm tra mã đúng 6 ký tự
2. Phòng tự xóa sau 30 giây nếu không có ai join
3. Tạo phòng mới

---

### ❌ Lỗi: Hình ảnh X/O không hiển thị

**Nguyên nhân:** Thiếu file `x.png` và `o.png`

**Giải pháp:**
Copy 2 file vào folder:
```
CaroGame.WinForms/bin/Debug/net6.0-windows/
```

---

### ❌ Lỗi: Không click được ô cờ sau khi Join room

**Nguyên nhân:** Thiếu event `OpponentJoined`

**Giải pháp:**
Đảm bảo trong `SignalRClient.cs` có:
```csharp
public event EventHandler OpponentJoined;

// Trong RegisterHandlers():
_connection.On("OpponentJoined", () =>
{
    OpponentJoined?.Invoke(this, EventArgs.Empty);
});
```

---

### ❌ Lỗi: CS0272 - Property 'Grid' không thể assign

**Nguyên nhân:** Property có `private set`

**Giải pháp:**
Trong `Models.cs`, sửa:
```csharp
// Từ:
public PlayerSymbol[,] Grid { get; private set; }

// Thành:
public PlayerSymbol[,] Grid { get; set; }
```

---

### ❌ Lỗi: Designer không mở được Form

**Nguyên nhân:** Code custom không tương thích với Designer

**Giải pháp:**
- Không dùng Designer, sửa code trực tiếp
- Hoặc tạo lại Form mới bằng Designer

---

## 🛠️ Tech Stack

- **Framework:** .NET 6.0
- **UI:** Windows Forms
- **Real-time:** SignalR
- **AI:** Minimax + Alpha-Beta Pruning
- **Data:** JSON file storage

---

## 📄 License

MIT License - Xem file [LICENSE](LICENSE) để biết thêm chi tiết.

---

## 👨‍💻 Author

**Your Name**
- GitHub: [@Hosihung-jihoon](https://github.com/Hosihung-jihoon)

---

## 🙏 Acknowledgments

- Microsoft .NET Team
- SignalR Documentation
- Minimax Algorithm Resources

---

**Enjoy playing! 🎮**
