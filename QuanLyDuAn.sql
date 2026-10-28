CREATE DATABASE QuanLyDuAn
USE QuanLyDuAn

CREATE TABLE NguoiDung (
    Manguoidung VARCHAR(20) PRIMARY KEY,
    Tendangnhap NVARCHAR(100) NOT NULL UNIQUE,
    matkhau VARCHAR(MAX) NOT NULL,
    Hoten NVARCHAR(200),
    Email NVARCHAR(200) UNIQUE,
    Vaitro NVARCHAR(20) CHECK (Vaitro IN ('Admin', 'PM', 'Accountant', 'Employee')),
    Ngaytao DATETIME DEFAULT GETDATE(),
    Trangthai BIT DEFAULT 1
);

CREATE TABLE DuAn (
    Maduan VARCHAR(20) PRIMARY KEY,
    Tenduan NVARCHAR(200) NOT NULL,
    Motada NVARCHAR(MAX),
    Ngaybatdau DATE,
    Ngayketthuc DATE,
    Trangthai NVARCHAR(20) CHECK (Trangthai IN ('Planning', 'InProgress', 'Completed', 'OnHold')),
    Nguoitao VARCHAR(20),
    FOREIGN KEY (Nguoitao) REFERENCES NguoiDung(Manguoidung)
);

CREATE TABLE CongViec (
    Macongviec VARCHAR(20) PRIMARY KEY,
    Maduan VARCHAR(20) NOT NULL,
    Tieude NVARCHAR(200) NOT NULL,
    Mota NVARCHAR(MAX),
    Trangthai NVARCHAR(20) CHECK (Trangthai IN ('Todo', 'InProgress', 'Done')),
    Mucdouutien NVARCHAR(10) CHECK (Mucdouutien IN ('Low', 'Medium', 'High')),
    Ngaybatdau DATE,
    Dateline DATE,
    Nguoitao VARCHAR(20),
    FOREIGN KEY (Maduan) REFERENCES DuAn(Maduan),
    FOREIGN KEY (Nguoitao) REFERENCES NguoiDung(Manguoidung)
);

CREATE TABLE PhanCong (
    Ma_phancong VARCHAR(20) PRIMARY KEY,
    Macongviec VARCHAR(20) NOT NULL,
    Manguoidung VARCHAR(20) NOT NULL,
    Ngayphancong DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (Macongviec) REFERENCES CongViec(Macongviec),
    FOREIGN KEY (Manguoidung) REFERENCES NguoiDung(Manguoidung)
);


CREATE TABLE ChamCong (
    Mabangchamcong VARCHAR(20) PRIMARY KEY,
    Manguoidung VARCHAR(20) NOT NULL,
    Macongviec VARCHAR(20) NOT NULL,
    Ngaylamviec DATE NOT NULL,
    Sogiolam DECIMAL(5,2) CHECK (Sogiolam >= 0),
    Trangthai NVARCHAR(20) CHECK (Trangthai IN ('Pending', 'Approved', 'Rejected')),
    Nguoiduyet VARCHAR(20),
    Ngaytao DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (Manguoidung) REFERENCES NguoiDung(Manguoidung),
    FOREIGN KEY (Macongviec) REFERENCES CongViec(Macongviec),
    FOREIGN KEY (Nguoiduyet) REFERENCES NguoiDung(Manguoidung)
);

CREATE TABLE NghiPhep (
    Manghiphep VARCHAR(20) PRIMARY KEY,
    Manguoidung VARCHAR(20) NOT NULL,
    Loainghiphep NVARCHAR(10) CHECK (Loainghiphep IN ('Leave', 'OT')),
    Ngaybatdau DATE NOT NULL,
    Ngayketthuc DATE NOT NULL,
    Sogiolam DECIMAL(5,2) CHECK (Sogiolam >= 0),
    Trangthai NVARCHAR(20) CHECK (Trangthai IN ('Pending', 'Approved', 'Rejected')),
    Nguoiduyet VARCHAR(20),
    FOREIGN KEY (Manguoidung) REFERENCES NguoiDung(Manguoidung),
    FOREIGN KEY (Nguoiduyet) REFERENCES NguoiDung(Manguoidung)
);

CREATE TABLE ChinhSachLuong (
    Machinhsach VARCHAR(20) PRIMARY KEY,
    Luonggiocoban DECIMAL(10,2) CHECK (Luonggiocoban >= 0),
    Hesonhanlamthem DECIMAL(5,2) CHECK (Hesonhanlamthem >= 1),
    Quytaclamtron NVARCHAR(50),
    Hieulucngay DATE NOT NULL
);

CREATE TABLE ChayBangLuong (
    Machayluong VARCHAR(20) PRIMARY KEY,
    Ngaybatdaukyluong DATE NOT NULL,
    Ngayketthuckyluong DATE NOT NULL,
    Trangthai NVARCHAR(20) CHECK (Trangthai IN ('Draft', 'Completed')),
    Ngaytao DATETIME DEFAULT GETDATE()
);

