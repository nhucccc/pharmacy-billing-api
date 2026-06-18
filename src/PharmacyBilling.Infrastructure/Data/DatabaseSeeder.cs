using Microsoft.EntityFrameworkCore;
using PharmacyBilling.Domain.Entities;
using PharmacyBilling.Domain.Enums;
using System.Reflection;

namespace PharmacyBilling.Infrastructure.Data;

/// <summary>
/// Seed dữ liệu mẫu - dùng EF Core thuần, hoạt động trên SQL Server và PostgreSQL
/// </summary>
public static class DatabaseSeeder
{
    private const string AdminHash   = "$2a$11$095qHU2JThJMau4faqkkUOB6N7Q8aWW1o2tsF9XRdZ7tTeZfiwTf2";
    private const string NurseHash   = "$2a$11$eIa.dbZb6M3GZoRRFjRMNe3/7bkmf4AW8kfC4D1DCe2lo7/.099Me";
    private const string DoctorHash  = "$2a$11$XpX3XKtxeRmdLQ2YZeiIIuxVgMG.7dH6QUnaY.3.aps0V27eJb6JG";
    private const string PatientHash = "$2a$11$bjYmtEjEQSgmpXHkLQcXm.4pquu5X3XhOFOUQSxMVIhNsKMB.xuRi";

    public static async Task SeedAsync(PharmacyDbContext context)
    {
        if (await context.Users.AnyAsync())
        {
            Console.WriteLine("[SEED] Already seeded.");
            return;
        }

        Console.WriteLine("[SEED] Seeding...");
        await SeedUsersAsync(context);
        await SeedMedicinesAsync(context);
        Console.WriteLine("[SEED] Done!");
    }

    private static async Task SeedUsersAsync(PharmacyDbContext ctx)
    {
        var d = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc);
        var users = new List<User>();

        void Add(string id, string user, string hash, string name, string email, string phone,
            UserRole role, string? code=null, string? dob=null, string? gender=null,
            string? addr=null, string? ins=null)
        {
            // Dùng reflection để set private properties
            var u = (User)Activator.CreateInstance(typeof(User), true)!;
            Set(u, "Id",           Guid.Parse(id));
            Set(u, "Username",     user);
            Set(u, "PasswordHash", hash);
            Set(u, "FullName",     name);
            Set(u, "Email",        email);
            Set(u, "PhoneNumber",  phone);
            Set(u, "Role",         role);
            Set(u, "IsActive",     true);
            Set(u, "IsDeleted",    false);
            Set(u, "CreatedAt",    d);
            Set(u, "PatientCode",  code);
            Set(u, "DateOfBirth",  dob != null ? DateTime.SpecifyKind(DateTime.Parse(dob), DateTimeKind.Utc) : (DateTime?)null);
            Set(u, "Gender",       gender);
            Set(u, "Address",      addr);
            Set(u, "InsuranceNumber", ins);
            users.Add(u);
        }

        Add("00000000-0000-0000-0000-000000000001","admin",   AdminHash,  "Nguyễn Văn Quản Trị", "admin@phongkham.vn",  "0901234567",UserRole.Admin);
        Add("00000000-0000-0000-0000-000000000002","nurse1",  NurseHash,  "Y tá Nguyễn Thị Y Tá","nurse@phongkham.vn",  "0902345678",UserRole.Nurse);
        Add("00000000-0000-0000-0000-000000000003","doctor1", DoctorHash, "BS. Trần Văn Bác Sĩ", "doctor@phongkham.vn", "0903456789",UserRole.Doctor);
        Add("00000000-0000-0000-0000-000000000004","patient1",PatientHash,"Lê Thị Bệnh Nhân",    "patient@phongkham.vn","0904567890",UserRole.Patient,"BN20260101AAAA","2000-05-10","Nữ","100 Lê Lợi, Q.1");
        Add("00000000-0000-0000-0000-000000000005","doctor2", DoctorHash, "BS. Nguyễn Thành Tú", "doctor2@phongkham.vn","0905678901",UserRole.Doctor);
        Add("00000000-0000-0000-0000-000000000006","nurse2",  NurseHash,  "Y tá Lê Thị Bích",    "nurse2@phongkham.vn", "0906789012",UserRole.Nurse);
        Add("00000000-0000-0000-0000-000000000007","bn_tranvanminh", PatientHash,"Trần Văn Minh","tvminh@gmail.com","0907890123",UserRole.Patient,"BN20260110ABCD","1985-03-15","Nam","123 Nguyễn Trãi, Q.1","HS4012345678");
        Add("00000000-0000-0000-0000-000000000008","bn_phamthilan",  PatientHash,"Phạm Thị Lan", "ptlan@gmail.com", "0908901234",UserRole.Patient,"BN20260112EFGH","1992-07-22","Nữ","456 Lê Lợi, Q.3","HS4023456789");
        Add("00000000-0000-0000-0000-000000000009","bn_levanhai",    PatientHash,"Lê Văn Hải",   "lvhai@gmail.com", "0909012345",UserRole.Patient,"BN20260201IJKL","1978-11-08","Nam","789 Võ Văn Tần, Q.3");
        Add("00000000-0000-0000-0000-000000000010","bn_nguyenthihoa",PatientHash,"Nguyễn Thị Hoa","nthoa@gmail.com","0910123456",UserRole.Patient,"BN20260215MNOP","2000-04-30","Nữ","321 Đinh Tiên Hoàng","HS4034567890");

