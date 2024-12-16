using System.Security.Claims;
using Application.DTOs;
using Domain.Models;
using Domain.StaticData;
using Infrastructure.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class StripController: ControllerBase
{

    private readonly IUnitOfWork _unitOfWork;

    public StripController(IOptions<StripModel> model,
        ProductService productService,
        CustomerService customerService, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpPost("Pay")]
    [Authorize(Roles = StaticData.UserRole)]
    public async Task<ActionResult> Pay()
    {
        //Console.WriteLine($"Stripe API Key: {StripeConfiguration.ApiKey}");
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);
        if (cart == null)
        {
            return BadRequest(new ServiceResult()
            {
                Success = false,
                Errors = new List<string>(){"Cart is Empty"}
            });
        }
        var options = new SessionCreateOptions()
        {
            LineItems = new List<SessionLineItemOptions>()
            {
                new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "SAR",
                        UnitAmountDecimal = cart.TotalPrice,
                        ProductData = new SessionLineItemPriceDataProductDataOptions()
                        {
                            Name = "Cart Purchase",
                        }
                    },
                    Quantity = cart.CartItems.Count
                },
            },
            Mode = "payment",
            SuccessUrl = "https://www.google.com",
            CancelUrl = "https://os.phil-opp.com/",
        };
        var service = new SessionService();
        //Console.WriteLine($"Stripe API Key: {StripeConfiguration.ApiKey}");
        Session session = await service.CreateAsync(options);
        return Ok(new { Url = session.Url });
    }
}