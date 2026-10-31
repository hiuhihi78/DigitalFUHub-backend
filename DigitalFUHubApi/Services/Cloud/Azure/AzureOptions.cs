namespace DigitalFUHubApi.Services.Cloud.Azure
{
	public class AzureOptions
	{
		public string DbConnectionString { get; set; } = string.Empty;
		public string ComputerVisionEndpoint { get; set; } = string.Empty;
		public string ComputerVisionSubscriptionKey { get; set; } = string.Empty;
		public string StorageEndPoint { get; set; } = string.Empty;
		public string StorageConnectionString { get; set; } = string.Empty;
		public string StorageContainerName { get; set; } = string.Empty;
	}
}
