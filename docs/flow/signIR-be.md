📡 SignalR: Đài Phát Thanh Thời Gian Thực
Chào bạn, BackendLogic_Pro đây. Để tôi giải thích vai trò của SignalR trong hệ thống này.

1. SignalR làm gì? (The Concept)
Hãy tưởng tượng HTTP truyền thống giống như bạn gửi thư tay. Bạn hỏi Server: "Có máy bay nào mới không?", Server trả lời "Có" hoặc "Không". Bạn phải hỏi liên tục thì mới cập nhật được (Polling - tốn kém).

SignalR giống như Đài Radio FM.

Frontend chỉ cần "dò kênh" (Subscribe) một lần duy nhất.
Server có tin mới (máy bay di chuyển) là tự động bắn "Piu piu" xuống cho Frontend ngay lập tức.
Không cần hỏi, cứ ngồi im mà nhận tiền (data).
2. Chỉ chỗ hoạt động (Code Map Review)
Theo đúng chuẩn Clean Architecture mà tôi vừa refactor, luồng đi như sau:

📍 Bước 1: Interface (Hợp đồng phát sóng)

File: 
QueenFlight.Core/Interfaces/IFlightBroadcaster.cs
Code: 
BroadcastFlightCountAsync(int count)
Ý nghĩa: Core ra lệnh "Tôi muốn phát sóng con số này", nhưng không biết phát bằng cách nào (Loa phường hay Sofa).
📍 Bước 2: Implementation (Đài phát thật - nằm ở API Layer)

File: 
QueenFlight.API/Services/SignalRFlightBroadcaster.cs
Code: _hubContext.Clients.Group(...).SendAsync(...)
Ý nghĩa: Đây là thằng cầm loa SignalR để hét lên ReceiveFlightUpdate.
📍 Bước 3: Hub (Cái Loa - Endpoint)

File: 
QueenFlight.API/Hubs/FlightHub.cs
Code: 
SubscribeToUpdates()
Ý nghĩa: Đây là nơi Frontend kết nối vào. Frontend gọi hàm này giống như vặn núm radio để bắt đầu nghe.
📍 Bước 4: Trigger (Người bóp cò)

File: 
QueenFlight.Infrastructure/Services/FlightDataService.cs
 (Dòng 141)
Code: _broadcaster.BroadcastFlightCountAsync(count)
Hành động: Sau khi lưu Redis xong, Worker bấm nút này để báo cáo cho toàn dân thiên hạ biết.