CREATE TABLE PhieuLuong (
    Maphieuluong VARCHAR(20) PRIMARY KEY,
    Machayluong VARCHAR(20) NOT NULL,
    Manguoidung VARCHAR(20) NOT NULL,
	Machinhsach VARCHAR(20) NOT NULL,
    Tonggiolam DECIMAL(10,2) CHECK (Tonggiolam >= 0),
    Tonggiolamthem DECIMAL(10,2) CHECK (Tonggiolamthem >= 0),
    Tongluong DECIMAL(12,2) CHECK (Tongluong >= 0),
    Khoantru DECIMAL(12,2) CHECK (Khoantru >= 0),
    Luongnhanduoc DECIMAL(12,2) CHECK (Luongnhanduoc >= 0),
    Ngaytao DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (Machayluong) REFERENCES ChayBangLuong(Machayluong),
    FOREIGN KEY (Manguoidung) REFERENCES NguoiDung(Manguoidung),
	FOREIGN KEY (Machinhsach) REFERENCES ChinhSachLuong(Machinhsach)
);


CREATE TABLE Audit_Log (
    Madangky VARCHAR(20) PRIMARY KEY,
    Manguoidung VARCHAR(20) NOT NULL,
    Hanhdong NVARCHAR(100) NOT NULL,
    Tentable NVARCHAR(100) NOT NULL,
    Madulieu VARCHAR(20) NOT NULL,
    Thoigian DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (Manguoidung) REFERENCES NguoiDung(Manguoidung)
);

CREATE TABLE RefreshToken
(
    MaRefreshToken BIGINT IDENTITY(1,1) PRIMARY KEY,
    Manguoidung VARCHAR(20) NOT NULL,
    TokenHash VARCHAR(64) NOT NULL UNIQUE,
    Ngaytao DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    Ngayhethan DATETIME2 NOT NULL,
    Ngaythuhoi DATETIME2 NULL,

    CONSTRAINT FK_RefreshToken_NguoiDung
        FOREIGN KEY (Manguoidung)
        REFERENCES NguoiDung(Manguoidung)
);

CREATE INDEX IX_RefreshToken_NguoiDung
ON RefreshToken(Manguoidung);

INSERT INTO NguoiDung
(Manguoidung, Tendangnhap, matkhau, Hoten, Email, Vaitro, Ngaytao, Trangthai)
VALUES
('ND001', 'admin01', '123456', N'Nguyễn Văn An', 'an01@gmail.com', 'Admin', '2026-01-01', 1),
('ND002', 'pm01', '123456', N'Trần Văn Bình', 'binh02@gmail.com', 'PM', '2026-01-02', 1),
('ND003', 'pm02', '123456', N'Lê Thị Chi', 'chi03@gmail.com', 'PM', '2026-01-03', 1),
('ND004', 'kt01', '123456', N'Phạm Văn Dũng', 'dung04@gmail.com', 'Accountant', '2026-01-04', 1),
('ND005', 'kt02', '123456', N'Hoàng Thị Em', 'em05@gmail.com', 'Accountant', '2026-01-05', 1),
('ND006', 'nv01', '123456', N'Đỗ Văn Giang', 'giang06@gmail.com', 'Employee', '2026-01-06', 1),
('ND007', 'nv02', '123456', N'Nguyễn Thị Hà', 'ha07@gmail.com', 'Employee', '2026-01-07', 1),
('ND008', 'nv03', '123456', N'Phan Văn Hùng', 'hung08@gmail.com', 'Employee', '2026-01-08', 1),
('ND009', 'nv04', '123456', N'Vũ Thị Lan', 'lan09@gmail.com', 'Employee', '2026-01-09', 1),
('ND010', 'nv05', '123456', N'Bùi Văn Minh', 'minh10@gmail.com', 'Employee', '2026-01-10', 1),
('ND011', 'nv06', '123456', N'Đặng Thị Nga', 'nga11@gmail.com', 'Employee', '2026-01-11', 1),
('ND012', 'nv07', '123456', N'Nguyễn Văn Oanh', 'oanh12@gmail.com', 'Employee', '2026-01-12', 1),
('ND013', 'nv08', '123456', N'Trịnh Văn Phúc', 'phuc13@gmail.com', 'Employee', '2026-01-13', 1),
('ND014', 'nv09', '123456', N'Lê Văn Quân', 'quan14@gmail.com', 'Employee', '2026-01-14', 1),
('ND015', 'nv10', '123456', N'Nguyễn Thị Sen', 'sen15@gmail.com', 'Employee', '2026-01-15', 1),
('ND016', 'nv11', '123456', N'Đỗ Thị Thảo', 'thao16@gmail.com', 'Employee', '2026-01-16', 1),
('ND017', 'nv12', '123456', N'Phạm Văn Trung', 'trung17@gmail.com', 'Employee', '2026-01-17', 1),
('ND018', 'nv13', '123456', N'Nguyễn Văn Vinh', 'vinh18@gmail.com', 'Employee', '2026-01-18', 1),
('ND019', 'nv14', '123456', N'Hoàng Văn Yên', 'yen19@gmail.com', 'Employee', '2026-01-19', 1),
('ND020', 'nv15', '123456', N'Đặng Văn Tùng', 'tung20@gmail.com', 'Employee', '2026-01-20', 1),
('ND021', 'nv16', '123456', N'Nguyễn Thị Mai', 'mai21@gmail.com', 'Employee', '2026-01-21', 1),
('ND022', 'nv17', '123456', N'Phạm Thị Hoa', 'hoa22@gmail.com', 'Employee', '2026-01-22', 1),
('ND023', 'nv18', '123456', N'Bùi Thị Hương', 'huong23@gmail.com', 'Employee', '2026-01-23', 1),
('ND024', 'nv19', '123456', N'Trần Văn Khoa', 'khoa24@gmail.com', 'Employee', '2026-01-24', 1),
('ND025', 'nv20', '123456', N'Nguyễn Văn Long', 'long25@gmail.com', 'Employee', '2026-01-25', 1),
('ND026', 'nv21', '123456', N'Lê Thị Ngọc', 'ngoc26@gmail.com', 'Employee', '2026-01-26', 1),
('ND027', 'nv22', '123456', N'Vũ Văn Sơn', 'son27@gmail.com', 'Employee', '2026-01-27', 1),
('ND028', 'nv23', '123456', N'Đỗ Văn Thành', 'thanh28@gmail.com', 'Employee', '2026-01-28', 1),
('ND029', 'nv24', '123456', N'Phan Thị Uyên', 'uyen29@gmail.com', 'Employee', '2026-01-29', 1),
('ND030', 'nv25', '123456', N'Nguyễn Đức Việt', 'viet30@gmail.com', 'Employee', '2026-01-30', 1);
GO


