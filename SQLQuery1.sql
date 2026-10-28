USE QuanLyDuAn;
GO

IF COL_LENGTH('dbo.ChamCong', 'IsDeleted') IS NULL
BEGIN
    ALTER TABLE dbo.ChamCong
    ADD IsDeleted BIT NOT NULL
        CONSTRAINT DF_ChamCong_IsDeleted DEFAULT 0;
END;
GO

IF COL_LENGTH('dbo.ChamCong', 'DeletedAt') IS NULL
BEGIN
    ALTER TABLE dbo.ChamCong
    ADD DeletedAt DATETIME NULL;
END;
GO

IF COL_LENGTH('dbo.ChamCong', 'LyDoTuChoi') IS NULL
BEGIN
    ALTER TABLE dbo.ChamCong
    ADD LyDoTuChoi NVARCHAR(500) NULL;
END;
GO

IF COL_LENGTH('dbo.NghiPhep', 'Lydo') IS NULL
BEGIN
    ALTER TABLE dbo.NghiPhep
    ADD Lydo NVARCHAR(500) NULL;
END;

IF COL_LENGTH('dbo.NghiPhep', 'LydoTuChoi') IS NULL
BEGIN
    ALTER TABLE dbo.NghiPhep
    ADD LydoTuChoi NVARCHAR(500) NULL;
END;
GO

/* =========================================================
   CHUẨN HÓA DỮ LIỆU MẪU THEO ĐỀ TÀI QUẢN LÝ DỰ ÁN VÀ CHẤM CÔNG
   Giữ nguyên mã định danh để bảo toàn các khóa ngoại.
   ========================================================= */
SET NOCOUNT ON;
GO
UPDATE d
SET Tenduan = N'Triển khai hệ thống quản lý dự án và chấm công - ' + x.TenCongTy,
    Motada = N'Triển khai nền tảng quản lý dự án, công việc, nhân sự, chấm công, nghỉ phép, làm thêm giờ và tính lương theo quy trình của ' + x.TenCongTy + N'.'
