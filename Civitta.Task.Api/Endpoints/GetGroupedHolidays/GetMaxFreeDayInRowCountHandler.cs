using Civitta.Task1.Api.Models;
using Civitta.Task1.Api.Services;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Client;
using static Civitta.Task1.Api.Services.EnricoService;
namespace Civitta.Task1.Api.Endpoints.GetGroupedHolidays
{
    public class GetMaxFreeDayInRowCountHandler : Endpoint<GetMaxFreeDayInRowCountRequest,List<GetMaxFreeDayInRowCountResponse>>
    {
        private readonly DatabaseContext _dbContext;
        private readonly IEnricoService _enricoService;
        public GetMaxFreeDayInRowCountHandler(DatabaseContext dbContext, IEnricoService enricoService)
        {
            _dbContext = dbContext;
            _enricoService = enricoService;
        }

        public override void Configure()
        {
            Get("/api/countries/holidayListByMonth");
            AllowAnonymous();
        }

        public override async Task HandleAsync(GetMaxFreeDayInRowCountRequest request, CancellationToken ct)
        {
            var country = _dbContext.Countries.Include(s=>s.Regions).FirstOrDefault(s => s.Code == request.CountryCode);
            var region = country.Regions.FirstOrDefault(s=>s.Code == request.Region);
            var regionId = region?.Id;

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
            if (country.DataFromDate.Year > request.Year || country.DataToDate.Year < request.Year)
            {
                AddError(r => r.Year, $"No data for Year {request.Year}. please select between {country.DataFromDate.Year} and {country.DataToDate.Year}");
            }

            ThrowIfAnyErrors();

            var groupedHolidays = _dbContext.Holidays
                .Where(s => s.CountryRegionId == regionId && s.CountryId == country.Id && s.Date.Year == request.Year)
                .OrderBy(s=>s.Date)
                .GroupBy(s=>s.Date.Month)
                .ToList();

         

            if (groupedHolidays.Count > 0 )
            {
                Response = groupedHolidays.Select(s => new GetMaxFreeDayInRowCountResponse
                {
                    Month = s.Key,
                    Holidays = s.Select(holiday => new HolidayItemModel
                    {
                        Date = holiday.Date,
                        Name = holiday.Name
                    }).ToList()
                }).ToList();
            }
            else
            {
                var holidayList = await _enricoService.GetHolidayForYear(request.Year,request.CountryCode,request.Region);


                Response = holidayList.OrderBy(s=>s.Date.Month).GroupBy(s=>s.Date.Month).Select(s => new GetMaxFreeDayInRowCountResponse
                {
                    Month = s.Key,
                    Holidays = s.Select(holiday => new HolidayItemModel
                    {
                        Date = new DateTime(holiday.Date.Year, holiday.Date.Month, holiday.Date.Day),
                        Name = holiday.Name.FirstOrDefault(s=>s.Lang == "en").Text
                    }).ToList()
                }).ToList();

                var holidays = holidayList.Select(s => new Holiday
                {
                    CountryId = country.Id,
                    CountryRegionId = regionId,
                    Date = new DateTime(s.Date.Year, s.Date.Month, s.Date.Day),
                    Name = s.Name.FirstOrDefault(s => s.Lang == "en").Text
                });

                await _dbContext.Holidays.AddRangeAsync(holidays);
                await _dbContext.SaveChangesAsync();
            }
        }

    }
}