/* =========================================================
   2. DUAN - 30 BẢN GHI
   ========================================================= */

INSERT INTO DuAn
(Maduan, Tenduan, Motada, Ngaybatdau, Ngayketthuc, Trangthai, Nguoitao)
VALUES
('DA001', N'Website bán hàng', N'Xây dựng website bán hàng trực tuyến', '2026-01-01', '2026-03-31', 'Completed', 'ND002'),
('DA002', N'Ứng dụng quản lý nhân sự', N'Quản lý nhân sự và chấm công', '2026-01-05', '2026-04-30', 'Completed', 'ND003'),
('DA003', N'Hệ thống quản lý kho', N'Quản lý nhập xuất tồn kho', '2026-02-01', '2026-05-31', 'Completed', 'ND002'),
('DA004', N'Ứng dụng bán hàng', N'Phần mềm bán hàng desktop', '2026-02-10', '2026-06-30', 'InProgress', 'ND003'),
('DA005', N'Website giáo dục', N'Nền tảng học trực tuyến', '2026-03-01', '2026-07-31', 'InProgress', 'ND002'),
('DA006', N'Quản lý thư viện', N'Hệ thống quản lý thư viện', '2026-03-05', '2026-06-30', 'Completed', 'ND003'),
('DA007', N'Quản lý bệnh viện', N'Hệ thống quản lý bệnh viện', '2026-03-10', '2026-09-30', 'InProgress', 'ND002'),
('DA008', N'Ứng dụng đặt xe', N'Ứng dụng đặt xe trực tuyến', '2026-04-01', '2026-08-31', 'InProgress', 'ND003'),
('DA009', N'Website du lịch', N'Website đặt tour du lịch', '2026-04-05', '2026-08-31', 'InProgress', 'ND002'),
('DA010', N'Hệ thống ngân hàng', N'Ứng dụng quản lý giao dịch', '2026-04-10', '2026-10-31', 'InProgress', 'ND003'),
('DA011', N'Quản lý khách sạn', N'Hệ thống quản lý phòng khách sạn', '2026-05-01', '2026-09-30', 'InProgress', 'ND002'),
('DA012', N'Ứng dụng giao đồ ăn', N'Nền tảng đặt món trực tuyến', '2026-05-05', '2026-10-31', 'InProgress', 'ND003'),
('DA013', N'Quản lý phòng khám', N'Quản lý bệnh nhân và lịch khám', '2026-05-10', '2026-09-30', 'InProgress', 'ND002'),
('DA014', N'Hệ thống tuyển dụng', N'Quản lý tuyển dụng nhân sự', '2026-05-15', '2026-08-31', 'Completed', 'ND003'),
('DA015', N'Quản lý công việc', N'Quản lý công việc nhóm', '2026-06-01', '2026-10-31', 'InProgress', 'ND002'),
('DA016', N'Website tin tức', N'Website tin tức trực tuyến', '2026-06-05', '2026-09-30', 'InProgress', 'ND003'),
('DA017', N'Quản lý trường học', N'Hệ thống quản lý trường học', '2026-06-10', '2026-11-30', 'InProgress', 'ND002'),
('DA018', N'Ứng dụng học ngoại ngữ', N'Ứng dụng học tiếng Anh', '2026-06-15', '2026-10-31', 'InProgress', 'ND003'),
('DA019', N'Quản lý nhà hàng', N'Hệ thống quản lý nhà hàng', '2026-07-01', '2026-11-30', 'InProgress', 'ND002'),
('DA020', N'Ứng dụng thể thao', N'Ứng dụng theo dõi thể thao', '2026-07-05', '2026-12-31', 'Planning', 'ND003'),
('DA021', N'Quản lý tài sản', N'Quản lý tài sản doanh nghiệp', '2026-07-10', '2026-11-30', 'Planning', 'ND002'),
('DA022', N'Hệ thống CRM', N'Quản lý quan hệ khách hàng', '2026-07-15', '2026-12-31', 'Planning', 'ND003'),
('DA023', N'Ứng dụng thời tiết', N'Ứng dụng dự báo thời tiết', '2026-08-01', '2026-10-31', 'Planning', 'ND002'),
('DA024', N'Quản lý bãi xe', N'Hệ thống quản lý bãi đỗ xe', '2026-08-05', '2026-11-30', 'Planning', 'ND003'),
('DA025', N'Ứng dụng công thức', N'Ứng dụng tính toán công thức', '2026-08-10', '2026-10-31', 'Planning', 'ND002'),
('DA026', N'Hệ thống chấm công', N'Quản lý chấm công nhân viên', '2026-08-15', '2026-12-31', 'Planning', 'ND003'),
('DA027', N'Quản lý tiền lương', N'Tính lương nhân viên', '2026-09-01', '2026-12-31', 'Planning', 'ND002'),
('DA028', N'Website tuyển sinh', N'Quản lý tuyển sinh trực tuyến', '2026-09-05', '2026-12-31', 'Planning', 'ND003'),
('DA029', N'Hệ thống AI', N'Nền tảng phổ cập kiến thức AI', '2026-09-10', '2027-02-28', 'Planning', 'ND002'),
('DA030', N'Quản lý dự án', N'Hệ thống quản lý dự án tổng thể', '2026-09-15', '2027-03-31', 'Planning', 'ND003');
GO


