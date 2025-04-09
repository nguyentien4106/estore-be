using EStore.Application.Constants;
using EStore.Application.Services.Telegram;
using EStore.Domain.Models.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EStore.Application.Files.Commands.UploadLargeFile
{
    public class UploadLargeFileHandler : ICommandHandler<UploadLargeFileCommand, AppResponse<FileEntityResponse>>
    {
        public async Task<AppResponse<FileEntityResponse>> Handle(UploadLargeFileCommand command, CancellationToken cancellationToken)
        {
            throw new Exception("test");

        }

        private static string GetBoundary(MediaTypeHeaderValue contentType)
        {
            var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;

            if (string.IsNullOrWhiteSpace(boundary))
            {
                throw new InvalidDataException("Missing content-type boundary.");
            }

            return boundary;
        }
    }
}
