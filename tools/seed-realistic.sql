-- ============================================================
-- SEED DATA THỰC TẾ - Pharmacy & Billing Service
-- Phòng khám Đa khoa Trung tâm
-- ============================================================

USE PharmacyDB;
GO

-- ============================================================
-- 1. USERS - 10 người dùng thực tế
-- ============================================================
-- Passwords: Admin@123, Nurse@123, Doctor@123, Patient@123

-- Cập nhật admin hiện có
UPDATE Users SET
  FullName = N'Nguyễn Văn Quản Trị',
  Email = 'admin@phongkham.vn',
  PhoneNumber = '0901234567'
WHERE Username = 'admin';

-- Thêm bác sĩ 2
IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'bsnguyenthanhtu')
INSERT INTO Users (Id, Username, PasswordHash, FullName, Email, PhoneNumber, Role, IsActive, IsDeleted, CreatedAt)
VALUES ('00000000-0000-0000-0000-000000000005', 'bsnguyenthanhtu',
  '$2a$11$XpX3XKtxeRmdLQ2YZeiIIuxVgMG.7dH6QUnaY.3.aps0V27eJb6JG',
  N'BS. Nguyễn Thành Tú', 'bsntu@phongkham.vn', '0912345678', 2, 1, 0, '2026-01-01');

-- Thêm y tá 2
IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'ytlethibich')
INSERT INTO Users (Id, Username, PasswordHash, FullName, Email, PhoneNumber, Role, IsActive, IsDeleted, CreatedAt)
VALUES ('00000000-0000-0000-0000-000000000006', 'ytlethibich',
  '$2a$11$eIa.dbZb6M3GZoRRFjRMNe3/7bkmf4AW8kfC4D1DCe2lo7/.099Me',
  N'Y tá Lê Thị Bích', 'ytbich@phongkham.vn', '0923456789', 3, 1, 0, '2026-01-01');

-- Thêm 4 bệnh nhân thực tế
IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'bn_tranvanminh')
INSERT INTO Users (Id, Username, PasswordHash, FullName, Email, PhoneNumber, Role, IsActive, IsDeleted, CreatedAt, PatientCode, DateOfBirth, Gender, Address, InsuranceNumber)
VALUES ('00000000-0000-0000-0000-000000000007', 'bn_tranvanminh',
  '$2a$11$bjYmtEjEQSgmpXHkLQcXm.4pquu5X3XhOFOUQSxMVIhNsKMB.xuRi',
  N'Trần Văn Minh', 'tvminh@gmail.com', '0934567890', 4, 1, 0, '2026-01-10',
  'BN20260110ABCD', '1985-03-15', N'Nam', N'123 Nguyễn Trãi, Q.1, TP.HCM', 'HS4012345678');

IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'bn_phamthilan')
INSERT INTO Users (Id, Username, PasswordHash, FullName, Email, PhoneNumber, Role, IsActive, IsDeleted, CreatedAt, PatientCode, DateOfBirth, Gender, Address, InsuranceNumber)
VALUES ('00000000-0000-0000-0000-000000000008', 'bn_phamthilan',
  '$2a$11$bjYmtEjEQSgmpXHkLQcXm.4pquu5X3XhOFOUQSxMVIhNsKMB.xuRi',
  N'Phạm Thị Lan', 'ptlan@gmail.com', '0945678901', 4, 1, 0, '2026-01-12',
  'BN20260112EFGH', '1992-07-22', N'Nữ', N'456 Lê Lợi, Q.3, TP.HCM', 'HS4023456789');

IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'bn_levanhai')
INSERT INTO Users (Id, Username, PasswordHash, FullName, Email, PhoneNumber, Role, IsActive, IsDeleted, CreatedAt, PatientCode, DateOfBirth, Gender, Address)
VALUES ('00000000-0000-0000-0000-000000000009', 'bn_levanhai',
  '$2a$11$bjYmtEjEQSgmpXHkLQcXm.4pquu5X3XhOFOUQSxMVIhNsKMB.xuRi',
  N'Lê Văn Hải', 'lvhai@gmail.com', '0956789012', 4, 1, 0, '2026-02-01',
  'BN20260201IJKL', '1978-11-08', N'Nam', N'789 Võ Văn Tần, Q.3, TP.HCM');

IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'bn_nguyenthihoa')
INSERT INTO Users (Id, Username, PasswordHash, FullName, Email, PhoneNumber, Role, IsActive, IsDeleted, CreatedAt, PatientCode, DateOfBirth, Gender, Address, InsuranceNumber)
VALUES ('00000000-0000-0000-0000-000000000010', 'bn_nguyenthihoa',
  '$2a$11$bjYmtEjEQSgmpXHkLQcXm.4pquu5X3XhOFOUQSxMVIhNsKMB.xuRi',
  N'Nguyễn Thị Hoa', 'nthoa@gmail.com', '0967890123', 4, 1, 0, '2026-02-15',
  'BN20260215MNOP', '2000-04-30', N'Nữ', N'321 Đinh Tiên Hoàng, Bình Thạnh', 'HS4034567890');

-- ============================================================
-- 2. MEDICINES - 20 thuốc thực tế phổ biến
-- ============================================================
DELETE FROM Medicines WHERE MedicineCode LIKE 'REAL%';

