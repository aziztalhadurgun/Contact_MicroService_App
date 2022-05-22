using Contact.Report.Config;
using Contact.Report.DataAccess;
using Contact.Report.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Contact.Report.BusinessLogic
{
    public interface IReportCreator
    {
        public Task BuildReport();
    }

    public class ReportCreator : IReportCreator
    {
        private readonly IHttpClientFactory _http;
        private readonly ILogger<ReportCreator> _logger;
        private readonly ReportDbContext _dbContext;
        private readonly UserDataConfig _userDataConfig;

        public ReportCreator(
            IHttpClientFactory http,
            ILogger<ReportCreator> logger,
            ReportDbContext dbContext,
            IOptions<UserDataConfig> userDataConfig
            )
        {
            _http = http;
            _logger = logger;
            _dbContext = dbContext;
            _userDataConfig = userDataConfig.Value;
        }

        public async Task BuildReport()
        {
            var httpClient = _http.CreateClient();

            var userData = await FetchUserData(httpClient);
            _logger.LogInformation($"users: {userData}");

        }

        private async Task<List<UserModel>> FetchUserData(HttpClient httpClient)
        {
            var endpoint = BuildUserServiceEndpoint();
            var userRecords = await httpClient.GetAsync(endpoint);
            var jsonSerializeOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var userData = await userRecords
                .Content
                .ReadFromJsonAsync<List<UserModel>>(jsonSerializeOptions);

            return userData ?? new List<UserModel>();
        }

        private string BuildUserServiceEndpoint()
        {
            var userServiceProtocol = _userDataConfig.UserDataProtocol;
            var userServiceHost = _userDataConfig.UserDataHost;
            var userServicePort = _userDataConfig.UserDataPort;

            return $"{userServiceProtocol}://{userServiceHost}:{userServicePort}/userDetails";
        }

    }
}
