using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using tech_curse_api.src.Application.Factory;
using tech_curse_api.src.Application.Interfaces;
using tech_curse_api.src.Application.Services;
using tech_curse_api.src.Application.Strategies;

namespace tech_curse_api.src.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();
            services.AddScoped<IPaymentService, PaymentService>();

            services.AddScoped<IPaymentStrategy, CreditCardPaymentStrategy>();
            services.AddScoped<PaymentStrategyFactory>();

            // Configurar MediatR e FluentValidation para a migração CQRS
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(tech_curse_api.src.Application.Common.Behaviors.ValidationBehavior<,>));

            return services;
        }
    }
}
