SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

USE PharmacyDB;
GO

-- ============================================================
-- CLEAN slate
-- ============================================================
DELETE FROM InvoiceItems;
DELETE FROM Invoices;
DELETE FROM DispensationItems;
DELETE FROM Dispensations;
DELETE FROM StockTransactions;
DELETE FROM Medicines;
DELETE FROM Users WHERE Id NOT IN (
  '00000000-0000-0000-0000-000000000001',
  '00000000-0000-0000-0000-000000000002',
  '00000000-0000-0000-0000-000000000003',
  '00000000-0000-0000-0000-000000000004'
);
GO

-- ============================================================
-- 1. FIX USERS hiện có + thêm bệnh nhân thực tế
-- ============================================================
UPDATE Users SET FullName=N'Nguyễn Văn Quản Trị', Email='admin@phongkham.vn', PhoneNumber='0901234567'
WHERE Id='00000000-0000-0000-0000-000000000001';

UPDATE Users SET FullName=N'Y tá Nguyễn Thị Y Tá', Email='nurse@phongkham.vn', PhoneNumber='0902345678'
WHERE Id='00000000-0000-0000-0000-000000000002';

UPDATE Users SET FullName=N'BS. Trần Văn Bác Sĩ', Email='doctor@phongkham.vn', PhoneNumber='0903456789'
WHERE Id='00000000-0000-0000-0000-000000000003';

UPDATE Users SET FullName=N'Lê Thị Bệnh Nhân', Email='patient@phongkham.vn', PhoneNumber='0904567890',
  PatientCode='BN20260101AAAA', DateOfBirth='2000-05-10', Gender=N'Nữ', Address=N'100 Lê Lợi, Q.1'
WHERE Id='00000000-0000-0000-0000-000000000004';

-- Bác sĩ 2
IF NOT EXISTS (SELECT 1 FROM Users WHERE Id='00000000-0000-0000-0000-000000000005')
INSERT INTO Users(Id,Username,PasswordHash,FullName,Email,PhoneNumber,Role,IsActive,IsDeleted,CreatedAt)
VALUES('00000000-0000-0000-0000-000000000005','doctor2',
  '$2a$11$XpX3XKtxeRmdLQ2YZeiIIuxVgMG.7dH6QUnaY.3.aps0V27eJb6JG',
  N'BS. Nguyễn Thành Tú','doctor2@phongkham.vn','0905678901',2,1,0,'2026-01-01');

-- Y tá 2
IF NOT EXISTS (SELECT 1 FROM Users WHERE Id='00000000-0000-0000-0000-000000000006')
INSERT INTO Users(Id,Username,PasswordHash,FullName,Email,PhoneNumber,Role,IsActive,IsDeleted,CreatedAt)
VALUES('00000000-0000-0000-0000-000000000006','nurse2',
  '$2a$11$eIa.dbZb6M3GZoRRFjRMNe3/7bkmf4AW8kfC4D1DCe2lo7/.099Me',
  N'Y tá Lê Thị Bích','nurse2@phongkham.vn','0906789012',3,1,0,'2026-01-01');

-- 4 bệnh nhân thực tế
IF NOT EXISTS (SELECT 1 FROM Users WHERE Id='00000000-0000-0000-0000-000000000007')
INSERT INTO Users(Id,Username,PasswordHash,FullName,Email,PhoneNumber,Role,IsActive,IsDeleted,CreatedAt,PatientCode,DateOfBirth,Gender,Address,InsuranceNumber)
VALUES('00000000-0000-0000-0000-000000000007','bn_tranvanminh',
  '$2a$11$bjYmtEjEQSgmpXHkLQcXm.4pquu5X3XhOFOUQSxMVIhNsKMB.xuRi',
  N'Trần Văn Minh','tvminh@gmail.com','0907890123',4,1,0,'2026-01-10',
  'BN20260110ABCD','1985-03-15',N'Nam',N'123 Nguyễn Trãi, Q.1, TP.HCM','HS4012345678');

IF NOT EXISTS (SELECT 1 FROM Users WHERE Id='00000000-0000-0000-0000-000000000008')
INSERT INTO Users(Id,Username,PasswordHash,FullName,Email,PhoneNumber,Role,IsActive,IsDeleted,CreatedAt,PatientCode,DateOfBirth,Gender,Address,InsuranceNumber)
VALUES('00000000-0000-0000-0000-000000000008','bn_phamthilan',
  '$2a$11$bjYmtEjEQSgmpXHkLQcXm.4pquu5X3XhOFOUQSxMVIhNsKMB.xuRi',
  N'Phạm Thị Lan','ptlan@gmail.com','0908901234',4,1,0,'2026-01-12',
  'BN20260112EFGH','1992-07-22',N'Nữ',N'456 Lê Lợi, Q.3, TP.HCM','HS4023456789');

