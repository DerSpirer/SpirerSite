using Backend.Api.Models.Email;

namespace Backend.Api.Services.EmailService;

public interface IEmailService
{
    Task<bool> LeaveMessage(LeaveMessageToolParams messageParams);
}