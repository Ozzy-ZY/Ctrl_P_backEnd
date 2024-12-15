namespace Application.DTOs;

public record AddressUpdateDTO(
    int Id,
    string AddressText,
    string BillingAddress,
    string FullName,
    string? CompanyName,
    string Country,
    string City,
    string State,
    string ZipCode,
    string Phone,
    string StreetAddress,
    string? Note);