IF NOT EXISTS (SELECT 1 FROM Users WHERE Id='00000000-0000-0000-0000-000000000009')
INSERT INTO Users(Id,Username,PasswordHash,FullName,Email,PhoneNumber,Role,IsActive,IsDeleted,CreatedAt,PatientCode,DateOfBirth,Gender,Address)
VALUES('00000000-0000-0000-0000-000000000009','bn_levanhai',
  '$2a$11$bjYmtEjEQSgmpXHkLQcXm.4pquu5X3XhOFOUQSxMVIhNsKMB.xuRi',
  N'Lê Văn Hải','lvhai@gmail.com','0909012345',4,1,0,'2026-02-01',
  'BN20260201IJKL','1978-11-08',N'Nam',N'789 Võ Văn Tần, Q.3, TP.HCM');

IF NOT EXISTS (SELECT 1 FROM Users WHERE Id='00000000-0000-0000-0000-000000000010')
INSERT INTO Users(Id,Username,PasswordHash,FullName,Email,PhoneNumber,Role,IsActive,IsDeleted,CreatedAt,PatientCode,DateOfBirth,Gender,Address,InsuranceNumber)
VALUES('00000000-0000-0000-0000-000000000010','bn_nguyenthihoa',
  '$2a$11$bjYmtEjEQSgmpXHkLQcXm.4pquu5X3XhOFOUQSxMVIhNsKMB.xuRi',
  N'Nguyễn Thị Hoa','nthoa@gmail.com','0910123456',4,1,0,'2026-02-15',
  'BN20260215MNOP','2000-04-30',N'Nữ',N'321 Đinh Tiên Hoàng, Bình Thạnh','HS4034567890');
GO

