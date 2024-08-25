namespace Civitta.Task1.Api.Models
{
    public class DayStatus
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string? Region { get; set; }
        public string CountryCode { get; set; }


        public bool IsWorkDay { get; set; }
        public bool IsHoliday { get; set; }
    }
}
