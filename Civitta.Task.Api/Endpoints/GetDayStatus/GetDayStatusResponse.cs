namespace Civitta.Task1.Api.Endpoints.GetDayStatus
{
    public class GetDayStatusResponse
    {
        public bool IsWorkDay { get; set; }
        public bool IsFreeDay { get; set; }
        public bool IsHoliday { get; set; }
    }
}