/* =========================================================
   3. CONGVIEC - 30 BẢN GHI
   ========================================================= */

INSERT INTO CongViec
(Macongviec, Maduan, Tieude, Mota, Trangthai, Mucdouutien, Ngaybatdau, Dateline, Nguoitao)
VALUES
('CV001','DA001',N'Phân tích yêu cầu',N'Thu thập yêu cầu hệ thống','Done','High','2026-01-01','2026-01-10','ND002'),
('CV002','DA001',N'Thiết kế giao diện',N'Thiết kế UI website','Done','Medium','2026-01-11','2026-01-20','ND002'),
('CV003','DA002',N'Thiết kế database',N'Thiết kế cơ sở dữ liệu','Done','High','2026-01-05','2026-01-15','ND003'),
('CV004','DA002',N'Xây dựng API',N'Phát triển API backend','Done','High','2026-01-16','2026-02-15','ND003'),
('CV005','DA003',N'Quản lý sản phẩm',N'CRUD sản phẩm','Done','Medium','2026-02-01','2026-02-20','ND002'),
('CV006','DA003',N'Quản lý kho',N'Quản lý tồn kho','Done','High','2026-02-21','2026-03-15','ND002'),
('CV007','DA004',N'Đăng nhập',N'Xây dựng chức năng đăng nhập','InProgress','High','2026-02-10','2026-03-10','ND003'),
('CV008','DA004',N'Quản lý khách hàng',N'CRUD khách hàng','InProgress','Medium','2026-03-01','2026-04-01','ND003'),
('CV009','DA005',N'Đăng ký tài khoản',N'Đăng ký người dùng','Done','Medium','2026-03-01','2026-03-20','ND002'),
('CV010','DA005',N'Bài giảng online',N'Xây dựng bài giảng','InProgress','High','2026-03-21','2026-05-01','ND002'),
('CV011','DA006',N'Quản lý sách',N'CRUD sách','Done','Medium','2026-03-05','2026-03-25','ND003'),
('CV012','DA006',N'Mượn trả sách',N'Quản lý mượn trả','Done','High','2026-03-26','2026-04-20','ND003'),
('CV013','DA007',N'Quản lý bệnh nhân',N'Thông tin bệnh nhân','InProgress','High','2026-03-10','2026-04-30','ND002'),
('CV014','DA008',N'Đặt xe',N'Chức năng đặt xe','InProgress','High','2026-04-01','2026-05-15','ND003'),
('CV015','DA009',N'Đặt tour',N'Đặt tour du lịch','Done','Medium','2026-04-05','2026-05-20','ND002'),
('CV016','DA010',N'Quản lý giao dịch',N'Quản lý giao dịch ngân hàng','InProgress','High','2026-04-10','2026-06-01','ND003'),
('CV017','DA011',N'Quản lý phòng',N'CRUD phòng khách sạn','InProgress','Medium','2026-05-01','2026-06-01','ND002'),
('CV018','DA012',N'Đặt món',N'Đặt món ăn trực tuyến','InProgress','High','2026-05-05','2026-06-15','ND003'),
('CV019','DA013',N'Đặt lịch khám',N'Đặt lịch khám bệnh','InProgress','High','2026-05-10','2026-06-20','ND002'),
('CV020','DA014',N'Tuyển dụng',N'Quản lý ứng viên','Done','Medium','2026-05-15','2026-06-30','ND003'),
('CV021','DA015',N'Tạo công việc',N'Tạo công việc dự án','InProgress','High','2026-06-01','2026-07-01','ND002'),
('CV022','DA016',N'Quản lý bài viết',N'Tạo và sửa bài viết','InProgress','Medium','2026-06-05','2026-07-15','ND003'),
('CV023','DA017',N'Quản lý sinh viên',N'CRUD sinh viên','InProgress','High','2026-06-10','2026-07-20','ND002'),
('CV024','DA018',N'Học từ vựng',N'Chức năng học từ vựng','InProgress','Medium','2026-06-15','2026-07-30','ND003'),
('CV025','DA019',N'Quản lý bàn',N'Quản lý bàn ăn','InProgress','Medium','2026-07-01','2026-08-01','ND002'),
('CV026','DA020',N'Theo dõi vận động',N'Theo dõi hoạt động thể thao','Todo','Low','2026-07-05','2026-08-15','ND003'),
('CV027','DA021',N'Quản lý tài sản',N'Nhập tài sản','Todo','Medium','2026-07-10','2026-08-20','ND002'),
('CV028','DA022',N'Quản lý khách hàng',N'CRM khách hàng','Todo','High','2026-07-15','2026-09-01','ND003'),
('CV029','DA029',N'Nội dung AI',N'Xây dựng nội dung phổ cập AI','Todo','High','2026-09-10','2026-10-01','ND002'),
('CV030','DA030',N'Quản lý dự án',N'Xây dựng module quản lý dự án','Todo','High','2026-09-15','2026-10-15','ND003');
GO


