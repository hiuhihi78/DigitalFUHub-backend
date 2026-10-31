namespace DigitalFUHubApi.Services.Cloud
{
	public interface IFileStorageService
	{
		/// <summary>Uploads a file and returns its public URL.</summary>
		Task<string> UploadFileAsync(IFormFile fileUpload, string filename);

		/// <summary>Removes a file by its full URL or filename. Returns false if the file does not exist
		/// or belongs to another storage provider.</summary>
		Task<bool> RemoveFileAsync(string fileUrlOrName);

		Task<Stream?> GetFileAsync(string filename);
	}
}
