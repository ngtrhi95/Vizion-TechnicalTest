using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Test.Domain.Entities;

namespace Test.Domain
{
    public static class RegisterComponent
    {
        public static void RegisterDBContext(this IServiceCollection services, IConfiguration Configuration)
        {
            services.Configure<TechnicalTestDatabaseSetting>(Configuration.GetSection("StoreDatabase"));
            services.Configure<BlobStorageSetting>(Configuration.GetSection("BlobStorage"));
        }
    }
}