/* =========================================================
   4. PHANCONG - 30 BẢN GHI
   ========================================================= */

INSERT INTO PhanCong
(Ma_phancong, Macongviec, Manguoidung, Ngayphancong)
VALUES
('PC001','CV001','ND006','2026-01-02'),
('PC002','CV002','ND007','2026-01-12'),
('PC003','CV003','ND008','2026-01-06'),
('PC004','CV004','ND009','2026-01-17'),
('PC005','CV005','ND010','2026-02-02'),
('PC006','CV006','ND011','2026-02-22'),
('PC007','CV007','ND012','2026-02-11'),
('PC008','CV008','ND013','2026-03-02'),
('PC009','CV009','ND014','2026-03-02'),
('PC010','CV010','ND015','2026-03-22'),
('PC011','CV011','ND016','2026-03-06'),
('PC012','CV012','ND017','2026-03-27'),
('PC013','CV013','ND018','2026-03-11'),
('PC014','CV014','ND019','2026-04-02'),
('PC015','CV015','ND020','2026-04-06'),
('PC016','CV016','ND021','2026-04-11'),
('PC017','CV017','ND022','2026-05-02'),
('PC018','CV018','ND023','2026-05-06'),
('PC019','CV019','ND024','2026-05-11'),
('PC020','CV020','ND025','2026-05-16'),
('PC021','CV021','ND026','2026-06-02'),
('PC022','CV022','ND027','2026-06-06'),
('PC023','CV023','ND028','2026-06-11'),
('PC024','CV024','ND029','2026-06-16'),
('PC025','CV025','ND030','2026-07-02'),
('PC026','CV026','ND006','2026-07-06'),
('PC027','CV027','ND007','2026-07-11'),
('PC028','CV028','ND008','2026-07-16'),
('PC029','CV029','ND009','2026-09-11'),
('PC030','CV030','ND010','2026-09-16');
GO


/* =========================================================
   5. CHAMCONG - 30 BẢN GHI
   ========================================================= */

INSERT INTO ChamCong
(Mabangchamcong, Manguoidung, Macongviec, Ngaylamviec, Sogiolam, Trangthai, Nguoiduyet, Ngaytao)
VALUES
('CC001','ND006','CV001','2026-01-05',8,'Approved','ND002','2026-01-05'),
('CC002','ND007','CV002','2026-01-15',8,'Approved','ND002','2026-01-15'),
('CC003','ND008','CV003','2026-01-20',7.5,'Approved','ND003','2026-01-20'),
('CC004','ND009','CV004','2026-02-01',8,'Approved','ND003','2026-02-01'),
('CC005','ND010','CV005','2026-02-10',8,'Approved','ND002','2026-02-10'),
('CC006','ND011','CV006','2026-02-25',7,'Approved','ND002','2026-02-25'),
('CC007','ND012','CV007','2026-03-01',8,'Approved','ND003','2026-03-01'),
('CC008','ND013','CV008','2026-03-05',8,'Approved','ND003','2026-03-05'),
('CC009','ND014','CV009','2026-03-10',8,'Approved','ND002','2026-03-10'),
('CC010','ND015','CV010','2026-03-25',8,'Approved','ND002','2026-03-25'),
('CC011','ND016','CV011','2026-04-01',8,'Approved','ND003','2026-04-01'),
('CC012','ND017','CV012','2026-04-05',7.5,'Approved','ND003','2026-04-05'),
('CC013','ND018','CV013','2026-04-10',8,'Approved','ND002','2026-04-10'),
('CC014','ND019','CV014','2026-04-15',8,'Pending',NULL,'2026-04-15'),
('CC015','ND020','CV015','2026-05-01',8,'Approved','ND002','2026-05-01'),
('CC016','ND021','CV016','2026-05-05',7,'Approved','ND003','2026-05-05'),
('CC017','ND022','CV017','2026-05-10',8,'Approved','ND002','2026-05-10'),
('CC018','ND023','CV018','2026-05-15',8,'Pending',NULL,'2026-05-15'),
('CC019','ND024','CV019','2026-06-01',8,'Approved','ND002','2026-06-01'),
('CC020','ND025','CV020','2026-06-05',8,'Approved','ND003','2026-06-05'),
('CC021','ND026','CV021','2026-06-10',8,'Approved','ND002','2026-06-10'),
('CC022','ND027','CV022','2026-06-15',7.5,'Approved','ND003','2026-06-15'),
('CC023','ND028','CV023','2026-07-01',8,'Rejected','ND002','2026-07-01'),
('CC024','ND029','CV024','2026-07-05',8,'Approved','ND003','2026-07-05'),
('CC025','ND030','CV025','2026-07-10',8,'Pending',NULL,'2026-07-10'),
('CC026','ND006','CV026','2026-07-15',8,'Approved','ND003','2026-07-15'),
('CC027','ND007','CV027','2026-08-01',8,'Approved','ND002','2026-08-01'),
('CC028','ND008','CV028','2026-08-05',7,'Pending',NULL,'2026-08-05'),
('CC029','ND009','CV029','2026-09-15',8,'Pending',NULL,'2026-09-15'),
('CC030','ND010','CV030','2026-09-20',8,'Pending',NULL,'2026-09-20');
GO


/* =========================================================
   6. NGHIPHEP - 30 BẢN GHI
   ========================================================= */

