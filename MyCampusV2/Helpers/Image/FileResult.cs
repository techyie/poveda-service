using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MyCampusV2.Helpers.Image
{
    public class FileResult
    {
        private readonly string filePath;
        private readonly string contentType;

        public FileResult(string filePath, string contentType = null)
        {
            this.filePath = filePath;
            this.contentType = contentType;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StreamContent(System.IO.File.OpenRead(filePath))
                };

                var contentType = this.contentType ?? "application/octet-stream";
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

                return response;
            }, cancellationToken);
        }
    }
}
