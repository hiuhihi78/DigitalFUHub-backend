namespace DigitalFUHubApi.Services.Cloud
{
	public static class CloudFileUrl
	{
		/// <summary>
		/// Returns the file name (storage key) for a full URL under <paramref name="publicBaseUrl"/>,
		/// the input itself if it is already a bare file name, or null if the URL belongs elsewhere
		/// (e.g. a file uploaded while the other cloud provider was active).
		/// </summary>
		public static string? GetFileName(string fileUrlOrName, string publicBaseUrl)
		{
			if (string.IsNullOrWhiteSpace(fileUrlOrName)) return null;

			if (!fileUrlOrName.Contains('/')) return fileUrlOrName;

			string prefix = publicBaseUrl.TrimEnd('/') + "/";
			if (!fileUrlOrName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) return null;

			string filename = fileUrlOrName.Substring(prefix.Length);
			return string.IsNullOrEmpty(filename) ? null : filename;
		}
	}
}
