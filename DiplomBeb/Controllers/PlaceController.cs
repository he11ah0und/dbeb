using DiplomBeb.Database;
using DiplomBeb.Database.Model;
using DiplomBeb.Filters;
using DiplomBeb.Utils.AdaptiveListing;
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
    [HttpGet("users/{placeId}/{skip}/{take}")]
    public async Task<IActionResult> GetUsersInPlace(ulong placeId, ulong skip, ulong take)
    {
        skip = Math.Clamp(skip, 0, 70);
        take = Math.Clamp(take, 10, 100);
        var authedId = ulong.Parse((string)HttpContext.Items["authed.id"]);
        var authedUser = _context.Users
            .Include(u => u.ApiRole)
            .Include(u => u.Place)
            .SingleOrDefault(u => u.Id == authedId);
        
        if (authedUser == null)
            return BadRequest();

        var inPlace = _context.Users
                .Include(u => u.Place)
                .SingleOrDefault(u => u.Place.Id == placeId && u.Id == authedId)
            is User;
        
        if (!authedUser.ApiRole.IsMoreUser() 
            && (authedUser.Place.Id != (ulong)placeId
                || !inPlace))
            return NoContent();
        
        var users = _context.Users
            .Include(u => u.Place)
            .Where(u => u.Place.Id == (ulong)placeId)
            .OrderByDescending(u => u.Score)
            .Skip((int)skip)
            .Take((int)take)
            .Select(u => new User
            {
                Name = u.Name,
                Middlename = u.Middlename,
                Surname = u.Surname,
                Score = u.Score,
                WebRole = u.WebRole,
            })
            .Cast<ListElement>()
            .ToArray();

        return Ok(users);
    }
    
    [AuthFilter("user")]
    [HttpGet("{placeId:long}/{userId:long}")]
    public async Task<IActionResult> IsInPlace(long placeId, long userId)
    {
        var authedId = ulong.Parse((string)HttpContext.Items["authed.id"]);
        var authedUser = _context.Users
            .Include(u => u.ApiRole)
            .Include(u => u.Place)
            .SingleOrDefault(u => u.Id == authedId);
        
        if (authedUser == null)
            return BadRequest();

        var inPlace = _context.Users
                .Include(u => u.Place)
                .SingleOrDefault(u => 
                    u.Place.Id == (ulong)placeId 
                    && u.Id == authedId 
                    && authedId == (ulong)userId)
            is User;
        
        if (!authedUser.ApiRole.IsMoreUser() 
            && (authedUser.Place.Id != (ulong)placeId
                || !inPlace))
            return NoContent();
        
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
    [HttpGet("total/{placeId}")]
    public async Task<IActionResult> GetPlaceUsers(ulong placeId)
    {
        var authedId = ulong.Parse((string)HttpContext.Items["authed.id"]);
        var authedUser = _context.Users
            .Include(u => u.ApiRole)
            .Include(u => u.Place)
            .SingleOrDefault(u => u.Id == authedId);

        if (authedUser == null)
            return NoContent();

        var inPlace = _context.Users
            .Include(u => u.Place)
            .SingleOrDefault(u => u.Place.Id == (ulong)placeId && u.Id == authedId)
            is User;
        
        if (!authedUser.ApiRole.IsMoreUser() 
            && (authedUser.Place.Id != (ulong)placeId
                || !inPlace))
            return NoContent();
        
        var users = _context.Users
            .Include(u => u.Place)
            .Count(u => u.Place.Id == (ulong)placeId);
        return Ok(users);
    }
    
    [AuthFilter("user")]
    [HttpGet("{placeId:int}")]
    public async Task<IActionResult> GetPlace(int placeId)
    {
        var authedId = ulong.Parse((string)HttpContext.Items["authed.id"]);
        var authedUser = _context.Users
            .Include(u => u.ApiRole)
            .Include(u => u.Place)
            .SingleOrDefault(u => u.Id == authedId);

        if (authedUser == null)
            return NoContent();

        var inPlace = _context.Users
                .Include(u => u.Place)
                .SingleOrDefault(u => u.Place.Id == (ulong)placeId && u.Id == authedId)
            is User;
        
        if (!authedUser.ApiRole.IsMoreUser() 
            && (authedUser.Place.Id != (ulong)placeId
                || !inPlace))
            return NoContent();
        
        var place = _context.Places
            .SingleOrDefault(p => p.Id == (ulong)placeId);
        return Ok(place);
    }
    
    [AuthFilter("admin")]
    [HttpGet("total")]
    public async Task<IActionResult> GetPlaceCounts()
    {
        var authedId = ulong.Parse((string)HttpContext.Items["authed.id"]);
        var authedUser = _context.Users
            .Include(u => u.ApiRole)
            .Include(u => u.Place)
            .SingleOrDefault(u => u.Id == authedId);

        if (authedUser == null)
            return NoContent();
        
        if (!authedUser.ApiRole.IsMoreUser())
            return NoContent();
        
        var places = _context.Places
            .Count();
        return Ok(places);
    }
}