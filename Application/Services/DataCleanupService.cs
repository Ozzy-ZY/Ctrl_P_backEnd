using Domain.Models;
using Infrastructure.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application.Services;

public class DataCleanupService : IHostedService, IAsyncDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private Timer _timer;

    public DataCleanupService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        async void Callback(object? _) => await Cleanup();

        _timer = new Timer(Callback, null, TimeSpan.Zero, TimeSpan.FromDays(5));
        return Task.CompletedTask;
    }

    private async Task Cleanup()
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var date = DateTime.Now;
        var users = await unitOfWork.Users
            .GetAllAsync();
        foreach (var user in users)
        {
            var expiredTokens = user.RefreshTokens?.Where(t => t.ExpiresOn < date)?? new List<RefreshToken>();
            foreach (var expiredToken in expiredTokens)
            {
                user.RefreshTokens?.Remove(expiredToken);
            }
        }
        await unitOfWork.CommitAsync();
    }
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await _timer.DisposeAsync();
    }
}