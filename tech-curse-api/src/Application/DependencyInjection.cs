using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using tech_curse_api.src.Application.Factory;
using tech_curse_api.src.Application.Interfaces;
using tech_curse_api.src.Application.Strategies;
using tech_curse_api.src.Application.Common.Behaviors;

namespace tech_curse_api.src.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IPaymentStrategy, CreditCardPaymentStrategy>();
            services.AddScoped<PaymentStrategyFactory>();

            // Configurar MediatR e FluentValidation para a migração CQRS
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
