using Civitta.Task1.Api.Models;
using Civitta.Task1.Api.Services;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Client;
using static Civitta.Task1.Api.Services.EnricoService;
namespace Civitta.Task1.Api.Endpoints.GetMaxFreeDayInRowCount
{
    public class GetMaxFreeDayInRowCountHandler : Endpoint<GetMaxFreeDayInRowCountRequest,GetMaxFreeDayInRowCountResponse>
    {
        private readonly DatabaseContext _dbContext;
        private readonly IEnricoService _enricoService;
        public GetMaxFreeDayInRowCountHandler(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override void Configure()
        {
            Get("/api/countries/maximumFreeDaysInRowForYear");
        }

        public override async Task HandleAsync(GetMaxFreeDayInRowCountRequest request, CancellationToken ct)
        {
            var freeDayCount = await _dbContext.MaximumFreeDayInRows
                .FirstOrDefaultAsync(s=>s.CountryCode == request.CountryCode && s.Year == request.Year && s.Region == request.Region);

            if (freeDayCount != null)
            {
                Response = new GetMaxFreeDayInRowCountResponse
                {
                    MaximumFreeDayInRowCount = freeDayCount.MaximumFreeDayInRowCount
                };
            }
            else
            {
                var holidayList = await _enricoService.GetHolidayForYear(request.Year,request.CountryCode,request.Region);

                var freeDayCountInRow = GetMaximumInYearFreeDaysInRow(holidayList);

                 Response = new GetMaxFreeDayInRowCountResponse
                {
                   MaximumFreeDayInRowCount = freeDayCountInRow
                 };

                _ = Task.Run(async () =>
                {
                    var newDayStatus = new MaximumFreeDayInRow
                    {
                        CountryCode = request.CountryCode,
                        Year = request.Year,
                        Region = request.Region,
                        MaximumFreeDayInRowCount = freeDayCountInRow,
                    };

                    await _dbContext.MaximumFreeDayInRows.AddAsync(newDayStatus);
                    await _dbContext.SaveChangesAsync();
                }, ct);
            }
        }

        public int GetMaximumInYearFreeDaysInRow(List<HolidayItemResponse> list)
        {
            list = list.OrderBy(h => new DateTime(h.Date.Year, h.Date.Month, h.Date.Day)).ToList();

            int maxFreeDaysInRow = 0;
            int currentStreak = 0;
            DateTime? previousDay = null;

            foreach (var holiday in list)
            {
                var currentDate = new DateTime(holiday.Date.Year, holiday.Date.Month, holiday.Date.Day);

                if (previousDay == null)
                {
                    previousDay = currentDate;
                    currentStreak = 1;
                    maxFreeDaysInRow = 1;
                    continue;
                }

                int difference = (currentDate - previousDay.Value).Days;

                if (difference == 1)
                {
                    currentStreak++;
                }
                else if (difference == 2 && previousDay.Value.DayOfWeek == DayOfWeek.Friday)
                {
                    currentStreak += 2;
                }
                else
                {
                    maxFreeDaysInRow = Math.Max(maxFreeDaysInRow, currentStreak);
                    currentStreak = 1;
                }

                if (currentDate.DayOfWeek == DayOfWeek.Friday)
                {
                    currentStreak += 2;
                }
                else if (currentDate.DayOfWeek == DayOfWeek.Saturday)
                {
                    currentStreak += 1;
                }

                maxFreeDaysInRow = Math.Max(maxFreeDaysInRow, currentStreak);

                previousDay = currentDate;
            }

            return maxFreeDaysInRow;
        }
    }
}
