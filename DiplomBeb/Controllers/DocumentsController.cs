using DiplomBeb.Database;
using DiplomBeb.Database.Model;
using DiplomBeb.Filters;
using DiplomBeb.Utils.AdaptiveListing;
using DiplomBeb.Utils.Networking;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomBeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly MainDBContext _context;
        private readonly ApiService _apiService;
        
        private const string DocumentsFolder = "docs";
        private string ServerDocsPath;

        public DocumentsController(MainDBContext context, ApiService apiService)
        {
            _context = context;
            _apiService = apiService;
            ServerDocsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DocumentsFolder);
            if (!Directory.Exists(ServerDocsPath))
                Directory.CreateDirectory(ServerDocsPath);
        }

        private bool SaveDocument(ulong documentId, MemoryStream ms)
        {
            var path = Path.Combine(ServerDocsPath, $"{documentId}.png");
            if (System.IO.File.Exists(path))
                return false;

            System.IO.File.WriteAllBytes(path, ms.ToArray());
            
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
                fs?.Close();
            }
            return memoryStream;
        }
        
        // user api
        [AuthFilter("user")]
        [HttpGet("{skip}/{take}")]
        public async Task<IActionResult> Docs(ulong skip, ulong take)
        {
            var authedId = ulong.Parse((string)HttpContext.Items["authed.id"]);
            var authedUser = _context.Users
                .SingleOrDefault(a => a.Id == authedId);
            
            if (authedUser == null)
                return NoContent();

            var docs = _context.Documents
                .Include(d => d.User)
                .Where(d => d.User.Id == authedUser.Id)
                .Skip((int)skip)
                .Take((int)take)
                .Cast<ListElement>()
                .ToArray();
            
            foreach (var listElement in docs)
                listElement.Set("link", Path.Combine(
                    _apiService.Options.Url,
                    $"Documents/down/{listElement.Get("id")}"));

            return Ok(docs);
        }
        
        [AuthFilter("user")]
        [HttpGet("total")]
        public async Task<IActionResult> Total()
        {
            var authedId = ulong.Parse((string)HttpContext.Items["authed.id"]);
            var authedUser = _context.Users
                .SingleOrDefault(a => a.Id == authedId);
            
            if (authedUser == null)
                return NoContent();

            var docs = _context.Documents
                .Include(d => d.User)
                .Count(d => d.User.Id == authedUser.Id);

            return Ok(docs);
        }
        
        [AuthFilter("user")]
        [HttpPost("up")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var authedId = ulong.Parse((string)HttpContext.Items["authed.id"]);
            var authedUser = _context.Users
                .SingleOrDefault(a => a.Id == authedId);
            if (authedUser == null)
                return Ok(false);
            
            if (file.Length == 0
                || !file.FileName.EndsWith(".png")
                || !file.ContentType.Contains("image/png"))
                return NoContent();
            var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            _context.Documents.Add(new Document()
            {
                User = authedUser,
            });

            await _context.SaveChangesAsync();
            
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
            
            var result = new FileStreamResult(documentStream, "image/png");
            
            return Ok(result);
        }
        
        // admin api
    }
}
