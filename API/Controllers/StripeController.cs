using System.Security.Claims;
using Application.DTOs;
using Application.Services;
using Domain.Models;
using Domain.StaticData;
using Infrastructure.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using ProductService = Stripe.ProductService;

namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class StripeController: ControllerBase
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly OrderingService _orderingService;
    private readonly CartService _cartService;
    private readonly ILogger<StripeController> _logger;
    private readonly IConfiguration _configuration;
    public StripeController( IUnitOfWork unitOfWork, OrderingService orderingService, ILogger<StripeController> logger,IConfiguration configuration, CartService cartService)
    {
        _unitOfWork = unitOfWork;
        _orderingService = orderingService;
        _logger = logger;
        _configuration = configuration;
        _cartService = cartService;
    }

    [HttpPost("Pay")]
    [Authorize(Roles = StaticData.UserRole)]
    public async Task<ActionResult> Pay()
    {
        //Console.WriteLine($"Stripe API Key: {StripeConfiguration.ApiKey}");
        string successfulUrl = Request.Headers["SuccessURL"];
        string failedUrl = Request.Headers["FailedURL"];
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
        SessionCreateOptions options = new SessionCreateOptions()
        {
            PaymentMethodTypes = ["card"],
            ClientReferenceId = userId.ToString(),
            LineItems = new List<SessionLineItemOptions>()
            {
                new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "SAR",
                        UnitAmount = (long?)cart.TotalPrice * 100,
                        ProductData = new SessionLineItemPriceDataProductDataOptions()
                        {
                            Name = "Cart Purchase",
                        }
                    },
                    Quantity = 1,
                },
            },
            Mode = "payment",
            SuccessUrl = successfulUrl??"https://control-p2.vercel.app/",
            CancelUrl = failedUrl?? "https://control-p2.vercel.app/",
        };
        var service = new SessionService();
        //Console.WriteLine($"Stripe API Key: {StripeConfiguration.ApiKey}");
        Session session = await service.CreateAsync(options);
        return Ok(new { Url = session.Url });
    }

[HttpPost("Strip-Webhook")]
public async Task<ActionResult> StripWebhook()
{
    _logger.LogInformation("Received Stripe webhook.");
    foreach (var header in Request.Headers)
    {
        _logger.LogInformation(header.Key + ": " + header.Value);
    }
    var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

    try
    {
        _logger.LogInformation("Parsing the Stripe event.");
        var stripEvent = EventUtility.ConstructEvent(
            json, 
            Request.Headers["Stripe-Signature"], 
            _configuration["Stripe:WebhookSecret"]
        );

        if (stripEvent.Type == EventTypes.CheckoutSessionCompleted)
        {
            _logger.LogInformation("Checkout session completed event detected.");
            var session = stripEvent.Data.Object as Session;
            var userId = int.Parse(session?.ClientReferenceId!);

            _logger.LogInformation("Fetching cart for user ID {UserId}.", userId);
            var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);

            if (cart == null)
            {
                _logger.LogWarning("No cart found for user ID {UserId}.", userId);
                return BadRequest(new { error = "No cart found" });
            }

            _logger.LogInformation("Creating order for user ID {UserId}.", userId);
            var result = await _orderingService.CreateOrderFromCart(userId, StaticData.BankTransfer);

            if (!result.Success)
            {
                _logger.LogError("Failed to create order for user ID {UserId}.", userId);
                return BadRequest($"{result.Errors.FirstOrDefault()}");
            }

            _logger.LogInformation("Order successfully created for user ID {UserId}.", userId);
            if (await _cartService.EmptyTheCart(userId) == false)
            {
                _logger.LogError("couldn't empty the cart for user ID {UserId}.", userId);
                return BadRequest(new { error = "couldn't empty the cart" });
            }
            await _unitOfWork.CommitAsync();
        }

        _logger.LogInformation("Stripe webhook processed successfully.");
        return Ok();
    }
    catch (StripeException ex)
    {
        _logger.LogError(ex, "StripeException occurred while processing the webhook.");
        return BadRequest(new { error = ex.Message });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An unexpected error occurred while processing the Stripe webhook.");
        return StatusCode(500, new { error = "Internal server error" });
    }
}

}