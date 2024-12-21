# Ctrl+P
> [!IMPORTANT]
> When in Development please include a File Called appsettings.json to your API Project that should contain similar Data to this
```
{
  "Logging": {
    "LogLevel": {
     "Default": "Information",
     "Microsoft.AspNetCore": "Warning"
   }
 },
  "Jwt": {
    "SecretKey": "A json Secret",
    "ValidAudience": "doesn't matter",
    "ValidIssuer": "your url",
      "AdminExpiryMinutes": 2,
       "UserExpiryMinutes": 1,
        "RefreshTokenExpiryHours": 1
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
   "DefaultConnection": "Data Source=Your DB server;Initial Catalog=Ctrl_P;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False"
  },
  "Stripe": {
    "SecretKey": "",
    "PublishableKey": "",
    "WebhookSecret": ""
  }
}
```
(Printing Solutions Agency)

**Project overview:** 

Control P is a modern printing agency dedicated to providing high-quality printing services both online and offline. With a focus on customer satisfaction, Control P aims to revolutionize the printing experience in Riyadh, Saudi Arabia, by offering a user-friendly online shop and a welcoming local store. Our goal is to cater to both individual and business clients with a wide range of customizable printing products, from business cards and brochures to banners and promotional materials.

**Functional Requirements:**

1. Admin Dashboard to Control the store:
- CRUD Operations on products and services.
- Order delivery tracking.
- Account locking mechanism.
- See customer messages.

1. User Interface to Access and Use the Store:
- Responsive interface on ALL devices.
- Animations.
- UI/UX design.

1. The functionality to Reserve Services and Buy Products:
- User can request custom orders.
- Buy products from stock.
- Online payment (PayPal, Bank transfer, POD).
- Watch your order progress.

1. User Authentication:
- Authenticate using username, Email, password.
- Authenticate using An External Service (Google, Facebook).
- Forgot Password Mitigation Mechanism. 

1. Profile Management:
- Add Addresses and Edit their Personal Data.

1. Cart and Wishlist Functionality:
- Add Products to the Cart or Wishlist.
- Make Orders out of Cart and add Products from Wishlist to cart.


Team Members and Roles:

1. **Project Manager:** Seif Ayman.
1. **System Architect:** Zyad Mohamed.
1. **Developer:** 
- Adel Saudi – Front-end developer.
- Salem Fathy – Front-end developer.
- Seif Omar – Back-end developer.
