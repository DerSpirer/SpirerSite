using System.Net;
using System.Net.Mail;
using Newtonsoft.Json;
using Backend.Api.Models.Email;
using Backend.Api.Enums;

namespace Backend.Api.Services.EmailService;

/// <summary>
/// Service for sending emails via Gmail SMTP
/// </summary>
public class GmailService : IEmailService, IDisposable
{
    private readonly ILogger<GmailService> _logger;
    private readonly SmtpClient _smtpClient;
    private readonly string _inboxEmail;

    public GmailService(
        IConfiguration configuration,
        ILogger<GmailService> logger)
    {
        _logger = logger;
        
        _inboxEmail = configuration[VaultKey.GmailInboxEmail]
            ?? throw new InvalidOperationException("Gmail inbox email is missing");
        
        string appPassword = configuration[VaultKey.GmailAppPassword]
            ?? throw new InvalidOperationException("Gmail app password is missing");
        
        _smtpClient = new SmtpClient("smtp.gmail.com", 587)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(_inboxEmail, appPassword)
        };
        
        _logger.LogInformation("GmailService initialized with inbox: {InboxEmail}", _inboxEmail);
    }

    public async Task<bool> LeaveMessage(LeaveMessageToolParams messageParams)
    {
        bool success = false;

        try
        {
            _logger.LogInformation("Leaving message with parameters: {MessageParams}", JsonConvert.SerializeObject(messageParams));
            
            using MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(_inboxEmail),
                To = { new MailAddress(_inboxEmail) },
                Subject = $"New message from {messageParams.fromName} - {messageParams.subject}",
                Body = @$"You have received a new message from {messageParams.fromName} ({messageParams.fromEmail}):

Subject:
{messageParams.subject}

Message:
{messageParams.body}",
                IsBodyHtml = false,
            };
            
            await _smtpClient.SendMailAsync(mailMessage);
            success = true;
            
            _logger.LogInformation("Email sent successfully with parameters: {MessageParams}", JsonConvert.SerializeObject(messageParams));
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to send email");
        }

        return success;
    }
    
    public void Dispose()
    {
        _smtpClient?.Dispose();
        GC.SuppressFinalize(this);
    }
}