FROM dbo.DuAn AS d
JOIN (VALUES
  ('DA001', N'Công ty Minh Long', N'Triển khai nền tảng quản lý dự án, công việc và chấm công cho Công ty Minh Long.'),
  ('DA002', N'Công ty An Phát', N'Số hóa hồ sơ nhân sự, tài khoản và phân quyền trên hệ thống quản trị nội bộ.'),
  ('DA003', N'Công ty Sao Việt', N'Triển khai quản lý danh mục dự án, công việc và phân công nhân sự.'),
  ('DA004', N'Công ty Thành Công', N'Xây dựng quy trình ghi nhận và phê duyệt giờ công theo ngày.'),
  ('DA005', N'Công ty Hoàng Hà', N'Triển khai cổng nhân viên cho đăng ký nghỉ phép và làm thêm giờ.'),
  ('DA006', N'Công ty Đại Dương', N'Xây dựng hệ thống chấm công theo công việc và dự án.'),
  ('DA007', N'Công ty Phương Nam', N'Tích hợp quy trình duyệt chấm công giữa nhân viên và quản lý dự án.'),
  ('DA008', N'Công ty Tân Tiến', N'Triển khai theo dõi tiến độ và phân công công việc theo dự án.'),
  ('DA009', N'Công ty Ánh Dương', N'Số hóa báo cáo giờ làm và khối lượng công việc.'),
  ('DA010', N'Công ty Hòa Bình', N'Triển khai quản lý công việc và tổng hợp timesheet.'),
  ('DA011', N'Công ty Nam Việt', N'Tích hợp bảng chấm công với danh sách nhân viên.'),
  ('DA012', N'Công ty Bắc Hà', N'Triển khai quy trình quản lý dự án và cập nhật trạng thái công việc.'),
  ('DA013', N'Công ty Đại Phát', N'Xây dựng biểu mẫu đăng ký nghỉ phép trực tuyến.'),
  ('DA014', N'Công ty Việt An', N'Triển khai quy trình duyệt nghỉ phép và làm thêm giờ.'),
  ('DA015', N'Công ty Hưng Thịnh', N'Xây dựng bảng lương dựa trên giờ công và giờ làm thêm.'),
  ('DA016', N'Công ty Gia Bảo', N'Triển khai chính sách lương theo giờ và hệ số làm thêm.'),
  ('DA017', N'Công ty Phú Quý', N'Xây dựng báo cáo tiến độ dự án và năng suất nhân sự.'),
  ('DA018', N'Công ty Kim Long', N'Triển khai dashboard quản lý nhân sự và tình trạng công việc.'),
  ('DA019', N'Công ty Vạn Thành', N'Tích hợp xác thực và phân quyền người dùng cho hệ thống quản trị.'),
  ('DA020', N'Công ty Đông Á', N'Triển khai API quản lý dự án và công việc qua API Gateway.'),
  ('DA021', N'Công ty Tâm Đức', N'Tối ưu dữ liệu chấm công và truy vấn báo cáo giờ làm.'),
  ('DA022', N'Công ty Thịnh Vượng', N'Triển khai lưu vết thao tác trên dữ liệu nhân sự và dự án.'),
  ('DA023', N'Công ty Bảo Minh', N'Cấu hình bảo mật HTTPS và kết nối các phân hệ quản lý.'),
  ('DA024', N'Công ty Hồng Hà', N'Kiểm thử quy trình phân công và cập nhật tiến độ công việc.'),
  ('DA025', N'Công ty Việt Nhật', N'Kiểm thử luồng ghi nhận, gửi duyệt và phê duyệt chấm công.'),
  ('DA026', N'Công ty Đại Thành', N'Kiểm thử đăng ký nghỉ phép, OT và xử lý yêu cầu.'),
  ('DA027', N'Công ty Bình Minh', N'Kiểm thử tính lương, khoản trừ và phiếu lương nhân viên.'),
  ('DA028', N'Công ty Trường Sơn', N'Xây dựng chức năng xuất báo cáo giờ công và bảng lương.'),
  ('DA029', N'Công ty Lộc Phát', N'Triển khai sao lưu, phục hồi và quản trị dữ liệu hệ thống.'),
  ('DA030', N'Công ty Việt Tiến', N'Nâng cấp hiệu năng và hoàn thiện hệ thống quản lý dự án, chấm công.')
) AS x(MaDuAn, TenCongTy, MoTa) ON x.MaDuAn = d.Maduan;
GO
UPDATE t
SET Maduan = x.MaDuAn, Tieude = x.TieuDe, Mota = x.MoTa
FROM dbo.CongViec AS t
JOIN (VALUES
  ('CV001', 'DA001', N'Khảo sát nghiệp vụ quản lý dự án và chấm công', N'Thu thập yêu cầu về dự án, nhân sự, phân công, timesheet, nghỉ phép và tính lương.'),
  ('CV002', 'DA002', N'Thiết kế mô hình cơ sở dữ liệu', N'Thiết kế các bảng người dùng, dự án, công việc, chấm công, nghỉ phép và bảng lương.'),
  ('CV003', 'DA003', N'Xây dựng API tài khoản và xác thực', N'Phát triển API đăng nhập, quản lý phiên và xác thực người dùng.'),
  ('CV004', 'DA004', N'Phân quyền theo vai trò', N'Áp dụng quyền Admin, PM, Kế toán và Nhân viên cho các chức năng.'),
  ('CV005', 'DA005', N'Quản lý danh sách dự án', N'Xây dựng chức năng xem, tìm kiếm, lọc và cập nhật dự án.'),
  ('CV006', 'DA006', N'Quản lý công việc theo dự án', N'Tạo công việc, đặt hạn, mức ưu tiên và trạng thái thực hiện.'),
  ('CV007', 'DA007', N'Phân công nhân viên vào công việc', N'Gán nhân sự phụ trách và lưu lịch sử phân công.'),
  ('CV008', 'DA008', N'Hiển thị Kanban công việc', N'Tổ chức công việc theo các cột Chưa làm, Đang thực hiện và Hoàn thành.'),
  ('CV009', 'DA009', N'Tìm kiếm và phân trang danh sách', N'Thêm bộ lọc theo trạng thái và phân trang tối đa 10 bản ghi.'),
  ('CV010', 'DA010', N'Cập nhật tiến độ công việc', N'Cho phép nhân viên cập nhật trạng thái và PM theo dõi tiến độ.'),
  ('CV011', 'DA011', N'Quản lý hồ sơ nhân sự', N'Hiển thị thông tin nhân viên, vai trò, email và trạng thái tài khoản.'),
  ('CV012', 'DA012', N'Thêm và chỉnh sửa nhân viên', N'Bổ sung biểu mẫu tạo mới và cập nhật thông tin nhân viên.'),
  ('CV013', 'DA013', N'Ghi nhận giờ công theo ngày', N'Cho phép nhân viên ghi nhận giờ làm gắn với công việc được giao.'),
  ('CV014', 'DA014', N'Kiểm tra dữ liệu timesheet', N'Kiểm tra ngày làm, số giờ hợp lệ và ngăn gửi dữ liệu sai.'),
  ('CV015', 'DA015', N'Gửi timesheet chờ phê duyệt', N'Nhân viên gửi bảng giờ công để quản lý dự án xem xét.'),
  ('CV016', 'DA016', N'Phê duyệt hoặc từ chối giờ công', N'PM phê duyệt hoặc từ chối timesheet và ghi nhận lý do.'),
  ('CV017', 'DA017', N'Lưu trữ mềm bản ghi chấm công', N'Ẩn bản ghi bị xóa và giữ lại thông tin phục vụ đối soát.'),
  ('CV018', 'DA018', N'Tổng hợp giờ công theo dự án', N'Tổng hợp giờ đã duyệt theo nhân viên, công việc và dự án.'),
  ('CV019', 'DA019', N'Đăng ký nghỉ phép', N'Nhân viên gửi yêu cầu nghỉ phép theo khoảng ngày và lý do.'),
  ('CV020', 'DA020', N'Đăng ký làm thêm giờ', N'Ghi nhận thời gian OT và công việc liên quan để xét duyệt.'),
  ('CV021', 'DA021', N'Quy trình duyệt nghỉ phép và OT', N'Quản lý xem xét, phê duyệt hoặc từ chối yêu cầu của nhân viên.'),
  ('CV022', 'DA022', N'Cấu hình chính sách lương theo giờ', N'Thiết lập lương giờ cơ bản, hệ số OT và quy tắc làm tròn.'),
  ('CV023', 'DA023', N'Tính lương từ giờ công đã duyệt', N'Tính lương thường, lương OT, khoản trừ và số tiền thực nhận.'),
  ('CV024', 'DA024', N'Quản lý kỳ lương và phiếu lương', N'Tạo kỳ lương, lập phiếu và hoàn tất kỳ sau khi đối soát.'),
  ('CV025', 'DA025', N'Xuất phiếu lương PDF', N'Cho phép nhân viên xem và lưu phiếu lương cá nhân thành PDF.'),
  ('CV026', 'DA026', N'Cấu hình route trên API Gateway', N'Định tuyến các API người dùng và quản trị qua Ocelot Gateway.'),
  ('CV027', 'DA027', N'Cấu hình HTTPS và CORS', N'Cho phép frontend HTTP/HTTPS localhost gọi gateway an toàn.'),
  ('CV028', 'DA028', N'Xuất báo cáo tổng hợp', N'Tổng hợp số liệu dự án, giờ công và bảng lương phục vụ quản lý.'),
  ('CV029', 'DA029', N'Ghi nhật ký thao tác hệ thống', N'Lưu người thực hiện, hành động, dữ liệu và thời điểm thay đổi.'),
  ('CV030', 'DA030', N'Kiểm thử tích hợp và hoàn thiện hệ thống', N'Kiểm tra luồng từ đăng nhập đến dự án, chấm công, nghỉ phép và tính lương.')
) AS x(MaCongViec, MaDuAn, TieuDe, MoTa) ON x.MaCongViec = t.Macongviec;
GO
-- Các dự án đã bắt đầu được đánh dấu đang thực hiện; gia hạn mốc đang quá hạn cho bộ dữ liệu demo.
UPDATE dbo.DuAn SET Trangthai = 'InProgress' WHERE Trangthai = 'Planning' AND Ngaybatdau <= '2026-10-19';
UPDATE dbo.DuAn SET Ngayketthuc = '2026-12-31' WHERE Trangthai = 'InProgress' AND Ngayketthuc < '2026-10-19';
-- Công việc chưa bắt đầu nhưng hạn cũ đã qua được đưa về trạng thái đang xử lý và gia hạn.
UPDATE dbo.CongViec SET Trangthai = 'InProgress', Dateline = '2026-12-31' WHERE Macongviec IN ('CV026','CV027','CV028','CV029','CV030') AND Trangthai = 'Todo';
GO
-- Đưa các yêu cầu chờ duyệt về kỳ tháng 10/2026 để không còn hồ sơ chờ từ nhiều tháng trước.
UPDATE dbo.ChamCong SET Ngaylamviec = '2026-10-13', Ngaytao = '2026-10-13' WHERE Mabangchamcong = 'CC014' AND Trangthai = 'Pending';
UPDATE dbo.ChamCong SET Ngaylamviec = '2026-10-14', Ngaytao = '2026-10-14' WHERE Mabangchamcong = 'CC018' AND Trangthai = 'Pending';
UPDATE dbo.ChamCong SET Ngaylamviec = '2026-10-15', Ngaytao = '2026-10-15' WHERE Mabangchamcong = 'CC025' AND Trangthai = 'Pending';
UPDATE dbo.ChamCong SET Ngaylamviec = '2026-10-16', Ngaytao = '2026-10-16' WHERE Mabangchamcong = 'CC028' AND Trangthai = 'Pending';
UPDATE dbo.ChamCong SET Ngaylamviec = '2026-10-17', Ngaytao = '2026-10-17' WHERE Mabangchamcong = 'CC029' AND Trangthai = 'Pending';
UPDATE dbo.ChamCong SET Ngaylamviec = '2026-10-18', Ngaytao = '2026-10-18' WHERE Mabangchamcong = 'CC030' AND Trangthai = 'Pending';
UPDATE dbo.NghiPhep SET Ngaybatdau = '2026-10-20', Ngayketthuc = '2026-10-20' WHERE Manghiphep = 'NP008' AND Trangthai = 'Pending';
UPDATE dbo.NghiPhep SET Ngaybatdau = '2026-10-21', Ngayketthuc = '2026-10-22' WHERE Manghiphep = 'NP014' AND Trangthai = 'Pending';
UPDATE dbo.NghiPhep SET Ngaybatdau = '2026-10-23', Ngayketthuc = '2026-10-24' WHERE Manghiphep = 'NP018' AND Trangthai = 'Pending';
UPDATE dbo.NghiPhep SET Ngaybatdau = '2026-10-25', Ngayketthuc = '2026-10-25' WHERE Manghiphep = 'NP025' AND Trangthai = 'Pending';
UPDATE dbo.NghiPhep SET Ngaybatdau = '2026-10-27', Ngayketthuc = '2026-10-28' WHERE Manghiphep = 'NP028' AND Trangthai = 'Pending';
UPDATE dbo.NghiPhep SET Ngaybatdau = '2026-10-29', Ngayketthuc = '2026-10-29' WHERE Manghiphep = 'NP029' AND Trangthai = 'Pending';
UPDATE dbo.NghiPhep SET Ngaybatdau = '2026-10-30', Ngayketthuc = '2026-10-30' WHERE Manghiphep = 'NP030' AND Trangthai = 'Pending';
GO
-- Chính sách lương ổn định theo giai đoạn, thay cho mức tăng giả định mỗi nửa tháng.
UPDATE dbo.ChinhSachLuong SET Luonggiocoban = 30000, Hesonhanlamthem = 1.5, Quytaclamtron = N'Làm tròn đến đồng', Hieulucngay = '2026-01-01' WHERE Machinhsach = 'CS001';
UPDATE dbo.ChinhSachLuong SET Luonggiocoban = 32000, Hesonhanlamthem = 1.5, Quytaclamtron = N'Làm tròn đến đồng', Hieulucngay = '2026-07-01' WHERE Machinhsach = 'CS002';
UPDATE dbo.ChinhSachLuong SET Luonggiocoban = 35000, Hesonhanlamthem = 1.5, Quytaclamtron = N'Làm tròn đến đồng', Hieulucngay = '2026-09-01' WHERE Machinhsach = 'CS003';
GO
-- Kỳ 09/2026 đã chốt; kỳ 10/2026 đang được chuẩn bị.
UPDATE dbo.ChayBangLuong SET Trangthai = 'Completed', Ngaytao = '2026-10-01' WHERE Machayluong = 'BL009';
UPDATE dbo.ChayBangLuong SET Trangthai = 'Draft', Ngaytao = '2026-10-01' WHERE Machayluong = 'BL010';
GO
-- Tính tổng lương gồm giờ thường và OT, khấu trừ mẫu 5%.
UPDATE pl
SET pl.Machinhsach = calc.Machinhsach,
    pl.Tongluong = calc.TongLuong,
    pl.Khoantru = ROUND(calc.TongLuong * 0.05, 0),
    pl.Luongnhanduoc = calc.TongLuong - ROUND(calc.TongLuong * 0.05, 0)
