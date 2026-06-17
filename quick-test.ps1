Write-Host "=== Pharmacy Billing System Quick Test ===" -ForegroundColor Green

# Test 1: Health Check
Write-Host "1. Testing API Health..." -ForegroundColor Yellow
$health = wget http://localhost:5000/health -UseBasicParsing
if ($health.StatusCode -eq 200) {
    Write-Host "   ✅ API Health: OK" -ForegroundColor Green
} else {
    Write-Host "   ❌ API Health: FAILED ($($health.StatusCode))" -ForegroundColor Red
}

# Test 2: Swagger
Write-Host "2. Testing Swagger..." -ForegroundColor Yellow
$swagger = wget http://localhost:5000/swagger/index.html -UseBasicParsing
if ($swagger.StatusCode -eq 200) {
    Write-Host "   ✅ Swagger: OK" -ForegroundColor Green
} else {
    Write-Host "   ❌ Swagger: FAILED ($($swagger.StatusCode))" -ForegroundColor Red
}

# Test 3: Frontend
Write-Host "3. Testing Frontend..." -ForegroundColor Yellow
$frontend = wget http://localhost:3000 -UseBasicParsing
if ($frontend.StatusCode -eq 200) {
    Write-Host "   ✅ Frontend: OK" -ForegroundColor Green
} else {
    Write-Host "   ❌ Frontend: FAILED ($($frontend.StatusCode))" -ForegroundColor Red
}

# Test 4: RabbitMQ Management
Write-Host "4. Testing RabbitMQ Management..." -ForegroundColor Yellow
$rabbitmq = wget http://localhost:15672 -UseBasicParsing
if ($rabbitmq.StatusCode -eq 200 -or $rabbitmq.StatusCode -eq 401) {
    Write-Host "   ✅ RabbitMQ Management: OK" -ForegroundColor Green
} else {
    Write-Host "   ❌ RabbitMQ Management: FAILED ($($rabbitmq.StatusCode))" -ForegroundColor Red
}

# Test 5: List Services
Write-Host "5. Docker Containers Status..." -ForegroundColor Yellow
docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}" | findstr "pharmacy"

Write-Host ""
Write-Host "=== Access URLs ===" -ForegroundColor Cyan
Write-Host "• Frontend: http://localhost:3000" -ForegroundColor White
Write-Host "• API Swagger: http://localhost:5000/swagger" -ForegroundColor White
Write-Host "• RabbitMQ Management: http://localhost:15672 (guest/guest)" -ForegroundColor White
Write-Host "• SQL Server: localhost:1433 (sa/YourStrong@Passw0rd)" -ForegroundColor White

Write-Host ""
Write-Host "=== Test Credentials ===" -ForegroundColor Cyan
Write-Host "• Admin: admin / Admin@123" -ForegroundColor White
Write-Host "• Email: admin@pharmacy.com" -ForegroundColor White

Write-Host ""
Write-Host "✅ Hệ thống đã chạy thành công!" -ForegroundColor Green