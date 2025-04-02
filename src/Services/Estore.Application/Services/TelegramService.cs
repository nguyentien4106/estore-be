using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TdLib;
using System.IO;
using System.Text.Json;
using SendGrid.Helpers.Mail;
using static TdLib.TdApi;
using Microsoft.AspNetCore.Http;

namespace Estore.Application.Services
{
    public class TelegramService : ITelegramService
    {
        private readonly TdClient _client;
        private readonly ILogger<TelegramService> _logger;
        private readonly TelegramConfiguration _config;
        private bool _isAuthenticated;
        private readonly string _authFilePath;
        public TelegramService(TelegramConfiguration telegramConfiguration,
            ILogger<TelegramService> logger)
        {
            _logger = logger;
            _config = telegramConfiguration;
            _client = new TdClient();
            _client.UpdateReceived += OnUpdateReceived;

            _authFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "telegram_auth.json");
            InitializeAsync().Wait();
        }

        private async Task InitializeAsync()
        {
            try
            {
                // Set log verbosity level
                await _client.ExecuteAsync(new TdApi.SetLogVerbosityLevel { NewVerbosityLevel = 1 });

                // Set TdLib parameters
                await _client.ExecuteAsync(new TdApi.SetTdlibParameters
                {
                    UseMessageDatabase = false,
                    UseSecretChats = false,
                    ApiId = _config.ApiId,
                    ApiHash = _config.ApiHash,
                    SystemLanguageCode = "en",
                    DeviceModel = "PC",
                    ApplicationVersion = "1.0",
                });

                await AuthenticateAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Telegram client");
                throw;
            }
        }

        private void OnUpdateReceived(object? sender, Update update)
        {
           
        }

        private async Task AuthenticateAsync()
        {
            try
            {
                // If no stored credentials or they failed, proceed with normal authentication
                while (!_isAuthenticated)
                {
                    var state = await _client.ExecuteAsync(new TdApi.GetAuthorizationState());

                    switch (state)
                    {
                        case TdApi.AuthorizationState.AuthorizationStateWaitPhoneNumber:
                            _logger.LogInformation("Sending phone number...");
                            await _client.ExecuteAsync(new TdApi.SetAuthenticationPhoneNumber
                            {
                                PhoneNumber = _config.PhoneNumber
                            });
                            break;

                        case TdApi.AuthorizationState.AuthorizationStateWaitCode:
                            _logger.LogInformation("Waiting for authentication code...");
                            // Use the code from configuration
                            var authCode = Console.ReadLine();
                            await _client.ExecuteAsync(new TdApi.CheckAuthenticationCode 
                            { 
                                Code = authCode
                            });
                            break;

                        case TdApi.AuthorizationState.AuthorizationStateWaitPassword:
                            _logger.LogInformation("Using 2FA password from configuration...");
                            await _client.ExecuteAsync(new TdApi.CheckAuthenticationPassword 
                            { 
                                Password = _config.TwoFactorPassword 
                            });
                            break;

                        case TdApi.AuthorizationState.AuthorizationStateReady:
                            _logger.LogInformation("Successfully authenticated!");
                            _isAuthenticated = true;
                            break;

                        case TdApi.AuthorizationState.AuthorizationStateLoggingOut:
                        case TdApi.AuthorizationState.AuthorizationStateClosing:
                        case TdApi.AuthorizationState.AuthorizationStateClosed:
                            _logger.LogInformation("Connection is closed");
                            break;

                        case TdApi.AuthorizationState.AuthorizationStateWaitTdlibParameters:
                            _logger.LogInformation("Waiting for TdLib parameters...");
                            break;

                        case TdApi.AuthorizationState.AuthorizationStateWaitRegistration:
                            _logger.LogInformation("Waiting for registration...");
                            break;

                        default:
                            _logger.LogWarning("Unknown authorization state: {State}", state);
                            break;
                    }

                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to authenticate with Telegram");
                throw;
            }
        }

        public async Task SendFileToChannelAsync(string filePath, string caption = null)
        {
            try
            {
                if (!_isAuthenticated)
                {
                    _logger.LogWarning("Not authenticated. Attempting to authenticate...");
                    await AuthenticateAsync();
                }

                var inputFile = new TdApi.InputFile.InputFileLocal { Path = filePath };
                await _client.ExecuteAsync(new TdApi.SendMessage
                {
                    ChatId = _config.ChannelId,
                    InputMessageContent = new TdApi.InputMessageContent.InputMessageDocument
                    {
                        Document = inputFile,
                        Caption = new TdApi.FormattedText { Text = caption ?? string.Empty }
                    }
                });

                _logger.LogInformation("File sent successfully to Telegram channel");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending file to Telegram channel");
                throw;
            }
        }

        public async Task SendFormFileToChannelAsync(IFormFile file, string caption = null)
        {
            try
            {
                if (!_isAuthenticated)
                {
                    _logger.LogWarning("Not authenticated. Attempting to authenticate...");
                    await AuthenticateAsync();
                }
                await UploadFileAndGetRemoteIdAsync(file);
                Console.WriteLine("✅ File sent successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending IFormFile to Telegram channel");
                throw;
            }
        }

        private async Task UploadFileAndGetRemoteIdAsync(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var tempPath = Path.Combine(Directory.GetCurrentDirectory(), "temp", file.FileName);
            await System.IO.File.WriteAllBytesAsync(tempPath, memoryStream.ToArray());

            await _client.ExecuteAsync(new TdApi.SendMessage
            {
                ChatId = _config.ChannelId, 
                InputMessageContent = new TdApi.InputMessageContent.InputMessageDocument
                {
                    Document = new TdApi.InputFile.InputFileLocal { Path = tempPath },
                    Caption = new TdApi.FormattedText { Text = "Uploading file..." }
                }
            });

        }

        public async Task<long> GetChatIdAsync()
        {
            var chat = await _client.ExecuteAsync(new TdApi.GetChat { ChatId = _config.ChannelId });

            return chat.Id;
        }
    }

  
}
