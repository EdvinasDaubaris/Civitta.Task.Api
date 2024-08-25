namespace Civitta.Task1.Api.Models
{
    public class Holiday
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public Country Country { get; set; }
        public int? CountryRegionId { get; set; }
        public CountryRegion? CountryRegion { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
    }
}
