namespace Application.DTOs;

public class UserInfoDTO
{
    public string LastName {get; set;}
    public string FirstName {get; set;}
    public string DisplayName {get; set;}
    public string Email {get; set;}
}

public class UserInfoDashboardDTO
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public DateTime JoinDate { get; set; }
    public bool IsLockedOut { get; set; }
    public int? LastOrderId { get; set; }
}