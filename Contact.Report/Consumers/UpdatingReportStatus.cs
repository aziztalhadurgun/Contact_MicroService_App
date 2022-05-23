using Contact.Report.BusinessLogic;
using Contact.Report.DataAccess;
using Contact.Report.Helpers;
using MassTransit;

namespace Contact.Report.Consumers
{
    public class UpdatingReportStatus : IConsumer<Reports>
    {
        private readonly ReportDbContext _dbContext;
        private readonly ILogger<UpdatingReportStatus> _logger;
        private readonly IReportCreator _reportCreator;

        public UpdatingReportStatus(
            ReportDbContext dbContext,
            ILogger<UpdatingReportStatus> logger,
            IReportCreator reportCreator)
        {
            _dbContext = dbContext;
            _logger = logger;
            _reportCreator = reportCreator;
        }

        public async Task Consume(ConsumeContext<Reports> context)
        {
            var report = context.Message;

            var buildReport = await _reportCreator.BuildReport();

            if (buildReport.StatusCode == 200)
            {
                _logger.LogInformation($"report completed: {buildReport}");
                report.Status = Constants.Completed;

                _dbContext.Update(report);
                await _dbContext.SaveChangesAsync();

                Console.WriteLine(buildReport.Message);
            }
            else
            {
                Console.WriteLine(buildReport.Message);
            }
        }
    }
}
