using Contact.Report.Config;
using Contact.Report.Helpers;
using Contact.Report.Models;
using IronXL;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Contact.Report.BusinessLogic
{
    public interface IReportCreator
    {
        public Task<ResponseModel> BuildReport();
    }

    public class ReportCreator : IReportCreator
    {
        private readonly IHttpClientFactory _http;
        private readonly ILogger<ReportCreator> _logger;
        private readonly UserDataConfig _userDataConfig;

        public ReportCreator(
            IHttpClientFactory http,
            ILogger<ReportCreator> logger,
            IOptions<UserDataConfig> userDataConfig
            )
        {
            _http = http;
            _logger = logger;
            _userDataConfig = userDataConfig.Value;
        }

        public async Task<ResponseModel> BuildReport()
        {
            try
            {
                var httpClient = _http.CreateClient();

                var userData = await FetchUserData(httpClient);
                _logger.LogInformation($"users: {userData}");

                var result = userData
                    .SelectMany(info => info.ContactInformations, (info, user) => new { info, user })
                    .Where(pair => pair.user.InformationType == Constants.Location)
                    .GroupBy(pair => pair.user.InformationDetail, pair => pair.info);

                var responseModel = new List<ReportModel>();

                foreach (var item in result)
                {
                    var location = item.Key;
                    var userCount = item.Count();

                    var phoneNumbers = item
                        .SelectMany(info => info.ContactInformations, (info, user) => new { info, user })
                        .Where(pair => pair.user.InformationType == Constants.PhoneNumber)
                        .GroupBy(pair => pair.user.InformationDetail, pair => pair.info);

                    var phoneNumberCount = phoneNumbers.Count();

                    responseModel.Add(new ReportModel
                    {
                        Location = location,
                        UserCount = userCount,
                        PhoneNumberCount = phoneNumberCount
                    });
                }

                CreateExcelFile(responseModel);

                return new ResponseModel(200, "Report generated successfully", responseModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BuildReport Ex: {ex}");
                return new ResponseModel(500, "An error occurred while generating the report");
            }

        }

        private void CreateExcelFile(List<ReportModel> model)
        {
            WorkBook workbook = WorkBook.Create(ExcelFileFormat.XLSX);

            var sheet = workbook.CreateWorkSheet("Report");
            sheet["A1"].Value = "Konum Bilgis";
            sheet["B1"].Value = "O konumda yer alan rehbere kayıtlı kişi sayısı";
            sheet["C1"].Value = "O konumda yer alan rehbere kayıtlı telefon numarası sayısı";

            int row = 2;
            foreach (var item in model)
            {
                sheet["A" + row].Value = item.Location;
                sheet["B" + row].Value = item.UserCount;
                sheet["C" + row].Value = item.PhoneNumberCount;
                row++;
            }

            workbook.SaveAs($"ExcelFiles/Report-{DateTime.Now}.xlsx");
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
