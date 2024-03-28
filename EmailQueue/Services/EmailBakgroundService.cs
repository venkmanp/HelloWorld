namespace EmailQueue.Services;

public class EmailBakgroundService : BackgroundService
{
    private 
    public EmailBakgroundService(IServiceProvider provider)
    {
        _provider = provider;
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}
