using DiplomBeb.Database;
using DiplomBeb.Database.Model;
using DiplomBeb.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomBeb.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlaceController : ControllerBase
{
    private readonly MainDBContext _context;
    public PlaceController(MainDBContext context)
    {
        _context = context;
    }
    
    [AuthFilter("user")]
    [HttpGet("users/{placeId:long}")]
    public async Task<IActionResult> GetUsersInPlace(long placeId)
    {
        var users = _context.Users
            .Include(u => u.Place)
            .Where(u => u.Place.Id == (ulong)placeId)
            .Select(u => new
            {
                u.Name,
                u.Middlename,
                u.Surname,
                u.Score,
                u.WebRole,
            })
            .ToArray();

        return Ok(users);
    }
    
    [AuthFilter("user")]
    [HttpGet("{placeId:long}/{userId:long}")]
    public async Task<IActionResult> IsInPlace(long placeId, long userId)
    {
        var users = _context.Users
            .Include(u => u.Place)
            .Where(u => u.Place.Id == (ulong)placeId)
            .Select(u => new User()
            {
                Id = u.Id,
                Name = u.Name,
                Middlename = u.Middlename,
                Surname = u.Surname,
                Score = u.Score,
                WebRole = u.WebRole,
            })
            .ToArray();

        var uco = users.LongLength;
        if (uco == 0)
        {
            return NoContent();
        }
        var uid = (ulong)userId;
        for (long i = 0; i < uco; i++)
        {
            var u = users[i];
            if (u.Id == uid)
            {
                return Ok(i + 1);
            }
        }
        return NoContent();
    }
    
    [AuthFilter("user")]
    [HttpGet("count/{placeId:long}")]
    public async Task<IActionResult> GetPlaceUsers(int placeId)
    {
        var users = _context.Users
            .Include(u => u.Place)
            .Count(u => u.Place.Id == (ulong)placeId);
        return Ok(users);
    }
    
    [AuthFilter("user")]
    [HttpGet("count")]
    public async Task<IActionResult> GetPlaceCounts()
    {
        var places = _context.Places
            .Count();
        return Ok(places);
    }
    
    [AuthFilter("user")]
    [HttpGet("{placeId:int}")]
    public async Task<IActionResult> GetPlace(int placeId)
    {
        var place = _context.Places
            .SingleOrDefault(p => p.Id == (ulong)placeId);
        return Ok(place);
    }
}