-- ============================================================
-- 2. MEDICINES - 20 thuốc thực tế
-- ============================================================
INSERT INTO Medicines(Id,MedicineCode,Name,ActiveIngredient,Manufacturer,CountryOfOrigin,Unit,UnitDescription,Category,UnitPrice,ImportPrice,StockQuantity,MinimumStock,Description,SideEffects,StorageConditions,RequiresPrescription,IsActive,IsDeleted,ExpiryDate,CreatedAt) VALUES
('10000000-0000-0000-0000-000000000001','REAL-TM001',N'Amlodipin 5mg',N'Amlodipine besylate 5mg',N'Dược Hậu Giang',N'Việt Nam',1,N'Hộp 3 vỉ x 10 viên',N'Tim mạch',8500,5000,150,30,N'Điều trị tăng huyết áp, đau thắt ngực',N'Phù mắt cá chân, đau đầu, hồi hộp',N'<30°C, tránh ánh sáng',1,1,0,'2027-06-30','2026-01-01'),
('10000000-0000-0000-0000-000000000002','REAL-TM002',N'Atorvastatin 20mg',N'Atorvastatin calcium 20mg',N'Pfizer',N'Mỹ',1,N'Hộp 3 vỉ x 10 viên',N'Tim mạch',12000,8000,200,30,N'Hạ cholesterol, triglyceride, phòng tim mạch',N'Đau cơ, tăng men gan',N'<25°C, tránh ẩm',1,1,0,'2027-12-31','2026-01-01'),
('10000000-0000-0000-0000-000000000003','REAL-TH001',N'Omeprazol 20mg',N'Omeprazole 20mg',N'AstraZeneca',N'Thụy Điển',1,N'Hộp 2 vỉ x 14 viên',N'Tiêu hóa',5500,3200,300,50,N'Ức chế bơm proton, điều trị loét dạ dày, GERD',N'Đau đầu, tiêu chảy, buồn nôn',N'<25°C, tránh ẩm và ánh sáng',1,1,0,'2027-08-31','2026-01-01'),
('10000000-0000-0000-0000-000000000004','REAL-TH002',N'Metoclopramide 10mg',N'Metoclopramide HCl 10mg',N'Dược phẩm TW1',N'Việt Nam',1,N'Hộp 10 vỉ x 10 viên',N'Tiêu hóa',800,500,400,50,N'Chống nôn, điều trị rối loạn nhu động dạ dày',N'Buồn ngủ, mệt mỏi, bồn chồn',N'<30°C, tránh ánh sáng',0,1,0,'2027-03-31','2026-01-01'),
('10000000-0000-0000-0000-000000000005','REAL-HH001',N'Salbutamol 4mg',N'Salbutamol sulfate 4mg',N'GlaxoSmithKline',N'Anh',1,N'Hộp 10 vỉ x 10 viên',N'Hô hấp',3500,2000,180,30,N'Đồng vận beta-2, điều trị hen phế quản, COPD',N'Hồi hộp, run rẩy, tăng nhịp tim',N'<25°C',1,1,0,'2027-09-30','2026-01-01'),
('10000000-0000-0000-0000-000000000006','REAL-HH002',N'Acetylcystein 200mg',N'Acetylcysteine 200mg',N'Sandoz',N'Đức',4,N'Hộp 20 gói',N'Hô hấp',6500,4000,250,30,N'Tiêu đờm đường hô hấp, giảm độ quánh của đờm',N'Buồn nôn, nôn, chảy nước mũi',N'<25°C, tránh ẩm',0,1,0,'2027-11-30','2026-01-01'),
('10000000-0000-0000-0000-000000000007','REAL-KS001',N'Amoxicillin 500mg',N'Amoxicillin trihydrate 500mg',N'Imexpharm',N'Việt Nam',1,N'Hộp 10 vỉ x 10 viên',N'Kháng sinh',2500,1500,500,100,N'Kháng sinh beta-lactam phổ rộng, nhiễm khuẩn HH/TN/TMH',N'Tiêu chảy, phát ban. Hiếm: sốc phản vệ',N'<25°C, tránh ẩm',1,1,0,'2027-04-30','2026-01-01'),
('10000000-0000-0000-0000-000000000008','REAL-KS002',N'Azithromycin 500mg',N'Azithromycin dihydrate 500mg',N'Pfizer',N'Mỹ',1,N'Hộp 1 vỉ x 3 viên',N'Kháng sinh',18000,12000,120,20,N'Macrolide, viêm phổi, viêm phế quản, nhiễm khuẩn da',N'Rối loạn tiêu hóa, đau bụng',N'<25°C',1,1,0,'2027-07-31','2026-01-01'),
('10000000-0000-0000-0000-000000000009','REAL-GD001',N'Paracetamol 500mg',N'Paracetamol 500mg',N'Dược Hậu Giang',N'Việt Nam',1,N'Hộp 10 vỉ x 10 viên',N'Giảm đau',1500,800,1000,100,N'Hạ sốt, giảm đau nhẹ-vừa',N'Quá liều gây độc gan nghiêm trọng',N'<30°C, tránh ẩm',0,1,0,'2028-01-31','2026-01-01'),
('10000000-0000-0000-0000-000000000010','REAL-GD002',N'Ibuprofen 400mg',N'Ibuprofen 400mg',N'Abbott',N'Mỹ',1,N'Hộp 2 vỉ x 10 viên',N'Giảm đau',3200,2000,350,50,N'NSAID, kháng viêm, giảm đau, hạ sốt',N'Đau dạ dày. Không dùng khi loét dạ dày',N'<25°C',0,1,0,'2027-10-31','2026-01-01'),
('10000000-0000-0000-0000-000000000011','REAL-TD001',N'Metformin 500mg',N'Metformin HCl 500mg',N'Merck',N'Đức',1,N'Hộp 5 vỉ x 20 viên',N'Nội tiết',4500,2800,200,30,N'Biguanide hạ đường huyết, ĐTĐ type 2',N'Rối loạn tiêu hóa, buồn nôn (giảm sau vài tuần)',N'<30°C, tránh ẩm',1,1,0,'2027-05-31','2026-01-01'),
('10000000-0000-0000-0000-000000000012','REAL-VT001',N'Vitamin C 1000mg',N'Ascorbic acid 1000mg',N'Bayer',N'Đức',1,N'Hộp 2 vỉ x 10 viên',N'Vitamin',4000,2500,600,50,N'Bổ sung vitamin C, tăng miễn dịch, chống oxy hóa',N'Rối loạn tiêu hóa khi liều cao',N'<25°C, tránh ánh sáng',0,1,0,'2028-03-31','2026-01-01'),
('10000000-0000-0000-0000-000000000013','REAL-VT002',N'Vitamin D3 1000IU',N'Cholecalciferol 1000IU',N'Sanofi',N'Pháp',1,N'Hộp 6 vỉ x 10 viên',N'Vitamin',8000,5000,180,20,N'Bổ sung D3, hỗ trợ hấp thu canxi, phòng loãng xương',N'Tăng canxi máu khi quá liều',N'<25°C, tránh ánh sáng',0,1,0,'2028-06-30','2026-01-01'),
('10000000-0000-0000-0000-000000000014','REAL-TK001',N'Diazepam 5mg',N'Diazepam 5mg',N'Roche',N'Thụy Sĩ',1,N'Hộp 2 vỉ x 10 viên',N'Thần kinh',9500,6000,8,10,N'Benzodiazepine. An thần, điều trị lo âu, mất ngủ, co giật',N'Buồn ngủ, lú lẫn, phụ thuộc thuốc lâu dài',N'<30°C. THUỐC KIỂM SOÁT.',1,1,0,'2027-02-28','2026-01-01'),
('10000000-0000-0000-0000-000000000015','REAL-MT001',N'Tobramycin nhỏ mắt 0.3%',N'Tobramycin 0.3%',N'Alcon',N'Mỹ',2,N'Chai 5ml',N'Nhãn khoa',45000,30000,60,10,N'Kháng sinh nhỏ mắt, viêm kết mạc/giác mạc',N'Kích ứng mắt tạm thời',N'2-8°C trong tủ lạnh',1,1,0,'2027-01-31','2026-01-01'),
('10000000-0000-0000-0000-000000000016','REAL-DU001',N'Cetirizin 10mg',N'Cetirizine HCl 10mg',N'UCB',N'Bỉ',1,N'Hộp 2 vỉ x 10 viên',N'Dị ứng',5000,3000,280,30,N'Kháng histamin thế hệ 2, dị ứng, viêm mũi, mề đay',N'Buồn ngủ nhẹ, khô miệng',N'<25°C',0,1,0,'2027-08-31','2026-01-01'),
('10000000-0000-0000-0000-000000000017','REAL-TD002',N'NaCl 0.9% 500ml',N'Sodium chloride 0.9%',N'Braun',N'Đức',2,N'Túi truyền 500ml',N'Dịch truyền',25000,15000,100,20,N'Dung dịch muối đẳng trương, bù dịch và điện giải',N'Quá tải dịch khi truyền nhiều',N'<25°C, không đông lạnh',1,1,0,'2028-01-31','2026-01-01'),
('10000000-0000-0000-0000-000000000018','REAL-BG001',N'Silymarin 70mg',N'Silymarin 70mg',N'Madaus',N'Đức',1,N'Hộp 5 vỉ x 10 viên',N'Tiêu hóa',12000,8000,5,10,N'Bảo vệ và phục hồi tế bào gan, viêm gan, xơ gan',N'Rối loạn tiêu hóa nhẹ',N'<25°C, tránh ẩm',0,1,0,'2027-07-31','2026-01-01'),
('10000000-0000-0000-0000-000000000019','REAL-XK001',N'Glucosamine 500mg',N'Glucosamine sulfate 500mg',N'Roussel',N'Pháp',1,N'Hộp 6 vỉ x 10 viên',N'Cơ xương khớp',15000,10000,90,15,N'Điều trị thoái hóa khớp, giảm đau cải thiện khớp',N'Đau bụng nhẹ, buồn nôn',N'<25°C',0,1,0,'2028-02-28','2026-01-01'),
('10000000-0000-0000-0000-000000000020','REAL-NM001',N'Xylometazoline nhỏ mũi 0.05%',N'Xylometazoline 0.05%',N'Novartis',N'Thụy Sĩ',2,N'Chai 10ml',N'Tai mũi họng',28000,18000,6,10,N'Co mạch thông mũi, viêm mũi cấp, viêm xoang',N'Kích ứng niêm mạc, nhờn thuốc >7 ngày',N'<25°C',0,1,0,'2027-05-31','2026-01-01');
GO

