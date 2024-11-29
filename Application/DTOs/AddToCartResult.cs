namespace Application.DTOs;

public class AddToCartResult
{
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
}