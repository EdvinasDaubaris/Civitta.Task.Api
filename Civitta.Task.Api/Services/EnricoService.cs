using RestSharp;
using static Civitta.Task1.Api.Endpoints.GetCountriesList.GetCountriesListHandler;
using static Civitta.Task1.Api.Services.EnricoService;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Civitta.Task1.Api.Services
{
    public interface IEnricoService
    {
        Task<bool> IsPublicHoliday(DateTime date, string countryCode, string? region);
        Task<bool> IsWorkDay(DateTime date, string countryCode, string? region);
        Task<List<HolidayItemResponse>> GetHolidayForYear(int year, string countryCode, string? region);
        Task<List<CountryResponse>> GetCountries();
    }

    public class EnricoService : IEnricoService
    {
        private readonly RestClient _client;
        private const string BaseUrl = "https://kayaposoft.com/enrico/json/v3.0";

        public EnricoService()
        {
            _client = new RestClient(BaseUrl);
        }
        public async Task<bool> IsPublicHoliday(DateTime date, string countryCode, string? region)
        {
            var request = new RestRequest("isPublicHoliday", Method.Get);
            request.AddParameter("date", date.ToString("yyyy-MM-dd"));
            request.AddParameter("country", countryCode);
            request.AddParameter("region", region);

            var response = await _client.GetAsync<PublicHolidayResponse>(request);
            return response?.IsPublicHoliday ?? false;
        }

        public async Task<bool> IsWorkDay(DateTime date, string countryCode, string? region)
        {
            var request = new RestRequest("isWorkDay", Method.Get);
            request.AddParameter("date", date.ToString("yyyy-MM-dd"));
            request.AddParameter("country", countryCode);
            request.AddParameter("region", region);

            var response = await _client.GetAsync<WorkDayResponse>(request);
            return response?.IsWorkDay ?? false;
        }

        public async Task<List<HolidayItemResponse>> GetHolidayForYear(int year, string countryCode, string? region)
        {
            var request = new RestRequest("getHolidaysForYear", Method.Get);
            request.AddParameter("year", year);
            request.AddParameter("country", countryCode);
            request.AddParameter("region", region);

            var response = await _client.GetAsync<List<HolidayItemResponse>>(request);
            return response;
        }

        public async Task<List<CountryResponse>> GetCountries()
        {
            var request = new RestRequest("getSupportedCountries", Method.Get);

            var response = await _client.GetAsync<List<CountryResponse>>(request);
            return response;
        }

        public class CountryResponse
        {
            public string CountryCode { get; set; }
            public string FullName { get; set; }
            public Date FromDate { get; set; }
            public Date ToDate { get; set; }
            public List<string> Regions { get; set; }
        }
        public class HolidayItemResponse
        {
            public Date Date { get; set; }
            public List<HolidayName> Name { get; set; }
        }


        public class HolidayName
        {
            public string Lang { get; set; }
            public string Text { get; set; }
        }
        public class Date
        {
            public int Day { get; set; }
            public int Month { get; set; }
            public int Year { get; set; }
            public int DayOfWeek { get; set; }
        }

        private class PublicHolidayResponse
        {
            public bool IsPublicHoliday { get; set; }
        }

        private class WorkDayResponse
        {
            public bool IsWorkDay { get; set; }
        }
    }
}