INSERT INTO Medicines (Id, MedicineCode, Name, ActiveIngredient, Manufacturer, CountryOfOrigin,
  Unit, UnitDescription, Category, UnitPrice, ImportPrice, StockQuantity, MinimumStock,
  Description, SideEffects, StorageConditions, RequiresPrescription, IsActive, IsDeleted,
  ExpiryDate, CreatedAt)
VALUES
-- Tim mạch
('10000000-0000-0000-0000-000000000001', 'REAL-TM001', N'Amlodipin 5mg', N'Amlodipine besylate 5mg',
  N'Công ty CP Dược Hậu Giang', N'Việt Nam', 'Vien', N'Hộp 3 vỉ x 10 viên',
  N'Tim mạch - Hạ áp', 8500, 5000, 150, 30,
  N'Thuốc điều trị tăng huyết áp và đau thắt ngực. Ức chế kênh canxi, giãn mạch ngoại biên.',
  N'Phù nề mắt cá chân, đau đầu, hồi hộp, đỏ bừng mặt.',
  N'Bảo quản nơi khô ráo, thoáng mát, tránh ánh sáng trực tiếp. Nhiệt độ <30°C.',
  1, 1, 0, '2027-06-30', '2026-01-01'),

('10000000-0000-0000-0000-000000000002', 'REAL-TM002', N'Atorvastatin 20mg', N'Atorvastatin calcium 20mg',
  N'Pfizer', N'Mỹ', 'Vien', N'Hộp 3 vỉ x 10 viên',
  N'Tim mạch - Hạ mỡ máu', 12000, 8000, 200, 30,
  N'Statin hạ cholesterol và triglyceride trong máu. Giảm nguy cơ đột quỵ và nhồi máu cơ tim.',
  N'Đau cơ, tăng men gan, rối loạn tiêu hóa nhẹ.',
  N'Bảo quản <25°C, tránh ẩm.', 1, 1, 0, '2027-12-31', '2026-01-01'),

-- Tiêu hóa
('10000000-0000-0000-0000-000000000003', 'REAL-TH001', N'Omeprazol 20mg', N'Omeprazole 20mg',
  N'AstraZeneca', N'Thụy Điển', 'Vien', N'Hộp 2 vỉ x 14 viên',
  N'Tiêu hóa - Dạ dày', 5500, 3200, 300, 50,
  N'Ức chế bơm proton, điều trị loét dạ dày tá tràng, trào ngược dạ dày thực quản.',
  N'Đau đầu, tiêu chảy, buồn nôn, đầy hơi.',
  N'Bảo quản <25°C, tránh ẩm. Tránh ánh sáng.', 1, 1, 0, '2027-08-31', '2026-01-01'),

('10000000-0000-0000-0000-000000000004', 'REAL-TH002', N'Metoclopramide 10mg', N'Metoclopramide hydrochloride 10mg',
  N'Công ty CP Dược phẩm Trung Ương 1', N'Việt Nam', 'Vien', N'Hộp 10 vỉ x 10 viên',
  N'Tiêu hóa - Chống nôn', 800, 500, 400, 50,
  N'Chống nôn, buồn nôn, điều trị rối loạn nhu động dạ dày.',
  N'Buồn ngủ, mệt mỏi, bồn chồn.',
  N'Bảo quản <30°C, tránh ánh sáng.', 0, 1, 0, '2027-03-31', '2026-01-01'),

-- Hô hấp
('10000000-0000-0000-0000-000000000005', 'REAL-HH001', N'Salbutamol 4mg', N'Salbutamol sulfate 4mg',
  N'GlaxoSmithKline', N'Anh', 'Vien', N'Hộp 10 vỉ x 10 viên',
  N'Hô hấp - Giãn phế quản', 3500, 2000, 180, 30,
  N'Đồng vận beta-2, điều trị hen phế quản và bệnh phổi tắc nghẽn mãn tính.',
  N'Hồi hộp, run rẩy, đau đầu, tăng nhịp tim.',
  N'Bảo quản <25°C.', 1, 1, 0, '2027-09-30', '2026-01-01'),

('10000000-0000-0000-0000-000000000006', 'REAL-HH002', N'Acetylcystein 200mg', N'Acetylcysteine 200mg',
  N'Sandoz', N'Đức', 'Goi', N'Hộp 20 gói',
  N'Hô hấp - Tiêu đờm', 6500, 4000, 250, 30,
  N'Tiêu chất nhầy đường hô hấp, giảm độ quánh của đờm. Dùng trong viêm phế quản.',
  N'Buồn nôn, nôn, chảy nước mũi.',
  N'Bảo quản <25°C, tránh ẩm.', 0, 1, 0, '2027-11-30', '2026-01-01'),

-- Kháng sinh
('10000000-0000-0000-0000-000000000007', 'REAL-KS001', N'Amoxicillin 500mg', N'Amoxicillin trihydrate 500mg',
  N'Công ty CP Dược phẩm Imexpharm', N'Việt Nam', 'Vien', N'Hộp 10 vỉ x 10 viên',
  N'Kháng sinh - Penicillin', 2500, 1500, 500, 100,
  N'Kháng sinh phổ rộng nhóm beta-lactam. Điều trị nhiễm khuẩn đường hô hấp, tiết niệu, tai mũi họng.',
  N'Tiêu chảy, buồn nôn, phát ban da. Hiếm gặp: sốc phản vệ.',
  N'Bảo quản <25°C, tránh ẩm.', 1, 1, 0, '2027-04-30', '2026-01-01'),

