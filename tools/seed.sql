-- Fix admin password (Admin@123)
UPDATE Users 
SET PasswordHash = '$2a$11$095qHU2JThJMau4faqkkUOB6N7Q8aWW1o2tsF9XRdZ7tTeZfiwTf2'
WHERE Username = 'admin';

-- Add nurse1 (Nurse@123) - Role 3 = Nurse
IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'nurse1')
INSERT INTO Users (Id, Username, PasswordHash, FullName, Email, Role, IsActive, IsDeleted, CreatedAt)
VALUES (
  '00000000-0000-0000-0000-000000000002',
  'nurse1',
  '$2a$11$eIa.dbZb6M3GZoRRFjRMNe3/7bkmf4AW8kfC4D1DCe2lo7/.099Me',
  N'Nguyen Thi Y Ta',
  'nurse1@pharmacy.com',
  3, 1, 0, GETUTCDATE()
);

-- Add doctor1 (Doctor@123) - Role 2 = Doctor
IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'doctor1')
INSERT INTO Users (Id, Username, PasswordHash, FullName, Email, Role, IsActive, IsDeleted, CreatedAt)
VALUES (
  '00000000-0000-0000-0000-000000000003',
  'doctor1',
  '$2a$11$XpX3XKtxeRmdLQ2YZeiIIuxVgMG.7dH6QUnaY.3.aps0V27eJb6JG',
  N'Tran Van Bac Si',
  'doctor1@pharmacy.com',
  2, 1, 0, GETUTCDATE()
);

-- Add patient1 (Patient@123) - Role 4 = Patient
IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'patient1')
INSERT INTO Users (Id, Username, PasswordHash, FullName, Email, Role, IsActive, IsDeleted, CreatedAt)
VALUES (
  '00000000-0000-0000-0000-000000000004',
  'patient1',
  '$2a$11$bjYmtEjEQSgmpXHkLQcXm.4pquu5X3XhOFOUQSxMVIhNsKMB.xuRi',
  N'Le Thi Benh Nhan',
  'patient1@pharmacy.com',
  4, 1, 0, GETUTCDATE()
);

SELECT Username, Email, Role, IsActive FROM Users ORDER BY Role;
