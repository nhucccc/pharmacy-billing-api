using Microsoft.EntityFrameworkCore;
using PharmacyBilling.Domain.Entities;
using PharmacyBilling.Domain.Enums;

namespace PharmacyBilling.Infrastructure.Data;

/// <summary>
/// Seed dữ liệu mẫu - dùng EF Core thuần, hoạt động trên cả SQL Server và PostgreSQL
/// </summary>
public static class DatabaseSeeder
{
    private const string AdminHash   = "$2a$11$095qHU2JThJMau4faqkkUOB6N7Q8aWW1o2tsF9XRdZ7tTeZfiwTf2"; // Admin@123
    private const string NurseHash   = "$2a$11$eIa.dbZb6M3GZoRRFjRMNe3/7bkmf4AW8kfC4D1DCe2lo7/.099Me";  // Nurse@123
    private const string DoctorHash  = "$2a$11$XpX3XKtxeRmdLQ2YZeiIIuxVgMG.7dH6QUnaY.3.aps0V27eJb6JG"; // Doctor@123
    private const string PatientHash = "$2a$11$bjYmtEjEQSgmpXHkLQcXm.4pquu5X3XhOFOUQSxMVIhNsKMB.xuRi"; // Patient@123

    public static async Task SeedAsync(PharmacyDbContext context)
    {
        if (await context.Users.AnyAsync())
        {
            Console.WriteLine("[SEED] Already seeded, skipping.");
            return;
        }

        Console.WriteLine("[SEED] Starting seed...");
        await SeedUsersAsync(context);
        await SeedMedicinesAsync(context);
        Console.WriteLine("[SEED] Completed!");
    }

