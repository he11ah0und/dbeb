using DiplomBeb.Database;
using DiplomBeb.Database.Model;
using DiplomBeb.Filters;
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
    [HttpGet("{skip:long}/{take:long}")]
    public async Task<IActionResult> Get(long skip, long take)
    {
        return Ok(_context.Library.Skip((int)skip).Take((int)take).ToArray());
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