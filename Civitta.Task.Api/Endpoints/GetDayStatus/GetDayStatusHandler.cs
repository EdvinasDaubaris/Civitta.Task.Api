using Civitta.Task1.Api.Models;
using Civitta.Task1.Api.Services;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Client;
using System.Diagnostics.Metrics;
namespace Civitta.Task1.Api.Endpoints.GetDayStatus
{
    public class GetDayStatusHandler : Endpoint<GetDayStatusRequest,GetDayStatusResponse>
    {
        private readonly DatabaseContext _dbContext;
        private readonly IEnricoService _enricoService;
        public GetDayStatusHandler(DatabaseContext dbContext, IEnricoService enricoService)
        {
            _dbContext = dbContext;
            _enricoService = enricoService;
        }

        public override void Configure()
        {
            Get("/api/countries/dayStatus");
            AllowAnonymous();
        }

        public override async Task HandleAsync(GetDayStatusRequest request, CancellationToken ct)
        {
            var country = _dbContext.Countries.Include(s => s.Regions).FirstOrDefault(s => s.Code == request.CountryCode);
            if (country == null)
            {
                AddError(r => r.CountryCode, "Country code is invalid");
            }
            if (country.Regions.Any())
            {
                if (string.IsNullOrEmpty(request.Region))
                {
                    AddError(r => r.Region, "You must select region for this country.");
                }
                else if (country.Regions.FirstOrDefault(s => s.Code == request.Region) == null)
                {
                    AddError(r => r.Region, "Region you selected does not exist.");
                }

            }
            if (country.DataFromDate > request.Date || country.DataToDate < request.Date)
            {
                AddError(r => r.Date, $"No data for Year {request.Date.ToString("d")}. please select between {country.DataFromDate.Year.ToString("d")} and {country.DataToDate.Year.ToString("d")}");
            }
            ThrowIfAnyErrors();




            var normalizeDate = new DateTime(request.Date.Year, request.Date.Month, request.Date.Day);
            var dayStatus = await _dbContext.DayStatuses
                .FirstOrDefaultAsync(s=>s.CountryCode == request.CountryCode && s.Date == normalizeDate && s.Region == request.Region);

            if (dayStatus != null)
            {
                Response = new GetDayStatusResponse
                {
                    IsWorkDay = dayStatus.IsWorkDay,
                    IsHoliday = dayStatus.IsHoliday,
                    IsFreeDay = !dayStatus.IsWorkDay || dayStatus.IsHoliday
                };
            }
            else
            {
                var isHoliday = await _enricoService.IsPublicHoliday(normalizeDate, request.CountryCode, request.Region);
                var isWorkday = await _enricoService.IsWorkDay(normalizeDate, request.CountryCode, request.Region);


                Response = new GetDayStatusResponse
                {
                    IsWorkDay = isWorkday,
                    IsHoliday = isHoliday,
                    IsFreeDay = !isWorkday || isHoliday
                };

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
            }



        }
    }
}
