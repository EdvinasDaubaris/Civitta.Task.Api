namespace Civitta.Task1.Api.Models
{
    public class MaximumFreeDayInRow
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public string? Region { get; set; }
        public string CountryCode { get; set; }

        public int MaximumFreeDayInRowCount { get; set; }
    }
}
