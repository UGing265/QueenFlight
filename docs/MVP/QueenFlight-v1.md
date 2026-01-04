# Project Title: QueenFlight Web - Real-time Aviation Tracking MVP

## 1. Project Overview & Purpose
**QueenFlight Web** là nền tảng theo dõi máy bay thời gian thực trên trình duyệt, sử dụng kiến trúc hiện đại (**Next.js + .NET**) để tối ưu tốc độ và trải nghiệm người dùng. Dự án tập trung vào việc hiển thị trực quan vị trí máy bay, cung cấp thông tin chuyến bay chi tiết và cảnh báo vùng lân cận cho người dùng mà không cần cài đặt ứng dụng. Hệ thống đóng vai trò như một "trạm radar cá nhân" xử lý dữ liệu thô từ các nhà cung cấp hàng không thành thông tin có ý nghĩa.

## 2. Key Objectives
*   **Real-time Accuracy:** Đồng bộ dữ liệu vị trí máy bay từ nguồn API uy tín với độ trễ dưới 10 giây.
*   **High Performance Data Processing:** Sử dụng sức mạnh của .NET để lọc và xử lý hàng nghìn máy bay cùng lúc mà không gây nghẽn server.
*   **Seamless User Experience:** Truy cập tức thì qua trình duyệt web trên mọi thiết bị (Mobile/Desktop).

## 3. Scope of Work (MVP Phase)

### In-Scope:
*   Giao diện bản đồ tương tác (Zoom, Pan, Click).
*   Hiển thị icon máy bay xoay theo hướng di chuyển (Heading).
*   Hiển thị vị trí người dùng (User Geolocation).
*   Popup thông tin chuyến bay: Số hiệu (Callsign), Độ cao, Tốc độ, Nước đăng ký.
*   Cơ chế Polling dữ liệu tự động từ **External API**.
*   Hệ thống lọc dữ liệu rác (máy bay không đủ thông tin) tại Backend.

### Out-of-Scope:
*   Lịch sử chuyến bay (Playback).
*   Dữ liệu lịch bay chi tiết (Giờ cất cánh/hạ cánh dự kiến - ETA/ETD) (Sẽ làm ở Phase 2 vì API này thường rất đắt).

## 4. Target Audience
*   **Aviation Enthusiasts:** Cần độ chính xác và tốc độ cập nhật nhanh.
*   **Travelers:** Muốn kiểm tra nhanh máy bay người thân đang ở đâu.

## 5. Technical Architecture & External APIs (Quan trọng)
Đây là trái tim của hệ thống. Chúng ta sẽ sử dụng các API cụ thể sau:

