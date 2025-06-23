using dserver.Database;
using dserver.Database.Model;
using dserver.Filters;
using dshared.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dserver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MainDBContext _context;

        private const string AvatarsFolder = "avatars";
        private static string ServerAvatarsPath;
        public UsersController(MainDBContext context)
        {
            _context = context;
            ServerAvatarsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AvatarsFolder);
            if (!Directory.Exists(ServerAvatarsPath))
                Directory.CreateDirectory(ServerAvatarsPath);
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
            skip = 0;
            take = Math.Clamp(take, 10, 100);
            var users = _context.Users
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
            var dco = users.LongLength;
            for (var i = 0; i < dco; i++)
            {
                var user = users[i];
                user.Set("id", skip + (ulong)i + 1);
            }
            return Ok(users);
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
                                        FROM dpback.users
                                    ) AS ranked_users
                                    WHERE id = {0}
                            """, id)
                .Select(u => new UserRank
                {
                    Id = u.Id,
                    Score = u.Score,
                    Position = u.Position
                })
                .Cast<ListElement>()
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
                .Where(a => a.Id == authedId)
                .Select(u => new User
                {
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
                .Cast<ListElement>()
                .ToArray();
            
            var userWithRank = _context.Set<UserRank>()
                .FromSqlRaw("""
                                    SELECT id, score, position
                                    FROM (
                                        SELECT 
                                            id, 
                                            score, 
                                            ROW_NUMBER() OVER (ORDER BY score DESC) AS position
                                        FROM dpback.users
                                    ) AS ranked_users
                                    WHERE id = {0}
                            """, authedId)
                .Select(u => new UserRank
                {
                    Id = u.Id,
                    Score = u.Score,
                    Position = u.Position
                })
                .Cast<ListElement>()
                .FirstOrDefault();
            
            devs[0].Set("id", userWithRank.Get("pos"));
            
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
                .OrderBy(u => u.Surname)
                .Skip((int)skip)
                .Take((int)take)
                .Select(
                    x => 
                        new User
                        {   Id = x.Id,
                            Name = x.Name,
                            Middlename = x.Middlename,
                            Surname = x.Surname,
                            Score = x.Score,
                            WebRole = x.WebRole,
                            Phone = x.Phone,
                            Email = x.Email,
                            Place = x.Place,
                        })
                .Cast<ListElement>()
                .ToArray();

            for (var i = 0; i < take; i++)
            {
                devs[i].Set("internalId", devs[i].Get("id"));
                devs[i].Set("id", i + 1 + skip);
            }
            
            return Ok(devs);
        }
        
        [AuthFilter("admin")]
        [HttpPatch("personal/{id}")] // update profile
        public async Task<IActionResult> PatchUser(
            ulong id,
            [FromBody] ListElement li)
        {
            var user = _context.Users
                .Include(user => user.Place)
                .SingleOrDefault(u => u.Id == id);
            if (user is null)
                return Ok(false);

            user.Email = li.Get("email", user.Email);
            if (ulong.TryParse(li.Get("phone", user.Phone.ToString()), out var phone))
                user.Phone = phone;
            
            user.Name = li.Get("name", user.Name);
            user.Surname = li.Get("surname", user.Surname);
            user.Middlename = li.Get("middlename", user.Middlename);
            if (user.Place is not null)
            {
                var localPlaceId = ulong.Parse(li.Get("place_id", user.Place.Id.ToString()));
                user.Place = _context.Places.SingleOrDefault(p =>
                    p.Id == localPlaceId);
            }
            else
            {
                var localPlaceId = ulong.Parse(li.Get("place_id", "0"));
                if (localPlaceId > 0)
                {
                    user.Place = _context.Places.SingleOrDefault(p =>
                        p.Id == localPlaceId);
                }
            }

            await _context.SaveChangesAsync();
            
            return Ok(true);
        }
        
        private bool SaveAvatar(ulong documentId, MemoryStream ms)
        {
            var path = Path.Combine(ServerAvatarsPath, $"{documentId}.png");
            if (System.IO.File.Exists(path))
                return false;
            try
            {
                System.IO.File.WriteAllBytes(path, ms.ToArray());
            }
            catch
            {
                return false;
            }
            return true;
        }
        private MemoryStream GetAvatar(ulong documentId)
        {
            var memoryStream = new MemoryStream();
            var path = Path.Combine(ServerAvatarsPath, $"{documentId}.png");
            if (!System.IO.File.Exists(path))
                return memoryStream;
            FileStream? fs = null;
            try
            {
                fs = System.IO.File.OpenRead(path);
                fs.CopyTo(memoryStream);
            }
            catch
            {
                return memoryStream;
            }
            finally
            {
                memoryStream.Position = 0;
                fs?.Close();
            }
            
            return memoryStream;
        }
        
        [AuthFilter("admin")]
        [HttpPatch("avatar/{id}")] // upload avatar
        public async Task<IActionResult> UploadAvatar(
            ulong id,
            [FromBody] ListElement li)
        {
            var user = _context.Users
                .Include(user => user.Place)
                .SingleOrDefault(u => u.Id == id);
            if (user is null)
                return Ok(false);

            user.Email = li.Get("email", user.Email);
            if (ulong.TryParse(li.Get("phone", user.Phone.ToString()), out var phone))
                user.Phone = phone;
            
            user.Name = li.Get("name", user.Name);
            user.Surname = li.Get("surname", user.Surname);
            user.Middlename = li.Get("middlename", user.Middlename);
            if (user.Place is not null)
            {
                var localPlaceId = ulong.Parse(li.Get("place_id", user.Place.Id.ToString()));
                user.Place = _context.Places.SingleOrDefault(p =>
                    p.Id == localPlaceId);
            }
            else
            {
                var localPlaceId = ulong.Parse(li.Get("place_id", "0"));
                if (localPlaceId > 0)
                {
                    user.Place = _context.Places.SingleOrDefault(p =>
                        p.Id == localPlaceId);
                }
            }

            await _context.SaveChangesAsync();
            
            return Ok(true);
        }
    }
}
