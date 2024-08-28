namespace Civitta.Task1.Api.Endpoints.GetDayStatus
{
    public class GetDayStatusRequest
    {
        public DateTime Date { get; set; }
        public string CountryCode { get; set; }
        public string? Region { get; set; }
    }
}