        await ctx.Users.AddRangeAsync(users);
        await ctx.SaveChangesAsync();
        Console.WriteLine($"[SEED] {users.Count} users.");
    }

    private static async Task SeedMedicinesAsync(PharmacyDbContext ctx)
    {
        if (await ctx.Medicines.AnyAsync()) return;

        var list = new List<Medicine>();

        void AddMed(string code, string name, string ai, string mfg, string country,
            MedicineUnit unit, decimal price, decimal importPrice, int stock, int minStock,
            string cat, bool rx)
        {
            var m = Medicine.Create(name, ai, unit, price, importPrice, stock, rx, mfg, cat, null, minStock);
            Set(m, "MedicineCode",    code);
            Set(m, "CountryOfOrigin", country);
            list.Add(m);
        }

        AddMed("REAL-TM001","Amlodipin 5mg","Amlodipine besylate 5mg","Dược Hậu Giang","Việt Nam",MedicineUnit.Vien,8500,5000,150,30,"Tim mạch",true);
        AddMed("REAL-TM002","Atorvastatin 20mg","Atorvastatin calcium 20mg","Pfizer","Mỹ",MedicineUnit.Vien,12000,8000,200,30,"Tim mạch",true);
        AddMed("REAL-TH001","Omeprazol 20mg","Omeprazole 20mg","AstraZeneca","Thụy Điển",MedicineUnit.Vien,5500,3200,300,50,"Tiêu hóa",true);
        AddMed("REAL-TH002","Metoclopramide 10mg","Metoclopramide HCl","Dược phẩm TW1","Việt Nam",MedicineUnit.Vien,800,500,400,50,"Tiêu hóa",false);
        AddMed("REAL-HH001","Salbutamol 4mg","Salbutamol sulfate 4mg","GSK","Anh",MedicineUnit.Vien,3500,2000,180,30,"Hô hấp",true);
        AddMed("REAL-HH002","Acetylcystein 200mg","Acetylcysteine 200mg","Sandoz","Đức",MedicineUnit.Goi,6500,4000,250,30,"Hô hấp",false);
        AddMed("REAL-KS001","Amoxicillin 500mg","Amoxicillin trihydrate 500mg","Imexpharm","Việt Nam",MedicineUnit.Vien,2500,1500,500,100,"Kháng sinh",true);
        AddMed("REAL-KS002","Azithromycin 500mg","Azithromycin dihydrate 500mg","Pfizer","Mỹ",MedicineUnit.Vien,18000,12000,120,20,"Kháng sinh",true);
        AddMed("REAL-GD001","Paracetamol 500mg","Paracetamol 500mg","Dược Hậu Giang","Việt Nam",MedicineUnit.Vien,1500,800,1000,100,"Giảm đau",false);
        AddMed("REAL-GD002","Ibuprofen 400mg","Ibuprofen 400mg","Abbott","Mỹ",MedicineUnit.Vien,3200,2000,350,50,"Giảm đau",false);
        AddMed("REAL-TD001","Metformin 500mg","Metformin HCl 500mg","Merck","Đức",MedicineUnit.Vien,4500,2800,200,30,"Nội tiết",true);
        AddMed("REAL-VT001","Vitamin C 1000mg","Ascorbic acid 1000mg","Bayer","Đức",MedicineUnit.Vien,4000,2500,600,50,"Vitamin",false);
        AddMed("REAL-VT002","Vitamin D3 1000IU","Cholecalciferol 1000IU","Sanofi","Pháp",MedicineUnit.Vien,8000,5000,180,20,"Vitamin",false);
        AddMed("REAL-TK001","Diazepam 5mg","Diazepam 5mg","Roche","Thụy Sĩ",MedicineUnit.Vien,9500,6000,8,10,"Thần kinh",true);
        AddMed("REAL-MT001","Tobramycin nhỏ mắt 0.3%","Tobramycin 0.3%","Alcon","Mỹ",MedicineUnit.Chai,45000,30000,60,10,"Nhãn khoa",true);
        AddMed("REAL-DU001","Cetirizin 10mg","Cetirizine HCl 10mg","UCB","Bỉ",MedicineUnit.Vien,5000,3000,280,30,"Dị ứng",false);
        AddMed("REAL-TD002","NaCl 0.9% 500ml","Sodium chloride 0.9%","Braun","Đức",MedicineUnit.Chai,25000,15000,100,20,"Dịch truyền",true);
        AddMed("REAL-BG001","Silymarin 70mg","Silymarin 70mg","Madaus","Đức",MedicineUnit.Vien,12000,8000,5,10,"Tiêu hóa",false);
        AddMed("REAL-XK001","Glucosamine 500mg","Glucosamine sulfate 500mg","Roussel","Pháp",MedicineUnit.Vien,15000,10000,90,15,"Cơ xương khớp",false);
        AddMed("REAL-NM001","Xylometazoline nhỏ mũi","Xylometazoline 0.05%","Novartis","Thụy Sĩ",MedicineUnit.Chai,28000,18000,6,10,"Tai mũi họng",false);

        await ctx.Medicines.AddRangeAsync(list);
        await ctx.SaveChangesAsync();
        Console.WriteLine($"[SEED] {list.Count} medicines.");
    }

    private static void Set(object obj, string prop, object? val)
    {
        var p = obj.GetType().GetProperty(prop,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        p?.SetValue(obj, val);
    }
}
