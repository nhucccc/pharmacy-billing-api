using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyBilling.Infrastructure.Data;

namespace PharmacyBilling.API.Controllers;

/// <summary>Health Check Endpoint</summary>
[Tags("Health")]
public class HealthController : BaseController
{
    private readonly PharmacyDbContext _context;

    public HealthController(PharmacyDbContext context) => _context = context;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Check(CancellationToken ct)
    {
        try
        {
            await _context.Database.CanConnectAsync(ct);
            return Ok(new
            {
                status = "healthy",
                service = "PharmacyBillingService",
                timestamp = DateTime.UtcNow,
                database = "connected"
            });
        }
        catch
        {
            return StatusCode(503, new
            {
                status = "unhealthy",
                service = "PharmacyBillingService",
                timestamp = DateTime.UtcNow,
                database = "disconnected"
            });
        }
    }
}
