using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Retro_Gamer.Services
{
    public interface IAmazonS3Bucket
    {
        Task<string> UploadImage(IFormFile file);
        Task Delete(string url);

    }
}