('10000000-0000-0000-0000-000000000008', 'REAL-KS002', N'Azithromycin 500mg', N'Azithromycin dihydrate 500mg',
  N'Pfizer', N'Mỹ', 'Vien', N'Hộp 1 vỉ x 3 viên',
  N'Kháng sinh - Macrolide', 18000, 12000, 120, 20,
  N'Kháng sinh macrolide, điều trị viêm phổi, viêm phế quản, nhiễm khuẩn da.',
  N'Rối loạn tiêu hóa, đau bụng, tiêu chảy.',
  N'Bảo quản <25°C.', 1, 1, 0, '2027-07-31', '2026-01-01'),

-- Giảm đau - Hạ sốt
('10000000-0000-0000-0000-000000000009', 'REAL-GD001', N'Paracetamol 500mg', N'Paracetamol 500mg',
  N'Công ty CP Dược Hậu Giang', N'Việt Nam', 'Vien', N'Hộp 10 vỉ x 10 viên',
  N'Giảm đau - Hạ sốt', 1500, 800, 1000, 100,
  N'Hạ sốt, giảm đau nhẹ đến vừa. An toàn khi dùng đúng liều.',
  N'Quá liều gây độc gan nghiêm trọng.',
  N'Bảo quản <30°C, tránh ẩm và ánh sáng.', 0, 1, 0, '2028-01-31', '2026-01-01'),

('10000000-0000-0000-0000-000000000010', 'REAL-GD002', N'Ibuprofen 400mg', N'Ibuprofen 400mg',
  N'Abbott', N'Mỹ', 'Vien', N'Hộp 2 vỉ x 10 viên',
  N'Giảm đau - Kháng viêm', 3200, 2000, 350, 50,
  N'Kháng viêm không steroid (NSAID), giảm đau, hạ sốt, kháng viêm.',
  N'Đau dạ dày, buồn nôn. Không dùng khi loét dạ dày.',
  N'Bảo quản <25°C.', 0, 1, 0, '2027-10-31', '2026-01-01'),

-- Tiểu đường
('10000000-0000-0000-0000-000000000011', 'REAL-TD001', N'Metformin 500mg', N'Metformin hydrochloride 500mg',
  N'Merck', N'Đức', 'Vien', N'Hộp 5 vỉ x 20 viên',
  N'Nội tiết - Tiểu đường', 4500, 2800, 200, 30,
  N'Hạ đường huyết nhóm biguanide. Điều trị đái tháo đường type 2.',
  N'Rối loạn tiêu hóa, buồn nôn, tiêu chảy (thường giảm sau vài tuần).',
  N'Bảo quản <30°C, tránh ẩm.', 1, 1, 0, '2027-05-31', '2026-01-01'),

-- Vitamin - Bổ sung
('10000000-0000-0000-0000-000000000012', 'REAL-VT001', N'Vitamin C 1000mg', N'Ascorbic acid 1000mg',
  N'Bayer', N'Đức', 'Vien', N'Hộp 2 vỉ x 10 viên',
  N'Vitamin - Khoáng chất', 4000, 2500, 600, 50,
  N'Bổ sung vitamin C, tăng cường miễn dịch, chống oxy hóa.',
  N'Rối loạn tiêu hóa khi dùng liều cao.',
  N'Bảo quản <25°C, tránh ánh sáng.', 0, 1, 0, '2028-03-31', '2026-01-01'),

('10000000-0000-0000-0000-000000000013', 'REAL-VT002', N'Vitamin D3 1000IU', N'Cholecalciferol 1000IU',
  N'Sanofi', N'Pháp', 'Vien', N'Hộp 6 vỉ x 10 viên',
  N'Vitamin - Khoáng chất', 8000, 5000, 180, 20,
  N'Bổ sung vitamin D3, hỗ trợ hấp thu canxi, phòng ngừa loãng xương.',
  N'Buồn nôn, tăng canxi máu khi quá liều.',
  N'Bảo quản <25°C, tránh ánh sáng.', 0, 1, 0, '2028-06-30', '2026-01-01'),

-- Thần kinh
('10000000-0000-0000-0000-000000000014', 'REAL-TK001', N'Diazepam 5mg', N'Diazepam 5mg',
  N'Roche', N'Thụy Sĩ', 'Vien', N'Hộp 2 vỉ x 10 viên',
  N'Thần kinh - An thần', 9500, 6000, 8, 10,  -- SẮP HẾT KHO
  N'An thần gây ngủ nhóm benzodiazepine. Điều trị lo âu, mất ngủ, co giật.',
  N'Buồn ngủ, lú lẫn, phụ thuộc thuốc khi dùng lâu dài.',
  N'Bảo quản <30°C. THUỐC KIỂM SOÁT ĐẶC BIỆT.', 1, 1, 0, '2027-02-28', '2026-01-01'),

-- Mắt - Tai - Mũi
('10000000-0000-0000-0000-000000000015', 'REAL-MT001', N'Tobramycin 0.3% nhỏ mắt', N'Tobramycin 0.3%',
  N'Alcon', N'Mỹ', 'Chai', N'Chai 5ml',
  N'Nhãn khoa', 45000, 30000, 60, 10,
  N'Kháng sinh nhỏ mắt điều trị viêm kết mạc, viêm giác mạc do vi khuẩn.',
  N'Kích ứng mắt tạm thời, cảm giác nóng rát.',
  N'Bảo quản 2-8°C trong tủ lạnh.', 1, 1, 0, '2027-01-31', '2026-01-01'),