FROM dbo.PhieuLuong AS pl
JOIN (
  SELECT p.Maphieuluong,
         CASE WHEN p.Machayluong BETWEEN 'BL001' AND 'BL006' THEN 'CS001'
              WHEN p.Machayluong IN ('BL007','BL008') THEN 'CS002' ELSE 'CS003' END AS Machinhsach,
         CAST(ROUND(p.Tonggiolam * r.Gio + p.Tonggiolamthem * r.Gio * r.HeSoOT, 0) AS DECIMAL(12,2)) AS TongLuong
  FROM dbo.PhieuLuong AS p
  CROSS APPLY (SELECT CASE WHEN p.Machayluong BETWEEN 'BL001' AND 'BL006' THEN CAST(30000 AS DECIMAL(10,2))
                           WHEN p.Machayluong IN ('BL007','BL008') THEN CAST(32000 AS DECIMAL(10,2))
                           ELSE CAST(35000 AS DECIMAL(10,2)) END AS Gio, CAST(1.5 AS DECIMAL(5,2)) AS HeSoOT) AS r
) AS calc ON calc.Maphieuluong = pl.Maphieuluong;
GO
-- Xóa các policy và kỳ tương lai không còn thuộc bộ dữ liệu demo, chỉ khi không có phiếu tham chiếu.
DELETE FROM dbo.ChinhSachLuong
WHERE Machinhsach NOT IN ('CS001','CS002','CS003')
  AND NOT EXISTS (SELECT 1 FROM dbo.PhieuLuong p WHERE p.Machinhsach = ChinhSachLuong.Machinhsach);
DELETE FROM dbo.ChayBangLuong
WHERE Machayluong > 'BL010'
  AND NOT EXISTS (SELECT 1 FROM dbo.PhieuLuong p WHERE p.Machayluong = ChayBangLuong.Machayluong);
GO
