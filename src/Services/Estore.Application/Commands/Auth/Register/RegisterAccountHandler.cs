using EStore.Application.Services.Webhooks;
using EStore.Domain.Enums;
using Microsoft.Extensions.Configuration;
using System.Text.Encodings.Web;

namespace EStore.Application.Commands.Auth.Register;

public class RegisterAccountHandler : ICommandHandler<RegisterAccountCommand, AppResponse<bool>>
{
    private readonly UserManager<User> _userManager;
    private readonly IWebhookService _webhookService;
    private readonly IConfiguration _configuration;
    private readonly AppSettings _appSettings;
    public RegisterAccountHandler(
        UserManager<User> userManager,
        IWebhookService webhookService,
        IConfiguration configuration,
        AppSettings appSettings)
    {
        _userManager = userManager;
        _webhookService = webhookService;
        _configuration = configuration;
        _appSettings = appSettings;
    }

    public async Task<AppResponse<bool>> Handle(RegisterAccountCommand command, CancellationToken cancellationToken)
    {
        var existingUserByEmail = await _userManager.FindByEmailAsync(command.Email);
        if (existingUserByEmail != null)
        {
            return AppResponse<bool>.Error("Email is already registered.");
        }

        var existingUserByUserName = await _userManager.FindByNameAsync(command.UserName);
        if (existingUserByUserName != null)
        {
            return AppResponse<bool>.Error("Username is already taken.");
        }

        var newUser = new User
        {
            Email = command.Email,
            UserName = command.UserName,
            PhoneNumber = command.PhoneNumber,
            FirstName = command.FirstName,
            LastName = command.LastName,
            Status = (int)AccountStatus.NotConfirmed // Set status to Pending until email is confirmed
        };

        var result = await _userManager.CreateAsync(newUser, command.Password);
        if (result.Succeeded)
        {
            // Generate email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            var encodedToken = UrlEncoder.Default.Encode(token);

            if (string.IsNullOrEmpty(_appSettings.ConfirmEmailBaseUrl))
            {
                // Log or handle missing configuration
                return AppResponse<bool>.Error("Email confirmation base URL is not configured.");
            }
            
            var confirmationLink = $"{_appSettings.ConfirmEmailBaseUrl}?userId={newUser.Id}&token={encodedToken}";

            // Send webhook for email confirmation
            await _webhookService.SendToWebhookAsync(new
            {
                UserId = newUser.Id,
                Email = newUser.Email,
                ConfirmationLink = confirmationLink
            });

            return AppResponse<bool>.Success(true, "User registered successfully. Please check your email to confirm your account.");
        }

        var errors = result.Errors.Select(e => e.Description).ToList();
        return AppResponse<bool>.Error(errors.FirstOrDefault() ?? "User registration failed.");
    }
}