using System.Linq.Expressions;
using System.Threading.Channels;
using dserver.Database;
using dserver.Database.Model;
using dserver.Filters;
using dserver.Services;
using dshared.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;

namespace dserver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly MainDBContext _context;
        private readonly Channel<ScoreParameters> _channel;
        
        
        private const string DocumentsFolder = "docs";
        private static string ServerDocsPath;

        public DocumentsController(MainDBContext context, Channel<ScoreParameters> channel)
        {
            _context = context;
            _channel = channel;
            ServerDocsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DocumentsFolder);
            if (!Directory.Exists(ServerDocsPath))
                Directory.CreateDirectory(ServerDocsPath);
        }
        public static bool RotateImage(ulong documentId, float angle)
        {
            var path = Path.Combine(ServerDocsPath, $"{documentId}.png");
            if (!System.IO.File.Exists(path))
                return false;
            
            // Load the original image
            var imageData = System.IO.File.ReadAllBytes(path);
            using var original = SKBitmap.Decode(imageData);
            int width = original.Width;
            int height = original.Height;

            // Вычисляем новый размер холста
            double radians = angle * Math.PI / 180.0;
            double cos = Math.Abs(Math.Cos(radians));
            double sin = Math.Abs(Math.Sin(radians));
            int newWidth = (int)Math.Ceiling(width * cos + height * sin);
            int newHeight = (int)Math.Ceiling(width * sin + height * cos);

            // Создаём новый битмап нужного размера
            using var rotatedBitmap = new SKBitmap(newWidth, newHeight);
            using var canvas = new SKCanvas(rotatedBitmap);
            canvas.Clear(SKColors.Transparent); // Прозрачный фон

            // Центрируем изображение и поворачиваем
            canvas.Translate(newWidth / 2f, newHeight / 2f);
            canvas.RotateDegrees(angle);
            canvas.Translate(-width / 2f, -height / 2f);

            // Рисуем изображение
            canvas.DrawBitmap(original, 0, 0);

            // Сохраняем результат
            using var image = SKImage.FromBitmap(rotatedBitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            System.IO.File.WriteAllBytes(path, data.ToArray());
            return true;
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

            List<ListElement> docs = [];
            if (filters.LongLength == 0)
            {
                 var q1 = _context.ModeratedDocuments
                    .Include(md => md.Document)
                    .Include(md => md.Document.DocumentType)
                    .Include(md => md.Document.User)
                    .Where(md => md.Document.User.Id == authedUser.Id)
                    .OrderBy(md => md.Document.Id)
                    .Skip((int)skip)
                    .Take((int)take)
                    .ToArray();

                var moderatedDocuments = q1.ToArray();
                var mdco = moderatedDocuments.LongLength;
                for (var i = 0; i < mdco; i++)
                {
                    var md = moderatedDocuments[i];
                    var doc = (ListElement)md.Document;
                    if (md.State)
                        doc.Set("cost", md.Document.DocumentType.Cost);
                    doc.Set("moderated", true);
                    doc.Set("state", md.State);
                    docs.Add(doc);
                }

                var docsf = Array.ConvertAll(_context.Documents
                        .Include(d => d.User)
                        .Include(d => d.DocumentType)
                        .Where(d => d.User.Id == authedUser.Id)
                        .OrderBy(d => d.Id)
                        .Skip((int)skip)
                        .Take((int)take)
                        .AsEnumerable()
                        .Except(moderatedDocuments.Select(md => md.Document))
                        .ToArray()
                    , input => (ListElement)input);
                
                docs.AddRange(docsf);
            }
            else
            {
                var q1 = _context.ModeratedDocuments
                    .Include(md => md.Document)
                    .Include(md => md.Document.DocumentType)
                    .Where(md => md.Document.User.Id == authedUser.Id 
                                 && filters.Contains(md.Document.DocumentType.Name))
                    .OrderBy(md => md.Document.Id)
                    .Skip((int)skip)
                    .Take((int)take);
                
                var moderatedDocuments = q1.ToArray();
                var mdco = moderatedDocuments.LongLength;
                for (var i = 0; i < mdco; i++)
                {
                    var md = moderatedDocuments[i];
                    var doc = (ListElement)md.Document;
                    if (md.State)
                        doc.Set("cost", md.Document.DocumentType.Cost);
                    doc.Set("moderated", true);
                    doc.Set("state", md.State);

                    docs.Add(doc);
                }
                
                var docsf = Array.ConvertAll(_context.Documents
                        .Include(d => d.User)
                        .Include(d => d.DocumentType)
                        .Where(d => d.User.Id == authedUser.Id
                                    && filters.Contains(d.DocumentType.Name))
                        .OrderBy(d => d.Id)
                        .Skip((int)skip)
                        .Take((int)take)
                        .AsEnumerable()
                        .Except(moderatedDocuments.Select(md => md.Document))
                        .ToArray()
                    , input => (ListElement)input);
                
                docs.AddRange(docsf);
            }

            return Ok(docs
                .ToArray());
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
            
            var modDocs = _context.ModeratedDocuments
                .Include(md => md.Document)
                .ThenInclude(d => d.User)
                .Where(md => md.Document.User.Id == authedUser.Id && documentIds.Contains(md.Document.Id))
                .ToArray();
            
            _context.Documents.RemoveRange(docs);
            _context.ModeratedDocuments.RemoveRange(modDocs);

            await _context.SaveChangesAsync();

            var dco = docs.LongLength;
            for (var i = 0; i < dco; i++)
                RemoveDocument(docs[i].Id);
            
            await _channel.Writer.WriteAsync(new ScoreParameters(authedId), HttpContext.RequestAborted);
            
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
        public const long MaxFileSize = 10 * 1024 * 1024;
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
                .Include(user => user.ApiRole)
                .SingleOrDefault(a => a.Id == authedId);
            if (authedUser == null)
                return NoContent();
            
            var doc = _context.Documents
                .Include(d => d.User)
                .SingleOrDefault(d => d.Id == docId);
            if (doc == null || (doc.User.Id != authedId && !authedUser.ApiRole.IsMoreUser()))
                return NoContent();
            
            var documentStream = GetDocument(doc.Id);
            
            if (documentStream.Length == 0)
                return Ok();
            
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
