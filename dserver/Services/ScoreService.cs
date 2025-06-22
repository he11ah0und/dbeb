using System.Threading.Channels;
using dserver.Database;
using dserver.Database.Model;
using Microsoft.EntityFrameworkCore;

namespace dserver.Services;
public record ScoreParameters(ulong UserId);
public class ScoreService : BackgroundService
{
    private readonly Channel<ScoreParameters> _channel;
    private IServiceScopeFactory _serviceScopeFactory;
    
    public ScoreService(Channel<ScoreParameters> channel, IServiceScopeFactory serviceScopeFactory)
    {
        _channel = channel;
        _serviceScopeFactory = serviceScopeFactory;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var parameters in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            await ProcessTaskAsync(parameters, stoppingToken);
        }
    }
    
    private async Task ProcessTaskAsync(ScoreParameters parameters, CancellationToken ct)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var _context = scope.ServiceProvider.GetRequiredService<MainDBContext>();
            
        var mds = _context.ModeratedDocuments
            .Include(md => md.Document)
            .Include(md => md.Document.User)
            .Include(md => md.Document.DocumentType)
            .Where(md => md.Document.User.Id == parameters.UserId)
            .AsAsyncEnumerable();
        ulong totalScore = 0;
        await foreach (var md in mds)
        {
            if (md.Document.DocumentType is not DocumentType dt)
                continue;
            totalScore += dt.Cost;
        }
        var user = _context.Users.SingleOrDefault(u => u.Id ==  parameters.UserId);
        if (user is null)
            return;
        
        user.Score = totalScore;
        
        await _context.SaveChangesAsync(ct);
    }
}