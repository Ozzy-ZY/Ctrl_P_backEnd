using Microsoft.EntityFrameworkCore;

namespace Domain.Models
{
    [Owned]
    public class RefreshToken
    {
        public string Token {get; set; }
        public DateTime ExpiresOn { get; set; }
        public bool IsExpired => DateTime.Now >= ExpiresOn;
        public DateTime CreatedAt {get; set; } = DateTime.Now;
        public DateTime? RevokedOn { get; set; }
        public bool IsActive => RevokedOn == null && !IsExpired;
    }
}
