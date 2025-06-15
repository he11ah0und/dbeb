using dserver.Database;
using dserver.Database.Model;
using dshared.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dserver.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LibraryController : ControllerBase
{
    private readonly MainDBContext _context;

    public LibraryController(MainDBContext context)
    {
        _context = context;
    }
    
    //[AuthFilter]
    [HttpPost("{skip}/{take}")]
    public async Task<IActionResult> Get(ulong skip, ulong take, [FromBody] string[] filters)
    {
        ListElement[] elements;
        if (filters.LongLength == 0)
        {
            elements = _context.Library
                .Skip((int)skip)
                .Take((int)take)
                .Cast<ListElement>()
                .ToArray();
        }
        else
        {
            elements = _context.LinkLibraryFilters
                .Include(llf => llf.LibraryFilter)
                .Include(llf => llf.Library)
                .Where(llf => filters.Contains(llf.LibraryFilter.Name))
                .OrderBy(l => l.Id)
                .Skip((int)skip)
                .Take((int)take)
                .Select(llf => llf.Library)
                .Cast<ListElement>()
                .ToArray();
        }

        return Ok(elements);
    }
    
    //[AuthFilter]
    [HttpPost("total")]
    public async Task<IActionResult> GetTotal([FromBody] string[] filters)
    {
        int count;
        if (filters.LongLength == 0)
        {
            count = _context.Library.Count();
        }
        else
        {
            count = _context.LinkLibraryFilters
                .Include(llf => llf.LibraryFilter)
                .Include(llf => llf.Library)
                .Count(llf => filters.Contains(llf.LibraryFilter.Name));
        }
        return Ok(count);
    }
    
    //[AuthFilter]
    [HttpGet("total_filters")]
    public async Task<IActionResult> TotalFilters()
    {
        return Ok(_context.LibraryFilters.Count());
    }

    //[AuthFilter]
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
    
    //[AuthFilter]
    [HttpGet("random/{count:long}")]
    public async Task<IActionResult> Random(long count)
    {
        return Ok(_context.Library
            .OrderBy(o => EF.Functions.Random())
            .Take((int)count)
            .Cast<ListElement>()
            .ToArray());
    }
}