### 1. Nguồn Dữ Liệu Bay (Live Tracking Data)
*   **API Chọn dùng:** [OpenSky Network API](https://opensky-network.org/apidoc/) (Free Tier cho Research/Open Source).
*   **Endpoint:** `GET /states/all`
*   **Lý do:** Đây là nguồn dữ liệu mở tốt nhất hiện nay, cung cấp tọa độ (Lat/Lon), độ cao (Barometric Altitude), vận tốc, hướng bay và mã ICAO24 của hàng nghìn máy bay cùng lúc.
*   **Xử lý trong .NET:** Dùng `HttpClient` để gọi định kỳ (Polling).

### 2. Nguồn Dữ Liệu Tĩnh (Static Metadata)
*   **API Chọn dùng:** AviationStack (Freemium) hoặc FlightAware AeroAPI (Nếu có ngân sách). Với MVP, ta sẽ dùng Database nội bộ kết hợp AviationStack Free.
*   **Mục đích:** OpenSky chỉ trả về mã máy bay (ví dụ: "888152"), ta cần API này để map mã đó thành tên hãng (ví dụ: "Vietnam Airlines"), loại máy bay (ví dụ: "Boeing 787").
*   **Xử lý trong .NET:** Cache dữ liệu này cực lâu (24h-1 tuần) vì thông tin này ít thay đổi.

### 3. Map & Geocoding
*   **API Chọn dùng:** Mapbox GL JS (Frontend) + Mapbox Geocoding API.
*   **Lý do:** Mapbox cho phép tùy biến giao diện bản đồ "Dark Mode" (rất ngầu cho giao diện radar) tốt hơn Google Maps và chi phí rẻ hơn.

## 6. High-Level Requirements

### Functional Requirements
*   Hệ thống phải tự động gọi OpenSky API mỗi **10-15 giây** để lấy trạng thái mới nhất.
*   Backend phải so sánh dữ liệu mới và cũ, chỉ gửi về Client những máy bay có sự thay đổi vị trí (**Delta update**) để tiết kiệm băng thông.
*   API phải hỗ trợ tìm kiếm máy bay theo Callsign (ví dụ: gõ "VN123" trả về tọa độ).

### System Performance
*   Backend phải xử lý việc mapping dữ liệu từ JSON thô của OpenSky sang Object của C# trong dưới **100ms**.

## 7. Recommended Tech Stack & Implementation Strategy
*   **Frontend:** Next.js (App Router) + Mapbox GL JS.
*   **Backend:** ASP.NET Core 9 Web API.
*   **Communication:** SignalR (với MessagePack Protocol).
*   **Database:** PostgreSQL (Lưu metadata sân bay/hãng bay).
*   **Caching:** Redis (Lưu trạng thái máy bay tạm thời).

## 8. Cơ chế Xử lý Dữ liệu trong .NET (Chi tiết kỹ thuật)
Để tận dụng tối đa sức mạnh của .NET, bạn sẽ triển khai theo mô hình sau:

### Hosted Service (Background Worker)
*   Tạo một class `FlightDataFetcherService : BackgroundService`.
*   Service này chạy ngầm vô tận, cứ 10 giây sẽ gọi OpenSky API một lần.
*   Sử dụng `IHttpClientFactory` để quản lý kết nối HTTP, tránh lỗi cạn kiệt socket (Socket Exhaustion).
*   Sử dụng thư viện **Polly** để tự động thử lại (Retry) nếu OpenSky bị lỗi mạng.

### Data Processing Pipeline (LINQ & Parallel)
*   Khi nhận cục dữ liệu khổng lồ từ OpenSky (JSON array), dùng `Parallel.ForEach` hoặc `LINQ` để lọc nhanh:
    *   Loại bỏ máy bay không có tọa độ.
    *   Tính toán xem máy bay nào đang ở trong vùng Việt Nam (Bounding Box) nếu muốn tối ưu.
*   Map dữ liệu JSON sang C# Record (`public record FlightState(...)`) để bất biến và nhanh nhẹn.

### Real-time Broadcasting
*   Sau khi xử lý xong, Service sẽ đẩy list máy bay sạch vào **SignalR Hub**.
*   Hub sẽ broadcast xuống tất cả Client đang kết nối.

## 9. Phân tích lợi ích khác biệt (Next.js + .NET + OpenSky)
1.  **Khả năng chịu lỗi (Resilience) của .NET:** Khi gọi External API (OpenSky), mạng có thể chập chờn. .NET có thư viện **Polly** tích hợp sẵn vào `HttpClient`. Bạn có thể cấu hình: "Nếu lỗi, chờ 2s rồi thử lại, thử quá 3 lần thì mới báo lỗi". Node.js phải cài thư viện ngoài và cấu hình phức tạp hơn.
2.  **Kiểu dữ liệu mạnh (Strongly Typed HttpClient):** Bạn có thể dùng `System.Net.Http.Json` để tự động convert JSON từ OpenSky thẳng vào C# Object. Nếu API đổi cấu trúc, code sẽ báo lỗi ngay lúc biên dịch (Compile time), tránh việc web chạy rồi mới crash (Runtime error) như bên JS.
3.  **Hiệu suất bộ nhớ (Memory Management):** Việc xử lý một mảng JSON chứa 5000-10000 máy bay liên tục mỗi 10 giây sẽ tạo ra nhiều rác bộ nhớ. .NET 8 quản lý **Garbage Collection** cực tốt, giúp server không bị ăn ram (Memory Leak) khi chạy lâu dài, điều mà Node.js thường gặp khó khăn nếu code không kỹ.

## 10. Known Constraints
*   **OpenSky Rate Limit:** Tài khoản miễn phí của OpenSky giới hạn số lần gọi API/ngày và tốc độ cập nhật. Dữ liệu có thể trễ so với thực tế khoảng 10-20 giây.
*   **Dữ liệu không đầy đủ:** OpenSky là cộng đồng đóng góp, một số máy bay quân sự hoặc trực thăng nhỏ có thể không hiện hoặc thiếu thông tin.
