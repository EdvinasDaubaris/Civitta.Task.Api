namespace Civitta.Task1.Api.Endpoints.GetDayStatus
{
    public class GetCountriesListRequest
    {
        public DateTime Date { get; set; }
        public string CountryCode { get; set; }
        public string? Region { get; set; }
    }
}
