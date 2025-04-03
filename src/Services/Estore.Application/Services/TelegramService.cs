using Microsoft.Extensions.Logging;
using TdLib;
using static TdLib.TdApi;
using Microsoft.AspNetCore.Http;
using Estore.Application.Helpers;
using Estore.Application.Services.Files;
namespace Estore.Application.Services
{
    public class TelegramService : ITelegramService
    {
        private readonly TdClient _client;
        private readonly ILogger<TelegramService> _logger;
        private readonly TelegramConfiguration _config;
        private bool _isAuthenticated;
        private readonly IEStoreDbContext _context;

        public TelegramService(TelegramConfiguration telegramConfiguration, ILogger<TelegramService> logger, IEStoreDbContext context)
        {
            _context = context;
            _logger = logger;
            _config = telegramConfiguration;
            _client = new TdClient();
            _client.UpdateReceived += OnUpdateReceived;
            InitializeAsync().Wait();
        }

        private async void OnUpdateReceived(object sender, Update update)
        {
            if(update is TdApi.Update.UpdateNewMessage updateNewMessage){
                _logger.LogInformation("New message received: {Message}", updateNewMessage.Message.Content);
            }

            if(update is TdApi.Update.UpdateFile file){
                if(file.File.Remote.IsUploadingCompleted){
                    //var localPath = file.File.Local.Path;
                    //var fileInfo = await _context.TeleFiles.FirstOrDefaultAsync(item => item.LocalPath == localPath);
                    //if(file is not null)
                    //{
                    //    fileInfo.RemoteFileId = file.File.Remote.Id;
                    //    await _context.CommitAsync();
                    //}
                    _logger.LogInformation("File ID received: {File}", file.File.Remote.Id);

                }
            }
        }

        public async Task<AppResponse<long>> UploadFileToStrorageAsync(IFormFile file, string userId)
        {
            try
            {
                if (!_isAuthenticated)
                {
                    _logger.LogWarning("Not authenticated. Attempting to authenticate...");
                    await AuthenticateAsync();
                }

                var result = await UploadFileAsync(file, userId);

                if (result.Succeed)
                {
                    var teleFile = TelegramFileInformation.Create(Guid.Parse(userId), file.FileName, FileHelper.GetFileSizeInMb(file.Length), FileHelper.DetermineFileType(file.FileName), result.Data, "");
                    await _context.TeleFiles.AddAsync(teleFile);
                    await _context.CommitAsync();
                }
                return AppResponse<long>.Success(1);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending IFormFile to Telegram channel");
                return AppResponse<long>.Error(ex.Message);
            }
        }
 
        private async Task<AppResponse<string>> UploadFileAsync(IFormFile file, string caption)
        {
            var tempPath = await FileHelper.CreateTempFileAsync(file);
            var handler = FileHandlerFactory.GetHandler(FileHelper.DetermineFileType(file.FileName), _config);
            var args = new FileHandlerArgs
            {
                TdClient = _client,
                LocalPath = tempPath,
                Caption = caption,
                File = file
            };

            return await handler.UploadFileAsync(args);
        }

        public async Task<AppResponse<string>> DownloadFileAsync(int fileId)
        {
            try
            {
                if (!_isAuthenticated)
                {
                    _logger.LogWarning("Not authenticated. Attempting to authenticate...");
                    await AuthenticateAsync();
                }
                await _client.ExecuteAsync(new DownloadFile{
                    FileId = fileId,
                    Priority = 32
                });

                return AppResponse<string>.Success("File downloaded successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file from Telegram");
                return AppResponse<string>.Error(ex.Message);
            }
        }

        public async Task<long> GetChatIdAsync()
        {
            var chat = await _client.ExecuteAsync(new TdApi.GetChat { ChatId = _config.ChannelId });

            return chat.Id;
        }

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

       
    }
}
