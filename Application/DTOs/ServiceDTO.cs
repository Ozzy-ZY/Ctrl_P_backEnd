using Microsoft.AspNetCore.Http;

namespace Application.DTOs;

public record ServiceDTO(string Name, string Description, IFormFile Image);