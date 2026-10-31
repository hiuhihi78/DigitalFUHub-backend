using Comons;
using DigitalFUHubApi.Comons;
using Microsoft.AspNetCore.Hosting.Server;
using System;
using System.Drawing;
using System.Net.Http.Headers;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using System.IO;


namespace DigitalFUHubApi.Services
{
	/// <summary>
	/// Cloud-independent captcha image pre-processing. Text extraction is done by ITextRecognitionService.
	/// </summary>
	public class CaptchaImageService
	{
		#region Clarify image
		public void ClarifyImage(string base64)
		{
#pragma warning disable CA1416 //bitmap just support for windown
			byte[] bytes = System.Convert.FromBase64String(base64);

			using (MemoryStream ms = new MemoryStream(bytes))
			{
				using (Bitmap image = new Bitmap(ms))
				{
					if (image == null) return;

					for (int y = 0; y < image.Height; y++)
					{
						for (int x = 0; x < image.Width; x++)
						{
							var pixel = image.GetPixel(x, y);
							if (pixel.R < 112 && pixel.G < 112 && pixel.B < 112)
							{
								System.Drawing.Color black = System.Drawing.Color.FromArgb(255, 0, 0, 0);
								image.SetPixel(x, y, black);
							}
							else
							{
								System.Drawing.Color white = System.Drawing.Color.FromArgb(255, 255, 255, 255);
								image.SetPixel(x, y, white);
							}
						}
					}
					string path = Constants.CAPTCHA_IMAGE_FILE_NAME;
					image.Save(path);
				}
			}
#pragma warning restore CA1416 // 
		}
		#endregion

		#region Clarify image  and convert into byte[]
		public byte[]? GetClarifyCaptchaImageByBase64(string base64)
		{
#pragma warning disable CA1416, SC8600 //bitmap just support for windown
			byte[] bytes = System.Convert.FromBase64String(base64);

			using (MemoryStream ms = new MemoryStream(bytes))
			{
				using (Bitmap image = new Bitmap(ms))
				{
					if (image == null) return null;

					for (int y = 0; y < image.Height; y++)
					{
						for (int x = 0; x < image.Width; x++)
						{
							var pixel = image.GetPixel(x, y);
							if (pixel.R < 112 && pixel.G < 112 && pixel.B < 112)
							{
								System.Drawing.Color black = System.Drawing.Color.FromArgb(255, 0, 0, 0);
								image.SetPixel(x, y, black);
							}
							else
							{
								System.Drawing.Color white = System.Drawing.Color.FromArgb(255, 255, 255, 255);
								image.SetPixel(x, y, white);
							}
						}
					}

					ImageConverter converter = new ImageConverter();
					return (byte[]?)converter.ConvertTo(image, typeof(byte[]));
				}
			}
#pragma warning restore CA1416, SC8600 // 
		}
		#endregion

		#region Extract text from image - Tesseract, Tesseract.Net.SDK
		/*
		public string ExtractTextFromImage()
		{
			using (var engine = new TesseractEngine("tessdata", "eng", EngineMode.Default))
			{
				string path = Constants.CAPTCHA_IMAGE_FILE_NAME;
				Pix pix = Pix.LoadFromFile(path);

				var page = engine.Process(pix);

				return page.GetText();
			}
		}
		*/
		#endregion

		#region Extract text from image - Tesseract, Tesseract.Net.SDK
		/*
		public string ExtractTextFromImage(string base64)
		{
			var currentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
			var tessDataPath = Path.Combine(currentFolder, "tessdata");

			using (var engine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default))
			{
				Pix pix = Pix.LoadFromMemory(GetClarifyCaptchaImageByBase64(base64));

				var page = engine.Process(pix);

				return page.GetText();
			}
		}
		*/
		#endregion
	}
}