-- ============================================================
-- 3. DISPENSATIONS
-- ============================================================
INSERT INTO Dispensations(Id,DispensationCode,PrescriptionId,PatientId,DoctorId,AppointmentId,PatientName,DoctorName,Diagnosis,Notes,Status,DispensedAt,DispensedBy,IsDeleted,CreatedAt) VALUES
('30000000-0000-0000-0000-000000000001','DISP202601150001','A0000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000007','00000000-0000-0000-0000-000000000003','B0000000-0000-0000-0000-000000000001',N'Trần Văn Minh',N'BS. Trần Văn Bác Sĩ',N'Tăng huyết áp độ I, rối loạn lipid máu',N'Theo dõi huyết áp tại nhà hàng ngày','3','2026-01-15 09:30:00','00000000-0000-0000-0000-000000000002',0,'2026-01-15 09:00:00'),
('30000000-0000-0000-0000-000000000002','DISP202601220001','A0000000-0000-0000-0000-000000000002','00000000-0000-0000-0000-000000000008','00000000-0000-0000-0000-000000000005','B0000000-0000-0000-0000-000000000002',N'Phạm Thị Lan',N'BS. Nguyễn Thành Tú',N'Viêm phế quản cấp, ho có đờm',N'Uống đủ liều kháng sinh, không tự ý ngừng','3','2026-01-22 14:20:00','00000000-0000-0000-0000-000000000006',0,'2026-01-22 14:00:00'),
('30000000-0000-0000-0000-000000000003','DISP202602050001','A0000000-0000-0000-0000-000000000003','00000000-0000-0000-0000-000000000009','00000000-0000-0000-0000-000000000003',NULL,N'Lê Văn Hải',N'BS. Trần Văn Bác Sĩ',N'Viêm loét dạ dày tá tràng, nhiễm H.pylori',N'Kiêng rượu bia, cay nóng. Tái khám sau 4 tuần','3','2026-02-05 10:45:00','00000000-0000-0000-0000-000000000002',0,'2026-02-05 10:30:00'),
('30000000-0000-0000-0000-000000000004','DISP202602180001','A0000000-0000-0000-0000-000000000004','00000000-0000-0000-0000-000000000010','00000000-0000-0000-0000-000000000005','B0000000-0000-0000-0000-000000000004',N'Nguyễn Thị Hoa',N'BS. Nguyễn Thành Tú',N'Viêm mũi dị ứng mãn tính, mề đay',N'Tránh tiếp xúc dị nguyên, đeo khẩu trang','3','2026-02-18 11:10:00','00000000-0000-0000-0000-000000000006',0,'2026-02-18 11:00:00'),
('30000000-0000-0000-0000-000000000005','DISP202606140001','A0000000-0000-0000-0000-000000000005','00000000-0000-0000-0000-000000000004','00000000-0000-0000-0000-000000000003',NULL,N'Lê Thị Bệnh Nhân',N'BS. Trần Văn Bác Sĩ',N'Cảm cúm, sốt nhẹ, đau họng',N'Uống nhiều nước, nghỉ ngơi','1',NULL,NULL,0,GETUTCDATE());
GO

