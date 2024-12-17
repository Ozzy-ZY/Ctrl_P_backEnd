using System.Text.Json.Serialization;

namespace Domain.Models;

public class Message
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string ContactEmail {get; set;}
    public string Subject {get; set;}
    public string Content {get; set;}
    [JsonIgnore]
    public virtual AppUser User { get; set; }
}