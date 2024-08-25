using Azure;
using Azure.Core;
using Civitta.Task1.Api.Models;
using Civitta.Task1.Api.Services;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Client;
using RestSharp;
using static Civitta.Task1.Api.Services.EnricoService;
namespace Civitta.Task1.Api.Endpoints.GetCountriesList
{
    public class GetCountriesListHandler : EndpointWithoutRequest<List<GetCountriesListResponse>>
    {
        private readonly DatabaseContext _dbContext;
        private readonly IMemoryCache _cache;
        private readonly IEnricoService _enricoService;
        public GetCountriesListHandler(DatabaseContext dbContext, IMemoryCache cache, IEnricoService enricoService)
        {
            _dbContext = dbContext;
            _cache = cache;
            _enricoService = enricoService;
        }

        public override void Configure()
        {
            Get("/api/countries/list");
            AllowAnonymous();
        }
       
        public override async Task HandleAsync(CancellationToken ct)
        {
            if (!_cache.TryGetValue("Countries", out List<GetCountriesListResponse> countries))
            {
                if(_dbContext.Countries.Count() == 0) // initialize
                {
                    await CreateCountriesAndRegions();
                }

                countries = await _dbContext.Countries.Select(country => new GetCountriesListResponse
                {
                    CountryCode = country.Code,
                    CountryName = country.Name,
                    Regions = country.Regions.Select(s => s.Code).ToList()
                }).ToListAsync();

                _cache.Set("Countries", countries, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) });
            }


            Response = countries;
        }

        public async Task CreateCountriesAndRegions()
        {
            var countriesToCreate = await _enricoService.GetCountries();
            foreach (var item in countriesToCreate)
            {
                var country = new Country
                {
                    Code = item.CountryCode,
                    Name = item.FullName,
                    DataFromDate = new DateTime(item.FromDate.Year, item.FromDate.Month, item.FromDate.Day),
                    DataToDate = new DateTime(item.ToDate.Year == 32767 ? 2999 : item.ToDate.Year, item.ToDate.Month, item.ToDate.Day),
                };
                _dbContext.Countries.Add(country);

                foreach (var region in item.Regions)
                {
                    _dbContext.CountryRegions.Add(new CountryRegion
                    {
                        Code = region,
                        Country = country
                    });
                }
            }

            _dbContext.SaveChanges();



        }
    }
}
