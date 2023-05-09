using Microsoft.Extensions.DependencyInjection;
using WordRepeaterBot.Application.Services;

namespace WordRepeaterBot.Application.Extensions;

public static class AddApplicationServicesExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ISettingsService, SettingsService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPhraseService, PhraseService>();

        return services;
    }
}
