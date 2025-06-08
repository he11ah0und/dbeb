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
    [HttpGet("get30")]
    public async Task<IActionResult> Get30(int page)
    {
        var totake = 30;
        var toskip = totake * (page - 1);
        return Ok(_context.Library.Skip(toskip).Take(totake).ToArray());
    }
    
    [AuthFilter]
    [HttpGet("pages")]
    public async Task<IActionResult> GetPages()
    {
        var totake = 30;
        var count = _context.Library.Count();
        var pages = Math.Round((double)count / (double)totake, MidpointRounding.ToPositiveInfinity);
        return Ok(pages);
    }
    
    [AuthFilter]
    [HttpGet("total")]
    public async Task<IActionResult> GetTotal()
    {
        return Ok(_context.Library.Count());
    }
    
    [AuthFilter]
    [HttpGet("get4")]
    public async Task<IActionResult> Get4()
    {
        return Ok(_context.Library.OrderBy(o => EF.Functions.Random()).Take(4));
    }
}