namespace Civitta.Task1.Api.Models
{
    public class CountryRegion
    {
        public int Id { get; set; } 
        public string Code { get; set; }
        public int CountryId { get; set; }
        public Country Country { get; set; }
    }
}