-- ============================================================
-- 4. DISPENSATION ITEMS
-- ============================================================
INSERT INTO DispensationItems(Id,DispensationId,MedicineId,MedicineName,ActiveIngredient,Quantity,UnitPrice,Dosage,Usage,DurationDays,IsDeleted,CreatedAt) VALUES
-- Phiếu 1: Tăng huyết áp
('40000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000001','10000000-0000-0000-0000-000000000001',N'Amlodipin 5mg',N'Amlodipine besylate 5mg',30,8500,N'1 viên/ngày',N'Uống sau bữa sáng',30,0,'2026-01-15'),
('40000000-0000-0000-0000-000000000002','30000000-0000-0000-0000-000000000001','10000000-0000-0000-0000-000000000002',N'Atorvastatin 20mg',N'Atorvastatin calcium 20mg',30,12000,N'1 viên/ngày',N'Uống buổi tối sau ăn',30,0,'2026-01-15'),
-- Phiếu 2: Viêm phế quản
('40000000-0000-0000-0000-000000000003','30000000-0000-0000-0000-000000000002','10000000-0000-0000-0000-000000000007',N'Amoxicillin 500mg',N'Amoxicillin trihydrate 500mg',21,2500,N'1 viên x 3 lần/ngày',N'Uống sau ăn, đủ 7 ngày',7,0,'2026-01-22'),
('40000000-0000-0000-0000-000000000004','30000000-0000-0000-0000-000000000002','10000000-0000-0000-0000-000000000006',N'Acetylcystein 200mg',N'Acetylcysteine 200mg',20,6500,N'1 gói x 2 lần/ngày',N'Pha nước ấm, sau ăn',10,0,'2026-01-22'),
('40000000-0000-0000-0000-000000000005','30000000-0000-0000-0000-000000000002','10000000-0000-0000-0000-000000000009',N'Paracetamol 500mg',N'Paracetamol 500mg',20,1500,N'1-2 viên khi sốt',N'Khi sốt >38.5°C, cách 6h',10,0,'2026-01-22'),
-- Phiếu 3: Loét dạ dày
('40000000-0000-0000-0000-000000000006','30000000-0000-0000-0000-000000000003','10000000-0000-0000-0000-000000000003',N'Omeprazol 20mg',N'Omeprazole 20mg',28,5500,N'1 viên x 2 lần/ngày',N'Uống trước ăn 30 phút',14,0,'2026-02-05'),
('40000000-0000-0000-0000-000000000007','30000000-0000-0000-0000-000000000003','10000000-0000-0000-0000-000000000008',N'Azithromycin 500mg',N'Azithromycin dihydrate 500mg',3,18000,N'1 viên/ngày',N'Đủ 3 ngày',3,0,'2026-02-05'),
('40000000-0000-0000-0000-000000000008','30000000-0000-0000-0000-000000000003','10000000-0000-0000-0000-000000000004',N'Metoclopramide 10mg',N'Metoclopramide HCl 10mg',30,800,N'1 viên x 3 lần/ngày',N'Trước ăn 15-30 phút',10,0,'2026-02-05'),
-- Phiếu 4: Dị ứng
('40000000-0000-0000-0000-000000000009','30000000-0000-0000-0000-000000000004','10000000-0000-0000-0000-000000000016',N'Cetirizin 10mg',N'Cetirizine HCl 10mg',20,5000,N'1 viên/ngày',N'Uống tối trước ngủ',20,0,'2026-02-18'),
('40000000-0000-0000-0000-000000000010','30000000-0000-0000-0000-000000000004','10000000-0000-0000-0000-000000000020',N'Xylometazoline nhỏ mũi',N'Xylometazoline 0.05%',1,28000,N'2-3 giọt x 3 lần/ngày',N'Không quá 7 ngày',7,0,'2026-02-18'),
-- Phiếu 5: Chờ xử lý
('40000000-0000-0000-0000-000000000011','30000000-0000-0000-0000-000000000005','10000000-0000-0000-0000-000000000009',N'Paracetamol 500mg',N'Paracetamol 500mg',20,1500,N'1-2 viên khi sốt',N'Cách 6h/lần',5,0,GETUTCDATE()),
('40000000-0000-0000-0000-000000000012','30000000-0000-0000-0000-000000000005','10000000-0000-0000-0000-000000000016',N'Cetirizin 10mg',N'Cetirizine HCl 10mg',5,5000,N'1 viên/ngày',N'Uống buổi tối',5,0,GETUTCDATE());