INSERT INTO NghiPhep
(Manghiphep, Manguoidung, Loainghiphep, Ngaybatdau, Ngayketthuc, Sogiolam, Trangthai, Nguoiduyet)
VALUES
('NP001','ND006','Leave','2026-01-10','2026-01-10',0,'Approved','ND002'),
('NP002','ND007','Leave','2026-01-15','2026-01-16',0,'Approved','ND002'),
('NP003','ND008','OT','2026-01-20','2026-01-20',3,'Approved','ND003'),
('NP004','ND009','Leave','2026-02-05','2026-02-05',0,'Approved','ND003'),
('NP005','ND010','OT','2026-02-10','2026-02-10',2,'Approved','ND002'),
('NP006','ND011','Leave','2026-02-15','2026-02-16',0,'Approved','ND002'),
('NP007','ND012','OT','2026-03-01','2026-03-01',4,'Approved','ND003'),
('NP008','ND013','Leave','2026-03-05','2026-03-05',0,'Pending',NULL),
('NP009','ND014','OT','2026-03-10','2026-03-10',2,'Approved','ND002'),
('NP010','ND015','Leave','2026-03-15','2026-03-16',0,'Approved','ND002'),
('NP011','ND016','OT','2026-04-01','2026-04-01',3,'Approved','ND003'),
('NP012','ND017','Leave','2026-04-05','2026-04-05',0,'Rejected','ND003'),
('NP013','ND018','OT','2026-04-10','2026-04-10',2,'Approved','ND002'),
('NP014','ND019','Leave','2026-04-15','2026-04-16',0,'Pending',NULL),
('NP015','ND020','OT','2026-05-01','2026-05-01',4,'Approved','ND002'),
('NP016','ND021','Leave','2026-05-05','2026-05-05',0,'Approved','ND003'),
('NP017','ND022','OT','2026-05-10','2026-05-10',3,'Approved','ND002'),
('NP018','ND023','Leave','2026-05-15','2026-05-16',0,'Pending',NULL),
('NP019','ND024','OT','2026-06-01','2026-06-01',2,'Approved','ND002'),
('NP020','ND025','Leave','2026-06-05','2026-06-05',0,'Approved','ND003'),
('NP021','ND026','OT','2026-06-10','2026-06-10',3,'Approved','ND002'),
('NP022','ND027','Leave','2026-06-15','2026-06-16',0,'Approved','ND003'),
('NP023','ND028','OT','2026-07-01','2026-07-01',2,'Rejected','ND002'),
('NP024','ND029','Leave','2026-07-05','2026-07-05',0,'Approved','ND003'),
('NP025','ND030','OT','2026-07-10','2026-07-10',4,'Pending',NULL),
('NP026','ND006','Leave','2026-08-01','2026-08-01',0,'Approved','ND002'),
('NP027','ND007','OT','2026-08-05','2026-08-05',3,'Approved','ND003'),
('NP028','ND008','Leave','2026-08-10','2026-08-11',0,'Pending',NULL),
('NP029','ND009','OT','2026-09-15','2026-09-15',2,'Pending',NULL),
('NP030','ND010','Leave','2026-09-20','2026-09-20',0,'Pending',NULL);
GO


/* =========================================================
   7. CHINHSACHLUONG - 30 BẢN GHI
   ========================================================= */

INSERT INTO ChinhSachLuong
(Machinhsach, Luonggiocoban, Hesonhanlamthem, Quytaclamtron, Hieulucngay)
VALUES
('CS001',25000,1.5,N'Làm tròn 1 giờ','2026-01-01'),
('CS002',26000,1.5,N'Làm tròn 1 giờ','2026-01-15'),
('CS003',27000,1.5,N'Làm tròn 1 giờ','2026-02-01'),
('CS004',28000,1.5,N'Làm tròn 1 giờ','2026-02-15'),
('CS005',29000,1.5,N'Làm tròn 1 giờ','2026-03-01'),
('CS006',30000,1.5,N'Làm tròn 1 giờ','2026-03-15'),
('CS007',31000,1.5,N'Làm tròn 1 giờ','2026-04-01'),
('CS008',32000,1.5,N'Làm tròn 1 giờ','2026-04-15'),
('CS009',33000,1.5,N'Làm tròn 1 giờ','2026-05-01'),
('CS010',34000,1.5,N'Làm tròn 1 giờ','2026-05-15'),
('CS011',35000,1.5,N'Làm tròn 1 giờ','2026-06-01'),
('CS012',36000,1.5,N'Làm tròn 1 giờ','2026-06-15'),
('CS013',37000,1.5,N'Làm tròn 1 giờ','2026-07-01'),
('CS014',38000,1.5,N'Làm tròn 1 giờ','2026-07-15'),
('CS015',39000,1.5,N'Làm tròn 1 giờ','2026-08-01'),
('CS016',40000,1.5,N'Làm tròn 1 giờ','2026-08-15'),
('CS017',41000,1.5,N'Làm tròn 1 giờ','2026-09-01'),
('CS018',42000,1.5,N'Làm tròn 1 giờ','2026-09-15'),
('CS019',43000,1.75,N'Làm tròn 1 giờ','2026-10-01'),
('CS020',44000,1.75,N'Làm tròn 1 giờ','2026-10-15'),
('CS021',45000,1.75,N'Làm tròn 1 giờ','2026-11-01'),
('CS022',46000,1.75,N'Làm tròn 1 giờ','2026-11-15'),
('CS023',47000,1.75,N'Làm tròn 1 giờ','2026-12-01'),
('CS024',48000,1.75,N'Làm tròn 1 giờ','2027-01-01'),
('CS025',49000,1.75,N'Làm tròn 1 giờ','2027-02-01'),
('CS026',50000,1.75,N'Làm tròn 1 giờ','2027-03-01'),
('CS027',51000,2.0,N'Làm tròn 1 giờ','2027-04-01'),
('CS028',52000,2.0,N'Làm tròn 1 giờ','2027-05-01'),
('CS029',53000,2.0,N'Làm tròn 1 giờ','2027-06-01'),
('CS030',54000,2.0,N'Làm tròn 1 giờ','2027-07-01');
GO


