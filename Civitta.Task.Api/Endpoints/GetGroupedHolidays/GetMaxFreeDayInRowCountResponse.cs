namespace Civitta.Task1.Api.Endpoints.GetGroupedHolidays
{
    public class GetMaxFreeDayInRowCountResponse
    {
        public int Month { get; set; }
        public List<HolidayItemModel> Holidays { get; set; }
    }

    public class HolidayItemModel
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }
    }
  
}
