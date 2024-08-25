using Civitta.Task1.Api.Models;
using Civitta.Task1.Api.Services;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Client;
namespace Civitta.Task1.Api.Endpoints.GetDayStatus
{
    public class GetCountriesListHandler : Endpoint<GetCountriesListRequest,GetCountriesListResponse>
    {
        private readonly DatabaseContext _dbContext;
        private readonly IEnricoService _enricoService;
        public GetCountriesListHandler(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override void Configure()
        {
            Get("/api/countries/dayStatus");
        }

        public override async Task HandleAsync(GetCountriesListRequest request, CancellationToken ct)
        {
            var normalizeDate = new DateTime(request.Date.Year, request.Date.Month, request.Date.Day);
            var dayStatus = await _dbContext.DayStatuses
                .FirstOrDefaultAsync(s=>s.CountryCode == request.CountryCode && s.Date == normalizeDate && s.Region == request.Region);

            if (dayStatus != null)
            {
                Response = new GetCountriesListResponse
                {
                    IsWorkDay = dayStatus.IsWorkDay,
                    IsHoliday = dayStatus.IsHoliday,
                    IsFreeDay = !dayStatus.IsWorkDay || dayStatus.IsHoliday
                };
            }
            else
            {
                var isHolidayTask = _enricoService.IsPublicHoliday(normalizeDate, request.CountryCode, request.Region);
                var isWorkdayTask = _enricoService.IsWorkDay(normalizeDate, request.CountryCode, request.Region);

                await Task.WhenAll(isHolidayTask, isWorkdayTask);

                bool isHoliday = await isHolidayTask;
                bool isWorkday = await isWorkdayTask;

                Response = new GetCountriesListResponse
                {
                    IsWorkDay = isWorkday,
                    IsHoliday = isHoliday,
                    IsFreeDay = !dayStatus.IsWorkDay || isHoliday
                };

                _ = Task.Run(async () =>
                {
                    var newDayStatus = new DayStatus
                    {
                        CountryCode = request.CountryCode,
                        Date = normalizeDate,
                        Region = request.Region,
                        IsWorkDay = isWorkday,
                        IsHoliday = isHoliday
                    };

                    await _dbContext.DayStatuses.AddAsync(newDayStatus);
                    await _dbContext.SaveChangesAsync();
                }, ct);
            }



        }
    }
}
