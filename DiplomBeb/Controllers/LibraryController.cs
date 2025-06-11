using DiplomBeb.Database;
using DiplomBeb.Database.Model;
using DiplomBeb.Filters;
using DiplomBeb.Utils.AdaptiveListing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomBeb.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LibraryController : ControllerBase
{
    private readonly MainDBContext _context;

    public LibraryController(MainDBContext context)
    {
        _context = context;
    }
    
    [AuthFilter]
    [HttpGet("{skip}/{take}")]
    public async Task<IActionResult> Get(ulong skip, ulong take)
    {
        var elements = _context.Library
            .Skip((int)skip)
            .Take((int)take)
            .Cast<ListElement>()
            .ToArray();
        return Ok(elements);
    }
    
    [AuthFilter]
    [HttpGet("total_filters")]
    public async Task<IActionResult> TotalFilters()
    {
        return Ok(_context.LibraryFilters.Count());
    }

    [AuthFilter]
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
    
    [AuthFilter]
    [HttpGet("total")]
    public async Task<IActionResult> GetTotal()
    {
        return Ok(_context.Library.Count());
    }
    
    [AuthFilter]
    [HttpGet("random/{count:long}")]
    public async Task<IActionResult> Random(long count)
    {
        return Ok(_context.Library.OrderBy(o => EF.Functions.Random()).Take((int)count));
    }
}