/* =========================================================
   8. CHAYBANGLUONG - 30 BẢN GHI
   ========================================================= */

INSERT INTO ChayBangLuong
(Machayluong, Ngaybatdaukyluong, Ngayketthuckyluong, Trangthai, Ngaytao)
VALUES
('BL001','2026-01-01','2026-01-31','Completed','2026-02-01'),
('BL002','2026-02-01','2026-02-28','Completed','2026-03-01'),
('BL003','2026-03-01','2026-03-31','Completed','2026-04-01'),
('BL004','2026-04-01','2026-04-30','Completed','2026-05-01'),
('BL005','2026-05-01','2026-05-31','Completed','2026-06-01'),
('BL006','2026-06-01','2026-06-30','Completed','2026-07-01'),
('BL007','2026-07-01','2026-07-31','Completed','2026-08-01'),
('BL008','2026-08-01','2026-08-31','Completed','2026-09-01'),
('BL009','2026-09-01','2026-09-30','Draft','2026-09-25'),
('BL010','2026-10-01','2026-10-31','Draft','2026-10-01'),
('BL011','2026-11-01','2026-11-30','Draft','2026-11-01'),
('BL012','2026-12-01','2026-12-31','Draft','2026-12-01'),
('BL013','2027-01-01','2027-01-31','Draft','2027-02-01'),
('BL014','2027-02-01','2027-02-28','Draft','2027-03-01'),
('BL015','2027-03-01','2027-03-31','Draft','2027-04-01'),
('BL016','2027-04-01','2027-04-30','Draft','2027-05-01'),
('BL017','2027-05-01','2027-05-31','Draft','2027-06-01'),
('BL018','2027-06-01','2027-06-30','Draft','2027-07-01'),
('BL019','2027-07-01','2027-07-31','Draft','2027-08-01'),
('BL020','2027-08-01','2027-08-31','Draft','2027-09-01'),
('BL021','2027-09-01','2027-09-30','Draft','2027-10-01'),
('BL022','2027-10-01','2027-10-31','Draft','2027-11-01'),
('BL023','2027-11-01','2027-11-30','Draft','2027-12-01'),
('BL024','2027-12-01','2027-12-31','Draft','2028-01-01'),
('BL025','2028-01-01','2028-01-31','Draft','2028-02-01'),
('BL026','2028-02-01','2028-02-29','Draft','2028-03-01'),
('BL027','2028-03-01','2028-03-31','Draft','2028-04-01'),
('BL028','2028-04-01','2028-04-30','Draft','2028-05-01'),
('BL029','2028-05-01','2028-05-31','Draft','2028-06-01'),
('BL030','2028-06-01','2028-06-30','Draft','2028-07-01');
GO


/* =========================================================
   9. PHIEULUONG - 30 BẢN GHI
   ========================================================= */

INSERT INTO PhieuLuong
(Maphieuluong, Machayluong, Manguoidung, Machinhsach,
 Tonggiolam, Tonggiolamthem, Tongluong, Khoantru, Luongnhanduoc, Ngaytao)
VALUES
('PL001','BL001','ND006','CS001',176,8,4400000,200000,4200000,'2026-02-01'),
('PL002','BL001','ND007','CS001',168,4,4200000,150000,4050000,'2026-02-01'),
('PL003','BL001','ND008','CS001',180,10,4500000,250000,4250000,'2026-02-01'),
('PL004','BL002','ND009','CS002',176,6,4576000,200000,4376000,'2026-03-01'),
('PL005','BL002','ND010','CS002',168,8,4368000,180000,4188000,'2026-03-01'),
('PL006','BL002','ND011','CS002',172,5,4472000,200000,4272000,'2026-03-01'),
('PL007','BL003','ND012','CS003',176,8,4752000,200000,4552000,'2026-04-01'),
('PL008','BL003','ND013','CS003',168,4,4536000,150000,4386000,'2026-04-01'),
('PL009','BL003','ND014','CS003',180,10,4860000,250000,4610000,'2026-04-01'),
('PL010','BL004','ND015','CS004',176,6,4928000,200000,4728000,'2026-05-01'),
('PL011','BL004','ND016','CS004',168,8,4704000,180000,4524000,'2026-05-01'),
('PL012','BL004','ND017','CS004',172,5,4816000,200000,4616000,'2026-05-01'),
('PL013','BL005','ND018','CS005',176,8,5104000,200000,4904000,'2026-06-01'),
('PL014','BL005','ND019','CS005',168,4,4932000,150000,4782000,'2026-06-01'),
('PL015','BL005','ND020','CS005',180,10,5220000,250000,4970000,'2026-06-01'),
('PL016','BL006','ND021','CS006',176,6,5376000,200000,5176000,'2026-07-01'),
('PL017','BL006','ND022','CS006',168,8,5292000,180000,5112000,'2026-07-01'),
('PL018','BL006','ND023','CS006',172,5,5332000,200000,5132000,'2026-07-01'),
('PL019','BL007','ND024','CS007',176,8,5456000,200000,5256000,'2026-08-01'),
('PL020','BL007','ND025','CS007',168,4,5208000,150000,5058000,'2026-08-01'),
('PL021','BL007','ND026','CS007',180,10,5580000,250000,5330000,'2026-08-01'),
('PL022','BL008','ND027','CS008',176,6,5632000,200000,5432000,'2026-09-01'),
('PL023','BL008','ND028','CS008',168,8,5376000,180000,5196000,'2026-09-01'),
('PL024','BL008','ND029','CS008',172,5,5504000,200000,5304000,'2026-09-01'),
('PL025','BL009','ND030','CS009',176,8,5890000,200000,5690000,'2026-09-25'),
('PL026','BL009','ND006','CS009',168,4,5680000,150000,5530000,'2026-09-25'),
('PL027','BL009','ND007','CS009',180,10,6030000,250000,5780000,'2026-09-25'),
('PL028','BL009','ND008','CS009',172,5,5800000,200000,5600000,'2026-09-25'),
('PL029','BL009','ND009','CS009',176,6,5900000,200000,5700000,'2026-09-25'),
('PL030','BL009','ND010','CS009',168,8,5780000,180000,5600000,'2026-09-25');
GO