    private static async Task SeedUsersAsync(PharmacyDbContext ctx)
    {
        var t = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Tạo users qua raw SQL để bypass private setters
        // Dùng parameterized query an toàn
        var insertSql = IsPostgres(ctx)
            ? @"INSERT INTO ""Users""(""Id"",""Username"",""PasswordHash"",""FullName"",""Email"",""PhoneNumber"",
                ""Role"",""IsActive"",""IsDeleted"",""CreatedAt"",""PatientCode"",""DateOfBirth"",""Gender"",""Address"",""InsuranceNumber"")
                VALUES(@p0,@p1,@p2,@p3,@p4,@p5,@p6,true,false,@p7,@p8,@p9,@p10,@p11,@p12)"
            : @"INSERT INTO Users(Id,Username,PasswordHash,FullName,Email,PhoneNumber,
                Role,IsActive,IsDeleted,CreatedAt,PatientCode,DateOfBirth,Gender,Address,InsuranceNumber)
                VALUES(@p0,@p1,@p2,@p3,@p4,@p5,@p6,1,0,@p7,@p8,@p9,@p10,@p11,@p12)";

        object?[] Row(string id, string user, string hash, string name, string email, string phone, int role,
            string? code=null, string? dob=null, string? gender=null, string? addr=null, string? ins=null)
            => new object?[] { id, user, hash, name, email, phone, role, t, code, dob!=null?DateTime.Parse(dob):null, gender, addr, ins };

        var rows = new[]
        {
            Row("00000000-0000-0000-0000-000000000001","admin",   AdminHash,  "Nguyễn Văn Quản Trị", "admin@phongkham.vn",  "0901234567",1),
            Row("00000000-0000-0000-0000-000000000002","nurse1",  NurseHash,  "Y tá Nguyễn Thị Y Tá","nurse@phongkham.vn",  "0902345678",3),
            Row("00000000-0000-0000-0000-000000000003","doctor1", DoctorHash, "BS. Trần Văn Bác Sĩ", "doctor@phongkham.vn", "0903456789",2),
            Row("00000000-0000-0000-0000-000000000004","patient1",PatientHash,"Lê Thị Bệnh Nhân",    "patient@phongkham.vn","0904567890",4,"BN20260101AAAA","2000-05-10","Nữ","100 Lê Lợi, Q.1"),
            Row("00000000-0000-0000-0000-000000000005","doctor2", DoctorHash, "BS. Nguyễn Thành Tú", "doctor2@phongkham.vn","0905678901",2),
            Row("00000000-0000-0000-0000-000000000006","nurse2",  NurseHash,  "Y tá Lê Thị Bích",    "nurse2@phongkham.vn", "0906789012",3),
            Row("00000000-0000-0000-0000-000000000007","bn_tranvanminh", PatientHash,"Trần Văn Minh","tvminh@gmail.com","0907890123",4,"BN20260110ABCD","1985-03-15","Nam","123 Nguyễn Trãi, Q.1","HS4012345678"),
            Row("00000000-0000-0000-0000-000000000008","bn_phamthilan",  PatientHash,"Phạm Thị Lan", "ptlan@gmail.com", "0908901234",4,"BN20260112EFGH","1992-07-22","Nữ","456 Lê Lợi, Q.3","HS4023456789"),
            Row("00000000-0000-0000-0000-000000000009","bn_levanhai",    PatientHash,"Lê Văn Hải",   "lvhai@gmail.com", "0909012345",4,"BN20260201IJKL","1978-11-08","Nam","789 Võ Văn Tần, Q.3"),
            Row("00000000-0000-0000-0000-000000000010","bn_nguyenthihoa",PatientHash,"Nguyễn Thị Hoa","nthoa@gmail.com","0910123456",4,"BN20260215MNOP","2000-04-30","Nữ","321 Đinh Tiên Hoàng","HS4034567890"),
        };

        foreach (var r in rows)
            await ctx.Database.ExecuteSqlRawAsync(insertSql, r!);

        Console.WriteLine($"[SEED] {rows.Length} users inserted.");
    }

    private static async Task SeedMedicinesAsync(PharmacyDbContext ctx)
    {
        if (await ctx.Medicines.AnyAsync()) return;

        var meds = new[]
        {
            ("REAL-TM001","Amlodipin 5mg","Amlodipine besylate 5mg","Dược Hậu Giang","Việt Nam",MedicineUnit.Vien,150,30,"Tim mạch",8500m,5000m,true),
            ("REAL-TM002","Atorvastatin 20mg","Atorvastatin calcium 20mg","Pfizer","Mỹ",MedicineUnit.Vien,200,30,"Tim mạch",12000m,8000m,true),
            ("REAL-TH001","Omeprazol 20mg","Omeprazole 20mg","AstraZeneca","Thụy Điển",MedicineUnit.Vien,300,50,"Tiêu hóa",5500m,3200m,true),
            ("REAL-TH002","Metoclopramide 10mg","Metoclopramide HCl","Dược phẩm TW1","Việt Nam",MedicineUnit.Vien,400,50,"Tiêu hóa",800m,500m,false),
            ("REAL-HH001","Salbutamol 4mg","Salbutamol sulfate 4mg","GSK","Anh",MedicineUnit.Vien,180,30,"Hô hấp",3500m,2000m,true),
            ("REAL-HH002","Acetylcystein 200mg","Acetylcysteine 200mg","Sandoz","Đức",MedicineUnit.Goi,250,30,"Hô hấp",6500m,4000m,false),
            ("REAL-KS001","Amoxicillin 500mg","Amoxicillin trihydrate 500mg","Imexpharm","Việt Nam",MedicineUnit.Vien,500,100,"Kháng sinh",2500m,1500m,true),
            ("REAL-KS002","Azithromycin 500mg","Azithromycin dihydrate 500mg","Pfizer","Mỹ",MedicineUnit.Vien,120,20,"Kháng sinh",18000m,12000m,true),
            ("REAL-GD001","Paracetamol 500mg","Paracetamol 500mg","Dược Hậu Giang","Việt Nam",MedicineUnit.Vien,1000,100,"Giảm đau",1500m,800m,false),
            ("REAL-GD002","Ibuprofen 400mg","Ibuprofen 400mg","Abbott","Mỹ",MedicineUnit.Vien,350,50,"Giảm đau",3200m,2000m,false),
            ("REAL-TD001","Metformin 500mg","Metformin HCl 500mg","Merck","Đức",MedicineUnit.Vien,200,30,"Nội tiết",4500m,2800m,true),
            ("REAL-VT001","Vitamin C 1000mg","Ascorbic acid 1000mg","Bayer","Đức",MedicineUnit.Vien,600,50,"Vitamin",4000m,2500m,false),
            ("REAL-VT002","Vitamin D3 1000IU","Cholecalciferol 1000IU","Sanofi","Pháp",MedicineUnit.Vien,180,20,"Vitamin",8000m,5000m,false),
            ("REAL-TK001","Diazepam 5mg","Diazepam 5mg","Roche","Thụy Sĩ",MedicineUnit.Vien,8,10,"Thần kinh",9500m,6000m,true),
            ("REAL-MT001","Tobramycin nhỏ mắt 0.3%","Tobramycin 0.3%","Alcon","Mỹ",MedicineUnit.Chai,60,10,"Nhãn khoa",45000m,30000m,true),
            ("REAL-DU001","Cetirizin 10mg","Cetirizine HCl 10mg","UCB","Bỉ",MedicineUnit.Vien,280,30,"Dị ứng",5000m,3000m,false),
            ("REAL-TD002","NaCl 0.9% 500ml","Sodium chloride 0.9%","Braun","Đức",MedicineUnit.Chai,100,20,"Dịch truyền",25000m,15000m,true),
            ("REAL-BG001","Silymarin 70mg","Silymarin 70mg","Madaus","Đức",MedicineUnit.Vien,5,10,"Tiêu hóa",12000m,8000m,false),
            ("REAL-XK001","Glucosamine 500mg","Glucosamine sulfate 500mg","Roussel","Pháp",MedicineUnit.Vien,90,15,"Cơ xương khớp",15000m,10000m,false),
            ("REAL-NM001","Xylometazoline nhỏ mũi","Xylometazoline 0.05%","Novartis","Thụy Sĩ",MedicineUnit.Chai,6,10,"Tai mũi họng",28000m,18000m,false),
        };

        foreach (var m in meds)
        {
            var med = Medicine.Create(m.Item2, m.Item3, m.Item6, m.Item10, m.Item11, m.Item7,
                m.Item12, m.Item4, m.Item9, null, m.Item8);
            // Set MedicineCode cố định thay vì auto-generate
            SetPrivate(med, "MedicineCode", m.Item1);
            SetPrivate(med, "CountryOfOrigin", m.Item5);
            await ctx.Medicines.AddAsync(med);
        }

        await ctx.SaveChangesAsync();
        Console.WriteLine($"[SEED] {meds.Length} medicines inserted.");
    }

    private static bool IsPostgres(PharmacyDbContext ctx)
        => ctx.Database.ProviderName?.Contains("Npgsql") == true;

    private static void SetPrivate(object obj, string prop, object? val)
        => obj.GetType().GetProperty(prop)?.SetValue(obj, val);
}
