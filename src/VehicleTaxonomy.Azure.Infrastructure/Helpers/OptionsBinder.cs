using Microsoft.Extensions.Configuration;

namespace VehicleTaxonomy.Azure.Infrastructure;

public static class OptionsBinder
{
    /// <summary>
    /// Creates and binds a new <typeparamref name="TOptions"/> instance
    /// using the specified <paramref name="configuration"/>. Useful for
    /// binding options during startup before the DI container is configured.
    /// </summary>
    public static TOptions Bind<TOptions>(IConfiguration configuration, string sectionName)
        where TOptions : class, new()
    {
        var section = configuration.GetSection(sectionName);
        var options = new TOptions();
        section.Bind(options);

        return options;
    }
}
