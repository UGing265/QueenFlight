# Project Title: QueenFlight Web - Real-time Aviation Tracking MVP

## 1. Project Overview & Purpose (Tổng quan dự án)
**QueenFlight Web** là nền tảng theo dõi máy bay thời gian thực trên trình duyệt, sử dụng kiến trúc hiện đại (**Next.js + .NET**) để tối ưu tốc độ. Hệ thống xử lý dữ liệu từ **AirLabs API**, sử dụng thuật toán dự đoán để hiển thị vị trí máy bay mượt mà trong khi vẫn duy trì mức chi phí thấp nhất.

## 2. Key Objectives (Mục tiêu chính)
*   **Real-time Accuracy (Độ chính xác):** Đồng bộ dữ liệu gốc từ AirLabs mỗi 120 giây.
*   **API Cost Optimization (Tối ưu chi phí):** Sống sót với giới hạn **1.000 requests/tháng** bằng kỹ thuật nội suy.
*   **High Performance (Hiệu suất cao):** Sử dụng **.NET** để lọc và xử lý hàng nghìn máy bay cùng lúc.
*   **Seamless UX (Trải nghiệm mượt mà):** Máy bay di chuyển liên tục **60fps** nhờ logic **Nội suy (Interpolation)** tại Frontend.

## 3. Scope of Work (Phạm vi công việc)
### In-Scope:
*   Giao diện bản đồ tương tác (Zoom, Pan, Click).
*   Hiển thị icon máy bay xoay theo hướng di chuyển (Heading).
*   Logic **Nội suy & Hiệu chỉnh (Interpolation & Re-sync):** Tự tính toán vị trí máy bay giữa các quãng nghỉ 120s của API.
*   Popup thông tin: Callsign, Độ cao, Tốc độ, Nước đăng ký.
*   Cơ chế **Thăm dò chậm (Slow Polling)** dữ liệu từ AirLabs.

### Out-of-Scope:
*   Lịch sử chuyến bay (Playback).
*   Dữ liệu ETA/ETD chi tiết.

## 4. Technical Architecture & External APIs (Kiến trúc & API)
### Nguồn Dữ Liệu Bay (Live Tracking):
*   **API:** AirLabs (Free Tier - 1.000 req/month).
*   **Tần suất:** ~120 giây/lần (tối đa 33 req/ngày).

### Nguồn Dữ Liệu Tĩnh (Static Metadata):
*   AviationStack hoặc Database nội bộ để map mã máy bay sang tên hãng và loại tàu bay.

### Map & Geocoding:
*   **Mapbox GL JS** để tùy biến giao diện radar.

## 5. High-Level Requirements (Yêu cầu hệ thống)
### Functional (Chức năng):
*   Backend tự động gọi AirLabs định kỳ.
*   Frontend tự động tính toán vị trí mới mỗi giây dựa trên vận tốc và hướng bay.
*   Hệ thống phải có hiệu ứng **Làm mượt (Smoothing)** khi đồng bộ dữ liệu thật để tránh giật lag.

### Performance (Hiệu suất):
*   Backend xử lý mapping và lọc dữ liệu thô dưới **100ms**.

## 6. Recommended Tech Stack (Công nghệ đề xuất)
*   **Frontend:** Next.js (App Router) + Mapbox GL JS.
*   **Backend:** ASP.NET Core 9 Web API.
*   **Communication:** SignalR (truyền dữ liệu gốc từ Backend xuống Client).
*   **Database:** PostgreSQL (Lưu metadata sân bay/hãng bay).
*   **Caching:** Redis (Lưu trạng thái máy bay cuối cùng để so sánh sai lệch và tính toán nội suy).

## 7. Cơ chế Xử lý Dữ liệu trong .NET & Logic Nội suy
### A. Tại Backend (.NET):
1.  **Hosted Service (Background Worker):** Chạy ngầm, cứ 120s gọi AirLabs một lần.
2.  **Polly:** Tự động **Thử lại (Retry)** nếu AirLabs lỗi mạng.
3.  **Data Processing:** Sử dụng LINQ/Parallel để lọc máy bay không đủ thông tin và lưu vào Redis để quản lý trạng thái.
4.  **Broadcasting:** Đẩy dữ liệu gốc xuống Frontend qua SignalR.

### B. Quy trình Nội suy tại Frontend (Interpolation & Re-sync):
*   **Bước 1 (Ground Truth):** Nhận dữ liệu thực từ SignalR: $(Lat_0, Lng_0)$, vận tốc ($v$), hướng ($\text{heading}$).
*   **Bước 2 (Interpolation Loop):** Trong 120s tiếp theo, Frontend tự tính:
    $$Lat_{new} = Lat_{old} + (v \times \cos(\text{heading}) \times \Delta t)$$
*   **Bước 3 (Re-sync):** Khi có dữ liệu thực mới, dùng Animation để kéo nhẹ icon về vị trí thật nếu có sai lệch, đảm bảo không bị nhảy vị trí đột ngột.

## 8. Phân tích lợi ích (Next.js + .NET + AirLabs)
*   **Khả năng chịu lỗi (Resilience):** .NET xử lý lỗi API ngoại vi cực tốt với Polly.
*   **Quản lý bộ nhớ (Memory Management):** Redis kết hợp với bộ thu gom rác của .NET 9 giúp hệ thống không bị "ăn RAM" khi xử lý hàng ngàn máy bay liên tục.
*   **Tiết kiệm chi phí:** Chiến lược nội suy giúp duy trì ứng dụng 24/7 chỉ với 1.000 requests/tháng.

## 9. Known Constraints & Assumptions (Ràng buộc & Giả định)
*   **AirLabs Rate Limit:** Giới hạn 1.000 req/tháng là rất chặt chẽ, bắt buộc phải dùng nội suy.
*   **Sai số tích lũy:** Vị trí tự tính toán sẽ lệch dần nếu máy bay thay đổi hướng trong 120s nghỉ của API.
