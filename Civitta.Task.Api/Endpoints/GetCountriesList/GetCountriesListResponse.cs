namespace Civitta.Task1.Api.Endpoints.GetCountriesList
{
    public class GetCountriesListResponse
    {
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public List<string> Regions { get; set; }
    }
}
