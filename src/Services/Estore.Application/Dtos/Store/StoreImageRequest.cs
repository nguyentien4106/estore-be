using Microsoft.AspNetCore.Http;

namespace Estore.Application.Dtos.Store;

public record StoreImageRequest(IFormFile File, string UserName);