using DiplomBeb.Database;
using DiplomBeb.Filters;
using DiplomBeb.Utils.AdaptiveListing;
using Microsoft.AspNetCore.Mvc;

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
    [HttpGet("filters/{skip}/{take}")]
    public async Task<IActionResult> GetFilters(ulong skip, ulong take)
    {
        var elements = _context.LibraryFilters
            .Skip((int)skip)
            .Take((int)take)
            .Cast<ListElement>()
            .ToArray();
        return Ok(elements);
    }
}