using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DynamicPrice.Core.Data;

namespace DynamicPrice.Tests.Fixtures;

public static class TestDbHelper
{
    public static ServiceProvider CreateServiceProvider(string dbName)
    {
        var services = new ServiceCollection();
        services.AddDbContext<DynamicPriceCoreContext>(opts => opts.UseInMemoryDatabase(dbName));
        return services.BuildServiceProvider();
    }

    public static DynamicPriceCoreContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<DynamicPriceCoreContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new DynamicPriceCoreContext(options);
    }

    public static string NewDbName(string prefix = "TestDb") => prefix + Guid.NewGuid();
}
