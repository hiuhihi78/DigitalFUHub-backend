namespace DigitalFUHubApi.Services.Cloud
{
	public interface ITextRecognitionService
	{
		/// <summary>Extracts all text lines from an image.</summary>
		Task<string> ExtractTextAsync(byte[] image);
	}
}
