using Application.Services;
using Application.Services.CategorizingModels;
using Application.Validators;
using Domain.Models.ShopModels;
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
            services.AddScoped<CartService>();
            services.AddScoped<CategoryService>();
            services.AddScoped<FrameService>();
            services.AddScoped<MaterialService>();
            services.AddScoped<SizeService>();
            services.AddScoped<ProductReviewsService>();
            services.AddScoped<WishlistService>();
            services.AddScoped<AddressService>();
            services.AddScoped<OrderingService>();
            services.AddValidatorsFromAssemblyContaining<ProductDtoValidator>();
            services.AddTransient<IAuthService, AuthService>();

            return services;
        }
    }
}