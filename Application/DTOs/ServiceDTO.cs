using Microsoft.AspNetCore.Http;

namespace Application.DTOs;

public record ServiceDTO(int Id, string Name, string Description, IFormFile Image);