-- Dị ứng
('10000000-0000-0000-0000-000000000016', 'REAL-DU001', N'Cetirizin 10mg', N'Cetirizine hydrochloride 10mg',
  N'UCB', N'Bỉ', 'Vien', N'Hộp 2 vỉ x 10 viên',
  N'Dị ứng - Kháng histamin', 5000, 3000, 280, 30,
  N'Kháng histamin thế hệ 2, điều trị dị ứng, viêm mũi dị ứng, mề đay.',
  N'Buồn ngủ nhẹ, khô miệng.',
  N'Bảo quản <25°C.', 0, 1, 0, '2027-08-31', '2026-01-01'),

-- Truyền dịch
('10000000-0000-0000-0000-000000000017', 'REAL-TD002', N'NaCl 0.9% 500ml', N'Sodium chloride 0.9%',
  N'Braun', N'Đức', 'Chai', N'Túi truyền 500ml',
  N'Dịch truyền', 25000, 15000, 100, 20,
  N'Dung dịch muối sinh lý đẳng trương, bù dịch và điện giải.',
  N'Quá tải dịch khi truyền quá nhiều.',
  N'Bảo quản <25°C, tránh đông lạnh.', 1, 1, 0, '2028-01-31', '2026-01-01'),

-- Bổ gan
('10000000-0000-0000-0000-000000000018', 'REAL-BG001', N'Silymarin 70mg', N'Silymarin 70mg',
  N'Madaus', N'Đức', 'Vien', N'Hộp 5 vỉ x 10 viên',
  N'Tiêu hóa - Bảo vệ gan', 12000, 8000, 5, 10,  -- SẮP HẾT KHO
  N'Bảo vệ và phục hồi tế bào gan, điều trị viêm gan, xơ gan.',
  N'Rối loạn tiêu hóa nhẹ.',
  N'Bảo quản <25°C, tránh ẩm.', 0, 1, 0, '2027-07-31', '2026-01-01'),

-- Xương khớp
('10000000-0000-0000-0000-000000000019', 'REAL-XK001', N'Glucosamine 500mg', N'Glucosamine sulfate 500mg',
  N'Roussel', N'Pháp', 'Vien', N'Hộp 6 vỉ x 10 viên',
  N'Cơ xương khớp', 15000, 10000, 90, 15,
  N'Điều trị thoái hóa khớp, giảm đau và cải thiện chức năng khớp.',
  N'Đau bụng nhẹ, buồn nôn, táo bón hoặc tiêu chảy.',
  N'Bảo quản <25°C.', 0, 1, 0, '2028-02-28', '2026-01-01'),

-- Nhỏ mũi
('10000000-0000-0000-0000-000000000020', 'REAL-NM001', N'Xylometazoline 0.05% nhỏ mũi', N'Xylometazoline 0.05%',
  N'Novartis', N'Thụy Sĩ', 'Chai', N'Chai 10ml',
  N'Tai mũi họng', 28000, 18000, 7, 10,  -- SẮP HẾT KHO
  N'Co mạch nhỏ mũi, thông mũi trong viêm mũi cấp, viêm xoang.',
  N'Kích ứng niêm mạc mũi, nhờn thuốc khi dùng quá 7 ngày.',
  N'Bảo quản <25°C.', 0, 1, 0, '2027-05-31', '2026-01-01');

-- ============================================================
-- 3. STOCK TRANSACTIONS - Lịch sử nhập kho thực tế
-- ============================================================
INSERT INTO StockTransactions (Id, MedicineId, TransactionType, Quantity, StockBefore, StockAfter,
  UnitPrice, ReferenceId, Note, CreatedBy, IsDeleted, CreatedAt)
VALUES
('20000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001',
  'IN', 200, 0, 200, 5000, NULL, N'Nhập kho lần đầu - Lô hàng tháng 1/2026', '00000000-0000-0000-0000-000000000001', 0, '2026-01-05'),
('20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000001',
  'OUT', 50, 200, 150, 8500, NULL, N'Xuất theo đơn bệnh nhân tháng 1', '00000000-0000-0000-0000-000000000006', 0, '2026-01-20'),
('20000000-0000-0000-0000-000000000003', '10000000-0000-0000-0000-000000000007',
  'IN', 500, 0, 500, 1500, NULL, N'Nhập kho lần đầu - Amoxicillin', '00000000-0000-0000-0000-000000000001', 0, '2026-01-05'),
('20000000-0000-0000-0000-000000000004', '10000000-0000-0000-0000-000000000009',
  'IN', 1000, 0, 1000, 800, NULL, N'Nhập kho lần đầu - Paracetamol', '00000000-0000-0000-0000-000000000001', 0, '2026-01-05'),
('20000000-0000-0000-0000-000000000005', '10000000-0000-0000-0000-000000000003',
  'IN', 300, 0, 300, 3200, NULL, N'Nhập kho lần đầu - Omeprazol', '00000000-0000-0000-0000-000000000001', 0, '2026-01-05'),
('20000000-0000-0000-0000-000000000006', '10000000-0000-0000-0000-000000000014',
  'IN', 50, 0, 50, 6000, NULL, N'Nhập kho Diazepam - Có kiểm soát', '00000000-0000-0000-0000-000000000001', 0, '2026-01-10'),
