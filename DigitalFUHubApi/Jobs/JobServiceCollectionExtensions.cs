using Comons;
using Quartz;

namespace DigitalFUHubApi.Jobs
{
	public static class JobServiceCollectionExtensions
	{
		/// <summary>
		/// Reads the "Jobs" setting and registers the Quartz scheduler with its jobs.
		/// A job is only scheduled when its "Enabled" flag is true.
		/// </summary>
		public static IServiceCollection AddJobServices(this IServiceCollection services, IConfiguration configuration)
		{
			var jobsSection = configuration.GetSection(Constants.JOBS);
			JobsOptions jobs = jobsSection.Get<JobsOptions>() ?? new JobsOptions();
			if (!jobs.Enabled)
				return services;

			services.Configure<JobsOptions>(jobsSection);

			services.AddQuartz(configure =>
			{
				configure.UseMicrosoftDependencyInjectionJobFactory();

				if (jobs.GetSessionIdMbBankJob.Enabled)
				{
					var jobKeyGetSessionIdMbBankJob = new JobKey("GetSessionIdMbBankJob");
					configure.AddJob<GetSessionIdMbBankJob>(jobKeyGetSessionIdMbBankJob)
							 .AddTrigger(trigger =>
								trigger.ForJob(jobKeyGetSessionIdMbBankJob)
								.StartNow()
								.WithSimpleSchedule(schedule =>
									schedule.WithIntervalInMinutes(25).RepeatForever()
									)
								);
				}

				if (jobs.HistoryTransactionMbBankJob.Enabled)
				{
					var jobKeyHistoryTransactionMbBankJob = new JobKey("HistoryTransactionMbBankJob");
					configure.AddJob<HistoryTransactionMbBankJob>(jobKeyHistoryTransactionMbBankJob)
							 .AddTrigger(trigger =>
								trigger.ForJob(jobKeyHistoryTransactionMbBankJob)
								.StartNow()
								.WithSimpleSchedule(schedule =>
									schedule.WithIntervalInSeconds(61).RepeatForever()
									)
								);
				}

				if (jobs.HistoryDepositTransactionMbBankJob.Enabled)
				{
					var jobKeyHistoryDepositTransactionMbBankJob = new JobKey("HistoryDepositTransactionMbBankJob");
					configure.AddJob<HistoryDepositTransactionMbBankJob>(jobKeyHistoryDepositTransactionMbBankJob)
							 .AddTrigger(trigger =>
								trigger.ForJob(jobKeyHistoryDepositTransactionMbBankJob)
								.StartNow()
								.WithSimpleSchedule(schedule =>
									schedule.WithIntervalInSeconds(111).RepeatForever()
									)
								);
				}

				if (jobs.HistoryWithdrawTransactionMbBankJob.Enabled)
				{
					var jobKeyHistoryWithdrawTransactionMbBankJob = new JobKey("HistoryWithdrawTransactionMbBankJob");
					configure.AddJob<HistoryWithdrawTransactionMbBankJob>(jobKeyHistoryWithdrawTransactionMbBankJob)
							 .AddTrigger(trigger =>
								trigger.ForJob(jobKeyHistoryWithdrawTransactionMbBankJob)
								.StartNow()
								.WithSimpleSchedule(schedule =>
									schedule.WithIntervalInSeconds(183).RepeatForever()
									)
								);
				}

				if (jobs.OrderStatusJob.Enabled)
				{
					var jobKeyOrderStatusJob = new JobKey("OrderStatusJob");
					configure.AddJob<OrderStatusJob>(jobKeyOrderStatusJob)
							 .AddTrigger(trigger =>
								trigger.ForJob(jobKeyOrderStatusJob)
								.StartNow()
								.WithSimpleSchedule(schedule =>
									schedule.WithIntervalInMinutes(30).RepeatForever()
									)
								);
				}

				if (jobs.UpdateStatusRequestDepositMoneyToExpired.Enabled)
				{
					var jobKeyUpdateStatusRequestDepositMoneyToExpiredJob = new JobKey("UpdateStatusRequestDepositMoneyToExpired");
					configure.AddJob<UpdateStatusRequestDepositMoneyToExpired>(jobKeyUpdateStatusRequestDepositMoneyToExpiredJob)
							 .AddTrigger(trigger =>
								trigger.ForJob(jobKeyUpdateStatusRequestDepositMoneyToExpiredJob)
								.StartNow()
								.WithSimpleSchedule(schedule =>
									schedule.WithIntervalInHours(24).RepeatForever()
									)
								);
				}
			});

			services.AddQuartzHostedService(options =>
			{
				options.WaitForJobsToComplete = true;
			});

			return services;
		}
	}
}