-- Cập nhật tồn kho sau xuất thuốc (phiếu 1-4 đã Dispensed)
UPDATE Medicines SET StockQuantity=StockQuantity-30 WHERE Id='10000000-0000-0000-0000-000000000001';
UPDATE Medicines SET StockQuantity=StockQuantity-30 WHERE Id='10000000-0000-0000-0000-000000000002';
UPDATE Medicines SET StockQuantity=StockQuantity-21 WHERE Id='10000000-0000-0000-0000-000000000007';
UPDATE Medicines SET StockQuantity=StockQuantity-20 WHERE Id='10000000-0000-0000-0000-000000000006';
UPDATE Medicines SET StockQuantity=StockQuantity-20 WHERE Id='10000000-0000-0000-0000-000000000009';
UPDATE Medicines SET StockQuantity=StockQuantity-28 WHERE Id='10000000-0000-0000-0000-000000000003';
UPDATE Medicines SET StockQuantity=StockQuantity-3  WHERE Id='10000000-0000-0000-0000-000000000008';
UPDATE Medicines SET StockQuantity=StockQuantity-30 WHERE Id='10000000-0000-0000-0000-000000000004';
UPDATE Medicines SET StockQuantity=StockQuantity-20 WHERE Id='10000000-0000-0000-0000-000000000016';
UPDATE Medicines SET StockQuantity=StockQuantity-1  WHERE Id='10000000-0000-0000-0000-000000000020';
GO

