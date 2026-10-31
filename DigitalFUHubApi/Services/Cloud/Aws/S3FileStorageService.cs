using Amazon.S3;
using Amazon.S3.Model;
using DigitalFUHubApi.Comons;
using Microsoft.Extensions.Options;
using System.Net;

namespace DigitalFUHubApi.Services.Cloud.Aws
{
	public class S3FileStorageService : IFileStorageService
	{
		private readonly IAmazonS3 s3;
		private readonly string bucketName;
		private readonly string publicBaseUrl;
		private readonly ILogger<S3FileStorageService> logger;

		public S3FileStorageService(IAmazonS3 s3, IOptions<AwsOptions> options, ILogger<S3FileStorageService> logger)
		{
			this.s3 = s3;
			bucketName = options.Value.S3BucketName;
			publicBaseUrl = options.Value.GetS3PublicBaseUrl();
			this.logger = logger;
		}

		public async Task<string> UploadFileAsync(IFormFile fileUpload, string filename)
		{
			using var stream = fileUpload.OpenReadStream();
			await s3.PutObjectAsync(new PutObjectRequest
			{
				BucketName = bucketName,
				Key = filename,
				InputStream = stream,
				ContentType = Util.Instance.GetContentType(filename)
			});
			return $"{publicBaseUrl}/{filename}";
		}

		public async Task<Stream?> GetFileAsync(string filename)
		{
			try
			{
				var response = await s3.GetObjectAsync(bucketName, filename);
				return response.ResponseStream;
			}
			catch (AmazonS3Exception e) when (e.StatusCode == HttpStatusCode.NotFound)
			{
				return null;
			}
		}

		public async Task<bool> RemoveFileAsync(string fileUrlOrName)
		{
			string? filename = CloudFileUrl.GetFileName(fileUrlOrName, publicBaseUrl);
			if (filename == null)
			{
				logger.LogWarning("Skip removing file not stored in S3 bucket: {Url}", fileUrlOrName);
				return false;
			}

			// S3 DeleteObject succeeds even when the key does not exist, so check first to keep the bool contract
			try
			{
				await s3.GetObjectMetadataAsync(bucketName, filename);
			}
			catch (AmazonS3Exception e) when (e.StatusCode == HttpStatusCode.NotFound)
			{
				return false;
			}

			await s3.DeleteObjectAsync(bucketName, filename);
			return true;
		}
	}
}
