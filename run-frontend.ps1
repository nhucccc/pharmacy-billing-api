Write-Host "========================================" -ForegroundColor Magenta
Write-Host "  KHOI DONG FRONTEND - Vue 3 Dev Server" -ForegroundColor Magenta
Write-Host "========================================" -ForegroundColor Magenta

Write-Host "`nURL: http://localhost:3000" -ForegroundColor Green
Write-Host "API proxy -> http://localhost:5000" -ForegroundColor Green
Write-Host "Nhan Ctrl+C de dung`n" -ForegroundColor Gray

Set-Location "C:\BTLFullstack\PharmacyBilling\frontend"
npm run dev
