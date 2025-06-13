using DiplomBeb.Database;
using DiplomBeb.Filters;
using DiplomBeb.Database.Model;
using DiplomBeb.Utils.AdaptiveListing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomBeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MainDBContext _context;

        public UsersController(MainDBContext context)
        {
            _context = context;
        }
        
        // PUBLIC APIs
        //[AuthFilter]
        [HttpGet("total")] // количество пользователей
        public async Task<IActionResult> GetTotal()
        {
            return Ok(_context.Users.Count());
        }
        //[AuthFilter]
        [HttpGet("top/{skip}/{take}")] // топ 100 пользователей
        public async Task<IActionResult> GetTop(ulong skip, ulong take)
        {
            skip = Math.Clamp(skip, 0, 70);
            take = Math.Clamp(take, 10, 100);
            var devs = _context.Users
                .Include(x => x.WebRole)
                .Select(
                    x => 
                        new User
                        {
                            Name = x.Name,
                            Middlename = x.Middlename,
                            Surname = x.Surname,
                            Score = x.Score,
                            WebRole = x.WebRole
                        })
                .OrderByDescending(u => u.Score)
                .Skip((int)skip)
                .Take((int)take)
                .Cast<ListElement>()
                .ToArray();
            return Ok(devs);
        }
        
        // USER APIs
        [AuthFilter("user")]
        [HttpGet("getbyid/{id}")] 
        public async Task<IActionResult> Get(ulong id)
        {
            var user = _context.Users
                .Include(u => u.WebRole)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Middlename,
                    x.Surname,
                    x.Score,
                    x.WebRole,
                    x.Place
                })
                .OrderByDescending(u => u.Score)
                .Where(a => a.Id == id)
                .Cast<ListElement>()
                .Single();
            return Ok(user);
        }
        
        [AuthFilter("user")]
        [HttpGet("toppos/{id}")]
        public async Task<IActionResult> GetTopPos(ulong id)
        {
            var authedId = ulong.Parse((string)HttpContext.Items["authed.id"]);
            if (_context.Users
                    .Include(u => u.ApiRole)
                    .SingleOrDefault(u => u.Id == authedId) is not User authedUser)
                return NoContent();
            
            if (authedId != id && !authedUser.ApiRole.IsMoreUser())
                id = authedId;
            
            var userWithRank = _context.Set<UserRank>()
                .FromSqlRaw("""
                                    SELECT id, score, position
                                    FROM (
                                        SELECT 
                                            id, 
                                            score, 
                                            ROW_NUMBER() OVER (ORDER BY score DESC) AS position
                                        FROM digitalpass.users
                                    ) AS ranked_users
                                    WHERE id = {0}
                            """, id)
                .Select(u => new UserRank
                {
                    Id = u.Id,
                    Score = u.Score,
                    Position = u.Position
                })
                .FirstOrDefault();

            return Ok(userWithRank);
        }
        
        [AuthFilter("user")]
        [HttpGet("personal")] // get personal account
        public async Task<IActionResult> GetPersonal()
        {
            var authedId = ulong.Parse((string)HttpContext.Items["authed.id"]);
            var devs = _context.Users
                .Include(u => u.ApiRole)
                .Include(u => u.WebRole)
                .Include(u => u.Documents)
                .Include(u => u.Place)
                .Select(u => new User
                {
                    Id = u.Id,
                    Score = u.Score,
                    Surname = u.Surname,
                    Name = u.Name,
                    Middlename = u.Middlename,
                    Email = u.Email,
                    Phone = u.Phone,
                    Place = u.Place,
                    Documents = u.Documents,
                    WebRole = u.WebRole,
                    ApiRole = u.ApiRole
                })
                .Where(a => a.Id == authedId)
                .Cast<ListElement>()
                .ToArray();
            return Ok(devs);
        }
        [AuthFilter("user")]
        [HttpPatch("personal")] // update profile
        public async Task<IActionResult> PatchPersonal(ListElement li)
        {
            var authedId = ulong.Parse((string)HttpContext.Items["authed.id"]);
            var authedUser = _context.Users
                .SingleOrDefault(a => a.Id == authedId);

            authedUser.Email = li.Get("email", authedUser.Email);
            if (ulong.TryParse(li.Get("phone", authedUser.Phone.ToString()), out var phone))
            {
                authedUser.Phone = phone;
            }
            
            authedUser.Name = li.Get("name", authedUser.Name);
            authedUser.Surname = li.Get("surname", authedUser.Surname);
            authedUser.Middlename = li.Get("middlename", authedUser.Middlename);

            await _context.SaveChangesAsync();
            
            return Ok();
        }
        
        
        
        // admin api
        [AuthFilter("admin")]
        [HttpGet("get/{skip:long}/{take:long}")]
        public async Task<IActionResult> Get(long skip, long take)
        {
            var devs = _context.Users
                .Include(x => x.WebRole)
                .Include(x => x.Place)
                .Select(
                    x => 
                        new User
                        {   
                            Id = x.Id,
                            Name = x.Name,
                            Middlename = x.Middlename,
                            Surname = x.Surname,
                            Score = x.Score,
                            WebRole = x.WebRole,
                            Phone = x.Phone,
                            Email = x.Email,
                            Place = x.Place,
                        })
                .OrderByDescending(u => u.Id)
                .Skip((int)skip)
                .Take((int)take)
                .Cast<ListElement>()
                .ToArray();
            return Ok(devs);
        }
        
        [AuthFilter("admin")]
        [HttpPatch("personal/{id}")] // update profile
        public async Task<IActionResult> PatchPersonal(
            ulong id,
            ListElement li)
        {
            var authedId = ulong.Parse((string)HttpContext.Items["authed.id"]);
            var authedUser = _context.Users
                .Include(u => u.ApiRole)
                .SingleOrDefault(a => a.Id == authedId);

            if (authedId != id && authedUser?.ApiRole?.Name == "admin")
                return Ok(false);

            authedUser.Email = li.Get("email", authedUser.Email);
            if (ulong.TryParse(li.Get("phone", authedUser.Phone.ToString()), out var phone))
                authedUser.Phone = phone;
            
            authedUser.Name = li.Get("name", authedUser.Name);
            authedUser.Surname = li.Get("surname", authedUser.Surname);
            authedUser.Middlename = li.Get("middlename", authedUser.Middlename);

            await _context.SaveChangesAsync();
            
            return Ok(true);
        }
    }
}
