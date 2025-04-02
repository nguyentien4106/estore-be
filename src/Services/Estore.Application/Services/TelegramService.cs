using Microsoft.Extensions.Logging;
using TdLib;
using static TdLib.TdApi;
using Microsoft.AspNetCore.Http;
using Estore.Application.Helpers;
namespace Estore.Application.Services
{
    public class TelegramService : ITelegramService
    {
        private readonly TdClient _client;
        private readonly ILogger<TelegramService> _logger;
        private readonly TelegramConfiguration _config;
        private bool _isAuthenticated;
        public TelegramService(TelegramConfiguration telegramConfiguration,
            ILogger<TelegramService> logger)
        {
            _logger = logger;
            _config = telegramConfiguration;
            _client = new TdClient();

            InitializeAsync().Wait();
        }

        public async Task<AppResponse<long>> UploadFileToStrorageAsync(IFormFile file, string caption = null)
        {
            try
            {
                if (!_isAuthenticated)
                {
                    _logger.LogWarning("Not authenticated. Attempting to authenticate...");
                    await AuthenticateAsync();
                }

                return await UploadFileAsync(file, caption);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending IFormFile to Telegram channel");
                return AppResponse<long>.Error(ex.Message);
            }
        }

        
        public async Task<long> GetChatIdAsync()
        {
            var chat = await _client.ExecuteAsync(new TdApi.GetChat { ChatId = _config.ChannelId });

            return chat.Id;
        }

        /// <summary>
        /// Deletes a message from a Telegram channel
        /// </summary>
        /// <param name="messageId">The ID of the message to delete</param>
        /// <param name="revoke">Whether to delete the message for all users</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task<AppResponse<bool>> DeleteMessageAsync(long messageId)
        {
            try
            {
                await _client.ExecuteAsync(new DeleteMessages() { 
                    ChatId = _config.ChannelId,
                    MessageIds = [messageId] 
                });

                return AppResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete message from Telegram channel");
                return AppResponse<bool>.Error(ex.Message);
            }
        }
        
        private async Task InitializeAsync()
        {
            try
            {
                await _client.ExecuteAsync(new SetLogVerbosityLevel { NewVerbosityLevel = 1 });

                await _client.ExecuteAsync(new SetTdlibParameters
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

        /// <summary>
        /// Uploads a file to Telegram and returns the remote message ID
        /// </summary>
        /// <param name="file">The file to upload</param>
        /// <param name="caption">Optional caption text for the file</param>
        /// <returns>The message ID of the uploaded file in the channel</returns>
        private async Task<AppResponse<long>> UploadFileAsync(IFormFile file, string caption)
        {
            var tempPath = await FileHelper.CreateTempFileAsync(file);

            var message = await _client.ExecuteAsync(new TdApi.SendMessage
            {
                ChatId = _config.ChannelId, 
                InputMessageContent = new TdApi.InputMessageContent.InputMessageDocument
                {
                    Document = new TdApi.InputFile.InputFileLocal { Path = tempPath },
                    Caption = new TdApi.FormattedText { Text = caption ?? string.Empty }
                }
            });

            return AppResponse<long>.Success(message.Id);
        }

    }
}
