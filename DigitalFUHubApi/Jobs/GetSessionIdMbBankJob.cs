using Comons;
using DataAccess.DAOs;
using DigitalFUHubApi.Comons;
using DigitalFUHubApi.Services;
using DigitalFUHubApi.Services.Cloud;
using DTOs.MbBank;
using Quartz;

namespace DigitalFUHubApi.Jobs
{
	public class GetSessionIdMbBankJob : IJob
	{
		private readonly CaptchaImageService captchaImageService;
		private readonly ITextRecognitionService textRecognitionService;
		private readonly MbBankService mbBankService;

		public GetSessionIdMbBankJob(MbBankService mbBankService, CaptchaImageService captchaImageService, ITextRecognitionService textRecognitionService)
		{
			this.mbBankService = mbBankService;
			this.captchaImageService = captchaImageService;
			this.textRecognitionService = textRecognitionService;
		}

		public async Task Execute(IJobExecutionContext context)
		{
			string base64CaptchaImage = string.Empty;
			string sessionId = string.Empty;

			#region TesseractEngine
			/*
			do
			{
				//get captcha
				do
				{
					var responseGetCaptcha = await mbBankService.GetCaptchaImage();
					if (responseGetCaptcha == null) continue;

					if (responseGetCaptcha.Code == Constants.MB_BANK_RESPONE_CODE_SUCCESS)
					{
						base64CaptchaImage = (string)responseGetCaptcha.Result;
					}
				}
				while (string.IsNullOrEmpty(base64CaptchaImage));

				//login
				var captchaRaw = captchaImageService.ExtractTextFromImage(base64CaptchaImage).Trim();
				if (captchaRaw == null) return;

				var captcha = String.Concat(captchaRaw.Where(c => !Char.IsWhiteSpace(c)));

				var response = await mbBankService.Login(captcha);
				if (response == null) continue;

				if (response.Code == Constants.MB_BANK_RESPONE_CODE_SUCCESS)
				{
					sessionId = (string)response.Result;
				}
			}
			while (string.IsNullOrEmpty(sessionId));
			*/
			#endregion

			#region Cloud text recognition (Azure Computer Vision / AWS Rekognition, see "Cloud" in appsettings.json)
			do
			{
				//get captcha
				do
				{
					var responseGetCaptcha = await mbBankService.GetCaptchaImage();
					if (responseGetCaptcha == null) continue;

					if (responseGetCaptcha.Code == Constants.MB_BANK_RESPONE_CODE_SUCCESS)
					{
						base64CaptchaImage = (string)responseGetCaptcha.Result;
					}
				}
				while (string.IsNullOrEmpty(base64CaptchaImage));

				//login
				try
				{
					byte[]? captchaImageStream = captchaImageService.GetClarifyCaptchaImageByBase64(base64CaptchaImage);
					if(captchaImageStream == null) continue;	

					var captchaRaw = await textRecognitionService.ExtractTextAsync(captchaImageStream);
					if (captchaRaw == null) return;

					var captcha = String.Concat(captchaRaw.Where(c => !Char.IsWhiteSpace(c)));

					var response = await mbBankService.Login(captcha);
					if (response == null) continue;

					if (response.Code == Constants.MB_BANK_RESPONE_CODE_SUCCESS)
					{
						sessionId = (string)response.Result;
					}
				}
				catch (Exception ex)
				{
					var message = new
					{
						Error = ex.Message,
					};
					Util.WriteFile("Data/err.json", message);
				}
				
			}
			while (string.IsNullOrEmpty(sessionId));
			#endregion

			// update sessionId in to json file
			MbBankAccount? mbBankAccount = mbBankService.GetMbBankAccount();
			if (mbBankAccount == null) return;
			mbBankAccount.SessionId = sessionId;

			Util.WriteFile(Constants.MB_BANK_DIRECTORY_PATH_STORE_ACCOUNT_DATA, mbBankAccount);

		}
	}
}
