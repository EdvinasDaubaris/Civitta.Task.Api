using Civitta.Task1.Api.Models;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Client;
namespace Civitta.Task1.Api.Endpoints.GetCountriesList
{
    public class GetCountriesListHandler : EndpointWithoutRequest<List<GetCountriesListResponse>>
    {
        private readonly DatabaseContext _dbContext;
        private readonly IMemoryCache _cache;
        public GetCountriesListHandler(DatabaseContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public override void Configure()
        {
            Get("/api/countries/list");
        }

        public override async Task HandleAsync(CancellationToken ct)
        {

            if (!_cache.TryGetValue("Countries", out List<GetCountriesListResponse> countries))
            {
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
    }
}
