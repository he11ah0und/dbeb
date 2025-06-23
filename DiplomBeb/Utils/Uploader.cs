using System.Net.Http.Headers;
using DiplomBeb.Services.Client;
using DiplomBeb.Utils.Networking;
using Microsoft.AspNetCore.Components.Forms;

namespace DiplomBeb.Utils;

public class Uploader
{
    public long MaxFileSize { get; set; }
    public string DefaultFileData { get; set; }
    public string DocumentName { get; set; }
    
    public Callbacker<Func<Task>> OnUploaded { get; set; }
    
    private IBrowserFile SelectedFile  { get; set; }
    private NotificationService _notificationService { get; set; }
    private BackendHttpClient _http { get; set; }
    public string FileData { get; private set; }
    
    public Uploader(BackendHttpClient http, NotificationService nots, string defaultFileData, long maxFileSize = 10 * 1024 * 1024)
    {
        _http = http;
        _notificationService = nots;
        DefaultFileData = defaultFileData;
        MaxFileSize = maxFileSize;
        FileData = DefaultFileData;
        OnUploaded = new (false);
    }
    private Stream? OpenReadFileStream()
    {
        try
        {
            return SelectedFile.OpenReadStream(MaxFileSize);
        }
        catch
        {
            return null;
        }
    }
    public async Task OnFileSelect(InputFileChangeEventArgs obj)
    {
        var file = obj.File;
        if (file.ContentType != "image/png"
            || file.Size > MaxFileSize)
        {
            _notificationService.AddNotification("Неподходящий файл.");
            return;
        }
        SelectedFile = file;
        
        var stream = OpenReadFileStream();
        if (stream is null)
        {
            FileData = DefaultFileData;
            _notificationService.AddNotification($"Ошибка открытия файла \"{file.Name}\".");
            return;
        }
        var buffer = new byte[stream.Length];
        await stream.ReadExactlyAsync(buffer);
        FileData = $"data:image;base64,{Convert.ToBase64String(buffer)}";
    }

    public async Task Upload()
    {
        if (string.IsNullOrEmpty(DocumentName))
        {
            _notificationService.AddNotification("Укажите имя файла!");
            return;
        }
        
        var stream = OpenReadFileStream();
        if (stream is null)
        {
            _notificationService.AddNotification("Неудалось прочитать файл!");
            return;
        }
        
        using var fileStreamContent = new StreamContent(stream);
        fileStreamContent.Headers.ContentType =
            new MediaTypeHeaderValue(SelectedFile.ContentType);

        using var client = new HttpClient();
        using var formData = new MultipartFormDataContent();
        
        formData.Add(fileStreamContent, "File", SelectedFile.Name);
        formData.Add(new StringContent(DocumentName), "Name");
        
        var res = await _http.Request(
            "Documents/up",
            formData,
            HttpMethod.Post);
        if (!res.IsOk)
        {
            _notificationService.AddNotification("Ошибка загрузки файла на сервер!");
            return;
        }
        await OnUploaded.InvokeAllAsync();
    }
}