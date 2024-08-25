namespace Civitta.Task1.Api.Endpoints.GetMaxFreeDayInRowCount
{
    public class GetMaxFreeDayInRowCountRequest
    {
        public int Year { get; set; }
        public string CountryCode { get; set; }
        public string? Region { get; set; }
    }
}