('20000000-0000-0000-0000-000000000007', '10000000-0000-0000-0000-000000000014',
  'OUT', 42, 50, 8, 9500, NULL, N'Xuất theo đơn tháng 1 và 2', '00000000-0000-0000-0000-000000000001', 0, '2026-02-01');

-- ============================================================
-- 4. DISPENSATIONS - 5 phiếu xuất thuốc thực tế đã hoàn thành
-- ============================================================
INSERT INTO Dispensations (Id, DispensationCode, PrescriptionId, PatientId, DoctorId,
  AppointmentId, PatientName, DoctorName, Diagnosis, Notes,
  Status, DispensedAt, DispensedBy, IsDeleted, CreatedAt)
VALUES
-- Phiếu 1: Bệnh nhân Trần Văn Minh - Tăng huyết áp - ĐÃ XUẤT
('30000000-0000-0000-0000-000000000001', 'DISP202601100001',
  'A0000000-0000-0000-0000-000000000001',
  '00000000-0000-0000-0000-000000000007',
  '00000000-0000-0000-0000-000000000003',
  'B0000000-0000-0000-0000-000000000001',
  N'Trần Văn Minh', N'BS. Trần Văn Bác Sĩ',
  N'Tăng huyết áp độ I, rối loạn lipid máu',
  N'Bệnh nhân cần theo dõi huyết áp tại nhà hàng ngày',
  'Dispensed', '2026-01-15 09:30:00', '00000000-0000-0000-0000-000000000002',
  0, '2026-01-15 09:00:00'),

-- Phiếu 2: Bệnh nhân Phạm Thị Lan - Viêm phế quản - ĐÃ XUẤT
('30000000-0000-0000-0000-000000000002', 'DISP202601200001',
  'A0000000-0000-0000-0000-000000000002',
  '00000000-0000-0000-0000-000000000008',
  '00000000-0000-0000-0000-000000000005',
  'B0000000-0000-0000-0000-000000000002',
  N'Phạm Thị Lan', N'BS. Nguyễn Thành Tú',
  N'Viêm phế quản cấp, ho có đờm',
  N'Uống thuốc đủ liều, không tự ý ngừng kháng sinh',
  'Dispensed', '2026-01-22 14:20:00', '00000000-0000-0000-0000-000000000006',
  0, '2026-01-22 14:00:00'),

-- Phiếu 3: Bệnh nhân Lê Văn Hải - Đau dạ dày - ĐÃ XUẤT
('30000000-0000-0000-0000-000000000003', 'DISP202602050001',
  'A0000000-0000-0000-0000-000000000003',
  '00000000-0000-0000-0000-000000000009',
  '00000000-0000-0000-0000-000000000003',
  NULL,
  N'Lê Văn Hải', N'BS. Trần Văn Bác Sĩ',
  N'Viêm loét dạ dày tá tràng, nhiễm H.pylori',
  N'Kiêng rượu bia, thức ăn cay nóng, tái khám sau 4 tuần',
  'Dispensed', '2026-02-05 10:45:00', '00000000-0000-0000-0000-000000000002',
  0, '2026-02-05 10:30:00'),

-- Phiếu 4: Bệnh nhân Nguyễn Thị Hoa - Dị ứng - ĐÃ XUẤT
('30000000-0000-0000-0000-000000000004', 'DISP202602180001',
  'A0000000-0000-0000-0000-000000000004',
  '00000000-0000-0000-0000-000000000010',
  '00000000-0000-0000-0000-000000000005',
  'B0000000-0000-0000-0000-000000000004',
  N'Nguyễn Thị Hoa', N'BS. Nguyễn Thành Tú',
  N'Viêm mũi dị ứng mãn tính, nổi mề đay',
  N'Tránh tiếp xúc dị nguyên, đeo khẩu trang khi ra ngoài',
  'Dispensed', '2026-02-18 11:10:00', '00000000-0000-0000-0000-000000000006',
  0, '2026-02-18 11:00:00'),

-- Phiếu 5: Bệnh nhân patient1 - Cảm cúm - ĐANG CHỜ XỬ LÝ
('30000000-0000-0000-0000-000000000005', 'DISP202606140001',
  'A0000000-0000-0000-0000-000000000005',
  '00000000-0000-0000-0000-000000000004',
  '00000000-0000-0000-0000-000000000003',
  NULL,
  N'Lê Thị Bệnh Nhân', N'BS. Trần Văn Bác Sĩ',
  N'Cảm cúm, sốt nhẹ, đau họng',
  N'Uống nhiều nước, nghỉ ngơi',
  'Pending', NULL, NULL,
  0, GETUTCDATE());

-- ============================================================
-- 5. DISPENSATION ITEMS - Chi tiết thuốc trong phiếu xuất
-- ============================================================
INSERT INTO DispensationItems (Id, DispensationId, MedicineId, MedicineName, ActiveIngredient, Quantity, UnitPrice, Dosage, Usage, DurationDays, IsDeleted, CreatedAt)
VALUES
-- Phiếu 1: Trần Văn Minh - Tăng huyết áp
('40000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000001',
  '10000000-0000-0000-0000-000000000001', N'Amlodipin 5mg', N'Amlodipine besylate 5mg',
  30, 8500, N'1 viên/ngày', N'Uống sau bữa sáng', 30, 0, '2026-01-15 09:00:00'),
