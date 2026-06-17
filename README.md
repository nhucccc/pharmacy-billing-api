# Pharmacy & Billing Service — Đề tài 05

> **Hệ thống đặt lịch & quản lý phòng khám**  
> Nhóm 3 — Pharmacy & Billing Service  
> Môn: Lập trình Fullstack  

---

## Kiến trúc

```
┌─────────────────────────────────────────┐
│         Frontend VueJS 3 (port 3000)    │
│         Ant Design Vue + Pinia          │
└──────────────┬──────────────────────────┘
               │ /api/*
        ┌──────▼──────┐
        │ Nginx Proxy │
        └──────┬──────┘
               │
   ┌───────────▼───────────────┐
   │   Pharmacy & Billing API  │  port 5000/8080
   │   ASP.NET Core 8          │
   │   Clean Architecture      │
   └──────┬──────┬─────────────┘
          │      │
    ┌─────▼─┐  ┌─▼──────────┐
    │SQL    │  │ RabbitMQ   │
    │Server │  │ Consumer   │
    └───────┘  └────────────┘
```

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend | ASP.NET Core 8, Clean Architecture |
| ORM | Entity Framework Core 9 + SQL Server |
| Auth | JWT Bearer + Refresh Token |
| Messaging | RabbitMQ (async) + REST fallback |
| Logging | Serilog |
| Validation | FluentValidation |
| API Docs | Swagger / OpenAPI |
| Frontend | Vue 3 + TypeScript + Ant Design Vue |
| State | Pinia |
| Container | Docker + docker-compose |

## Cấu trúc project

```
PharmacyBilling/
├── src/
│   ├── PharmacyBilling.Domain/         # Entities, Events, Interfaces
│   ├── PharmacyBilling.Application/    # Services, DTOs, Validators
│   ├── PharmacyBilling.Infrastructure/ # EF Core, Repositories, JWT, RabbitMQ
│   └── PharmacyBilling.API/            # Controllers, Middleware, Program.cs
├── frontend/                           # Vue 3 + Ant Design Vue
└── docker-compose.yml
```

## Phân quyền (4 roles)

| Role | Quyền |
|------|-------|
| **Admin** | Toàn quyền: CRUD thuốc, xem báo cáo, quản lý người dùng |
| **Bác sĩ** | Xem danh sách thuốc, xem phiếu xuất |
| **Y tá / Tiếp tân** | Nhập kho, xuất thuốc, thu viện phí |
| **Bệnh nhân** | Xem hóa đơn & đơn thuốc của mình |

## Chạy với Docker

```bash
# Clone hoặc vào thư mục project
cd PharmacyBilling

# Build và chạy toàn bộ hệ thống
docker-compose up --build -d

# Kiểm tra logs
docker-compose logs -f pharmacy-api

# Dừng hệ thống
docker-compose down
```

### Truy cập sau khi chạy

| Service | URL |
|---------|-----|
| **Frontend** | http://localhost:3000 |
| **API** | http://localhost:5000 |
| **Swagger UI** | http://localhost:5000/swagger |
| **RabbitMQ Management** | http://localhost:15672 (guest/guest) |
| **Health Check** | http://localhost:5000/health |

## Chạy Development

### Backend

```bash
cd src/PharmacyBilling.API
dotnet run
# API chạy tại http://localhost:5000
```

### Frontend

```bash
cd frontend
npm install
npm run dev
# Frontend chạy tại http://localhost:3000
```

## Tài khoản mặc định

| Username | Password | Role |
|----------|----------|------|
| `admin` | `Admin@123` | Admin |

> Seed data được tạo tự động khi migration chạy lần đầu.

## API Endpoints

### Auth
- `POST /api/auth/login` — Đăng nhập, nhận JWT
- `POST /api/auth/register` — Đăng ký (Admin tạo user)
- `POST /api/auth/register/patient` — Bệnh nhân tự đăng ký
- `POST /api/auth/refresh` — Refresh token
- `POST /api/auth/logout` — Đăng xuất
- `GET  /api/auth/me` — Thông tin user hiện tại

### Medicines (Kho thuốc)
- `GET    /api/medicines` — Danh sách (search, filter, paging)
- `GET    /api/medicines/{id}` — Chi tiết
- `GET    /api/medicines/categories` — Danh sách nhóm thuốc
- `GET    /api/medicines/low-stock` — Thuốc sắp hết
- `POST   /api/medicines` — Thêm thuốc [Admin]
- `PUT    /api/medicines/{id}` — Cập nhật [Admin]
- `DELETE /api/medicines/{id}` — Xóa [Admin]
- `POST   /api/medicines/{id}/import-stock` — Nhập kho [Admin, Nurse]
- `POST   /api/medicines/{id}/adjust-stock` — Điều chỉnh kho [Admin]

### Dispensations (Phiếu xuất thuốc)
- `GET  /api/dispensations` — Danh sách
- `GET  /api/dispensations/{id}` — Chi tiết
- `POST /api/dispensations` — Tạo phiếu (REST fallback)
- `POST /api/dispensations/{id}/process` — Xuất thuốc từ kho
- `POST /api/dispensations/{id}/cancel` — Hủy phiếu

### Invoices (Hóa đơn viện phí)
- `GET  /api/invoices` — Danh sách
- `GET  /api/invoices/{id}` — Chi tiết
- `POST /api/invoices` — Tạo hóa đơn
- `POST /api/invoices/{id}/pay` — Thu tiền
- `POST /api/invoices/{id}/cancel` — Hủy
- `GET  /api/invoices/reports/revenue` — Báo cáo doanh thu [Admin]

### Dashboard & Users
- `GET /api/dashboard` — Thống kê tổng quan
- `GET /api/users` — Danh sách người dùng [Admin]
- `GET /health` — Health check

## RabbitMQ Integration

Service **lắng nghe** event `prescription.created` từ **Medical Record Service**:

```json
// Exchange: pharmacy.events
// Queue: pharmacy.prescription.created  
// Routing key: prescription.created
{
  "prescriptionId": "uuid",
  "patientId": "uuid",
  "doctorId": "uuid",
  "patientName": "Nguyễn Văn A",
  "doctorName": "BS. Trần Thị B",
  "diagnosis": "Viêm họng",
  "items": [
    { "medicineId": "uuid", "quantity": 10, "dosage": "2 viên/ngày" }
  ]
}
```

Khi nhận event → tự động tạo `Dispensation` với status `Pending`.

## Database Schema (PharmacyDB)

- **Users** — Tài khoản 4 vai trò + JWT refresh token
- **Medicines** — Kho thuốc với giá, tồn kho
- **StockTransactions** — Lịch sử nhập/xuất kho
- **Dispensations** — Phiếu xuất thuốc (ref PrescriptionId từ service khác)
- **DispensationItems** — Chi tiết thuốc trong phiếu
- **Invoices** — Hóa đơn viện phí
- **InvoiceItems** — Chi tiết hóa đơn