-- ============================================================
-- 5. INVOICES
-- ============================================================
INSERT INTO Invoices(Id,InvoiceCode,PatientId,DispensationId,AppointmentId,PatientName,PatientCode,InsuranceNumber,DoctorName,ExaminationFee,MedicineFee,OtherFees,DiscountAmount,InsuranceCoverage,Status,PaymentMethod,PaidAt,CollectedBy,Notes,IsDeleted,CreatedAt) VALUES
('50000000-0000-0000-0000-000000000001','INV20260115ABCD01','00000000-0000-0000-0000-000000000007','30000000-0000-0000-0000-000000000001','B0000000-0000-0000-0000-000000000001',N'Trần Văn Minh','BN20260110ABCD','HS4012345678',N'BS. Trần Văn Bác Sĩ',200000,615000,50000,0,234000,2,1,'2026-01-15 10:00:00','00000000-0000-0000-0000-000000000002',N'BHYT chi trả 27% (tổng 865k * 27%)',0,'2026-01-15 09:45:00'),
('50000000-0000-0000-0000-000000000002','INV20260122EFGH01','00000000-0000-0000-0000-000000000008','30000000-0000-0000-0000-000000000002','B0000000-0000-0000-0000-000000000002',N'Phạm Thị Lan','BN20260112EFGH','HS4023456789',N'BS. Nguyễn Thành Tú',150000,212500,30000,0,156500,2,2,'2026-01-22 15:30:00','00000000-0000-0000-0000-000000000006',N'BHYT 40%. Chuyển khoản Vietcombank',0,'2026-01-22 14:30:00'),
('50000000-0000-0000-0000-000000000003','INV20260205IJKL01','00000000-0000-0000-0000-000000000009','30000000-0000-0000-0000-000000000003',NULL,N'Lê Văn Hải','BN20260201IJKL',NULL,N'BS. Trần Văn Bác Sĩ',200000,232000,100000,53200,0,2,1,'2026-02-05 11:30:00','00000000-0000-0000-0000-000000000002',N'Khách hàng thân thiết, giảm 10%',0,'2026-02-05 11:00:00'),
('50000000-0000-0000-0000-000000000004','INV20260218MNOP01','00000000-0000-0000-0000-000000000010','30000000-0000-0000-0000-000000000004','B0000000-0000-0000-0000-000000000004',N'Nguyễn Thị Hoa','BN20260215MNOP','HS4034567890',N'BS. Nguyễn Thành Tú',150000,128000,0,0,111200,2,3,'2026-02-18 12:00:00','00000000-0000-0000-0000-000000000006',N'Thanh toán thẻ Visa',0,'2026-02-18 11:30:00'),
('50000000-0000-0000-0000-000000000005','INV20260614XXXX01','00000000-0000-0000-0000-000000000004',NULL,NULL,N'Lê Thị Bệnh Nhân',NULL,NULL,N'BS. Trần Văn Bác Sĩ',150000,0,0,0,0,'1',NULL,NULL,NULL,N'Chờ kết quả xuất thuốc',0,GETUTCDATE());
GO

-- ============================================================
-- 6. INVOICE ITEMS
-- ============================================================
INSERT INTO InvoiceItems(Id,InvoiceId,ItemName,ItemType,Quantity,UnitPrice,Note,IsDeleted,CreatedAt) VALUES
('60000000-0000-0000-0000-000000000001','50000000-0000-0000-0000-000000000001',N'Phí khám nội tổng quát','EXAMINATION',1,200000,NULL,0,'2026-01-15'),
('60000000-0000-0000-0000-000000000002','50000000-0000-0000-0000-000000000001',N'Amlodipin 5mg','MEDICINE',30,8500,NULL,0,'2026-01-15'),
('60000000-0000-0000-0000-000000000003','50000000-0000-0000-0000-000000000001',N'Atorvastatin 20mg','MEDICINE',30,12000,NULL,0,'2026-01-15'),
('60000000-0000-0000-0000-000000000004','50000000-0000-0000-0000-000000000001',N'Xét nghiệm lipid máu','OTHER',1,50000,N'Cholesterol, Triglyceride, HDL, LDL',0,'2026-01-15'),
('60000000-0000-0000-0000-000000000005','50000000-0000-0000-0000-000000000002',N'Phí khám hô hấp','EXAMINATION',1,150000,NULL,0,'2026-01-22'),
('60000000-0000-0000-0000-000000000006','50000000-0000-0000-0000-000000000002',N'Amoxicillin 500mg','MEDICINE',21,2500,NULL,0,'2026-01-22'),
('60000000-0000-0000-0000-000000000007','50000000-0000-0000-0000-000000000002',N'Acetylcystein 200mg','MEDICINE',20,6500,NULL,0,'2026-01-22'),
('60000000-0000-0000-0000-000000000008','50000000-0000-0000-0000-000000000002',N'Paracetamol 500mg','MEDICINE',20,1500,NULL,0,'2026-01-22'),
('60000000-0000-0000-0000-000000000009','50000000-0000-0000-0000-000000000002',N'Chụp X-quang phổi','OTHER',1,30000,NULL,0,'2026-01-22'),
('60000000-0000-0000-0000-000000000010','50000000-0000-0000-0000-000000000003',N'Phí khám tiêu hóa','EXAMINATION',1,200000,NULL,0,'2026-02-05'),
('60000000-0000-0000-0000-000000000011','50000000-0000-0000-0000-000000000003',N'Omeprazol 20mg','MEDICINE',28,5500,NULL,0,'2026-02-05'),
('60000000-0000-0000-0000-000000000012','50000000-0000-0000-0000-000000000003',N'Azithromycin 500mg','MEDICINE',3,18000,NULL,0,'2026-02-05'),
('60000000-0000-0000-0000-000000000013','50000000-0000-0000-0000-000000000003',N'Metoclopramide 10mg','MEDICINE',30,800,NULL,0,'2026-02-05'),
('60000000-0000-0000-0000-000000000014','50000000-0000-0000-0000-000000000003',N'Nội soi dạ dày','OTHER',1,100000,N'Phát hiện loét dạ dày',0,'2026-02-05'),
('60000000-0000-0000-0000-000000000015','50000000-0000-0000-0000-000000000004',N'Phí khám dị ứng','EXAMINATION',1,150000,NULL,0,'2026-02-18'),
('60000000-0000-0000-0000-000000000016','50000000-0000-0000-0000-000000000004',N'Cetirizin 10mg','MEDICINE',20,5000,NULL,0,'2026-02-18'),
('60000000-0000-0000-0000-000000000017','50000000-0000-0000-0000-000000000004',N'Xylometazoline nhỏ mũi','MEDICINE',1,28000,NULL,0,'2026-02-18'),
('60000000-0000-0000-0000-000000000018','50000000-0000-0000-0000-000000000005',N'Phí khám tổng quát','EXAMINATION',1,150000,NULL,0,GETUTCDATE());
GO

