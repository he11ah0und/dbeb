using DiplomBeb.Database;
using DiplomBeb.Filters;
using DiplomBeb.Utils.AdaptiveListing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomBeb.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LogsController : ControllerBase
{
    private readonly MainDBContext _context;

    public LogsController(MainDBContext context)
    {
        _context = context;
    }
    
    [AuthFilter("admin")]
    [HttpGet("{skip}/{take}")]
    public async Task<IActionResult> GetLogs(ulong skip, ulong take)
    {
        var elements = _context.Logs
            .Include(l => l.User)
            .ThenInclude(u => u.WebRole)
            .Skip((int)skip)
            .Take((int)take)
            .Cast<ListElement>()
            .ToArray();
        return Ok(elements);
    }
    
    [AuthFilter("admin")]
    [HttpGet("total")]
    public async Task<IActionResult> TotalLogs()
    {
        return Ok(_context.Logs.Count());
    }
}