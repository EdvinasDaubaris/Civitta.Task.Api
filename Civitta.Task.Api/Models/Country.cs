namespace Civitta.Task1.Api.Models
{
    public class Country
    {
        public int Id { get; set; } 
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime DataFromDate { get; set; }
        public DateTime DataToDate { get; set; }
        public ICollection<CountryRegion> Regions { get; set; }
    }
}
