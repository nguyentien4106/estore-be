using Microsoft.AspNetCore.Http;
using TdLib;

namespace Estore.Application.Dtos.Files;

public class FileHandlerArgs
{
    public TdClient TdClient { get; set; }

    public IFormFile File { get; set; }

    public string Caption { get; set; }

    public string LocalPath { get; set; }
}