('40000000-0000-0000-0000-000000000002', '30000000-0000-0000-0000-000000000001',
  '10000000-0000-0000-0000-000000000002', N'Atorvastatin 20mg', N'Atorvastatin calcium 20mg',
  30, 12000, N'1 viên/ngày', N'Uống vào buổi tối sau ăn', 30, 0, '2026-01-15 09:00:00'),

-- Phiếu 2: Phạm Thị Lan - Viêm phế quản
('40000000-0000-0000-0000-000000000003', '30000000-0000-0000-0000-000000000002',
  '10000000-0000-0000-0000-000000000007', N'Amoxicillin 500mg', N'Amoxicillin trihydrate 500mg',
  21, 2500, N'1 viên x 3 lần/ngày', N'Uống sau bữa ăn, đủ 7 ngày', 7, 0, '2026-01-22 14:00:00'),
('40000000-0000-0000-0000-000000000004', '30000000-0000-0000-0000-000000000002',
  '10000000-0000-0000-0000-000000000006', N'Acetylcystein 200mg', N'Acetylcysteine 200mg',
  20, 6500, N'1 gói x 2 lần/ngày', N'Pha với nước ấm, uống sau ăn', 10, 0, '2026-01-22 14:00:00'),
('40000000-0000-0000-0000-000000000005', '30000000-0000-0000-0000-000000000002',
  '10000000-0000-0000-0000-000000000009', N'Paracetamol 500mg', N'Paracetamol 500mg',
  20, 1500, N'1-2 viên khi sốt', N'Uống khi sốt >38.5°C, cách 6 giờ/lần', 10, 0, '2026-01-22 14:00:00'),

-- Phiếu 3: Lê Văn Hải - Đau dạ dày
('40000000-0000-0000-0000-000000000006', '30000000-0000-0000-0000-000000000003',
  '10000000-0000-0000-0000-000000000003', N'Omeprazol 20mg', N'Omeprazole 20mg',
  28, 5500, N'1 viên x 2 lần/ngày', N'Uống trước bữa ăn 30 phút', 14, 0, '2026-02-05 10:30:00'),
('40000000-0000-0000-0000-000000000007', '30000000-0000-0000-0000-000000000003',
  '10000000-0000-0000-0000-000000000008', N'Azithromycin 500mg', N'Azithromycin dihydrate 500mg',
  3, 18000, N'1 viên/ngày', N'Uống 1 lần/ngày, đủ 3 ngày', 3, 0, '2026-02-05 10:30:00'),
('40000000-0000-0000-0000-000000000008', '30000000-0000-0000-0000-000000000003',
  '10000000-0000-0000-0000-000000000004', N'Metoclopramide 10mg', N'Metoclopramide hydrochloride 10mg',
  30, 800, N'1 viên x 3 lần/ngày', N'Uống trước ăn 15-30 phút', 10, 0, '2026-02-05 10:30:00'),

-- Phiếu 4: Nguyễn Thị Hoa - Dị ứng
('40000000-0000-0000-0000-000000000009', '30000000-0000-0000-0000-000000000004',
  '10000000-0000-0000-0000-000000000016', N'Cetirizin 10mg', N'Cetirizine hydrochloride 10mg',
  20, 5000, N'1 viên/ngày', N'Uống vào buổi tối trước khi ngủ', 20, 0, '2026-02-18 11:00:00'),
('40000000-0000-0000-0000-000000000010', '30000000-0000-0000-0000-000000000004',
  '10000000-0000-0000-0000-000000000020', N'Xylometazoline 0.05% nhỏ mũi', N'Xylometazoline 0.05%',
  1, 28000, N'2-3 giọt/lần x 3 lần/ngày', N'Nhỏ mũi, không dùng quá 7 ngày', 7, 0, '2026-02-18 11:00:00'),

-- Phiếu 5: patient1 - Đang chờ
('40000000-0000-0000-0000-000000000011', '30000000-0000-0000-0000-000000000005',
  '10000000-0000-0000-0000-000000000009', N'Paracetamol 500mg', N'Paracetamol 500mg',
  20, 1500, 30000, N'1-2 viên khi sốt', N'Uống khi sốt, cách 6 giờ', 5, 0, GETUTCDATE()),
('40000000-0000-0000-0000-000000000012', '30000000-0000-0000-0000-000000000005',
  '10000000-0000-0000-0000-000000000016', N'Cetirizin 10mg', N'Cetirizine hydrochloride 10mg',
  5, 5000, 25000, N'1 viên/ngày', N'Uống buổi tối', 5, 0, GETUTCDATE());

-- Update stock sau khi xuất (các phiếu đã Dispensed)
UPDATE Medicines SET StockQuantity = StockQuantity - 30 WHERE Id = '10000000-0000-0000-0000-000000000001'; -- Amlodipin
UPDATE Medicines SET StockQuantity = StockQuantity - 30 WHERE Id = '10000000-0000-0000-0000-000000000002'; -- Atorvastatin
UPDATE Medicines SET StockQuantity = StockQuantity - 21 WHERE Id = '10000000-0000-0000-0000-000000000007'; -- Amoxicillin
UPDATE Medicines SET StockQuantity = StockQuantity - 20 WHERE Id = '10000000-0000-0000-0000-000000000006'; -- Acetylcystein
UPDATE Medicines SET StockQuantity = StockQuantity - 20 WHERE Id = '10000000-0000-0000-0000-000000000009'; -- Paracetamol
UPDATE Medicines SET StockQuantity = StockQuantity - 28 WHERE Id = '10000000-0000-0000-0000-000000000003'; -- Omeprazol
UPDATE Medicines SET StockQuantity = StockQuantity - 3  WHERE Id = '10000000-0000-0000-0000-000000000008'; -- Azithromycin
UPDATE Medicines SET StockQuantity = StockQuantity - 30 WHERE Id = '10000000-0000-0000-0000-000000000004'; -- Metoclopramide
UPDATE Medicines SET StockQuantity = StockQuantity - 20 WHERE Id = '10000000-0000-0000-0000-000000000016'; -- Cetirizin
UPDATE Medicines SET StockQuantity = StockQuantity - 1  WHERE Id = '10000000-0000-0000-0000-000000000020'; -- Xylometazoline

