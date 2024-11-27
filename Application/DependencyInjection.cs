using Application.Services;
using Application.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ServicesService>();
<<<<<<< HEAD
=======
            services.AddScoped<CartService>();
>>>>>>> origin/master
            services.AddScoped<CategoryService>(); // Add this line
            services.AddValidatorsFromAssemblyContaining<ProductDtoValidator>();
            services.AddTransient<IAuthService, AuthService>();

            return services;
        }
    }
}