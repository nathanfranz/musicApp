using Microsoft.Extensions.DependencyInjection;
using Music.DL.Interfaces;
using Music.DL.Repos;

namespace Music.DL;

public static class RegisterAssembly
{
    public static IServiceCollection RegisterDl(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<AppleMusicRepo>();

        serviceCollection.AddHttpClient(AppleMusicRepo.ClientName, client =>
        {
            client.BaseAddress = new Uri(AppleMusicRepo.ClientBaseUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        serviceCollection.AddScoped<IMusicRepoFactory, MusicRepoFactory>();

        return serviceCollection;
    }
}
