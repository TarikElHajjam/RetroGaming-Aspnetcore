using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Retro_Gamer.Services
{
    /// <summary>
    /// Upload images to S3 buckets or delete them from it
    /// </summary>
    public class AmazonS3Bucket : IAmazonS3Bucket
    {  //Every photo uploaded to s3 bucket have a url like this
        //copy and past first part of the link
        //https://elasticbeanstalk-xxxxxx-xxxxxxx.amazonaws.com/ 

        private string filePath = "Your link here";
        IAmazonS3 s3Client = new AmazonS3Client("Your Access key",
               "Your Security key", "Region");
        public async Task<string> UploadImage(IFormFile file)
        {

            await using var newMemoryStream = new MemoryStream();
            file.CopyTo(newMemoryStream);
            string uniqueName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = newMemoryStream,
                Key = uniqueName,
                ///Bucket name
                BucketName = "Bucket name",
                CannedACL = S3CannedACL.PublicRead
            };

            var fileTransferUtility = new TransferUtility(s3Client);
            await fileTransferUtility.UploadAsync(uploadRequest);
            //Returns the files url when it gets uploaded to s3 bucket 
            return filePath + uploadRequest.Key;
        }


        public async Task Delete(string url)
        {
            //trims the url and leave only the name of the file.
            var fileName = url.Remove(0, 75);
            await s3Client.DeleteObjectAsync(new Amazon.S3.Model.DeleteObjectRequest()
            ///Bucket name access point
            {
                BucketName = "access point bucket",
                Key = fileName
            });
        }
    }
}
