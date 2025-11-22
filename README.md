# Winform_FinalExam

# 🌐 Caro Game - Hướng dẫn chạy Online Mode

## ⚠️ QUAN TRỌNG

**Phải chạy Server TRƯỚC khi chơi Online!**

---

## 📋 Các bước thực hiện

### Bước 1: Build Solution

```
Ctrl + Shift + B
```

---

### Bước 2: Chạy Server

Mở folder:
```
CaroGame.Server/bin/Debug/net6.0/
```

Double-click file `CaroGame.Server.exe`

✅ **Thành công khi thấy:**
```
===========================================
🎮 Caro Game SignalR Server Started!
===========================================
Server URL: https://localhost:5001
Hub Endpoint: https://localhost:5001/gamehub
===========================================
```

⚠️ **Giữ cửa sổ này mở trong suốt quá trình chơi!**

---

### Bước 3: Chạy 2 Game Clients

Mở folder:
```
CaroGame.WinForms/bin/Debug/net6.0-windows/
```

- Double-click `CaroGame.WinForms.exe` → **Player 1**
- Double-click `CaroGame.WinForms.exe` lần nữa → **Player 2**

---

### Bước 4: Tạo và Join phòng

| Player 1 (Host) | Player 2 (Guest) |
|-----------------|------------------|
| 1. Click **"Play Online"** | 1. Click **"Play Online"** |
| 2. Chọn bàn cờ (3x3 hoặc 19x19) | 2. Chọn bàn cờ (giống Player 1) |
| 3. Tab **"Create Room"** | 3. Tab **"Join Room"** |
| 4. Click **"Create Room"** | 4. Nhập mã 6 ký tự |
| 5. Copy mã (VD: `ABC123`) | 5. Click **"Join"** |
| 6. Gửi mã cho bạn, chờ... | 6. Game bắt đầu! |

---

## 🎮 Bắt đầu chơi

- **Player 1 (Host)**: Quân X, đi trước
- **Player 2 (Guest)**: Quân O, đi sau
- **Chat**: Gõ tin nhắn → Enter hoặc click Send

---

## 🔧 Xử lý lỗi thường gặp

### ❌ "Failed to connect to server"
→ Server chưa chạy. Quay lại **Bước 2**.

### ❌ "Room not found"
→ Mã phòng sai hoặc hết hạn. Tạo phòng mới.

### ❌ Không click được ô cờ
→ Chưa đến lượt bạn, hoặc đối thủ chưa join.

### ❌ Lỗi SSL Certificate
Mở CMD (Admin), chạy:
```
dotnet dev-certs https --trust
```

---

## 📝 Tóm tắt nhanh

```
1. Chạy Server (.exe)     ← BẮT BUỘC
2. Chạy Game 1 (.exe)     ← Player 1
3. Chạy Game 2 (.exe)     ← Player 2
4. Create Room            ← Player 1
5. Join Room (nhập mã)    ← Player 2
6. Chơi game! 🎮
```
