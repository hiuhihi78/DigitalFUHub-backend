namespace DigitalFUHubApi.Services.Cloud.Aws
{
	public class AwsOptions
	{
		public string DbConnectionString { get; set; } = string.Empty;
		public string Region { get; set; } = string.Empty;

		// Optional: leave empty to use the default credential chain (IAM role, ~/.aws profile, env vars)
		public string AccessKey { get; set; } = string.Empty;
		public string SecretKey { get; set; } = string.Empty;

		public string S3BucketName { get; set; } = string.Empty;

		// Optional: e.g. a CloudFront domain. Defaults to https://{bucket}.s3.{region}.amazonaws.com
		public string S3PublicBaseUrl { get; set; } = string.Empty;

		public string GetS3PublicBaseUrl() => string.IsNullOrWhiteSpace(S3PublicBaseUrl)
			? $"https://{S3BucketName}.s3.{Region}.amazonaws.com"
			: S3PublicBaseUrl.TrimEnd('/');
	}
}
