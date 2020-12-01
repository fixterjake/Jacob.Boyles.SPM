using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using SPM.Web.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SPM.Web.Services
{
    public class BlobStorageService
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public BlobStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetValue<string>("AzureConnectionString");
        }

        /// <summary>
        /// Function to upload a file to an azure blob storage container
        /// and return the url for saving in the database
        /// </summary>
        /// <param name="file">The file uploaded via the form</param>
        /// <returns>String of the url</returns>
        public async Task<string> UploadFile(IFormFile file)
        {
            try
            {
                // Create azure storage client
                var client = new BlobServiceClient(_connectionString);

                // Get the teams storage container
                var container = client.GetBlobContainerClient(_configuration.GetValue<string>("AzureContainerName"));

                // Get reference to blob
                var blob = container.GetBlobClient($"{file.GetHashCode()}-{file.FileName}");

                // Upload image to blob
                await blob.UploadAsync(file.OpenReadStream());

                // Return the url of the image
                return blob.Uri.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        public async Task DeleteFile(string name)
        {
            try
            {
                // Create azure storage client
                var client = new BlobServiceClient(_connectionString);

                // Get the teams storage container
                var container = client.GetBlobContainerClient(_configuration.GetValue<string>("AzureContainerName"));

                // Delete blob from storage container
                await container.DeleteBlobAsync(name);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
