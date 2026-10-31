using Amazon.Rekognition;
using Amazon.Rekognition.Model;

namespace DigitalFUHubApi.Services.Cloud.Aws
{
	public class RekognitionTextService : ITextRecognitionService
	{
		private readonly IAmazonRekognition rekognition;

		public RekognitionTextService(IAmazonRekognition rekognition)
		{
			this.rekognition = rekognition;
		}

		public async Task<string> ExtractTextAsync(byte[] image)
		{
			using var imageStream = new MemoryStream(image);
			var response = await rekognition.DetectTextAsync(new DetectTextRequest
			{
				Image = new Image { Bytes = imageStream }
			});

			var lines = response.TextDetections
				.Where(x => x.Type == TextTypes.LINE)
				.Select(x => x.DetectedText);
			return string.Join(Environment.NewLine, lines).Trim();
		}
	}
}
