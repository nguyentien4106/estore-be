using EStore.Application.Models.Files;
using Microsoft.AspNetCore.Http;

namespace EStore.Application.Commands.Files.UploadFile;

public record UploadFileR2Command(IFormFile File, string UserName) : ICommand<AppResponse<FileEntityResult>>;
