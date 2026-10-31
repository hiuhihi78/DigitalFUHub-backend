using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Rekognition;
using Amazon.Runtime;
using Amazon.S3;
using Comons;
using DigitalFUHubApi.Services.Cloud.Aws;
using DigitalFUHubApi.Services.Cloud.Azure;

namespace DigitalFUHubApi.Services.Cloud
{
	public static class CloudServiceCollectionExtensions
	{
		/// <summary>
		/// Reads the "Cloud" setting and registers the matching IFileStorageService / ITextRecognitionService.
		/// Only the selected provider's SDK clients are registered and validated.
		/// </summary>
		public static IServiceCollection AddCloudServices(this IServiceCollection services, IConfiguration configuration)
		{
			CloudProvider provider = GetCloudProvider(configuration);

			switch (provider)
			{
				case CloudProvider.Azure:
					var azureSection = configuration.GetSection(Constants.AZURE);
					ValidateRequired(azureSection.Get<AzureOptions>(), Constants.AZURE,
						nameof(AzureOptions.StorageEndPoint),
						nameof(AzureOptions.StorageConnectionString),
						nameof(AzureOptions.StorageContainerName),
						nameof(AzureOptions.ComputerVisionEndpoint),
						nameof(AzureOptions.ComputerVisionSubscriptionKey));

					services.Configure<AzureOptions>(azureSection);
					services.AddSingleton<IFileStorageService, AzureBlobStorageService>();
					services.AddSingleton<ITextRecognitionService, AzureComputerVisionService>();
					break;

				case CloudProvider.AWS:
					var awsSection = configuration.GetSection(Constants.AWS);
					var aws = awsSection.Get<AwsOptions>();
					ValidateRequired(aws, Constants.AWS,
						nameof(AwsOptions.Region),
						nameof(AwsOptions.S3BucketName));

					var awsSdkOptions = new AWSOptions { Region = RegionEndpoint.GetBySystemName(aws!.Region) };
					if (!string.IsNullOrWhiteSpace(aws.AccessKey) && !string.IsNullOrWhiteSpace(aws.SecretKey))
					{
						awsSdkOptions.Credentials = new BasicAWSCredentials(aws.AccessKey, aws.SecretKey);
					}

					services.Configure<AwsOptions>(awsSection);
					services.AddDefaultAWSOptions(awsSdkOptions);
					services.AddAWSService<IAmazonS3>();
					services.AddAWSService<IAmazonRekognition>();
					services.AddSingleton<IFileStorageService, S3FileStorageService>();
					services.AddSingleton<ITextRecognitionService, RekognitionTextService>();
					break;
			}

			return services;
		}

		public static CloudProvider GetCloudProvider(IConfiguration configuration)
		{
			string? value = configuration[Constants.CLOUD];
			if (!Enum.TryParse(value, ignoreCase: true, out CloudProvider provider) || !Enum.IsDefined(provider))
			{
				throw new InvalidOperationException(
					$"Invalid \"{Constants.CLOUD}\" setting '{value}' in appsettings.json. Allowed values: {string.Join(", ", Enum.GetNames<CloudProvider>())}.");
			}
			return provider;
		}

		/// <summary>
		/// Returns "{Cloud}:DbConnectionString" if set, otherwise falls back to ConnectionStrings:DB.
		/// </summary>
		public static string GetDbConnectionString(IConfiguration configuration)
		{
			CloudProvider provider = GetCloudProvider(configuration);
			string section = provider == CloudProvider.AWS ? Constants.AWS : Constants.AZURE;
			string? connectionString = configuration[$"{section}:DbConnectionString"];
			if (string.IsNullOrWhiteSpace(connectionString))
			{
				connectionString = configuration.GetConnectionString("DB");
			}
			if (string.IsNullOrWhiteSpace(connectionString))
			{
				throw new InvalidOperationException(
					$"No database connection string configured. Set \"{section}:DbConnectionString\" or \"ConnectionStrings:DB\" in appsettings.json.");
			}
			return connectionString;
		}

		private static void ValidateRequired(object? options, string section, params string[] propertyNames)
		{
			var missing = propertyNames
				.Where(name => string.IsNullOrWhiteSpace(options?.GetType().GetProperty(name)?.GetValue(options) as string))
				.Select(name => $"{section}:{name}")
				.ToList();

			if (missing.Any())
			{
				throw new InvalidOperationException(
					$"\"{Constants.CLOUD}\" is set to '{section}' but these settings are missing in appsettings.json: {string.Join(", ", missing)}");
			}
		}
	}
}
