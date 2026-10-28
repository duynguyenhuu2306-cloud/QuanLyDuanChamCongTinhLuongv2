# Workly frontend

Frontend HTML/CSS/JavaScript thuần, gọi API thông qua Ocelot gateway.

## Chạy

Sau khi bật máy, chạy `START-WORKLY.bat` ở thư mục gốc dự án. Script khởi chạy frontend qua HTTP và HTTPS, tạo/xuất chứng chỉ phát triển localhost bằng .NET SDK, rồi mở solution trong Visual Studio để bạn chạy cấu hình multi-start `New Profile` (F5) cho các API. Lần đầu, chấp nhận hộp thoại Windows hỏi có tin cậy chứng chỉ localhost không.

Hoặc chạy frontend riêng bằng PowerShell:

```powershell
cd .\frontend
node .\server.js
```

Lệnh `node server.js` chỉ bật HTTP. Để bật cả hai giao thức, hãy dùng `START-WORKLY.bat` để cấu hình chứng chỉ và biến môi trường HTTPS.

Trang chủ được phục vụ từ `trangchu.html`. Mở bằng **`http://localhost:5500/`** hoặc **`https://localhost:5501/`**. HTTP và HTTPS dùng cổng riêng. Không mở trực tiếp bằng `file://`.

Gateway mặc định ở `https://localhost:7199`; đổi địa chỉ trong `config.js` nếu cần. Gateway đã cho phép CORS từ hai địa chỉ frontend trên; khởi động lại API Gateway sau khi cập nhật mã nguồn.

## API đã nối

- Dự án: `GET/POST /api-user/duan`
- Công việc: `GET/POST /api-user/congviec`
- Phân công: `GET /api-user/phancong`
- Chấm công: `GET /api-user/chamcong`, `GET /pending`, `POST`, `PUT /{id}/approve`, `PUT /{id}/reject`
- Nghỉ phép/OT: `GET /api-user/nghiphep`, `GET /pending`, `POST`, `PUT /{id}/approve`, `PUT /{id}/reject`, `GET /export/csv`
- Phiếu lương và kỳ lương: `GET/POST` API tương ứng; hoàn tất kỳ lương qua `PUT /{id}/complete`
- Nhân sự và chính sách lương: API Admin tương ứng.
- Kanban công việc: cập nhật trạng thái, sửa/xóa công việc và thêm/gỡ phân công qua API.
- Nghỉ phép / OT: nhân viên chỉ xem yêu cầu của mình; PM/Admin duyệt theo dự án phụ trách.
- Bảng lương: tạo kỳ Draft, tính phiếu lương bằng `POST /api-admin/chaybangluong/{id}/generate`, sau đó hoàn tất kỳ; nhân viên chỉ xem phiếu của mình qua `/api-user/phieuluong/me`. Phiếu lương có thể in hoặc lưu thành PDF từ trình duyệt.

Các API yêu cầu JWT và giới hạn quyền theo vai trò. Access token hết hạn thì cần đăng nhập lại; gateway hiện chưa công khai route refresh token. API chưa có endpoint xuất CSV cho dự án/công việc/bảng lương.

Trước khi chạy trên cơ sở dữ liệu hiện có, chạy lần lượt `Database/Migrations/20261010_timesheet_hardening.sql` và `Database/Migrations/20261014_leave_ot_module.sql` trong SQL Server. Sau đó build và khởi động lại `API_User`, `API_Admin` và `API_GateWay` để nạp route mới.