-- ============================================================
-- 6. INVOICES - Hóa đơn viện phí thực tế
-- ============================================================
INSERT INTO Invoices (Id, InvoiceCode, PatientId, DispensationId, AppointmentId,
  PatientName, PatientCode, InsuranceNumber, DoctorName,
  ExaminationFee, MedicineFee, OtherFees, DiscountAmount, InsuranceCoverage,
  Status, PaymentMethod, PaidAt, CollectedBy, Notes, IsDeleted, CreatedAt)
VALUES
-- HĐ 1: Trần Văn Minh - ĐÃ THANH TOÁN (tiền mặt)
('50000000-0000-0000-0000-000000000001', 'INV20260115ABCD01',
  '00000000-0000-0000-0000-000000000007',
  '30000000-0000-0000-0000-000000000001',
  'B0000000-0000-0000-0000-000000000001',
  N'Trần Văn Minh', 'BN20260110ABCD', 'HS4012345678', N'BS. Trần Văn Bác Sĩ',
  200000, 615000, 50000, 0, 246000,  -- BHYT chi trả 40%
  'Paid', 'Cash', '2026-01-15 10:00:00',
  '00000000-0000-0000-0000-000000000002',
  N'BHYT chi trả 40% phí khám và thuốc',
  0, '2026-01-15 09:45:00'),

-- HĐ 2: Phạm Thị Lan - ĐÃ THANH TOÁN (chuyển khoản)
('50000000-0000-0000-0000-000000000002', 'INV20260122EFGH01',
  '00000000-0000-0000-0000-000000000008',
  '30000000-0000-0000-0000-000000000002',
  'B0000000-0000-0000-0000-000000000002',
  N'Phạm Thị Lan', 'BN20260112EFGH', 'HS4023456789', N'BS. Nguyễn Thành Tú',
  150000, 212500, 30000, 0, 156500,  -- BHYT chi trả 40%
  'Paid', 'BankTransfer', '2026-01-22 15:30:00',
  '00000000-0000-0000-0000-000000000006',
  N'Chuyển khoản Vietcombank',
  0, '2026-01-22 14:30:00'),

-- HĐ 3: Lê Văn Hải - ĐÃ THANH TOÁN (tiền mặt, không có BHYT)
('50000000-0000-0000-0000-000000000003', 'INV20260205IJKL01',
  '00000000-0000-0000-0000-000000000009',
  '30000000-0000-0000-0000-000000000003',
  NULL,
  N'Lê Văn Hải', 'BN20260201IJKL', NULL, N'BS. Trần Văn Bác Sĩ',
  200000, 232000, 100000, 53200, 0,  -- Giảm 10%
  'Paid', 'Cash', '2026-02-05 11:30:00',
  '00000000-0000-0000-0000-000000000002',
  N'Khách hàng thân thiết - giảm 10%',
  0, '2026-02-05 11:00:00'),

-- HĐ 4: Nguyễn Thị Hoa - ĐÃ THANH TOÁN (thẻ)
('50000000-0000-0000-0000-000000000004', 'INV20260218MNOP01',
  '00000000-0000-0000-0000-000000000010',
  '30000000-0000-0000-0000-000000000004',
  'B0000000-0000-0000-0000-000000000004',
  N'Nguyễn Thị Hoa', 'BN20260215MNOP', 'HS4034567890', N'BS. Nguyễn Thành Tú',
  150000, 128000, 0, 0, 111200,  -- BHYT chi trả 40%
  'Paid', 'Card', '2026-02-18 12:00:00',
  '00000000-0000-0000-0000-000000000006',
  N'Thanh toán thẻ Visa',
  0, '2026-02-18 11:30:00'),

-- HĐ 5: patient1 - ĐANG CHỜ THANH TOÁN
('50000000-0000-0000-0000-000000000005', 'INV20260614XXXX01',
  '00000000-0000-0000-0000-000000000004',
  NULL, NULL,
  N'Lê Thị Bệnh Nhân', NULL, NULL, N'BS. Trần Văn Bác Sĩ',
  150000, 0, 0, 0, 0,
  'Pending', NULL, NULL, NULL,
  N'Hóa đơn khám ban đầu, chờ kết quả xuất thuốc',
  0, GETUTCDATE());

