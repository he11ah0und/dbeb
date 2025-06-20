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
    public class DocumentsController : ControllerBase
    {
        private readonly MainDBContext _context;
        
        private const string DocumentsFolder = "docs";
        private string ServerDocsPath;

        public DocumentsController(MainDBContext context)
        {
            _context = context;
            ServerDocsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DocumentsFolder);
            if (!Directory.Exists(ServerDocsPath))
                Directory.CreateDirectory(ServerDocsPath);
        }

        private bool SaveDocument(ulong documentId, MemoryStream ms)
        {
            var path = Path.Combine(ServerDocsPath, $"{documentId}.png");
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

        private bool RemoveDocument(ulong documentId)
        {
            var path = Path.Combine(ServerDocsPath, $"{documentId}.png");
            if (!System.IO.File.Exists(path))
                return false;
            try
            {
                System.IO.File.Delete(path);
            }
            catch
            {
                return false;
            }
            return true;
        }
        private MemoryStream GetDocument(ulong documentId)
        {
            var memoryStream = new MemoryStream();
            var path = Path.Combine(ServerDocsPath, $"{documentId}.png");
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
        
        // user api
        [AuthFilter("user")]
        [HttpPost("{skip}/{take}")]
        public async Task<IActionResult> Docs(ulong skip, ulong take, [FromBody] string[] filters)
        {
            var authedId = ulong.Parse((string)HttpContext.Items["authed.id"]);
            var authedUser = _context.Users
                .SingleOrDefault(a => a.Id == authedId);
            
            if (authedUser == null)
                return NoContent();

            ListElement[] docs;
            if (filters.LongLength == 0)
            {
                docs = _context.Documents
                    .Include(d => d.User)
                    .Where(d => d.User.Id == authedUser.Id)
                    .Skip((int)skip)
                    .Take((int)take)
                    .Cast<ListElement>()
                    .ToArray();
            }
            else
            {
                docs = _context.Documents
                    .Include(d => d.DocumentType)
                    .Include(d => d.User)
                    .Where(d => d.User.Id == authedUser.Id && filters.Contains(d.DocumentType.Name))
                    .Skip((int)skip)
                    .Take((int)take)
                    .Cast<ListElement>()
                    .ToArray();
            }

            return Ok(docs);
        }
        
        [AuthFilter("user")]
        [HttpPost("remove")]
        public async Task<IActionResult> Remove([FromBody] ulong[] documentIds)
        {
            var authedId = ulong.Parse((string)HttpContext.Items["authed.id"]);
            var authedUser = _context.Users
                .SingleOrDefault(a => a.Id == authedId);
            
            if (authedUser == null)
                return Ok(false);

            var docs = _context.Documents
                .Include(d => d.User)
                .Where(d => d.User.Id == authedUser.Id && documentIds.Contains(d.Id))
                .ToArray();
            
            _context.RemoveRange(docs);

            await _context.SaveChangesAsync();

            var dco = docs.LongLength;
            for (var i = 0; i < dco; i++)
                RemoveDocument(docs[i].Id);
            
            return Ok(true);
        }
        
        [AuthFilter("user")]
        [HttpPost("total")]
        public async Task<IActionResult> Total([FromBody] string[] filters)
        {
            var authedId = ulong.Parse((string)HttpContext.Items["authed.id"]);
            var authedUser = _context.Users
                .SingleOrDefault(a => a.Id == authedId);
            
            if (authedUser == null)
                return NoContent();

            int count;

            if (filters.LongLength == 0)
            {
                count = _context.Documents
                    .Include(d => d.User)
                    .Count(d => d.User.Id == authedId);
            }
            else
            {
                count = _context.Documents
                    .Include(d => d.DocumentType)
                    .Include(d => d.User)
                    .Count(d => d.User.Id == authedId && filters.Contains(d.DocumentType.Name));
            }
            return Ok(count);
        }
        private long MaxFileSize { get; set; } = 10 * 1024 * 1024;
        [AuthFilter("user")]
        [HttpPost("up")]
        public async Task<IActionResult> Upload([FromForm]string name, IFormFile file)
        {
            var authedId = ulong.Parse((string)HttpContext.Items["authed.id"]);
            var authedUser = _context.Users
                .SingleOrDefault(a => a.Id == authedId);
            if (authedUser == null)
                return Ok(false);
            
            if (file.Length == 0
                || !file.FileName.EndsWith(".png")
                || !file.ContentType.Contains("image/png")
                || file.Length >= MaxFileSize)
                return Ok(false);
            
            var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            var doc = new Document()
            {
                User = authedUser,
                Name = name,
                Publish_date = DateTime.UtcNow,
            };
            _context.Documents.Add(doc);

            await _context.SaveChangesAsync();

            if (!SaveDocument(doc.Id, ms))
                return Ok(false);
            
            return Ok(true);
        }

        [AuthFilter("user")]
        [HttpGet("down/{docId}")]
        public async Task<IActionResult> Download(ulong docId)
        {
            var authedId = ulong.Parse((string)HttpContext.Items["authed.id"]);
            var authedUser = _context.Users
                .SingleOrDefault(a => a.Id == authedId);
            if (authedUser == null)
                return NoContent();
            
            var doc = _context.Documents
                .Include(d => d.User)
                .SingleOrDefault(d => d.Id == docId && d.User.Id == authedUser.Id);
            if (doc == null)
                return NoContent();
            
            var documentStream = GetDocument(doc.Id);
            
            if (documentStream.Length == 0)
                return NoContent();
            
            return File(documentStream, "image/png");
        }
        
        [AuthFilter("user")]
        [HttpGet("total_types")]
        public async Task<IActionResult> TotalFilters()
        {
            return Ok(_context.DocumentTypes.Count());
        }
        
        [AuthFilter("user")]
        [HttpGet("types/{skip}/{take}")]
        public async Task<IActionResult> GetTypes(ulong skip, ulong take)
        {
            var elements = _context.DocumentTypes
                .Skip((int)skip)
                .Take((int)take)
                .Cast<ListElement>()
                .ToArray();
            return Ok(elements);
        }
        
        // admin api
    }
}