-- ============================================================
-- 7. STOCK TRANSACTIONS - lịch sử nhập kho
-- ============================================================
INSERT INTO StockTransactions(Id,MedicineId,TransactionType,Quantity,StockBefore,StockAfter,UnitPrice,Note,CreatedBy,IsDeleted,CreatedAt) VALUES
('20000000-0000-0000-0000-000000000001','10000000-0000-0000-0000-000000000001','IN',200,0,200,5000,N'Nhập kho lần đầu tháng 1/2026','00000000-0000-0000-0000-000000000001',0,'2026-01-03'),
('20000000-0000-0000-0000-000000000002','10000000-0000-0000-0000-000000000007','IN',500,0,500,1500,N'Nhập kho lần đầu tháng 1/2026','00000000-0000-0000-0000-000000000001',0,'2026-01-03'),
('20000000-0000-0000-0000-000000000003','10000000-0000-0000-0000-000000000009','IN',1000,0,1000,800,N'Nhập kho lần đầu tháng 1/2026','00000000-0000-0000-0000-000000000001',0,'2026-01-03'),
('20000000-0000-0000-0000-000000000004','10000000-0000-0000-0000-000000000001','OUT',30,200,170,8500,N'Xuất theo phiếu DISP202601150001','00000000-0000-0000-0000-000000000002',0,'2026-01-15'),
('20000000-0000-0000-0000-000000000005','10000000-0000-0000-0000-000000000014','IN',50,0,50,6000,N'Nhập kho Diazepam - Có kiểm soát đặc biệt','00000000-0000-0000-0000-000000000001',0,'2026-01-10'),
('20000000-0000-0000-0000-000000000006','10000000-0000-0000-0000-000000000014','OUT',42,50,8,9500,N'Xuất theo đơn tháng 1 và 2/2026','00000000-0000-0000-0000-000000000001',0,'2026-02-15'),
('20000000-0000-0000-0000-000000000007','10000000-0000-0000-0000-000000000018','IN',50,0,50,8000,N'Nhập kho Silymarin','00000000-0000-0000-0000-000000000001',0,'2026-01-05'),
('20000000-0000-0000-0000-000000000008','10000000-0000-0000-0000-000000000018','OUT',45,50,5,12000,N'Xuất theo đơn tháng 1-2/2026','00000000-0000-0000-0000-000000000002',0,'2026-02-20');
GO

-- ============================================================
-- KẾT QUẢ CUỐI
-- ============================================================
SELECT '=== USERS ===' T, COUNT(*) N FROM Users
UNION ALL SELECT '=== MEDICINES ===', COUNT(*) FROM Medicines WHERE IsDeleted=0
UNION ALL SELECT '=== DISPENSATIONS ===', COUNT(*) FROM Dispensations WHERE IsDeleted=0
UNION ALL SELECT '=== DISP_ITEMS ===', COUNT(*) FROM DispensationItems WHERE IsDeleted=0
UNION ALL SELECT '=== INVOICES ===', COUNT(*) FROM Invoices WHERE IsDeleted=0
UNION ALL SELECT '=== INVOICE_ITEMS ===', COUNT(*) FROM InvoiceItems WHERE IsDeleted=0
UNION ALL SELECT '=== STOCK_TXN ===', COUNT(*) FROM StockTransactions WHERE IsDeleted=0;

SELECT MedicineCode, Name, StockQuantity, MinimumStock,
  CASE WHEN StockQuantity<=MinimumStock THEN '⚠ SAP HET' ELSE 'OK' END TinhTrang
FROM Medicines WHERE IsDeleted=0 ORDER BY StockQuantity ASC;