INSERT INTO Audit_Log
(Madangky, Manguoidung, Hanhdong, Tentable, Madulieu, Thoigian)
VALUES
('AL001', 'ND001', N'Đăng nhập', N'NguoiDung', 'ND001', '2026-09-01 08:00:00'),
('AL002', 'ND002', N'Đăng nhập', N'NguoiDung', 'ND002', '2026-09-01 08:15:00'),
('AL003', 'ND003', N'Thêm dữ liệu', N'DuAn', 'DA001', '2026-09-02 09:00:00'),
('AL004', 'ND004', N'Cập nhật dữ liệu', N'DuAn', 'DA002', '2026-09-02 10:30:00'),
('AL005', 'ND005', N'Xóa dữ liệu', N'DuAn', 'DA003', '2026-09-03 14:00:00'),
('AL006', 'ND006', N'Tạo công việc', N'CongViec', 'CV001', '2026-09-03 08:30:00'),
('AL007', 'ND007', N'Cập nhật công việc', N'CongViec', 'CV002', '2026-09-04 09:15:00'),
('AL008', 'ND008', N'Xóa công việc', N'CongViec', 'CV003', '2026-09-04 15:00:00'),
('AL009', 'ND009', N'Phân công công việc', N'PhanCong', 'PC001', '2026-09-05 08:45:00'),
('AL010', 'ND010', N'Phân công công việc', N'PhanCong', 'PC002', '2026-09-05 10:00:00'),
('AL011', 'ND011', N'Tạo bảng chấm công', N'ChamCong', 'CC001', '2026-09-06 08:00:00'),
('AL012', 'ND012', N'Cập nhật bảng chấm công', N'ChamCong', 'CC002', '2026-09-06 16:30:00'),
('AL013', 'ND013', N'Duyệt chấm công', N'ChamCong', 'CC003', '2026-09-07 09:00:00'),
('AL014', 'ND014', N'Từ chối chấm công', N'ChamCong', 'CC004', '2026-09-07 09:30:00'),
('AL015', 'ND015', N'Tạo đơn nghỉ phép', N'NghiPhep', 'NP001', '2026-09-08 08:20:00'),
('AL016', 'ND016', N'Cập nhật đơn nghỉ phép', N'NghiPhep', 'NP002', '2026-09-08 11:00:00'),
('AL017', 'ND017', N'Duyệt nghỉ phép', N'NghiPhep', 'NP003', '2026-09-09 09:10:00'),
('AL018', 'ND018', N'Từ chối nghỉ phép', N'NghiPhep', 'NP004', '2026-09-09 14:30:00'),
('AL019', 'ND019', N'Tạo chính sách lương', N'ChinhSachLuong', 'CS001', '2026-09-10 08:00:00'),
('AL020', 'ND020', N'Cập nhật chính sách lương', N'ChinhSachLuong', 'CS002', '2026-09-10 10:15:00'),
('AL021', 'ND021', N'Xóa chính sách lương', N'ChinhSachLuong', 'CS003', '2026-09-11 15:00:00'),
('AL022', 'ND022', N'Tạo kỳ lương', N'ChayBangLuong', 'BL001', '2026-09-12 08:30:00'),
('AL023', 'ND023', N'Cập nhật kỳ lương', N'ChayBangLuong', 'BL002', '2026-09-12 11:00:00'),
('AL024', 'ND024', N'Hoàn thành kỳ lương', N'ChayBangLuong', 'BL003', '2026-09-13 16:00:00'),
('AL025', 'ND025', N'Tạo phiếu lương', N'PhieuLuong', 'PL001', '2026-09-15 08:00:00'),
('AL026', 'ND026', N'Cập nhật phiếu lương', N'PhieuLuong', 'PL002', '2026-09-15 10:30:00'),
('AL027', 'ND027', N'Xóa phiếu lương', N'PhieuLuong', 'PL003', '2026-09-16 14:00:00'),
('AL028', 'ND028', N'Xem phiếu lương', N'PhieuLuong', 'PL004', '2026-09-16 15:30:00'),
('AL029', 'ND029', N'Tìm kiếm dữ liệu', N'NguoiDung', 'ND029', '2026-09-17 09:00:00'),
('AL030', 'ND030', N'Đăng xuất', N'NguoiDung', 'ND030', '2026-09-17 17:00:00');

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
