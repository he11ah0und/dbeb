using System.Diagnostics;
using DiplomBeb.Database;
using DiplomBeb.Filters;
using DiplomBeb.Database.Model;
using DiplomBeb.Systems.Auth;
using Microsoft.AspNetCore.Identity;
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
        [AuthFilter]
        [HttpGet("total")] // количество пользователей
        public async Task<IActionResult> GetTotal()
        {
            return Ok(_context.Users.Count());
        }
        [AuthFilter]
        [HttpGet("top")] // топ 100 пользователей
        public async Task<IActionResult> GetTop()
        {
            var devs = await _context.Users
                .Include(x => x.WebRole)
                .Select(
                    x => 
                        new
                        {
                            x.Name,
                            x.Middlename,
                            x.Surname,
                            x.Score,
                            x.WebRole
                        })
                .OrderByDescending(u => u.Score)
                .Take(100)
                .ToListAsync();
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
                .Take(100)
                .Single(a => a.Id == id);
            return Ok(user);
        }
        
        [AuthFilter("user")]
        [HttpGet("top/{id}")]
        public async Task<IActionResult> GetTopPos(ulong id)
        {
            var userWithRank = _context.Set<UserRank>()
                .FromSqlRaw("""
                            
                                    SELECT id, score, position
                                    FROM (
                                        SELECT 
                                            id, 
                                            score, 
                                            ROW_NUMBER() OVER (ORDER BY score DESC) AS position
                                        FROM portfolio.users
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
            var dev = await _context.Users
                .Include(u => u.ApiRole)
                .Include(u => u.WebRole)
                .Include(u => u.Documents)
                .Include(u => u.Place)
                .Select(u => new
                {
                    u.Id,
                    u.Score,
                    u.Surname,
                    u.Name,
                    u.Middlename,
                    u.Email,
                    u.Phone,
                    u.Place,
                    u.Documents,
                    u.WebRole,
                    u.ApiRole
                })
                .FirstOrDefaultAsync(
                a => 
                    a.Id == authedId);
            return Ok(dev);
        }
        
        // admin api
        [AuthFilter("admin")]
        [HttpGet("get30")]
        public async Task<IActionResult> Get30(int page)
        {
            var totake = 30;
            var toskip = totake * (page - 1);

            var devs = await _context.Users
                .Include(x => x.WebRole)
                .Include(x => x.Place)
                .Select(
                    x => 
                        new
                        {   
                            x.Id,
                            x.Name,
                            x.Middlename,
                            x.Surname,
                            x.Score,
                            x.WebRole,
                            x.Phone,
                            x.Email,
                            x.Place,
                        })
                .OrderByDescending(u => u.Id)
                .Skip(toskip)
                .Take(totake)
                .ToListAsync();
            return Ok(devs);
        }
        
        [AuthFilter("admin")]
        [HttpGet("pages")]
        public async Task<IActionResult> GetPages()
        {
            var users = _context.Users
                .Include(x => x.WebRole)
                .Include(x => x.Place)
                .Select(x =>
                    new
                    {
                        x.Id,
                        x.Name,
                        x.Middlename,
                        x.Surname,
                        x.Score,
                        x.WebRole,
                        x.Phone,
                        x.Email,
                        x.Place,
                    })
                .OrderByDescending(u => u.Id)
                .Count();
            var totake = 30;
            var pages = users / totake;
            return Ok(pages);
        }
    }
}
