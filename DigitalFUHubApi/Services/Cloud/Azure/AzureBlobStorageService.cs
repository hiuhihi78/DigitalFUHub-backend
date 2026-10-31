using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DigitalFUHubApi.Comons;
using Microsoft.Extensions.Options;

namespace DigitalFUHubApi.Services.Cloud.Azure
{
	public class AzureBlobStorageService : IFileStorageService
	{
		private readonly BlobContainerClient container;
		private readonly string publicBaseUrl;
		private readonly ILogger<AzureBlobStorageService> logger;

		public AzureBlobStorageService(IOptions<AzureOptions> options, ILogger<AzureBlobStorageService> logger)
		{
			var azure = options.Value;
			container = new BlobContainerClient(azure.StorageConnectionString, azure.StorageContainerName);
			publicBaseUrl = $"{azure.StorageEndPoint.TrimEnd('/')}/{azure.StorageContainerName}";
			this.logger = logger;
		}

		public async Task<string> UploadFileAsync(IFormFile fileUpload, string filename)
		{
			var blobClient = container.GetBlobClient(filename);
			var blobHttpHeader = new BlobHttpHeaders { ContentType = Util.Instance.GetContentType(filename) };
			await blobClient.UploadAsync(fileUpload.OpenReadStream(), new BlobUploadOptions { HttpHeaders = blobHttpHeader });
			return $"{publicBaseUrl}/{filename}";
		}

		public async Task<Stream?> GetFileAsync(string filename)
		{
			BlobClient blobClient = container.GetBlobClient(filename);
			if (!await blobClient.ExistsAsync())
			{
				return null;
			}
			return await blobClient.OpenReadAsync();
		}

		public async Task<bool> RemoveFileAsync(string fileUrlOrName)
		{
			string? filename = CloudFileUrl.GetFileName(fileUrlOrName, publicBaseUrl);
			if (filename == null)
			{
				logger.LogWarning("Skip removing file not stored in Azure container: {Url}", fileUrlOrName);
				return false;
			}

			BlobClient blobClient = container.GetBlobClient(filename);
			var response = await blobClient.DeleteIfExistsAsync();
			return response.Value;
		}
	}
}
