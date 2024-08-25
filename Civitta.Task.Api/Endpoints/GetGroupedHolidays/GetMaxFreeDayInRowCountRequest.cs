namespace Civitta.Task1.Api.Endpoints.GetGroupedHolidays
{
    public class GetMaxFreeDayInRowCountRequest
    {
        public int Year { get; set; }
        public string CountryCode { get; set; }
        public string? Region { get; set; }
    }
}
