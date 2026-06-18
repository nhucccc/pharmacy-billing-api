USE PharmacyDB;
-- Hash đúng cho Admin@123
UPDATE Users SET PasswordHash = '$2a$11$095qHU2JThJMau4faqkkUOB6N7Q8aWW1o2tsF9XRdZ7tTeZfiwTf2' WHERE Username = 'admin';
-- Hash đúng cho Nurse@123
UPDATE Users SET PasswordHash = '$2a$11$eIa.dbZb6M3GZoRRFjRMNe3/7bkmf4AW8kfC4D1DCe2lo7/.099Me' WHERE Username IN ('nurse1','nurse2');
-- Hash đúng cho Doctor@123
UPDATE Users SET PasswordHash = '$2a$11$XpX3XKtxeRmdLQ2YZeiIIuxVgMG.7dH6QUnaY.3.aps0V27eJb6JG' WHERE Username IN ('doctor1','doctor2');
-- Hash đúng cho Patient@123
UPDATE Users SET PasswordHash = '$2a$11$bjYmtEjEQSgmpXHkLQcXm.4pquu5X3XhOFOUQSxMVIhNsKMB.xuRi' WHERE Username IN ('patient1','bn_tranvanminh','bn_phamthilan','bn_levanhai','bn_nguyenthihoa');

SELECT Username, LEN(PasswordHash) AS HashLen, LEFT(PasswordHash, 7) AS Prefix FROM Users ORDER BY Role;
