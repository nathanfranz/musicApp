using Microsoft.Extensions.DependencyInjection;
using Music.BL.Interfaces;
using Music.BL.Services;
using Music.DL;

namespace Music.BL;

public static class RegisterAssembly
{
    public static IServiceCollection RegisterBl(this IServiceCollection serviceCollection)
    {
        serviceCollection.RegisterDl();
        
        serviceCollection.AddScoped<AppleMusicService>();
        serviceCollection.AddScoped<IMusicServiceFactory, MusicServiceFactory>();

        return serviceCollection;
    }
}
