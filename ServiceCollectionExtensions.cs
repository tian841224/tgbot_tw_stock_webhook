public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLazyScoped<TService>(this IServiceCollection services)
        where TService : class
    {
        services.AddScoped<TService>();
        services.AddScoped(provider => new Lazy<TService>(() => 
            provider.GetRequiredService<TService>()));
        return services;
    }

    public static IServiceCollection AddLazyScoped<TService, TImplementation>(
        this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        services.AddScoped<TService, TImplementation>();
        services.AddScoped(provider => new Lazy<TService>(() => 
            provider.GetRequiredService<TService>()));
        return services;
    }
}