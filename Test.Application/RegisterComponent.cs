using System;
using Microsoft.Extensions.DependencyInjection;
using Test.Application.Implementations;
using Test.Application.Interfaces;

namespace Test.Application
{
    public static class RegisterComponent
    {
        public static void RegisterBookingComponent(this IServiceCollection services)
        {
            services.AddTransient<ITechnicalTestService, TechnicalTestService>();
        }
    }
}
