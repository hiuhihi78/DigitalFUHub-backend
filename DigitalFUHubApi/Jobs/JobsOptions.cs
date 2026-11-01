namespace DigitalFUHubApi.Jobs
{
	// Bound from the "Jobs" section in appsettings.json
	public class JobsOptions
	{
		public bool Enabled { get; set; } = true;
		public JobOptions GetSessionIdMbBankJob { get; set; } = new();
		public JobOptions HistoryTransactionMbBankJob { get; set; } = new();
		public JobOptions HistoryDepositTransactionMbBankJob { get; set; } = new();
		public JobOptions HistoryWithdrawTransactionMbBankJob { get; set; } = new();
		public JobOptions OrderStatusJob { get; set; } = new();
		public JobOptions UpdateStatusRequestDepositMoneyToExpired { get; set; } = new();
	}

	public class JobOptions
	{
		// Defaults to true so a job keeps running when its section is missing
		public bool Enabled { get; set; } = true;
	}
}
