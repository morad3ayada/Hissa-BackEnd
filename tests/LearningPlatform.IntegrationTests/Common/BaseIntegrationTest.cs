using LearningPlatform.IntegrationTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace LearningPlatform.IntegrationTests.Common;

public abstract class BaseIntegrationTest : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private readonly IServiceScope _scope;

    protected readonly HttpClient Client;
    protected readonly IServiceProvider Services;

    protected BaseIntegrationTest(CustomWebApplicationFactory factory)
    {
        Client = factory.CreateClient();
        _scope = factory.Services.CreateScope();
        Services = _scope.ServiceProvider;
    }

    public void Dispose()
    {
        _scope.Dispose();
        GC.SuppressFinalize(this);
    }
}
