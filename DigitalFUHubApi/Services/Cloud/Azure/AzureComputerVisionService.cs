using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Options;
using System.Text;

namespace DigitalFUHubApi.Services.Cloud.Azure
{
	public class AzureComputerVisionService : ITextRecognitionService
	{
		private readonly ComputerVisionClient client;

		public AzureComputerVisionService(IOptions<AzureOptions> options)
		{
			var azure = options.Value;
			client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(azure.ComputerVisionSubscriptionKey))
			{
				Endpoint = azure.ComputerVisionEndpoint
			};
		}

		public async Task<string> ExtractTextAsync(byte[] image)
		{
			using var imageStream = new MemoryStream(image);

			ReadInStreamHeaders textHeaders = await client.ReadInStreamAsync(imageStream);
			string operationId = textHeaders.OperationLocation[^36..];

			ReadOperationResult results;
			do
			{
				results = await client.GetReadResultAsync(Guid.Parse(operationId));
				if (results.Status == OperationStatusCodes.Running || results.Status == OperationStatusCodes.NotStarted)
				{
					await Task.Delay(500);
				}
			}
			while (results.Status == OperationStatusCodes.Running || results.Status == OperationStatusCodes.NotStarted);

			StringBuilder sb = new();
			foreach (ReadResult page in results.AnalyzeResult.ReadResults)
			{
				foreach (Line line in page.Lines)
				{
					sb.AppendLine(line.Text);
				}
			}
			return sb.ToString().Trim();
		}
	}
}