-- ============================================================
-- 7. INVOICE ITEMS
-- ============================================================
INSERT INTO InvoiceItems (Id, InvoiceId, ItemName, ItemType, Quantity, UnitPrice, Note, IsDeleted, CreatedAt)
VALUES
-- HĐ 1
('60000000-0000-0000-0000-000000000001', '50000000-0000-0000-0000-000000000001', N'Phí khám bệnh', 'EXAMINATION', 1, 200000, 200000, N'Khám nội tổng quát', 0, '2026-01-15 09:45:00'),
('60000000-0000-0000-0000-000000000002', '50000000-0000-0000-0000-000000000001', N'Amlodipin 5mg', 'MEDICINE', 30, 8500, 255000, NULL, 0, '2026-01-15 09:45:00'),
('60000000-0000-0000-0000-000000000003', '50000000-0000-0000-0000-000000000001', N'Atorvastatin 20mg', 'MEDICINE', 30, 12000, 360000, NULL, 0, '2026-01-15 09:45:00'),
('60000000-0000-0000-0000-000000000004', '50000000-0000-0000-0000-000000000001', N'Xét nghiệm lipid máu', 'OTHER', 1, 50000, 50000, N'Cholesterol, Triglyceride', 0, '2026-01-15 09:45:00'),
-- HĐ 2
('60000000-0000-0000-0000-000000000005', '50000000-0000-0000-0000-000000000002', N'Phí khám bệnh', 'EXAMINATION', 1, 150000, 150000, N'Khám hô hấp', 0, '2026-01-22 14:30:00'),
('60000000-0000-0000-0000-000000000006', '50000000-0000-0000-0000-000000000002', N'Amoxicillin 500mg', 'MEDICINE', 21, 2500, 52500, NULL, 0, '2026-01-22 14:30:00'),
('60000000-0000-0000-0000-000000000007', '50000000-0000-0000-0000-000000000002', N'Acetylcystein 200mg', 'MEDICINE', 20, 6500, 130000, NULL, 0, '2026-01-22 14:30:00'),
('60000000-0000-0000-0000-000000000008', '50000000-0000-0000-0000-000000000002', N'Paracetamol 500mg', 'MEDICINE', 20, 1500, 30000, NULL, 0, '2026-01-22 14:30:00'),
('60000000-0000-0000-0000-000000000009', '50000000-0000-0000-0000-000000000002', N'Chụp X-quang phổi', 'OTHER', 1, 30000, 30000, NULL, 0, '2026-01-22 14:30:00'),
-- HĐ 3
('60000000-0000-0000-0000-000000000010', '50000000-0000-0000-0000-000000000003', N'Phí khám bệnh', 'EXAMINATION', 1, 200000, 200000, N'Khám tiêu hóa', 0, '2026-02-05 11:00:00'),
('60000000-0000-0000-0000-000000000011', '50000000-0000-0000-0000-000000000003', N'Omeprazol 20mg', 'MEDICINE', 28, 5500, 154000, NULL, 0, '2026-02-05 11:00:00'),
('60000000-0000-0000-0000-000000000012', '50000000-0000-0000-0000-000000000003', N'Azithromycin 500mg', 'MEDICINE', 3, 18000, 54000, NULL, 0, '2026-02-05 11:00:00'),
('60000000-0000-0000-0000-000000000013', '50000000-0000-0000-0000-000000000003', N'Metoclopramide 10mg', 'MEDICINE', 30, 800, 24000, NULL, 0, '2026-02-05 11:00:00'),
('60000000-0000-0000-0000-000000000014', '50000000-0000-0000-0000-000000000003', N'Nội soi dạ dày', 'OTHER', 1, 100000, 100000, N'Phát hiện loét dạ dày', 0, '2026-02-05 11:00:00'),
-- HĐ 4
('60000000-0000-0000-0000-000000000015', '50000000-0000-0000-0000-000000000004', N'Phí khám bệnh', 'EXAMINATION', 1, 150000, 150000, N'Khám dị ứng', 0, '2026-02-18 11:30:00'),
('60000000-0000-0000-0000-000000000016', '50000000-0000-0000-0000-000000000004', N'Cetirizin 10mg', 'MEDICINE', 20, 5000, 100000, NULL, 0, '2026-02-18 11:30:00'),
('60000000-0000-0000-0000-000000000017', '50000000-0000-0000-0000-000000000004', N'Xylometazoline nhỏ mũi', 'MEDICINE', 1, 28000, 28000, NULL, 0, '2026-02-18 11:30:00'),
-- HĐ 5 (chờ)
('60000000-0000-0000-0000-000000000018', '50000000-0000-0000-0000-000000000005', N'Phí khám bệnh', 'EXAMINATION', 1, 150000, 150000, N'Khám tổng quát', 0, GETUTCDATE());

-- ============================================================
-- 8. KẾT QUẢ
-- ============================================================
SELECT '=== USERS ===' AS [Table], COUNT(*) AS [Count] FROM Users
UNION ALL SELECT '=== MEDICINES ===' , COUNT(*) FROM Medicines
UNION ALL SELECT '=== DISPENSATIONS ===' , COUNT(*) FROM Dispensations
UNION ALL SELECT '=== INVOICES ===' , COUNT(*) FROM Invoices
UNION ALL SELECT '=== INVOICE ITEMS ===' , COUNT(*) FROM InvoiceItems
UNION ALL SELECT '=== STOCK TRANSACTIONS ===' , COUNT(*) FROM StockTransactions;

-- Thuốc sắp hết kho
SELECT MedicineCode, Name, StockQuantity, MinimumStock,
  CASE WHEN StockQuantity <= MinimumStock THEN N'⚠ SẮP HẾT' ELSE N'OK' END AS TinhTrang
FROM Medicines WHERE IsDeleted = 0
ORDER BY StockQuantity ASC;
