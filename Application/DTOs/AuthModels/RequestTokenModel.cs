namespace Application.DTOs.AuthModels
{
    public class RequestTokenModel
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}